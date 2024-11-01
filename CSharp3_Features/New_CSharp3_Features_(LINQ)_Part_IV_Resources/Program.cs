using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

// These examples show:
// - The usage of anonymous types in C#.
namespace AnonymousTypes
{
    public class Program
    {
        #region Types and Methods for UsageOfAnonymousTypes():
        // Our toy: class Person with some properties and ctors.
        public class Person : IComparable<Person>, IEquatable<Person>
        {
            // Dctor.
            public Person()
            { }


            // Another ctor awaiting a string (the name).
            public Person(string name)
            {
                Name = name;
            }


            public int Age { get; set; }


            public string Name { get; set; }


            public string Country { get; set; }


            public override string ToString()
            {

                return string.Format("Name: {0}, Age: {1}, Country: {2}", Name, Age, Country);
            }


            public int CompareTo(Person other)
            {
                return Age.CompareTo(other.Age);
            }


            public override int GetHashCode()
            {
                return Age.GetHashCode() ^
                        (null != Name
                            ? Name.GetHashCode()
                            : 0);
            }


            public override bool Equals(object obj)
            {
                return null != obj && GetType().Equals(obj.GetType()) && Equals((Person)obj);
            }


            public bool Equals(Person other)
            {
                return null != other && Age.Equals(other.Age) && object.Equals(Name, other.Name);
            }
        }


        /// <summary>
        /// This method can also deal with anonymous types. These types have no visible names, but
        /// their compiler generated type can be inferred by the method call, it is accessible via
        /// type parameter T.
        /// </summary>
        /// <typeparam name="T">Input and output type of the transformation.</typeparam>
        /// <param name="element">The element to be transformed.</param>
        /// <param name="transformFunc">The function performing the transformation.</param>
        /// <returns>The transformed result.</returns>
        private static T Transform<T>(T element, Func<T, T> transformFunc)
        {
            // Of course we can not call any properties on the object element, because we have no
            // constraints defined on the type argument T. In the case of anonymous types we
            // could't even formulate a constraint.
            return transformFunc(element);
        } 
        #endregion


