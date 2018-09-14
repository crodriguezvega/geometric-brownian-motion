using System.Windows.Media;
using System.ComponentModel;

namespace GeometricBrownianMotion.Models
{
  public class SamplePath : INotifyPropertyChanged
  {
    public Points WorldPoints;
    public Points CanvasPoints;
    public Brush Stroke { get; set; }

    private string _path;
    public string Path
    {
      get => _path;
      set
      {
        if (_path == value)
        {
          return;
        }
        _path = value;
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
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
  }
}