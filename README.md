Plant
=====

Plant is a test factory for .NET 4.0.  It is very much like FactoryGirl (http://github.com/thoughtbot/factory_girl/) for Ruby.  The goal of this project is to allow you to reduce noise and duplication across your tests when creating models.  

Download
--------

Github: http://github.com/jbrechtel/plant
Nu:  (SOON!)


Features
--------

Currently Plant supports

* Object creation via setting properties
* Object creation Specifying constructor arguments
* Overriding default property and constructor argument values
* Modular object definition
* Lazyily evauated property and constructor argument values

Targetted features can be found in the issues list for the project.  Some specific ones are

* Allowing multiple different definitions for one object
* Sequenced properties
* Specify associated instances
* Allow user to specify after-create actions on models (to save the model to the DB after creation, for instance)

Terms
-----

A 'Plant' is the thing that creates your objects for you.  
A 'Blueprint' is what a user provides to tell a plant how to create an object.

Defining a Blueprint
--------------------

A Blueprint is just a class that implements the Blueprint interface.  The interface defines one method, SetupPlant, which takes a BasePlant object.  SetupPlant is a generic method which whose generic argument is the Type that you're setting up and an anonymous object with the appropriate properties.

Note that currently property validation occurs during object creation, not DefinePropertiesOf.

    class PersonBlueprint : Blueprint
    {
      public void SetupPlant(BasePlant plant)
      {
        plant.DefinePropertiesOf<Person>(new
                               {
                                  FirstName = "Barbara",
                                  MiddleName = "Elaine",
                                  LastName = "Brechtel",
                                  Address = "111 South Main St.",
                                  City = "Gulfport",
                                  State = "MS",
                                  EmailAddress = "barbara@brechtel.com"s
                               });
      }
    }
  
Use DefineConstructionOf instead of DefinePropertiesOf when an object should be created via constructor arguments.

    class PersonBlueprint : Blueprint
    {
      public void SetupPlant(BasePlant plant)
      {
        plant.DefineConstructionOf<Person>(new
                               {
                                  FirstName = "Barbara",
                                  MiddleName = "Elaine",
                                  LastName = "Brechtel",
                                  Address = "111 South Main St.",
                                  City = "Gulfport",
                                  State = "MS",
                                  EmailAddress = "barbara@brechtel.com"s
                               });
      }
    }

Lazily evaluated properties
---------------------------

To define a Blueprint with a lazily evaluated property, set the value to new LazyProperty<TPropertyType>(lambda) like so:

    class PersonBlueprint : Blueprint
    {
      public void SetupPlant(BasePlant plant)
      {
        plant.DefinePropertiesOf<Person>(new
                               {
                                  UniqueID = new LazyProperty<int>(() => IDGenerator.GenerateNewID())
                               });
      }
    }
  
where TPropertyType (int in this case) is the type of the property and also that returned from the lambda.
  
Usage
-----

To create a new Plant, you'll typically want to tell it which Assembly to look in for Blueprints.  You can do this via

    var plant = new BasePlant().WithBlueprintsFromAssemblyOf<PersonBlueprint>();
  
where PersonBlueprint is one of the Blueprints you have defined.  Plant will then load blueprints from any other type that implements the Blueprint interface in that assembly.

To retrieve the default instance of an object

    var person = plant.Create<Person>();
  
To retrieve an instance of a person with specific parts of the default blueprint overridden

    var person = plant.Create<Person>(new { EmailAddress = "john@doe.com" });
  
Multiple properties can be overridden in one call

    var person = plant.Create<Person>(new { EmailAddress = "john@doe.com", State = "GA" });