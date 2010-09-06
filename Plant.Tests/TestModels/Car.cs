namespace Plant.Tests.TestModels
{
  public class Car
  {
    public string Make { get; private set; }

    public Car(string make)
    {
      Make = make;
    }
  }
}