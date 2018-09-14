using System;
using System.Globalization;
using System.Windows;
using GeometricBrownianMotion.ViewModels;

namespace GeometricBrownianMotion.Views
{
  /// <summary>
  /// Interaction logic for GMBWindow.xaml
  /// </summary>
  public partial class GMBWindow : Window
  {
    private readonly ViewModel _viewModel;

    public GMBWindow()
    {
      InitializeComponent();

      _viewModel = new ViewModel();
      this.DataContext = _viewModel;
    }

    /// <summary>
    /// Input validation.
    /// </summary>
    private bool ValidateInput()
    {
      var numPathsParsing = int.TryParse(txNumPaths.Text,
                                         NumberStyles.Integer,
                                         CultureInfo.CurrentCulture,
                                         out var numPaths);

      var numSamplesParsing = int.TryParse(txNumSamples.Text,
                                           NumberStyles.Integer,
                                           CultureInfo.CurrentCulture,
                                           out var numSamples);

      var initialValueParsing = double.TryParse(txInitialValue.Text,
                                                NumberStyles.Float,
                                                CultureInfo.CurrentCulture,
                                                out var initialValue);

      var muParsing = double.TryParse(txMu.Text,
                                      NumberStyles.Float,
                                      CultureInfo.CurrentCulture,
                                      out var mu);

      var sigmaParsing = double.TryParse(txSigma.Text,
                                         NumberStyles.Float,
                                         CultureInfo.CurrentCulture,
                                         out var sigma);

      var TParsing = double.TryParse(txT.Text,
                                     NumberStyles.Float,
                                     CultureInfo.CurrentCulture,
                                     out var T);

      // Check input
      _viewModel.InputError = String.Empty;
      if (!(numPathsParsing && numSamplesParsing && initialValueParsing && muParsing && sigmaParsing && TParsing))
      {
        _viewModel.InputError = "Incorrect input values";
        startStopBtn.IsChecked = false;
        return false;
      }

      if (numPaths < 0 || numSamples < 0 || initialValue < 0 || sigma < 0 || T < 0)
      {
        _viewModel.InputError = "Incorrect input values";
        startStopBtn.IsChecked = false;
        return false;
      }

      _viewModel.NumberOfPaths = numPaths;
      _viewModel.NumberOfSamples = numSamples;
      _viewModel.InitialValue = initialValue;
      _viewModel.Mu = mu;
      _viewModel.Sigma = sigma;
      _viewModel.T = T;

      return true;
    }

    /// <summary>
    /// Start/Stop button action.
    /// </summary>
    private async void StartStopAction(object sender, RoutedEventArgs e)
    {
      if (startStopBtn.IsChecked == true)
      {
        if (ValidateInput())
        {
          await _viewModel.Start(chartCanvas.Width, chartCanvas.Height);
          startStopBtn.IsChecked = false;
        }
      }
      else
      {
        _viewModel.Stop();
      }
    }
  }
}