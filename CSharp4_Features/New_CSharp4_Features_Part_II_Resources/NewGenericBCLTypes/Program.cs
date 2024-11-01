using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// This example shows:
// - The type Lazy<T>.
// - Tuples, arrays and structural comparison.
namespace NewGenericTypes
{
    public class Program
    {
        #region Types for LazyExamples:
        /// <summary>
        /// This type encapsulates a very expensive resource.
        /// </summary>
        public class ExpensiveResource
        {
        }


        /// <summary>
        /// A type having some expensive resources wrapped with Lazy(Of T). 
        /// </summary>
        public class TypeWithLazyInitialization
        {
            /*-----------------------------------------------------------------------------------*/
            // Basic Usage of Lazy<T>: 

            // This is a standard way to deal with resources you want to be created for only one
            // time, because they are so expensive to create:
            private ExpensiveResource _expensiveResource;
            public ExpensiveResource ExpensiveResource
            {
                get
                {
                    if (null != _expensiveResource)
                    {
                        _expensiveResource = new ExpensiveResource();
                    }
                    return _expensiveResource;
                }
            }


            // New in .Net 4: Wrap your expensive resource with an instance of the new generic type
            // Lazy<T>, which deals with the deferred initialization on its own:
            private Lazy<ExpensiveResource> _expensiveResourceEx = new Lazy<ExpensiveResource>();
            public ExpensiveResource ExpensiveResourceEx
            {
                get
                {
                    // If you need to know, whether the wrapped instance was already created, you
                    // can query Lazy's property IsValueCreated:
                    if (!_expensiveResourceEx.IsValueCreated)
                    {
                        Debug.WriteLine("ExpensiveResource acquired.");
                    }

                    // The most important property of Lazy is "Value": Accessing Value will either
                    // return the already created instance of the wrapped type, or create and
                    // return it in place. However only one instance of ExpensiveResource will be
                    // created.
                    return _expensiveResourceEx.Value;
                }
            }


            /*-----------------------------------------------------------------------------------*/
            // Using Lazy<T> with Initializer Functions:

            // Not all types that you want to initialize lazily, provide a parameterless ctor. To
            // handle this situation with the type Lazy<T>, you can call Lazy<T>'s ctor accepting
            // an initializer function. The initializer function will then be executed in a lazy
            // manner and its result will be provided as Lazy<T>.Value. The type of the awaited
            // initializer function is Func<LazyReturnType>.
            private Lazy<FileStream> _wrappedFileStream = new Lazy<FileStream>(() => new FileStream("Test.txt", FileMode.Open));

            // Sidebar: If you try initializing a type w/o a parameterless ctor with Lazy<T> you'll
            // get a MissingMemberException when you access Value.

            public FileStream FileStream
            {
                get
                {
                    return _wrappedFileStream.Value;
                }
            }


            // Of course you can use also more advanced code to implement your initializer
            // function, and Lazy<T>'s wrapped type doesn't even need to be instantiable (e.g.
            // being an interface type):
            private Lazy<IEnumerable<string>> _desktopsReadOnlyTextFileNames =
                new Lazy<IEnumerable<string>>(() =>
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    var desktopsTextFiles = Directory.EnumerateFiles(desktopPath, "*.txt", SearchOption.AllDirectories);

                    return
                        from fileName in desktopsTextFiles
                        let fileInfo = new FileInfo(fileName)
                        where fileInfo.IsReadOnly
                        orderby fileInfo.Name
                        select fileName;
                });


            public IEnumerable<string> GetDesktopsMyReadOnlyTextFileName()
            {
                return _desktopsReadOnlyTextFileNames.Value;
            }


            private string _searchPattern = "*.txt";
            // Invalid! You can not access the non-static member _searchPattern in the field
            // initializer of another field of the same type; this may happen quickly when using
            // Lazy<T> with initializer functions:
            //private Lazy<IEnumerable<string>> _desktopsReadOnlyTextFileNames2 =
            //    new Lazy<IEnumerable<string>>(() =>
            //    {
            //        string desktopPath =
            //            Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //        var desktopsTextFiles =
            //            Directory.EnumerateFiles(desktopPath,
            //                _searchPattern,
            //                SearchOption.AllDirectories);
            //
            //        return
            //            from fileName in desktopsTextFiles
            //            let fileInfo = new FileInfo(fileName)
            //            where fileInfo.IsReadOnly
            //            orderby fileInfo.Name
            //            select fileName;
            //    });

