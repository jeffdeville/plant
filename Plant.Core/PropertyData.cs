using System;
using System.Reflection;

namespace Plant.Core
{
  public class PropertyData
  {
    public string Name { get; private set; }
    private readonly Type type;

    public PropertyData(PropertyInfo propertyInfo)
    {
      Name = propertyInfo.Name;
      type = propertyInfo.PropertyType;
    }

    public override bool Equals(object other)
    {
      if (other is PropertyData)
        return Equals((PropertyData) other);
      return false;
    }

    public bool Equals(PropertyData other)
    {
      if (other == null) return false;
      return Name.Equals(other.Name) && type.Equals(other.type);
    }

    public override int GetHashCode()
    {
      return type.GetHashCode() + Name.GetHashCode();
    }
  }
}