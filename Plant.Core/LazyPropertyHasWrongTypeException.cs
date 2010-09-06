using System;

namespace Plant.Core
{
  public class LazyPropertyHasWrongTypeException : Exception
  {
    public LazyPropertyHasWrongTypeException(string message) : base(message)
    {
    }
  }
}