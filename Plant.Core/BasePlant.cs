using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blueprints = System.Collections.Generic.Dictionary<System.Type, object>;

namespace Plant.Core
{
  public class BasePlant
  {
    private readonly Blueprints propertyBlueprints = new Blueprints();
    private readonly Blueprints constructorBlueprints = new Blueprints();

    public BasePlant()
    {
      
    }

    public virtual T Create<T>()
    {
      var instance = constructorBlueprints.ContainsKey(typeof(T)) ? CreateInstanceWithDefaults<T>() : CreateInstanceWithEmptyConstructor<T>();


      if(propertyBlueprints.ContainsKey(typeof(T)))
        SetProperties(propertyBlueprints[typeof(T)], instance);
      return instance;
    }

    private static T CreateInstanceWithEmptyConstructor<T>()
    {
      return Activator.CreateInstance<T>();
    }

    private T CreateInstanceWithDefaults<T>()
    {
      var type = typeof (T);
      var constructor = type.GetConstructors().First();
      var paramNames = constructor.GetParameters().Select(p => p.Name.ToLower()).ToList();
      var props = GetProps(type);

      return (T)constructor.Invoke(props.
        OrderBy(prop => paramNames.IndexOf(prop.Item1)).
        Select(prop => prop.Item2).ToArray());
    }

    private IEnumerable<Tuple<string, object>> GetProps(Type type)
    {
      var defaults = constructorBlueprints[type];
      return defaults.GetType().GetProperties().Select(prop => new Tuple<string,object>(prop.Name.ToLower(), prop.GetValue(defaults,null)));
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
                                      if (typeof(ILazyProperty).IsAssignableFrom(value.GetType()))
                                        AssignLazyPropertyResult(instance, instanceProperty, value);
                                      else
                                        instanceProperty.SetValue(instance, value, null);
                                    });
    }

    private static void AssignLazyPropertyResult<T>(T instance, PropertyInfo instanceProperty, object value)
    {
      var lazyProperty = (ILazyProperty)value;
      
      if(lazyProperty.Func.Method.ReturnType != instanceProperty.PropertyType)
        throw new LazyPropertyHasWrongTypeException(string.Format("Cannot assign type {0} to property {1} of type {2}", 
          lazyProperty.Func.Method.ReturnType, 
          instanceProperty.Name, 
          instanceProperty.PropertyType));

      instanceProperty.SetValue(instance, lazyProperty.Func.DynamicInvoke(), null);
    }

    public virtual void DefinePropertiesOf<T>(object defaults)
    {
      propertyBlueprints.Add(typeof(T), defaults);
    }

    public void DefineConstructionOf<T>(object defaults)
    {
      constructorBlueprints.Add(typeof (T), defaults);
    }

    public BasePlant WithBlueprintsFromAssemblyOf<T>()
    {
      var assembly = typeof (T).Assembly;
      var blueprintTypes = assembly.GetTypes().Where(t => typeof (Blueprint).IsAssignableFrom(t));
      blueprintTypes.ToList().ForEach(blueprintType =>
                                    {
                                      var blueprint = (Blueprint)Activator.CreateInstance(blueprintType);
                                      blueprint.SetupPlant(this);
                                    });
      return this;

    }

  }
}
