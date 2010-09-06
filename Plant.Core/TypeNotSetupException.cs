using System;

namespace Plant.Core
{
  public class TypeNotSetupException : Exception
  {
    public TypeNotSetupException(string message) : base(message)
    {
    }
  }
}