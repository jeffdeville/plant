namespace Plant.Tests.TestModels
{
  public class Book
  {
    public string Author { get; private set; }
    public string Publisher { get; private set; }

    public Book(string author, string publisher)
    {
      Author = author;
      Publisher = publisher;
    }
  }
}