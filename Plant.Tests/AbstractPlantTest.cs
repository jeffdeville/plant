using NUnit.Framework;
using Plant.Core;
using Plant.Tests.TestModels;

namespace Plant.Tests
{
  [TestFixture]
  public class AbstractPlantTest
  {
    [Test]
    public void Should_Create_Instance_Of_Specified_Type()
    {
      Assert.IsInstanceOf(typeof(Person), new TestPlant().Create<Person>());
    }

    [Test]
    public void Should_Create_Instance_With_Requested_Properties()
    {
      Assert.AreEqual("James", new TestPlant().Create<Person>(new { FirstName = "James"}).FirstName);
    }

    [Test]
    public void Should_Use_Default_Instance_Values()
    {
      //this test relies on the setup of TestPlant below
      Assert.AreEqual("Barbara", new TestPlant().Create<Person>().FirstName);
    }

    [Test]
    [ExpectedException(typeof(PropertyNotFoundException))]
    public void Should_Throw_PropertyNotFound_Exception_When_Given_Invalid_Property()
    {
      new TestPlant().Create<Person>(new { Foo = "Nothing" });
    }

    [Test]
    public void Should_Setup_Type_Without_Defaults()
    {
      Assert.AreEqual("Toyota", new TestPlant().Create<Car>(new { Make = "Toyota"}).Make);
    }

  }

  class TestPlant : AbstractPlant
  {
    public override void Setup()
    {
      Seed<Person>(new 
                     {
                       FirstName = "Barbara"
                     });
    }
  }


}
