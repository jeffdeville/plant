namespace Plant.Tests.TestModels
{
  public class WriteOnceMemoryModule
  {
    private int? _value = null;
    public int Value
    {
      get { return _value.GetValueOrDefault(); } 
      set 
      {
        if (_value == null) _value = value;
      }
    }
  }
}