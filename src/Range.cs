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
    public double min;
    public double Min
    {
      get
      {
        return min;
      }
      set
      {
        if (min == value)
        {
          return;
        }
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
        if (max == value)
        {
          return;
        }
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