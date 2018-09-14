namespace GeometricBrownianMotion.Models
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