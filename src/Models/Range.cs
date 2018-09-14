using System.ComponentModel;

namespace GeometricBrownianMotion.Models
{
  public class Range : INotifyPropertyChanged
  {
    private double _min;
    public double Min
    {
      get => _min;
      set
      {
        _min = value;
        this.NotifyPropertyChanged("Min");
      }
    }

    public double _max;
    public double Max
    {
      get => _max;
      set
      {
        _max = value;
        this.NotifyPropertyChanged("Max");
      }
    }

    public Range(double min, double max)
    {
      Min = min;
      Max = max;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
  }
}