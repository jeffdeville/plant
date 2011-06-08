namespace Plant.Tests.TestModels
{
  public class House
  {
    public readonly string Color;
    public readonly int SquareFoot;
    public string Summary { get; set; }
    public House(string color, int squareFoot)
    {
      Color = color;
      SquareFoot = squareFoot;
    }
  }
}