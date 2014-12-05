using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometricBrownianMotion
{
  public class AxisY
  {
    public Range Range { get; set; }

    public AxisY(double min, double max)
    {
      Range = new Range(min, max);
    }
  }
}