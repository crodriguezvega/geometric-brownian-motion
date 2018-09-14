using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;

namespace GeometricBrownianMotion.Models
{
  public class Points : List<Point>
  {
    public override string ToString()
    {
      var sb = new StringBuilder();
      this.ForEach(p => sb.Append(p.ToString(CultureInfo.InvariantCulture) + " "));
      return sb.ToString();
    }
  }
}