using System;
using System.Collections.Generic;
using System.Linq;

namespace Plant.Core
{
  public abstract class AbstractPlant
  {
    private readonly IDictionary<Type, object> defaultValuesByType = new Dictionary<Type, object>();

    protected AbstractPlant()
    {
      Setup();
    }

    public virtual T Create<T>() where T : new()
    {
      var instance = new T();
      if(defaultValuesByType.ContainsKey(typeof(T)))
        SetProperties(defaultValuesByType[typeof(T)], instance);
      return instance;
    }

    public virtual T Create<T>(object userSpecifiedProperties) where T : new()
    {
      var instance = Create<T>();
      SetProperties(userSpecifiedProperties, instance);
      return instance;
    }

    private static void SetProperties<T>(object propertyValues, T instance)
    {
      var properties = propertyValues.GetType().GetProperties().ToList();
      properties.ForEach(property =>
                                    {
                                      var instanceProperty = instance.GetType().GetProperties().FirstOrDefault(prop => prop.Name == property.Name);
                                      if(instanceProperty == null) throw new PropertyNotFoundException();
                                      var value = property.GetValue(propertyValues, null);
                                      instanceProperty.SetValue(instance, value, null);
                                    });
    }

    protected virtual void Seed<T>(object defaults)
    {
      defaultValuesByType.Add(typeof(T), defaults);
    }

    public abstract void Setup();
  }
}
