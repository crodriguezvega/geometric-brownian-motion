using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GeometricBrownianMotion
{
  public class ViewModel : INotifyPropertyChanged
  {
    public string inputError;
    public string InputError
    {
      get
      {
        return inputError;
      }
      set
      {
        if (inputError == value)
        {
          return;
        }
        inputError = value;
        this.NotifyPropertyChanged("InputError");
      }
    }

    public AxisX X { get; set; }
    public AxisY Y { get; set; }
    public ObservableCollection<SamplePath> SamplePaths { get; set; }

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
  }
}