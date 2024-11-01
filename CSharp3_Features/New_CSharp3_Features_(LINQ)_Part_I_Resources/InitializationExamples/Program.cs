using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

// These examples show:
// - How automatically implemented properties work.
// - How object initializers and embedded object initializers work.
// - How collection initializers work.
// - The pitfalls of object initializers within the variable-section of a using context.
// - How implicitly typed arrays work.
namespace InitializationExamples
{
    // Our toy: class Person with some properties and ctors.
    public class Person : IDisposable
    {
        // Dctor.
        public Person()
        {}


        // Another ctor awaiting a string (the name).
        public Person(string name)
        {
            Name = name;
        }

        public event EventHandler Disposed;


        protected virtual void OnDisposed(EventArgs e)
        {
            if (null != Disposed)
            {
                Disposed(this, e);
            }
        }


        // The classic C#1/2 way to define a readonly property of type Location (see below). To put
        // embedded object initializers into effect _home needs not to be readonly.
        private readonly Location _home = new Location();
        public Location Home { get { return _home; } }


        /*---------------------------------------------------------------------------------------*/
        // Automatically Implemented Properties:

        // In C#3 we can use automatically implemented properties, which provide a very compact
        // syntax.
        public int Age { get; set; }
        public string Name { get; set; }
        public List<Person> Friends { get; set; }


        // Implement IDisposable (no additional finalizer as there are no unmanaged resources to be
        // freed):
        public void Dispose()
        {
            Debug.WriteLine("Disposing");
            OnDisposed(EventArgs.Empty);
        }


        /*---------------------------------------------------------------------------------------*/
        // Object and Collection Initializers can be used for Members as well:

        private Location _placeOfBirth = new Location { Country = "GE", Town = "Frankfurt" };


        private readonly IList<string> _nickNames = new List<string> { "Sally", "Willy" };
    }


    // A simple class Location.
    public class Location
    {
        // Just these two properties:
        public string Country { get; set; }
        public string Town { get; set; }
    }


    internal class Program
    {
        // Awaits a string array, but does nothing else.
        internal static void AwaitsAStringArray(string[] stringArray)
        {
            // Pass
        }

		// Awaits a two-level-jagged int array, but does nothing else.
		public static void AwaitsATwoLevelJaggedIntArray(int[][] jaggedArray)
		{
			// Pass
		}

        // Throws an exception, nothing else.
        internal static string GetName()
        {
            throw new Exception("No new name for you!");
        }


        private static void ObjectInitializers()
        {
            /*-----------------------------------------------------------------------------------*/
            // Object Initializers:

            // The C#2 way to create a new Person object and to initialize property Age afterwards. 
            Person john = new Person();
            john.Age = 42;

            // The C#3 way to create a new Person object and apply an object initializer to
            // initialize the property Age. Mind, that object construction and initialization is
            // done within _one_ expression.
            Person jake = new Person { Age = 32 };

            // Sidebar: You can also initialize accessible fields with the collection initializer
            // syntax.

            // If applied, multiple entries in the object initializer are separated by commas.
            // Object initializers can be used in concert with ctor calls as well.
            Person joe2 = new Person { Name = "Joe", Age = 36 };
            Person joe3 = new Person("Joe") { Age = 36 }; // In effect equivalent to joe2.

            // Sidebar: The individually assigned properties/fields are assigned in exactly the
            // order you write them in the object initializer. After assigning the last item in the
            // object initializer, you are allowed to leave a dangling comma.

            // Next, create a Person object and apply an object initializer on property Home with
            // an embedded object initializer for type Location:
            Person luke = new Person
            {
                Name = "Luke",
                Age = 37,
                // Mind, that the backing field of property Home was already created in all Person's
                // ctors. But Home's properties Country and Town are initialized in the embedded
                // object initializer right here. The language specification refers to this as
                // "setting the properties of an embedded object".
                // To put embedded object initializers into effect Home's backing field (_home in
                // this case) needs not to be readonly.
                Home = { Country = "US", Town = "Boston" }
            };

            // However, you are not allowed to advise event handlers in an object initializer:
            Person david = new Person 
            {
                Name = "David",
                // Invalid!
                //Disposed += delegate 
                //{
                //    Debug.WriteLine("Good bye David");
                //}
            };
        }