            // Sidebar: If an exception is thrown on accessing Value...
            // - ... from the dctor of the wrapped type: This exception escapes Value and needs to
            //   be handled by Value's caller. The wrapped typed is tried to be created on each
            //   future call of Value, until the initialization succeeds.
            // - ... from the initializer function: This exception is cached, escapes Value and
            //   needs to be handled by Value's caller. That the exception is cached means, that the
            //   initialization function will never be called again, the original exception will be
            //   rethrown always.
            // This behavior on dealing with exceptions is slightly different with threadsafe
            // Lazy<T> instances.


            /*-----------------------------------------------------------------------------------*/
            // Using Lazy<T> for threadsafe Initialization: 

            // If you know that multiple threads will access the resource to be initialized lazily,
            // you often write some code to make the code threadsafe. It may look like this:
            private readonly object _expensiveResourceThreadSafeMonitor = new object();
            private ExpensiveResource _expensiveResourceThreadSafeClassic;
            public ExpensiveResource ExpensiveResourceThreadSafeClassic
            {
                get
                {
                    lock (_expensiveResourceThreadSafeMonitor)
                    {
                        if (null != _expensiveResourceThreadSafeClassic)
                        {
                            _expensiveResourceThreadSafeClassic = new ExpensiveResource();
                        }
                        return _expensiveResourceThreadSafeClassic;
                    }
                }
            }

            // To initialize Lazy<T> you can use a special ctor, in which you can switch to
            // _threadsafe_ initialization. It looks like this: 
            private Lazy<ExpensiveResource> _expensiveResourceThreadSafe =
                new Lazy<ExpensiveResource>(true);
            // A call to Lazy<T>(true) is equivalent to
            // Lazy<T>(LazyThreadSafetyMode.ExecutionAndPublication); so you can rewrite the
            // threadsafe Lazy<T> initialization to this:
            private Lazy<ExpensiveResource> _expensiveResourceThreadSafe2 =
                new Lazy<ExpensiveResource>(LazyThreadSafetyMode.ExecutionAndPublication);

            // With the above explained way of initializing, only one thread can execute (enter)
            // the wrapped initializer (e.g. execute Value), all other threads are getting blocked.
            // The value created by the entered thread is then returned to all other threads respectively
            // This behavior can be problematic as it may end up in deadlocks, if the initializer
            // code does itself lock explicitly and a nested lock situation may appear.

            #region Provoke a Deadlock:
            // A monitor and a Lazy<int> object that locks against this monitor in its
            // initialization routine:
            private static readonly object monitor = new object();
            private readonly Lazy<int> lazyValue =
                new Lazy<int>(() =>
                    {
                        lock (monitor)
                        {
                            return 42;
                        }
                    },
                    LazyThreadSafetyMode.ExecutionAndPublication);


            public void ProvokeDeadlock()
            {
                // Starting another thread that updates some data:
                Task task =
                    Task.Factory.StartNew(() =>
                        {
                            lock (monitor)
                            {
                                // Provoke deadlock to occur every time:
                                Thread.Sleep(1000);

                                // Oops! A nested lock situation, because also Value locked the
                                // monitor from a different thread.
                                int value = lazyValue.Value;
                            }
                        });
                Thread.Sleep(100);

                // After a while: get the lazyly initialized value:
                Console.WriteLine(lazyValue.Value);
            }            
            #endregion