        private static void UsageOfAnonymousTypes()
        {
            /*-----------------------------------------------------------------------------------*/
            // Simple Operations on Anonymous Types:

            // These uses are _not_ the typical uses for anonymous types. Please understand the 
            // examples just as a presentation of anonymous types' features.

            // The properties of anonymous types are implicitly typed. If you initialize X with an
            // int literal, then the type of X is inferred to be int. Instances of anonymous types
            // itself must be handled by automatically type-inferred variables, because there
            // exists no type name.
            var aPoint = new { X = 5, Y = 67 };
            // Invalid! Properties of anonymous types mustn't be explicitly typed!
            //var floatyPoint = new { double X = 5, double Y = 67 }; 

            // As the properties of an anonymous type are public, you can access the properties to
            // read their values. But you can not write their properties after the initialization.
            int xFromaPoint = aPoint.X;
            // Invalid! Anonymous types are immutable!
            //aPoint.X = 34; 

            // Sidebar: The syntax is very similar to JavaScript's object literals.

            // Sidebar: After assigning the last item of the anonymous type, you are allowed to
            // leave a dangling comma (you can also leave it on arrays).

            // Rereferring to an anonymous type's properties while initializing other properties
            // in Invalid as well.
            // Invalid!
            //var aPoint2 = new { X = 17, Y = X * 2 }; 

            // Anonymous types can be handled by generic methods as well, the anonymous type is
            // automatically inferred, a type name is never required. As can be seen we pass an
            // anonymous function as second argument, which reads the properties on the passed
            // anonymous type's instance. - There is no other way to do it, e.g. the implementation
            // of Transform could not call any members on the passed object, as it can know
            // anything about its interface (see definition of Transform).
            var anotherPoint = Transform(aPoint, p => new { X = p.X * 2, Y = p.Y * 2 });

            // For two anonymous types to be considered the same type, the property names and types
            // must match, and the properties must be in the same order. This is called "structural
            // equality" and is very handy for immutable types. The following two declarations
            // produce two different anonymous types at compile time:
            var aPoint2 = new { X = 5, Y = 67 };
            var anotherPoint2 = new { Y = 12, X = 16 };
            // Notice that in order to create arrays of anonymous types, structural equality is
            // absolutely necessary! Arrays of anonymously typed objects must be _implicitly_ typed
            // (or object-typed) arrays: 
            var arrayOfPoints = new[]
                                 {
                                    new { X = 5, Y = 67 }, 
                                    new { X = 12, Y = 16 }
                                 };
            // Due to the structural equality of anonymous types, they can also be used as results
            // of the conditional operator:
            var whichPoint = 
                0 == DateTime.Now.Second % 2
                    ? new { X = 5, Y = 67 }
                    : new { X = 12, Y = 16 };

            // Invalid! Notice that you can not define a generic collection of anonymous types'
            // instances, as you can not provide a type-argument:
            //var listOfPoints = new List<?>
            //                     {
            //                        new { X = 5, Y = 67 }, 
            //                        new { X = 12, Y = 16 }
            //                     };

            // The automatically created types are reference types, but the compiler generates an
            // Equals() and a GetHashCode() method that implement _value type_ semantics.
            var p1 = new { X = 23, Y = 67 };
            var p2 = new { X = 23, Y = 67 };
            // Value semantics means that these assertions hold true:
            Debug.Assert(p1.Equals(p2));
            Debug.Assert(p1.GetHashCode() == p2.GetHashCode());
            // But the == operator is _not_ automatically defined, so the references will be
            // compared!
            Debug.Assert(p1 != p2);

            // Here the value semantics with userdefined types come into play: 
            var pair1 = new
            {
                Person1 = new Person("Ann") { Age = 45 },
                Person2 = new Person("Tom") { Age = 47 }
            };
            var pair2 = new
            {
                Person1 = new Person("Ann") { Age = 45 },
                Person2 = new Person("Tom") { Age = 47 }
            };
            // This will call Equals(Person) on each Person property (not Equals(object)!), because
            // the type Person implements IEquateable<T>.
            Debug.Assert(pair1.Equals(pair2));
            // This will call GetHashCode() on each Person property and will (somehow) calculate an
            // effective hashcode for the objects of anonymous type.
            Debug.Assert(pair1.GetHashCode() == pair2.GetHashCode());

            // Anonymous types provide a simple override of ToString(), which dumps the list of
            // property names and their values:
            var p3 = new { X = 15, Y = 20 };
            Debug.WriteLine(p3);
            // Will print the string "{ X = 15, Y = 20 }" to the debug console.

            // Anonymous types can also be cascaded. The property Point is itself automatically
            // inferred to be of anonymous type. - Notice how this notation resembles JSON
            // (JavaScript Object Notation) known from JavaScript.
            var distanceAndPoint = new
            {
                Distance = 34,
                Point = new
                {
                    x = 45,
                    y = 43
                }
            };

            // You can also initialize Delegate members within anonymous type, e.g. with anonymous
            // functions:
            var withDelegateInstance = new
            {
                ADelegate = (Action<int>)delegate(int x) { Debug.WriteLine(x); }
            };
            withDelegateInstance.ADelegate(4);


            /*-----------------------------------------------------------------------------------*/
            // Usage of Anonymous Types in Results of LINQ Queries as Custom Projection:

            // These uses are the typical uses for anonymous types. I.e. anonymous types have been
            // introduced for LINQ into the C# language. 

            // Anonymous types can be used as item type of result sequences. This is handy, when
            // the result of a query is a new projection.
            var persons = new[] // The array persons is the data source.
            {
                new Person("Claire"){Age = 32, Country="Netherlands"},
                new Person("Marina"){Age = 25, Country="Italy"},
                new Person("Nico"){Age = 32, Country="Germany"},
                new Person("Roberta"){Age = 23, Country="USA"}
            };

            // Here we do only create a new projection over all persons by selecting just the Age
            // and the Name property, this is called custom projection. So {Age, Name} will make
            // up a new type 'a and the result will be of type IEnumerable<'a>.
            var results =
                from person in persons
                select new { Age = person.Age, Name = person.Name, Country = person.Country };
            // As can be seen anonymous types do somehow "infect" the subsequent code they’re used
            // in, because you have to use implicit typing all over. Here the type of item is
            // anonymous:
            foreach (var item in results)
            {
                Console.WriteLine(item);
            }

            // There exists a further simplification - the projection initializer. The explicit
            // naming and assignment of the properties with the initializer syntax in the anonymous
            // type can be left away, then their names default (read "do project") to the names of
            // the source properties. Interestingly the compiler treats both types identically, so
            // the type of results is compatible to this result, though.
            results = from person in persons
                      select new { person.Age, person.Name, person.Country };
            foreach (var item in results)
            {
                Console.WriteLine(item);
            }

            // Esp. the usage of anonymous types to influence the projection is interesting. For
            // example you can simply _hide_ information from a projection:
            var onlyAgesAndNames =
                from person in persons
                // Mind, that the Country will _not_ be selected:
                select new { Age = person.Age, Name = person.Name };
            foreach (var item in onlyAgesAndNames)
            {
                Console.WriteLine(item);
            }


            // It is also possible to introduce _new_ data into the projection that was not present
            // in the original data:
            var theAgesByTwo =
                from person in persons
                // Mind, that the property AgeByTwo is calculated in the projection, it is _not_
                // present in the original data:
                select new { AgeByTwo = person.Age * 2 };
            foreach (var item in theAgesByTwo)
            {
                Console.WriteLine(item);
            }
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Test Methods:

            UsageOfAnonymousTypes();
        }
    }
}