        private static void CollectionInitializers()
        {
            /*-----------------------------------------------------------------------------------*/
            // Collection Initializers:

            // In C#2 a Collection must be created and filled with a considerable amount of
            // statements.
            List<Person> friends = new List<Person>();
            friends.Add(new Person("Rose"));
            friends.Add(new Person("Poppy"));
            friends.Add(new Person("Daisy"));

            // In C#3 a Collection can be initialized with a Collection initializer. Construction
            // and filling of the Collection is done within only _one_ expression. For friends2 the
            // dctor is called and then the method Add(Person) is called for three times underneath
            // (for Rose, Poppy and Daisy).
            List<Person> friends2 = new List<Person> // In effect equivalent to friends.
            {
                new Person("Rose"), new Person("Poppy"), new Person("Daisy")
            };

            // Sidebar: After assigning the last item in the Collection initializer, you are
            // allowed to leave a dangling comma (you can also leave it on arrays).

            // It is also possible to call any other ctor in concert with the Collection
            // initializer.
            const int capacity = 3;
            List<Person> friends3 = new List<Person>(capacity)
            {
                new Person("Rose"), new Person("Poppy"), new Person("Daisy")
            };

            // All sorts of collections can be used with Collection initializers, as long as they
            // implement the interface IEnumerable (or IEnumerable<T>) and provide any overload of
            // the method Add(). -> So it is also possible to use Dictionary<T, U> with Collection
            // initializers.

            // Here Dictionary name2Grade's Collection initializer calls the dctor and then the
            // method Add(string, string) for three times underneath.
            Dictionary<string, string> name2Grade = new Dictionary<string, string> 
            {
                {"Rose", "Engineer"}, {"Poppy", "Lead"}, {"Daisy", "Apprentice"}
            };

            Person luke = new Person
            {
                Name = "Luke",
                Age = 37
            };
            // Finally object initializers and Collection initializers can be used together.
            Person trish = new Person("Trish")
            {
                // Indeed it is possible to write more object and Collection initializers down
                // in a cascaded manner (friends of friends of friends ...).
                // But it's not allowed to set trish as a friend of Ethan while trish is being
                // initialized, trish can't be accessed yet, because it isn't initialized.
                Friends = new List<Person>
                {
                    luke, friends[0], new Person { Name = "Ethan", /* Friends = ... */}
                }
            };
        }