            // To solve this problem you can call Lazy<T>'s ctor with the argument
            // LazyThreadSafetyMode.PublicationOnly. Now the behavior of Lazy<T> changes: _every_
            // thread can enter the initializer code, but the result of first finishing thread will
            // be returned (published) to all threads. It looks like this:
            private Lazy<ExpensiveResource> _expensiveResourceDeadLockSafe3 =
                new Lazy<ExpensiveResource>(LazyThreadSafetyMode.PublicationOnly);
            // The downside: the initializer code could run for multiple times and the superfluous
            // instances get discarded.
            //
            // The code shown in the region "Provoke a Deadlock" can be modified to avoid the
            // deadlock by moving the line "int value = lazyValue.Value;" to a location outside of
            // the lock block. - Or by passing LazyThreadSafetyMode.PublicationOnly to Lazy's ctor.
            // Why is that code threadsafe, if all threads can enter? - Well, still does the
            // monitor lock exclusively, but this time the calls to Thread.Sleep() can execute and
            // _any_ of the executing non-blocked threads will finally get the Value.
        }
        #endregion


        private static void LazyExamples()
        {
            /*-----------------------------------------------------------------------------------*/
            // Usage of Lazy<T>:

            // Please have a look into the type TypeWithLazyInitialization!

            TypeWithLazyInitialization typeWithLazyInitialization =
                new TypeWithLazyInitialization();

            using (FileStream stream = typeWithLazyInitialization.FileStream)
            { }

            var result = typeWithLazyInitialization.GetDesktopsMyReadOnlyTextFileName();
        }


        #region Types for TupleExamples:
        public static Tuple<bool, int> ParseInt(string input)
        {
            int theResult;
            bool withSuccess = int.TryParse(input, out theResult);

            // Equivalent to a (bool * int) (a tuple of bool and int) in F#.
            return Tuple.Create(withSuccess, theResult);
        }
        #endregion


