using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

// These examples show:
// - Duck Typing and Consistency.
namespace DuckTypingConsistency
{
    public class Program
    {
        #region Types and Methods for DuckTypingConsistency():


        #region Types for Subtyping and Polymorphism:
        public interface IMeasureable
        {
            int Length { get; }
        }


        public class StringAdapter : IMeasureable
        {
            private readonly string _original;


            public StringAdapter(string original)
            {
                _original = original ?? string.Empty;
            }


            public int Length
            {
                get { return _original.Length; }
            }
        }


        public class ArrayAdapter : IMeasureable
        {
            private readonly int _length;


            public ArrayAdapter(Array original)
            {
                _length = null != original
                    ? original.Length
                    : 0;
            }


            public int Length
            {
                get { return _length; }
            }
        }
        #endregion


        /*---------------------------------------------------------------------------------------*/
        // With Subtyping and Polymorphism:

        private static void SubTyping(IMeasureable measureable)
        {
            int length = measureable.Length;
            Debug.WriteLine(length);
        }


        /*---------------------------------------------------------------------------------------*/
        // With a generic Delegate:

        // As you see for typically all generic methods that await Delegate instances: no
        // constraints on the type T, because the usage of T is encapsulated in the Delegate
        // instance.
        private static void GenericTypingWDelegate<T>(T t, Func<T, int> lengthProvider)
        {
            int length = lengthProvider(t);
            Debug.WriteLine(length);
        }


        /*---------------------------------------------------------------------------------------*/
        // With Duck Typing and dynamic:

        private static void DuckTypingAndDynamic(dynamic it)
        {
            int length = it.Length;
            Debug.WriteLine(length);
        }


        /*---------------------------------------------------------------------------------------*/
        // With Duck Typing and Reflection:

        private static void DuckTypingAndReflection(object it)
        {
            Type anythingsType = it.GetType();
            object length =
                anythingsType.InvokeMember("Length",
                    BindingFlags.GetProperty,
                    null,
                    it,
                    null);
            Debug.WriteLine((int)length);
        }
        #endregion


        private static void DuckTypingConsistency()
        {
            /*-----------------------------------------------------------------------------------*/
            // Consistency with Subtyping and Polymorphism:

            // Consistency with a common .Net interface. Then adapters are required, i.e. you have
            // to code types to adapt your Length-calls (read: implementing the required
            // interface):
            SubTyping(new StringAdapter("hello"));
            SubTyping(new ArrayAdapter(new[] { "hi", "there" }));
            // Polymorphism will check that the relation of the contributing types is correct at
            // compile time (StringAdapter and ArrayAdapter both implement IMeasureable). This is
            // enough to get a subtype-based consistency; also a subclass-based consistency can be
            // be put into effect by the application of (maybe abstract) base classes. Subclassing 
            // is more typesafe, as the compiler performs more checks at compile time; therefor
            // subtyping with e.g. interfaces couples more loosely.


            /*-----------------------------------------------------------------------------------*/
            // Intermezzo - One more Way: Consistency with generic Methods and Lambda Expressions:

            // Consistency with the lightweight variant with generic methods and lambda expressions
            // as Delegate instances. - This is lightweight, because no common .Net interfaces or
            // explicitly coded adapter-types are required (which is just a neat solution):
            GenericTypingWDelegate("hello", s => s.Length);
            GenericTypingWDelegate(new[] { "hi", "there" }, a => a.Length);
            // Generic typing in .Net allows only to "keep a type open" to be filled at compile
            // time. - If you want to call methods on an instance of that left-open type within
            // your generic type, you once again need to commit for a common compile time-checked
            // type in a _constraint_ for the generic type.
            // But in this example we use a generic method and we pass an instance of a Delegate as
            // lambda expression. - The compiler can _infer_ the types of the parameter (string and
            // string[]) and of the returned value (int) and construct the correct _constructed
            // type_ for the generic method. Within the "constructed" method, the Delegate instance
            // that holds the lambda expression can then be called requiring a minimum from a type
            // (the Delegate in this case) to be consistent (because the instance of the left-open
            // type is only used (e.g. methods called) in the _lambda expression_, not in the
            // generic type).
            // This is exactly the way how LINQ can deal with sequences of different types; generic
            // methods together with type inference allow lightweight consistency, even anonymous
            // types play well in these scenarios.
            // In generics you can also control their variances more precisely to define an
            // LSP-style consistency (since C#4).


            /*-----------------------------------------------------------------------------------*/
            // And now with Duck Typing and dynamic Dispatch:

            // The idea of duck typing is to express that we as programmers know about the presence
            // of an object's feature (e.g. a property), but can't tell that object's type to the
            // compiler. It would be required to make the object's type visible to the compiler in
            // order to check the call of the property in a statically typed language. In C#4 we
            // are able to switch to dynamic typing and enabling duck typing: the object looks like
            // it has an integer Length property, so it has that property, period. - The DLR then
            // tries to manage the call or fail with a RuntimeBinderException.
            // With duck typing consistency is not a problem, because objects (e.g. as parameters)
            // using dynamic are completely variant.

            DuckTypingAndDynamic("hello");
            DuckTypingAndDynamic(new[] {"hi", "there"});

            // Dynamic dispatch: Both calls of DuckTypingAndDynamic() invoke two completely
            // unrelated Length properties, String.Length and Array.Length, by examining the types
            // dynamically at execution time and binding the property "Length" lately. Notice, that
            // no polymorphism could be applied, because the types String and Array are completely
            // unrelated! Once again: duck typing means, that we assume e.g. methods to be present
            // as a "convention"; here the property Length is expected to be present.


            #region This code shows how Reflection could be used instead of dynamic dispatch:
            /*-----------------------------------------------------------------------------------*/
            // Finally with Duck Typing and Reflection:

            DuckTypingAndReflection("hello");
            DuckTypingAndReflection(new[] { "hi", "there" });
            #endregion
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:


            DuckTypingConsistency();
        }
    }
}