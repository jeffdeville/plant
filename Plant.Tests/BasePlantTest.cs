using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Plant.Core;
using Plant.Tests.TestBlueprints;
using Plant.Tests.TestModels;

namespace Plant.Tests
{
  [TestFixture]
  public class BasePlantTest
  {
    [Test]
    public void Should_Create_Instance_Of_Specified_Type()
    {
      Assert.IsInstanceOf(typeof(Person), new BasePlant().Create<Person>());
    }

    [Test]
    public void Should_Create_Instance_With_Requested_Properties()
    {
      Assert.AreEqual("James", new BasePlant().Create<Person>(new { FirstName = "James" }).FirstName);
    }

    [Test]
    public void Should_Use_Default_Instance_Values()
    {
      var testPlant = new BasePlant();
      testPlant.Define<Person>(new { FirstName = "Barbara" });
      Assert.AreEqual("Barbara", testPlant.Create<Person>().FirstName);
    }

    [Test]
    [ExpectedException(typeof(PropertyNotFoundException))]
    public void Should_Throw_PropertyNotFound_Exception_When_Given_Invalid_Property()
    {
      new BasePlant().Create<Person>(new { Foo = "Nothing" });
    }

    [Test]
    public void Should_Setup_Type_Without_Defaults()
    {
      Assert.AreEqual("Toyota", new BasePlant().Create<Car>(new { Make = "Toyota" }).Make);
    }

    [Test]
    public void Should_Load_Blueprints_From_Assembly()
    {
      var plant = new BasePlant().WithBlueprintsFromAssemblyOf<TestBlueprint>();
      Assert.AreEqual("Elaine", plant.Create<Person>().MiddleName);
    }

    [Test]
    public void Should_Lazily_Evaluate_Delegate_Properties()
    {
      var plant = new BasePlant();
      string lazyMiddleName = null;
      plant.Define<Person>(new
                             {
                               MiddleName = new LazyProperty<string>(() => lazyMiddleName)
                             });

      Assert.AreEqual(null, plant.Create<Person>().MiddleName);
      lazyMiddleName = "Johnny";
      Assert.AreEqual("Johnny", plant.Create<Person>().MiddleName);
    }

    [Test]
    [ExpectedException(typeof(LazyPropertyHasWrongTypeException))]
    public void Should_Give_Reasonable_Exception_When_Lazy_Property_Definition_Returns_Wrong_Type()
    {
      var plant = new BasePlant();
      plant.Define<Person>(new
      {
        MiddleName = new LazyProperty<int>(() => 5)
      });

      plant.Create<Person>();
    }
  }
  namespace TestBlueprints
  {
    class TestBlueprint : Blueprint
    {
      public void SetupPlant(BasePlant plant)
      {
        plant.Define<Person>(new
                               {
                                 MiddleName = "Elaine"
                               });
      }
    }
  }
}

