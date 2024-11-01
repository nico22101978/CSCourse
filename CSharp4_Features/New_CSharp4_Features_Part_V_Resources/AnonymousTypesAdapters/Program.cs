using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Reflection;


// These examples show:
// - How dynamic enables passing/transporting of anonymous types' instances.
// - How dynamic enables the usage of operators in generic types.
// - How dynamic simplifies working with .Net remoting.
namespace DynamicEnabler
{
    #region Types for DynamicDispatchAndRemoting():
    /// <summary>
    /// Why didn't we use automatically implemented properties here? Well, serialization depends on
    /// the names of the fields of a type getting serialized (this is esp. true for the
    /// BinaryFormatter, which is heavily used for .Net remoting). And for automatically
    /// implemented properties the name of the field may change when the type is modified. So an
    /// instance's data being serialized in past may no longer deserialize successfully later on,
    /// if the type has been modified in the meantime. Put simple: don't use automatically
    /// implemented properties in serializable types!
    /// </summary>
    [Serializable]
    public class TT
    {
        public TT()
        {
            AnInt = 42;
            AString = "Hi";
        }

        private int _anInt;
        public int AnInt
        {
            get { return _anInt; }
            set { _anInt = value; }
        }


        private string _aString;
        public string AString
        {
            get { return _aString; }
            set { _aString = value; }
        }
    }
    #endregion


    public class Program
    {
        #region Types and Methods for AnonymousTypeAdapters():
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


            /// <summary>
            /// The US Postal Service Code (USPS) of the state.
            /// </summary>
            public string State { get; set; }


            public string Company { get; set; }


            public int Age { get; set; }


            public string Name { get; set; }


            public IEnumerable<Person> Friends { get; set; }


            public override string ToString()
            {
                return string.Format("Name: {0}, Age: {1}, State: {2}", Name, Age, State);
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
                return Equals(obj as Person);
            }


            public bool Equals(Person other)
            {
                return null != other
                            ? Age.Equals(other.Age) && object.Equals(Name, other.Name)
                            : false;
            }
        }


        public class Product
        {
            public string Name { get; set; }


            public decimal Price { get; set; }
        }


        /// <summary>
        /// Prints the passed item to console.
        /// </summary>
        /// <param name="item">An item having the readable properties or fields Name and Price.
        /// </param>
        public static void PrintItem(dynamic item)
        {
            // Ok! The property-looking-items Name and Price will be dynamically dispatched. A
            // RuntimeBinderException will be thrown, if the run time type does not support any of
            // these "properties".
            Console.WriteLine("Name: {0}, Price: {1}", item.Name, item.Price);
        }


        #region Alternatives to Dynamic:
        // The name of this method is different from the others to avoid unwanted overloading
        // resolutions. 
        public static void PrintItemO(object item)
        {
            // Basically the same as PrintItem<T>(T).

            // We need to delegate it to ToString() (or cast down):
            Console.WriteLine(item.ToString());
            // Sure, we could also use reflection to get the information about item's properties,
            // but then we'd tightly couple PrintItemO() to a certain type
            // (similar to downcasting).
        }
        

        // The name of this method is different from the others to avoid unwanted overloading
        // resolutions. (Generics are always the best match...)
        public static void PrintItemG<T>(T item)
        {
            // Invalid! W/o constraints (or downcasting) you can not access specific members of the generic function parameter (it is of type object).
            // Console.WriteLine("Name: {0}, Price: {1}", item.Name, item.Price);

            // But we could delegate it to ToString():
            Console.WriteLine(item.ToString());
        }


        public static void PrintItemG<T>(T item, Func<T, string> stringProvider)
        {
            // Alternatively we could also pass an additional Delegate instance, which will provide
            // a string for the passed item to be printed on the console. This is exactly the way
            // LINQ works: decoupling algorithms and data:
            Console.WriteLine(stringProvider(item));
        }
        #endregion 
        #endregion


        private static void AnonymousTypeAdapters()
        {
            /*-----------------------------------------------------------------------------------*/
            // Exploiting dynamic and Anonymous Types to implement easy to use Adapters:

            // Passing directly compatible types:
            var products = new List<Product> 
            {
                new Product { Name = "Unobtaininum", Price = decimal.MaxValue },
                new Product { Name = "Marmite (250g)", Price = 7.85m }
            };

            foreach (var item in products)
            {
                // The dynamic dispatching within PrintItem() will work as expected: Product.Name
                // and Product.Age will be called res.
                PrintItem(item);
            }

            // Passing incompatible types (using an adapter):
            var persons = new List<Person> 
            {
                new Person { Name = "Gloria", Age = 45},
                new Person { Name = "Nelly", Age = 43}
            };

            foreach (var item in persons)
            {
                // Because the type Person does not provide the required readable properties or
                // fields Name and Price (i.e. passing a Person to PrintItem() would throw a
                // RuntimeBinderException) we need to adapt Person. - E.g. with an anonymous type:
                PrintItem(new { Name = item.Name, Price = item.Age });
            }
        }


        #region Methods for EnablingOperatorsInGenerics():
        public static TResult SumAllItems<T, TResult>(T data) where T : IEnumerable<TResult>
        {
            //TResult sum = default(TResult); // This code will be replaced by this code:
            // As sum is now dynamic dynamic dispatch is enabled.
            dynamic sum = default(TResult);

            foreach (TResult item in data)
            {
                // OK! The operator '+' can be applied on sum. During run time it is tried to
                // perform a normal operator overload resolution, and will find any (incl.
                // user-defined ones) binary "+"-operators. In this case the "+" -operator on the
                // run time type of sum (and item, as the whole expression is then a dynamic
                // expression).
                sum = sum + item; 

                // Only the _interface_ of type arguments can be checked within generic code; the
                // interface must only be specified in the generic's constraint (i.e. the where
                // specification). In .Net static methods/properties/fields/delegates don't belong
                // to this interface.
                // Consequence: you can't use operators on "open" types, because they are static
                // methods!
            }
            return sum;
        }
        #endregion


        private static void EnablingOperatorsInGenerics()
        {
            /*-----------------------------------------------------------------------------------*/
            // With dynamic Dispatch it is possible to call Operators on open Types:

            int sum = SumAllItems<IEnumerable<int>, int>(new int[] { 1, 2, 3, 4, 5, 6 });
        }


        private static void DynamicDispatchAndRemoting()
        {
            /*-----------------------------------------------------------------------------------*/
            // .Net Remoting works Fine with dynamic Dispatch!

            AppDomain appDomain = AppDomain.CreateDomain("AnotherDomain");

            // The type dynamic let's us work with a .Net remoting proxy (in this case a proxy to
            // an object created within another AppDomain) w/o having the used type (TT in this
            // case) in access:
            dynamic tt =
                appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                    "DynamicEnabler.TT");

            // Then we can use dispatching to access the properties of the marshaled instance:
            int anInt = tt.AnInt;
            string aString = tt.AString;
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            AnonymousTypeAdapters();


            EnablingOperatorsInGenerics();


            DynamicDispatchAndRemoting();
        }
    }
}