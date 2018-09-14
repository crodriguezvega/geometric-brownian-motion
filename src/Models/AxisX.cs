namespace GeometricBrownianMotion.Models
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