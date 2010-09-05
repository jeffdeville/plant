using System;
using System.Collections.Generic;
using System.Linq;

namespace Plant.Core
{
  public class BasePlant
  {
    private readonly IDictionary<Type, object> blueprints = new Dictionary<Type, object>();

    protected BasePlant()
    {
      Setup();
    }

    public virtual T Create<T>() where T : new()
    {
      var instance = new T();
      if(blueprints.ContainsKey(typeof(T)))
        SetProperties(blueprints[typeof(T)], instance);
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

    protected virtual void CreateBlueprint<T>(object defaults)
    {
      blueprints.Add(typeof(T), defaults);
    }

    public virtual void Setup()
    {
      
    }
  }
}