        public static void TupleExamples()
        {
            /*-----------------------------------------------------------------------------------*/
            // Basic Usage of Tuples:

            // There are eight overloads of the generic type Tuple. These types can then be used to
            // directly express Tuples with up to seven components. The components of a Tuple are
            // immutable after the Tuple has been created.

            // A 1-Tuple, maybe not very useful on its own:
            Tuple<int> oneTuple = new Tuple<int>(42);
            // A 2-Tuple, or pair, similar to a KeyValuePair<>, but Tuples are reference types:
            Tuple<int, string> twoTuple = new Tuple<int, string>(2, "two");
            // Since Tuple<T1, T2> is generic, we can also create a 2-Tuple of two strings. The
            // types of twoTuple and stringPair are unrelated.
            Tuple<string, string> stringPair = new Tuple<string, string>("zwei", "two");
            // The components of a Tuple can be accessed with the read only properties Item1,
            // Item2, ... Item7, Rest: (An index-based access as for arrays is not possible.)
            Debug.Assert("zwei".Equals(stringPair.Item1));
            Debug.Assert("two".Equals(stringPair.Item2));
            // Invalid! You can not assign the individual components of a Tuple after creation.
            //stringPair.Item2 = "three";


            /*-----------------------------------------------------------------------------------*/
            // Simplified Creation of Tuples:

            // Esp. if you consider Tuples having more than two components, the ctor call to create
            // such Tuples can be very long-winded. - You have to specify the types of all of the
            // components, as well as the values of the components; at least the application of
            // implicit typing (the var keyword) helps a little. Instead of Tuples' ctors you can
            // also use the static class Tuple together with one of its Create()-factory-methods.
            // These methods exploit C#'s ability to infer constructed types from arguments passed
            // to a method.

            // Sidebar: You can find a similar concept in C++' std::pair<typename T, typename U>
            // and the factory-function std::make_pair<typename T, typename U>(T t, U u).
            var stringPair2 = Tuple.Create("zwei", "two");
            Debug.Assert(typeof(Tuple<string, string>).Equals(stringPair2.GetType()));
            // With the Tuple.Create()-methods the creation of septuples is rather brief:
            var septuple = Tuple.Create("one", 2, "drei", 4.0, 5, "six", 7.0f);


            /*-----------------------------------------------------------------------------------*/
            // Tuples with more than seven Components:

            // With the present eight overloads of the generic type Tuple you can't define Tuples
            // with more than seven components. - At least not directly; but you can use another
            // Tuple as a _component_ of your primary Tuple instance to create a kind of Tuple
            // cascade. In fact the eighth component of the octuple type is not named Item8;
            // instead it is named Rest to declare it as a place to continue the components of your
            // Tuple. - Of course you could cascade Tuples of Tuples not only at the eighth, but on
            // any components. However the usage of the eighth component is explicitly reserved and
            // supported (e.g. in the implementation of the ToString()-method, debugger display and
            // to handle 7+N-Tuples returned from F#) to create 7+N-Tuples.
            // Hint: If you find yourself needing to extend your Tuples over seven components, you
            // should consider using another, maybe a userdefined type.

            // 1. Create a nontuple (9-tuple) with the ctor:
            var nontuple =
                new Tuple<string, int, string, double, int, string, float, Tuple<string, int>>(
                    "one", 2, "drei", 4.0, 5, "six", 7.0f, Tuple.Create("acht", 9));
            // As you see, the TRest type argument is Tuple<string, int> and in the ctor-
            // call a new instance of a Tuple<string, int> is passed to initialize the component
            // Rest. The eighth and nineth components can be accessed like this:
            Debug.Assert("acht".Equals(nontuple.Rest.Item1));
            Debug.Assert(9 == nontuple.Rest.Item2);

            // 2. Create a nontuple (9-tuple) with the Create()-factory-method:
            // The outcome is slightly different when you use Tuple's Create()-factory-methods to
            // create Tuples with more than seven components. The eighth argument will always be 
            // wrapped in a Tuple. Just inspect this example:
            Tuple<string, int, string, double, int, string, float, Tuple<Tuple<string, int>>>
                kindofNontuple =
                    Tuple.Create("one", 2, "drei", 4.0, 5, "six", 7.0f, Tuple.Create("acht", 9));
            // This time the TRest type argument is Tuple<Tuple<string, int>> although in the ctor-
            // call just a new instance of a Tuple<string, int> is passed to initialize the
            // component Rest. - This is because Create() wraps the passed item8 into another
            // Tuple. This time the eighth and ninth components can be accessed like this:
            Debug.Assert("acht".Equals(kindofNontuple.Rest.Item1.Item1));
            Debug.Assert(9 == kindofNontuple.Rest.Item1.Item2);

            // 3. Creation of octuples as "border-case":
            // The Rest argument of a Tuple needs to be another Tuple if you use Tuple's ctor. It
            // is not possible to create an octuple by initializing Rest with a non-Tuple instance
            // in the ctor: 
            //var octuple =
            //    new Tuple<int, int, int, int, int, int, int, int>(1, 2, 3, 4, 5, 6, 7, 8); // Invalid! Throws an ArgumentException: The last element of an eight element tuple must be a Tuple.
            //
            // You can wrap Rest's argument within another Tuple (notice, how a 1-Tuple comes in
            // handy in this situation):
            var octupleOk =
                new Tuple<int, int, int, int, int, int, int, Tuple<int>>(1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    new Tuple<int>(8));
            // Or you can use the overload of the Create()-factory-method that accepts eight
            // arguments. The eighth argument is automatically wrapped into a Tuple<int>: 
            var octupleOk2 = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8);

            // String representation and debugger display:
            // The string representation of Tuples is purposeful (just a csv list of the Tuple's
            // components) and the display in the debugger is similar:
            Debug.Assert("(2, two)".Equals(twoTuple.ToString()));
            // If the Tuple has more than seven components, i.e. the Rest component contains
            // another Tuple, the string representation will be flattened accordingly:
            Debug.Assert("(one, 2, drei, 4, 5, six, 7, acht, 9)".Equals(nontuple.ToString()));


            /*-----------------------------------------------------------------------------------*/
            // Tuples to create APIs that are better composeable:

            // Sometimes you need to deal with operations that work on data and produce not only
            // one single result, but multiple results; the method int.TryParse() is an example of
            // such an operation:
            {
                string input = "42";
                int result;
                bool withSuccess = int.TryParse(input, out result);
            }
            // So far, so good. It is working, but the way the multiple results are provided by the
            // method int.TryParse() are not very appealing. Syntacically we have to deal with an
            // ordinary C# return value as well as with an out parameter. A further result of these
            // syntax inconveniences is the inability of return values and out-parameters to compose
            // well:

            // Assume we have this sequence:
            string[] sequenceOfInputs = { "17", "two", "42", "forty", "twentytwo" };

