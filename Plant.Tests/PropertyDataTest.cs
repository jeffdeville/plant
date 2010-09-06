using NUnit.Framework;
using Plant.Core;

namespace Plant.Tests
{
  [TestFixture]
  public class PropertyDataTest
  {
    [Test]
    public void Should_Be_Equal_To_Same_PropertyData_Object()
    {
      var propInfo = new { Property1 = 3 }.GetType().GetProperties()[0];
      var propData = new PropertyData(propInfo);
      Assert.IsTrue(propData.Equals(propData));
    }

    [Test]
    public void Should_Return_False_For_PropertyInfo_Objects_With_Different_Names()
    {
      var type = new { Prop1 = 10, Prop2 = 20 }.GetType();
      var propData1 = new PropertyData(type.GetProperties()[0]);
      var propData2 = new PropertyData(type.GetProperties()[1]);
      Assert.IsFalse(propData1.Equals(propData2));
    }

    [Test]
    public void Should_Return_True_For_PropertyInfo_Objects_With_Same_Name_And_Type()
    {
      var propData1 = new PropertyData(new {Property1 = 1}.GetType().GetProperties()[0]);
      var propData2 = new PropertyData(new {Property1 = 3, Property2 = 10}.GetType().GetProperties()[0]);
      Assert.IsTrue(propData1.Equals(propData2));
    }

    [Test]
    public void Should_Return_False_For_PropertyInfo_Objects_With_Different_Types()
    {
      var prop1 = new PropertyData(new {Property1 = 3}.GetType().GetProperties()[0]);
      var prop2 = new PropertyData(new {Property1 = "foo"}.GetType().GetProperties()[0]);
      Assert.IsFalse(prop1.Equals(prop2));
    }

    [Test]
    public void Should_Return_False_When_Comparing_A_NonNull_With_A_Null_Object()
    {
      var propInfo = new PropertyData(new {Property1 = 3}.GetType().GetProperties()[0]);
      Assert.IsFalse(propInfo.Equals(null));
    }

    [Test]
    public void Should_Return_Different_Hashcode_For_Properties_Of_Different_Types()
    {
      var prop1 = new PropertyData(new { Property1 = 3 }.GetType().GetProperties()[0]);
      var prop2 = new PropertyData(new { Property1 = "foo" }.GetType().GetProperties()[0]);
      Assert.AreNotEqual(prop1.GetHashCode(), prop2.GetHashCode());
    }

    [Test]
    public void Should_Return_Same_Hashcode_For_Same_PropertyInfo_Objects()
    {
      var propInfo = new PropertyData(new {Property1 = 3}.GetType().GetProperties()[0]);
      Assert.AreEqual(propInfo.GetHashCode(), propInfo.GetHashCode());
    }

    [Test]
    public void Should_Return_Same_Hashcode_For_PropertyInfo_Objects_With_Same_Name_And_Type()
    {
      var prop1 = new PropertyData(new {Property1 = 3}.GetType().GetProperties()[0]);
      var prop2 = new PropertyData(new { Property1 = 3, Property2 = 4 }.GetType().GetProperties()[0]); //C# compiler likes to be clever about anonymous types
      Assert.AreEqual(prop1.GetHashCode(), prop2.GetHashCode());
    }

    [Test]
    public void Should_Return_Different_Hashcode_For_PropertyInfo_Objects_With_Different_Names()
    {
      var prop1 = new PropertyData(new {Property1 = 3}.GetType().GetProperties()[0]);
      var prop2 = new PropertyData(new {Property2 = 3}.GetType().GetProperties()[0]);
      Assert.AreNotEqual(prop1.GetHashCode(), prop2.GetHashCode());
    }
  }
}