using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using GeometricBrownianMotion.Models;

namespace GeometricBrownianMotion.ViewModels
{
  public class ViewModel : INotifyPropertyChanged
  {
    private CancellationTokenSource _drawingToken;
    private CancellationTokenSource _rescalingToken;

    private string _inputError;
    public string InputError
    {
      get => _inputError;
      set
      {
        if (_inputError == value)
        {
          return;
        }
        _inputError = value;
        this.NotifyPropertyChanged("InputError");
      }
    }

    public AxisX X { get; set; }
    public AxisY Y { get; set; }
    public ObservableCollection<SamplePath> SamplePaths { get; set; }

    // Input parameters
    public int NumberOfPaths { get; set; }
    public int NumberOfSamples { get; set; }
    public double InitialValue { get; set; }
    public double Mu { get; set; }
    public double Sigma { get; set; }
    public double T { get; set; }

    public ViewModel()
    {
      X = new AxisX(0, 1);
      Y = new AxisY(0, 1);
      SamplePaths = new ObservableCollection<SamplePath>();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
      }
    }

    /// <summary>
    /// Generates a geometric brownian motion sample path.
    /// </summary>
    /// <param name="rng">Random number generator.</param>
    private Points GBMPath(Random rng)
    {
      var dt = T / (NumberOfSamples - 1);
      var St = InitialValue;

      var worldPoints = new Points() { new Point(0, St) };

      for (var i = 1; i < NumberOfSamples; ++i)
      {
        var Z = Normal.Sample(rng, 0.0, 1.0);
        var S = St * Math.Exp((Mu - (Math.Pow(Sigma, 2) / 2)) * dt + Sigma * Math.Sqrt(dt) * Z);
        St = S;

        worldPoints.Add(new Point(i * dt, St));
      }

      return worldPoints;
    }

    /// <summary>
    /// Converts the values of the geometric brownian motion sample path to pixel coordinates in the canvas.
    /// </summary>
    /// <param name="worldPoints">Values of the geometric sample path.</param>
    /// <param name="width">Canvas width.</param>
    /// <param name="height">Canvas height.</param>
    /// <param name="rangeX">Range of values of the sample path in the X axis.</param>
    /// <param name="rangeY">Range of values of the sample path in the Y axis.</param>
    private Points ConvertWorldPointsToCanvasPoints(Points worldPoints, double width, double height, Range rangeX, Range rangeY)
    {
      var canvasPoints = new Points();
      worldPoints.ForEach(wp => canvasPoints.Add(ConvertPoint(wp, width, height, rangeX, rangeY)));
      return canvasPoints;
    }

    /// <summary>
    /// Converts one value of the geometric brownian motion sample path to pixel coordinates in the canvas.
    /// </summary>
    /// <param name="pt">Single point from the sample path.</param>
    /// <param name="width">Canvas width.</param>
    /// <param name="height">Canvas height.</param>
    /// <param name="rangeX">Range of values of the sample path in the X axis.</param>
    /// <param name="rangeY">Range of values of the sample path in the Y axis.</param>
    private Point ConvertPoint(Point pt, double width, double height, Range rangeX, Range rangeY)
    {
      return new Point()
      {
        X = (pt.X - rangeX.Min) * width / (rangeX.Max - rangeX.Min),
        Y = height - (pt.Y - rangeY.Min) * height / (rangeY.Max - rangeY.Min)
      };
    }

    /// <summary>
    /// Determines whether it is needed to rescale sample paths already drawn.
    /// </summary>
    /// <param name="yValues">Values of the sample path in the Y axis.</param>
    private bool IsRescalingNeeded(IEnumerable<double> yValues)
    {
      var vals = yValues.ToList();
      if (Y.Range.Min > vals.Min() || Y.Range.Max < vals.Max())
      {
        // Cancel any rescaling being executed because the ranges are going to change
        _rescalingToken.Cancel();

        var min = Y.Range.Min;
        if (Y.Range.Min > vals.Min())
        {
          min = vals.Min();
        }

        var max = Y.Range.Max;
        if (Y.Range.Max < vals.Max())
        {
          max = vals.Max();
        }

        Y.Range.Min = min - (min * 0.05);
        Y.Range.Max = max + (max * 0.05);
        _rescalingToken = new CancellationTokenSource();

        return true;
      }

      return false;
    }

    /// <summary>
    /// Rescales sample paths to accomodate to new values in the range of the Y axis.
    /// </summary>
    /// <param name="rescalingIndex">Index in the list of sample paths that need rescaling.</param>
    /// <param name="canvasWidth">Width of drawing canvas.</param>
    /// <param name="canvasHeight">Height of drawing canvas.</param>
    private async void Rescale(int rescalingIndex, double canvasWidth, double canvasHeight)
    {
      for (var i = 0; i < rescalingIndex; i++)
      {
        var samplePath = SamplePaths[i];

        try
        {
          samplePath.CanvasPoints = await Task.Run(() => ConvertWorldPointsToCanvasPoints(samplePath.WorldPoints,
                                                                                          canvasWidth,
                                                                                          canvasHeight,
                                                                                          X.Range,
                                                                                          Y.Range), _rescalingToken.Token);
          Debug.WriteLine("Rescaled sample path " + i);
        }
        catch (OperationCanceledException ex)
        {
          Debug.WriteLine("Rescaling cancelled in iteration " + i + ": " + ex.Message);
          break;
        }

        samplePath.Path = samplePath.CanvasPoints.ToString();
      }
    }

    /// <summary>
    /// Triggers the Monte Carlo generation of geometric brownian motion sample paths.
    /// </summary>
    /// <param name="canvasWidth">Width of drawing canvas.</param>
    /// <param name="canvasHeight">Height of drawing canvas.</param>
    public async Task Start(double canvasWidth, double canvasHeight)
    {
      _drawingToken = new CancellationTokenSource();
      _rescalingToken = new CancellationTokenSource();

      SamplePaths.Clear();
      Y.Range.Min = 0;
      Y.Range.Max = 1;
      X.Range.Min = 0;
      X.Range.Max = T;

      var brushesType = typeof(Brushes);
      var colors = brushesType.GetProperties();
      var rng = new MersenneTwister();

      for (int i = 0; i < NumberOfPaths; ++i)
      {
        var samplePath = new SamplePath();

        try
        {
          samplePath.WorldPoints = await Task.Run(() => GBMPath(rng), _drawingToken.Token);

          var yValues = samplePath.WorldPoints.Select(p => p.Y);
          if (IsRescalingNeeded(yValues))
          {
            Rescale(i, canvasWidth, canvasHeight);
          }

          samplePath.CanvasPoints = await Task.Run(() => ConvertWorldPointsToCanvasPoints(samplePath.WorldPoints,
                                                                                          canvasWidth,
                                                                                          canvasHeight,
                                                                                          X.Range,
                                                                                          Y.Range), _drawingToken.Token);
        }
        catch (OperationCanceledException ex)
        {
          Debug.WriteLine("Drawing cancelled in iteration " + i + ": " + ex.Message);
          break;
        }

        samplePath.Stroke = (Brush)colors.ElementAt(i % colors.Length).GetValue(null, null);
        samplePath.Path = samplePath.CanvasPoints.ToString();
        SamplePaths.Add(samplePath);
        Debug.WriteLine("Drawn sample path " + i);
      }
    }

    /// <summary>
    /// Stops the Monte Carlo generation of geometric brownian motion sample paths.
    /// </summary>
    public void Stop()
    {
      _drawingToken.Cancel();
      _rescalingToken.Cancel();
    }
  }
}