        private static void ImplicitlyTypedArrays()
        {
            // Must install System.ValueTuple
            (int, string)[] cars = new[] { // A car represented by System.ValueTuple
                              // (int Horsepower * string Name)
                (horsePower: 200, name: "500 Abarth Assetto Corse"),
                (horsePower:130, name:"Ford Focus 2.0"),
                (horsePower:55, name:"VW Kaffeemühle"),
                (horsePower:354,name:"Lotus Esprit V8"),
                (horsePower:112, name: "Alfa Romeo Giulia ti Super") 
            };

            IList

            List<int> numbers = new List<int> {1,2,3,4,5,6,7,8,9 };
            numbers.ForEach(number => numbers.Add(10));

            /*-----------------------------------------------------------------------------------*/
            // Implicitly typed Arrays:

            // A newly created array can be initially filled in a straight forward manner in
            // C#1/C#2 (with an array initializer).
            string[] array1a = new string[] { "Monica", "Rebecca", "Lydia" };
            string[] array1b = { "Monica", "Rebecca", "Lydia" };

            // In C#3 it is allowed to leave the typename away on the new keyword. Mind, that this
            // syntax is only allowed, if the initial array items are implicitly convertible same
            // type.
            string[] array2 = new[] { "Monica", "Rebecca", "Lydia" };

            // This new initialization syntax makes sense, when used together with method passing.
            // AwaitsAStringArray({ "Monica", "Rebecca", "Lydia" }) // Looks good, but is invalid.
            AwaitsAStringArray(new string[] { "Monica", "Rebecca", "Lydia" }); // C#1/2 way: explicit.
            // Here it is obvious that the awaited type (string[]) will be automatically inferred.
            AwaitsAStringArray(new[] { "Monica", "Rebecca", "Lydia" }); // New in C#3: implicit.

            // Arrays of length 0 can not be instantiated with the anonymous arrays' syntax:
            //string[] emptyArray1 = new[] { }; // Invalid!
            //string[] emptyArray2 = new[0] { }; // Invalid!
            string[] emptyArray3 = { }; // OK! Uses array literal.
            string[] emptyArray4 = new string[0]; // OK! Uses array with length initialization.

            // Under certain circumstances implicitly typed arrays can be initialized with
            // initializer lists having mixed static types of their items:
            // - There must be one type, into which all the initializer items can be converted to
            //   implicitly.
            // - Only the static types of initializer items' expressions are considered for this
            //   conversion.
            // - If these bullets do not meet, or if all the initializer items are typeless
            //   expressions (null literals or anonymous methods with no casts) the array
            //   initializer will fail to compile.

			/*-----------------------------------------------------------------------------------*/
			// Implicitly typed jagged Arrays:

			int[][] jaggedArray = {new[]{1, 2, 3, 4}, new[]{6, 7}, new[]{8, 9, 0}};
			AwaitsATwoLevelJaggedIntArray(new[]{new[]{1, 2, 3, 4}, new[]{6, 7}, new[]{8, 9, 0}});
        }



        public static void ObjectOrCollectionInitializersAndExceptions()
        {
            /*-----------------------------------------------------------------------------------*/
            // Object and Collection Initializers and Exceptions:

            Person phil = null;
            try
            {
                // If an Exception is thrown on the initialization of the Name property, the ctor
                // of Person will be called, but phil will only refer to the null reference (i.e
                // the default value of Person).
                phil = new Person { Name = GetName() };
            }
            catch (Exception)
            {
                Debug.Assert(default(Person) == phil);
            }

            // The same is true for ValueTypes:
            Point point = default(Point);
            try
            {
                point = new Point { X = GetName().Length };
            }
            catch (Exception)
            {
                Debug.Assert(default(Point) == point);
            }


            /*-----------------------------------------------------------------------------------*/
            // This Example shows the Problems with Object and Collection Initializers in using-
            // Contexts:

            try
            {
                // Initializes a new Person object and initializes the Name property. But the
                // initialization method GetName() will throw.
                // The effect of this Exception being thrown by the initialization of a property
                // in a using context is serious: the method Dispose() will not be called!
                using (Person person = new Person { Name = GetName() })
                {
                    Debug.WriteLine("You'll never see this message.");
                }
            }
            catch (Exception)
            {
            }

            // Why? Well, the object initialization is rewritten by the compiler in a manner like
            // that:
            try
            {
                Person tempLocal = new Person();
                tempLocal.Name = GetName();
                using (Person person = tempLocal)
                {
                    Debug.WriteLine("You'll never see this message.");
                }
            }
            catch (Exception)
            {
            }
            // As you can see the ctor call to Person as well as the assignment to the property
            // Name is done _before_ the using context's initialization. So it is clear why the
            // Exception could escape the using context.
            // In fact object initializers as well as collection initializers should be avoided
            // within using context, because of the presented danger. The code analysis will mark
            // this problem with the message CA2000.
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            ObjectInitializers();


            CollectionInitializers();


            ImplicitlyTypedArrays();


            ObjectOrCollectionInitializersAndExceptions();
        }
    }
}