            // , how can we get a sequence of ints from it?
            // E.g. with LINQ like that:

            // 1. With cumbersome LINQ:
            // Even with LINQ we can not write very elegant code. Esp. the call to int.TryParse()
            // and the correct handling of its results hinders us to express ourselves with simple
            // expressions, instead we need multiple statements. Because multiple statments are
            // needed, we have to use statement lambdas instead of expression lambdas, and
            // statement lambdas can't be used in query expressions, we have to use the method
            // chaining notation for LINQ. Also we have introduce an intermediate result of type
            // int? to communicate the inability to convert a string to an int, as well as the mere
            // result to the next step of the LINQ chain... So we end up with a kind of wierd
            // looking chaining notation.
            var sequenceOfInts =
                sequenceOfInputs
                    .Select(
                        item =>
                        {
                            int result;
                            return int.TryParse(item, out result) ? (int?)result : (int?)null;
                        })
                    .Where(item => item.HasValue);

            // 2. With LINQ and Tuples:
            // This time we call ParseInt() instead of int.TryParse(). ParseInt() directly returns
            // a Tuple<bool, int> that contains all the required information to use the result in
            // a straightforward manner. The returned value composes in a simple way, a conversion
            // to an intermediate type like int? is not needed. It is no longer required to work
            // with multiple statements, so a simple query expression only using expression lambdas
            // is sufficient.
            var sequenceOfInts2 =
                from item in sequenceOfInputs
                let convertedItem = ParseInt(item) // Returns Tuple<bool, int>.
                where convertedItem.Item1
                select convertedItem.Item2;

            // The idea of composability by the usage of Tuples was derived from functional 
            // programming languages (e.g. the whole concept of Lisp is based on lists; Lisp's
            // lists are a form of Tuples). - In these languages it is strove to avoid side
            // effects, and assigning to parameters (as it is the case for ref/out-parameters in
            // C#) is a form of side effect; rather should functions only communicate via return
            // values. Esp. F# does automatically wrap APIs like int.TryParse() by returning a
            // Tuple<bool, int> in order to be better composable.


            /*-----------------------------------------------------------------------------------*/
            // Tuples as Alternative to anonymous Types in anonymous Functions (e.g. in TPL Code):

