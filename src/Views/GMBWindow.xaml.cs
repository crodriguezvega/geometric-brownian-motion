using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GeometricBrownianMotion
{
  /// <summary>
  /// Interaction logic for GMBWindow.xaml
  /// </summary>
  public partial class GMBWindow : Window
  {
    private readonly ViewModel viewModel;

    public GMBWindow()
    {
      InitializeComponent();

      viewModel = new ViewModel();
      this.DataContext = viewModel;
    }

    /// <summary>
    /// Input validation.
    /// </summary>
    private bool ValidateInput()
    {
      int numPaths = 0;
      bool numPathsParsing = int.TryParse(txNumPaths.Text,
                                          NumberStyles.Integer,
                                          CultureInfo.CurrentCulture,
                                          out numPaths);
      int numSamples = 0;
      bool numSamplesParsing = int.TryParse(txNumSamples.Text,
                                            NumberStyles.Integer,
                                            CultureInfo.CurrentCulture,
                                            out numSamples);
      double initialValue = 0;
      bool initialValueParsing = double.TryParse(txInitialValue.Text,
                                                 NumberStyles.Float,
                                                 CultureInfo.CurrentCulture,
                                                 out initialValue);
      double mu = 0;
      bool muParsing = double.TryParse(txMu.Text,
                                       NumberStyles.Float,
                                       CultureInfo.CurrentCulture,
                                       out mu);
      double sigma = 0;
      bool sigmaParsing = double.TryParse(txSigma.Text,
                                          NumberStyles.Float,
                                          CultureInfo.CurrentCulture,
                                          out sigma);
      double T = 0;
      bool TParsing = double.TryParse(txT.Text,
                                      NumberStyles.Float,
                                      CultureInfo.CurrentCulture,
                                      out T);

      // Check input
      viewModel.InputError = String.Empty;
      if (!(numPathsParsing && numSamplesParsing && initialValueParsing && muParsing && sigmaParsing && TParsing))
      {
        viewModel.InputError = "Incorrect input values";
        startStopBtn.IsChecked = false;
        return false;
      }

      if (numPaths < 0 || numSamples < 0 || initialValue < 0 || sigma < 0 || T < 0)
      {
        viewModel.InputError = "Incorrect input values";
        startStopBtn.IsChecked = false;
        return false;
      }

      viewModel.NumberOfPaths = numPaths;
      viewModel.NumberOfSamples = numSamples;
      viewModel.InitialValue = initialValue;
      viewModel.Mu = mu;
      viewModel.Sigma = sigma;
      viewModel.T = T;

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
          await viewModel.Start(chartCanvas.Width, chartCanvas.Height);
          startStopBtn.IsChecked = false;
        }
      }
      else
      {
        viewModel.Stop();
      }
    }
  }
}