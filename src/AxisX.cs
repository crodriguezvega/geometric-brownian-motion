using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometricBrownianMotion
{
  public class AxisX
  {
    public Range Range { get; set; }

    public AxisX(double min, double max)
    {
      Range = new Range(min, max);
    }
  }
}