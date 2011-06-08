using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Properties = System.Collections.Generic.IDictionary<Plant.Core.PropertyData, object>;
using Blueprints = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.IDictionary<Plant.Core.PropertyData, object>>;


namespace Plant.Core
{
  public class BasePlant
  {
    private readonly Blueprints propertyBlueprints = new Blueprints();
    private readonly Blueprints constructorBlueprints = new Blueprints();
    private readonly IDictionary<Type, CreationStrategy> creationStrategies = new Dictionary<Type, CreationStrategy>();
    private readonly IDictionary<Type, Action<object>> postBuildActions = new Dictionary<Type, Action<object>>();

    private T CreateViaProperties<T>(Properties userProperties)
    {
      var instance = CreateInstanceWithEmptyConstructor<T>();
      SetProperties(Merge(propertyBlueprints[typeof (T)], userProperties), instance);
      return instance;
    }

    private T CreateViaConstructor<T>(Properties userProperties)
    {
      var type = typeof(T);
      var constructor = type.GetConstructors().First();
      var paramNames = constructor.GetParameters().Select(p => p.Name.ToLower()).ToList();
      var defaultProperties = constructorBlueprints[type];

      var props = Merge(defaultProperties, userProperties);

      return
        (T)
        constructor.Invoke(
          props.Keys.OrderBy(prop => paramNames.IndexOf(prop.Name.ToLower())).
          Select(prop => props[prop]).ToArray());
    }

    private Properties Merge(Properties defaults, Properties overrides)
    {
      return defaults.Keys.Union(overrides.Keys).ToDictionary(key => key,
                            key => overrides.ContainsKey(key) ? overrides[key] : defaults[key]);
    }

    private static T CreateInstanceWithEmptyConstructor<T>()
    {
      return Activator.CreateInstance<T>();
    }

    public virtual T Create<T>(object userSpecifiedProperties = null)
    {
      var userSpecifiedPropertyList = ToPropertyList(userSpecifiedProperties);

      T constructedObject = default(T);
      if(StrategyFor<T>() == CreationStrategy.Constructor)
        constructedObject = CreateViaConstructor<T>(userSpecifiedPropertyList);
      else
        constructedObject = CreateViaProperties<T>(userSpecifiedPropertyList);
      
      if (postBuildActions.ContainsKey(typeof(T)))
        postBuildActions[typeof (T)](constructedObject);
        
      return constructedObject;
    }

    private CreationStrategy StrategyFor<T>()
    {
      if(creationStrategies.ContainsKey(typeof(T)))
        return creationStrategies[typeof (T)];
      throw new TypeNotSetupException(string.Format("No creation strategy defined for type: {0}", typeof(T)));

    }

    private static void SetProperties<T>(Properties properties, T instance)
    {
      properties.Keys.ToList().ForEach(property =>
                                    {
                                      var instanceProperty = instance.GetType().GetProperties().FirstOrDefault(prop => prop.Name == property.Name);
                                      if (instanceProperty == null) throw new PropertyNotFoundException(property.Name, properties[property]);

                                      var value = properties[property];
                                      if (typeof(ILazyProperty).IsAssignableFrom(value.GetType()))
                                        AssignLazyPropertyResult(instance, instanceProperty, value);
                                      else
                                        instanceProperty.SetValue(instance, value, null);
                                    });
    }

    private static void AssignLazyPropertyResult<T>(T instance, PropertyInfo instanceProperty, object value)
    {
      var lazyProperty = (ILazyProperty)value;

      if (lazyProperty.Func.Method.ReturnType != instanceProperty.PropertyType)
        throw new LazyPropertyHasWrongTypeException(string.Format("Cannot assign type {0} to property {1} of type {2}",
          lazyProperty.Func.Method.ReturnType,
          instanceProperty.Name,
          instanceProperty.PropertyType));
        // I can pass in the instance as a parameter to this function, but only if I'm using property-setters
      instanceProperty.SetValue(instance, lazyProperty.Func.DynamicInvoke(), null);
    }

      /// <summary>
      /// The post-build method will accept the object that was just constructed as a parameter, so that you
      /// can assign other values to it. Unfortunately, you have to do the casting yourself, for now.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="defaults"></param>
      /// <param name="afterPropertyPopulation"></param>
    public virtual void DefinePropertiesOf<T>(object defaults, Action<object> afterPropertyPopulation)
    {
        DefinePropertiesOf<T>(defaults);
        postBuildActions[typeof (T)] = afterPropertyPopulation;
    }

    public virtual void DefinePropertiesOf<T>(object defaults)
    {
        creationStrategies.Add(typeof(T), CreationStrategy.Property);
        AddDefaultsTo<T>(propertyBlueprints, defaults);
    }

    public void DefineConstructionOf<T>(object defaults)
    {
      creationStrategies.Add(typeof(T), CreationStrategy.Constructor);
      AddDefaultsTo<T>(constructorBlueprints, defaults);
    }

    private void AddDefaultsTo<T>(Blueprints blueprints, object defaults)
    {
      blueprints.Add(typeof(T), ToPropertyList(defaults));
    }

    private IDictionary<PropertyData, object> ToPropertyList(object obj)
    {
      if(obj == null) return new Dictionary<PropertyData, object>();
      return obj.GetType().GetProperties().ToDictionary(prop => new PropertyData(prop), prop => prop.GetValue(obj, null));
    }

    public BasePlant WithBlueprintsFromAssemblyOf<T>()
    {
      var assembly = typeof(T).Assembly;
      var blueprintTypes = assembly.GetTypes().Where(t => typeof(Blueprint).IsAssignableFrom(t));
      blueprintTypes.ToList().ForEach(blueprintType =>
                                    {
                                      var blueprint = (Blueprint)Activator.CreateInstance(blueprintType);
                                      blueprint.SetupPlant(this);
                                    });
      return this;

    }

  }
}
