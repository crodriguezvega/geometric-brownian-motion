using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeometricBrownianMotion
{
  public class Points : List<Point>
  {
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      this.ForEach(p => sb.Append(p.ToString() + " "));
      return sb.ToString();
    }
  }
}