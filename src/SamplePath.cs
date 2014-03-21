using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GeometricBrownianMotion
{
  public class SamplePath : INotifyPropertyChanged
  {
    public Points WorldPoints;
    public Points CanvasPoints;
    public Brush Stroke { get; set; }

    private string path;
    public string Path
    {
      get
      {
        return path;
      }
      set
      {
        if (path == value)
        {
          return;
        }
        path = value;
        this.NotifyPropertyChanged("Path");
      }
    }

    public SamplePath()
    {
      WorldPoints = new Points();
      CanvasPoints = new Points();
      Stroke = Brushes.Black;
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