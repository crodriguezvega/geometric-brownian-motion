using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GeometricBrownianMotion
{
  public class Range : INotifyPropertyChanged
  {
    private double min;
    public double Min
    {
      get
      {
        return min;
      }
      set
      {
        min = value;
        this.NotifyPropertyChanged("Min");
      }
    }

    public double max;
    public double Max
    {
      get
      {
        return max;
      }
      set
      {
        max = value;
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
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
      }
    }
  }
}