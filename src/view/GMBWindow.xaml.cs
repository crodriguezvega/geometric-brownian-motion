﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;

using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;

namespace GeometricBrownianMotion
{
  /// <summary>
  /// Interaction logic for GMBWindow.xaml
  /// </summary>
  public partial class GMBWindow : Window
  {
    private CancellationTokenSource drawingToken;
    private CancellationTokenSource rescalingToken;

    private ViewModel viewModel;

    public GMBWindow()
    {
      InitializeComponent();

      viewModel = new ViewModel();
      this.DataContext = viewModel;
    }

    /// <summary>
    /// Generates a geometric brownian motion sample path.
    /// </summary>
    /// <param name="rng">Random number generator.</param>
    /// <param name="numSamples">Number of samples of the path.</param>
    /// <param name="initialValue">Initial value of the sample path.</param>
    /// <param name="mu">Drift of the sample path.</param>
    /// <param name="sigma">Standard deviation of the sample path.</param>
    /// <param name="T">Time length.</param>
    private Task<Points> GBMPath(Random rng, int numSamples, double initialValue, double mu, double sigma, double T)
    {
      return Task.Run(() =>
      {
        double dt = T / numSamples;
        double St = initialValue;

        var worldPoints = new Points();
        worldPoints.Add(new Point(0, St));

        for (int i = 1; i <= numSamples; ++i)
        {
          double Z = Normal.Sample(rng, 0.0, 1.0);
          double S = St * Math.Exp((mu - (Math.Pow(sigma, 2) / 2)) * dt + sigma * Math.Sqrt(dt) * Z);
          St = S;

          worldPoints.Add(new Point(i * dt, St));
        }

        return worldPoints;
      }, drawingToken.Token);
    }

    /// <summary>
    /// Converts the values of the geometric brownian motion sample path to pixel coordinates in the canvas.
    /// </summary>
    /// <param name="worldPoints">Values of the geometric sample path.</param>
    /// <param name="token">Token to be able to cancel the task.</param>
    /// <param name="width">Canvas width.</param>
    /// <param name="height">Canvas height.</param>
    /// <param name="rangeX">Range of values of the sample path in the X axis.</param>
    /// <param name="rangeY">Range of values of the sample path in the Y axis.</param>
    Task<Points> ConvertWorldPointsToCanvasPoints(Points worldPoints, CancellationToken token, double width, double height, Range rangeX, Range rangeY)
    {
      return Task.Run(() =>
      {
        var canvasPoints = new Points();
        worldPoints.ForEach(wp => canvasPoints.Add(ConvertPoint(wp, width, height, rangeX, rangeY)));
        return canvasPoints;
      }, token);
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
      if (viewModel.Y.Range.Min > yValues.Min() || viewModel.Y.Range.Max < yValues.Max())
      {
        // Cancel any rescaling being executed because the ranges are going to change
        rescalingToken.Cancel();

        var min = viewModel.Y.Range.Min;
        if (viewModel.Y.Range.Min > yValues.Min())
        {
          min = yValues.Min();
        }

        var max = viewModel.Y.Range.Max;
        if (viewModel.Y.Range.Max < yValues.Max())
        {
          max = yValues.Max();
        }

        viewModel.Y.Range.Min = min - (min * 0.05);
        viewModel.Y.Range.Max = max + (max * 0.05);
        rescalingToken = new CancellationTokenSource();

        return true;
      }

      return false;
    }

    /// <summary>
    /// Rescales sample paths to accomodate to new values in the range of the Y axis.
    /// </summary>
    /// <param name="rescalingIndex">Index in the list of sample paths that need rescaling.</param>
    private async void Rescale(int rescalingIndex)
    {
      for (var i = 0; i < rescalingIndex; i++)
      {
        var samplePath = viewModel.SamplePaths[i];

        try
        {
          samplePath.CanvasPoints = await ConvertWorldPointsToCanvasPoints(samplePath.WorldPoints, rescalingToken.Token, chartCanvas.Width, chartCanvas.Height, viewModel.X.Range, viewModel.Y.Range);
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
    /// Draws a geometric brownian motion sample path.
    /// </summary>
    private async void Draw()
    {
      int numPaths = 0; bool numPathsParsing = int.TryParse(txNumPaths.Text, out numPaths);
      int numSamples = 0; bool numSamplesParsing = int.TryParse(txNumSamples.Text, out numSamples);
      double initialValue = 0; bool initialValueParsing = double.TryParse(txInitialValue.Text, out initialValue);
      double mu = 0; bool muParsing = double.TryParse(txMu.Text, out mu);
      double sigma = 0; bool sigmaParsing = double.TryParse(txSigma.Text, out sigma);
      double T = 0; bool TParsing = double.TryParse(txT.Text, out T);

      // Check input
      viewModel.InputError = String.Empty;
      if (!(numPathsParsing && numSamplesParsing && initialValueParsing && muParsing && sigmaParsing && TParsing))
      {
        viewModel.InputError = "Incorrect input values";
        startStopBtn.IsChecked = false;
        return;
      }

      if (numPaths < 0 || numSamples < 0 || initialValue < 0 || sigma < 0 || T < 0)
      {
        viewModel.InputError = "Incorrect input values";
        startStopBtn.IsChecked = false;
        return;
      }

      Type brushesType = typeof(Brushes);
      var colors = brushesType.GetProperties();
      var rng = new MersenneTwister();
      viewModel.X.Range.Max = T;

      for (int i = 0; i < numPaths; ++i)
      {
        var samplePath = new SamplePath();

        try
        {
          samplePath.WorldPoints = await GBMPath(rng, numSamples, initialValue, mu, sigma, T);

          var yValues = samplePath.WorldPoints.Select(p => p.Y);
          if (IsRescalingNeeded(yValues))
          {
            Rescale(i);
          }

          samplePath.CanvasPoints = await ConvertWorldPointsToCanvasPoints(samplePath.WorldPoints, drawingToken.Token, chartCanvas.Width, chartCanvas.Height, viewModel.X.Range, viewModel.Y.Range);
        }
        catch (OperationCanceledException ex)
        {
          Debug.WriteLine("Drawing cancelled in iteration " + i + ": " + ex.Message);
          break;
        }

        samplePath.Stroke = (Brush)colors.ElementAt(i % colors.Length).GetValue(null, null);
        samplePath.Path = samplePath.CanvasPoints.ToString();
        viewModel.SamplePaths.Add(samplePath);
        Debug.WriteLine("Drawn sample path " + i);
      }

      startStopBtn.IsChecked = false;
    }

    /// <summary>
    /// Start/Stop button action.
    /// </summary>
    private void StartStopAction(object sender, RoutedEventArgs e)
    {
      if (startStopBtn.IsChecked == true)
      {
        drawingToken = new CancellationTokenSource();
        rescalingToken = new CancellationTokenSource();

        viewModel.SamplePaths.Clear();
        viewModel.Y.Range.Min = 0;
        viewModel.Y.Range.Max = 1;
        viewModel.X.Range.Min = 0;
        viewModel.X.Range.Max = 1;

        Draw();
      }
      else
      {
        drawingToken.Cancel();
        rescalingToken.Cancel();
      }
    }
  }
}