            // We can use anonymous types as return values of anonymous functions; this is esp.
            // interesting in TPL code:
            var aFuture =
                Task.Factory.StartNew(
                    () =>
                    {
                        DateTime then = DateTime.Now;
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < 200000; ++i)
                        {
                            sb.Append("doesn't matter");
                        }

                        return new
                        {
                            String = sb.ToString(),
                            Length = sb.Length,
                            Took = DateTime.Now - then
                        };
                    }
                );
            var resultFromFuture = aFuture.Result;
            Debug.WriteLine("Resulting string length: {0}, required time: {1}",
                resultFromFuture.Length,
                resultFromFuture.Took);

            // And here basically the same code expressed with a Tuple as return type: 
            Task<Tuple<string, int, TimeSpan>> aFuture2 =
                Task.Factory.StartNew(
                    () =>
                    {
                        DateTime then = DateTime.Now;
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < 200000; ++i)
                        {
                            sb.Append("doesn't matter");
                        }

                        return Tuple.Create(sb.ToString(), sb.Length, DateTime.Now - then);
                    }
                );

            Tuple<string, int, TimeSpan> resultFromFuture2 = aFuture2.Result;
            Debug.WriteLine("Resulting string length: {0}, required time: {1}",
                resultFromFuture2.Item2,
                resultFromFuture2.Item3);
            // Using of Tuples can be better than anonymous types, if you need to return the result
            // of an anonymous function from a _named_ function!


            /*-----------------------------------------------------------------------------------*/
            // Passing and Accepting Tuples from C# to F# and vice versa:

            // F# functions returning tuples can be directly used with the .Net Tuple types:
            Tuple<int, string> tupleFromFSharp = Module1.AsTuple(2, "two");

            // F# functions accepting tuples need to be called from C# by passing the tuple's
            // components as individual arguments:
            int product = Module1.AcceptTuple(4, 4);

            // Also 7+N-Tuples returned from F# can be handled; they will be automatically
            // expressed as Tuples exploiting the Rest components to build cascading Tuples:
            var largeTupleFromFSharp =
                Module1.AsLargeTuple("one", 2, "drei", 4, 5, "sechs", "seven", 8, "nine", 10, 11);

            // Sidebar: Before .Net 4 you had to reference the assembly fslib.dll and to add a
            // using directive to the namespace Microsoft.FSharp.Core to use Tuples returned from
            // F# in your C# code.


            /*-----------------------------------------------------------------------------------*/
            // Tuples and Comparison:

            Tuple<int, string, Tuple<int, int>> leftHand =
                Tuple.Create(2, "hi", Tuple.Create(15, 20));

            Tuple<int, string, Tuple<int, int>> rightHand =
                Tuple.Create(2, "hi", Tuple.Create(15, 20));

            // As Tuples implement the interface IStructuralEquatable explicitly. You can use this
            // new interface, in order to equality-compare the two Tuples in a structural
            // manner. Structural comparison means, that each component of the Tuples is compared
            // one after the other, and the comparison will return false, when the first component
            // comparison returns false; or otherwise returns true, if all components have been
            // compared. 

            // You can control how the structural comparison is performed, by calling an overload
            // of the Equals()-method that is provided by the IStructuralEquatable interface. With
            // that overload you can pass an IEqualityComparer that is used with each component:
            bool isStructuralEqual1 =
                ((IStructuralEquatable)leftHand).Equals(rightHand,
                    StructuralComparisons.StructuralEqualityComparer);
            Debug.Assert(isStructuralEqual1);

            // The special StructuralComparisons.StructuralEqualityComparer can be used to apply a
            // structural comparison with each component, it works like this:
            // - If the to-be-compared Tuples are not of the same type (i.e. different counts or 
            //   types of components) the immediate result of Equals() is false.
            // Then the components are compared:
            // - If both components are null, the result is true.
            // - If only one of both components is null, the result is false.
            // - If the first component can be cast to IStructuralEquatable (e.g. is a Tuple or an
            //   array), then Equals(c, StructuralComparisons.StructuralEqualityComparer) is
            //   called. That means, a structural in-depth comparison takes place, if
            //   IStructuralEquatable is supported on that component. Also mind, that it is only
            //   required to check the first (or left-hand side) component in order to apply the
            //   comparison.
            // - If the first component can _not_ be cast to IStructuralEquatable, then Equals(c)
            //   will be called.
            // - If the call to Equals() (any overload) returns true, the next component will be
            //   equality-compared. This is continued until a equality-comparison returns false,
            //   then the result of the structural comparison is false; or the result of the last
            //   equality-comparison is returned.

            // The good news is that Tuple's implementation of Equals() (as inherited from Object) 
            // performs a structural comparison by default already! - So no explicit downcasting to 
            // IStructuralEquatable, as well as no call to the extra overload of Equals() is
            // required. - Instead you can directly call Equals() to have a structural
            // equality-comparison happen:
            bool isStructuralEqualSimple = leftHand.Equals(rightHand);
            Debug.Assert(isStructuralEqualSimple);

            // Sidebar: Tuples do also implement the interface IStructuralComparable explicitly.
            // The structural comparison on IStructuralComparable works similar to 
            // IStructuralComparable and will not be explained in more detailed here. There is one
            // important difference: IStructuralComparable.CompareTo() will throw an
            // ArgumentException if the to-be-compared Tuples are not of the same type (i.e.
            // different counts or types of components).

            // Ok, but when do we need to pass anything different from
            // StructuralComparisons.StructuralEqualityComparer? - Consider the Tuple rightHand2;
            // it differs from leftHand only on the value of the second member, which happens to be
            // an upper case "Hi" instead of a lower case "hi". As the type string does not
            // implement IStructuralEquatable, string's "standard" Equals() method is called. But
            // string's Equals() considers strings written in different cases to be unequal; so the
            // structural comparison will evaluate to false:
            Tuple<int, string, Tuple<int, int>> rightHand2 =
                Tuple.Create(2, "Hi", Tuple.Create(15, 20));
            bool isStructuralUnEqualSimple = leftHand.Equals(rightHand2);
            Debug.Assert(!isStructuralUnEqualSimple);

            // To get a case-unaware comparison of the strings contained in the Tuple, you need to
            // pass a suitable string comparer, e.g. StringComparer.InvariantCultureIgnoreCase. 
            bool isStructuralEqual2 =
                ((IStructuralEquatable)leftHand).Equals(rightHand2,
                    StringComparer.InvariantCultureIgnoreCase);
            Debug.Assert(isStructuralEqual2);
            // Btw. this works, because all the StringComparers are indeed unaware of the
            // to-be-compared type. The overload Equals(object, object) is always called, which
            // delegates the work to the respective calls to leftHand.itemN.Equals(rightHand.itemN) etc.,
            // but a special variant of string.Equals() to compare strings in a case-insensitive
            // way.
            // Mind, that you can only pass comparers that implement the _non-generic_ interface
            // IComparer! The passed comparer will only be used with the first level of the Tuple
            // (i.e. the Tuple's components that are itself of type Tuple will not be compared
            // component-wise). If you need a userdefined structural comparison you need to 
            // implement your own IComparer.

            // As Tuples provide a purposeful override of GetHashCode(), you can use them as keys
            // for Dictionaries. A Tuple's hashcode is calculated by combining all hashcodes of its
            // components (similar to anonymous types).
            var dictionary
                = new Dictionary<Tuple<int, string>, string>
                {
                    { Tuple.Create(1, "one"), "eins" },
                    { Tuple.Create(2, "two"), "zwei" }
                };
            string theValue = dictionary[Tuple.Create(2, "two")];
            Debug.Assert("zwei".Equals(theValue));

            // Sidebar:
            // - If you need to compare Tuples with different component types or with different 
            //   counts of components you need to pass a user defined comparer that handles this.
            // - Tuples do _not_ provide the operators == and != in a special manner, they do just
            //   a comparison of references.
            // - The interfaces IStructuralEquatable and IStructuralComparable do _not_ inherit
            //   from the interfaces IEquatable and IComparable.
        }


        private static void ArraysAndStructuralComparison()
        {
            /*-----------------------------------------------------------------------------------*/
            // Arrays also support structural Comparison:

            string[] a = { "1", "2", "3" };
            string[] b = { "1", "2", "3" };

            // New in .Net 4: arrays implement IStructuralEquatable and IStructuralComparable. Mind,
            // that IStructuralEquatable and IStructuralComparable are implemented explicitly on
            // arrays; after casting to any of that interfaces you can call the extra overloads of
            // Equals() or CompareTo():
            bool structuralEqualStringArrays =
                ((IStructuralEquatable)a).Equals(b,
                    StructuralComparisons.StructuralEqualityComparer);
            // !! If the lengths of the arrays are different, Equals(obj, cmp) will return false
            // without even comparing any elements of the arrays!! On the other hand the arrays
            // to be compared can be of different, but related element-types.
            Debug.Assert(structuralEqualStringArrays);
            // !! The implementation of Equals() that have been inherited from Object does _not_
            // implement structural comparison: !!
            Debug.Assert(!a.Equals(b));

            // And IStructuralComparable:
            int[] c = { 1, 3, 2 };
            int[] d = { 1, 2, 3 };

            // The result of the structural comparison, is the result of the first compared pair of
            // components that is not 0.
            int resultOfComparison =
                ((IStructuralComparable)c).CompareTo(d,
                    StructuralComparisons.StructuralComparer);
            // The array c is greater than d, because the first element of each of the arrays is
            // equal (that continues the comparison with the next element), but the second element
            // of c (3) is greater than the second element of d (2). The remaining elements are not
            // analyzed, the returned result is 3.CompareTo(2). If two elements are null, they are
            // considered equal; the value null is less than any other value.
            // !! If the lengths or types of the arrays not related, an ArgumentException will be
            // thrown without even comparing any elements of the arrays!!
            Debug.Assert(0 < resultOfComparison);
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            LazyExamples();


            TupleExamples();


            ArraysAndStructuralComparison();            
        }
    }
}