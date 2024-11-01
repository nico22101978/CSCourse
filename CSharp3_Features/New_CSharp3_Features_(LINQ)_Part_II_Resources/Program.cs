using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;


// These examples show:
// - The evolution from static utility methods to extension methods.
// - The pervasiveness of extension methods.
// - How the name resolution of extension methods works.
namespace ExtensionMethodsExamples
{
#region Methods for ExtensionMethodsFunHouse():
    internal static class FunHouseExtensions
    {
        private static SpeechSynthesizer _speechSynthesizer ;
        public static SpeechSynthesizer SpeechSynthesizer 
        {
            get 
            {
                if (null == _speechSynthesizer)
                {
                    _speechSynthesizer = new SpeechSynthesizer();
                }
                return _speechSynthesizer;
            }
        }
        
        public static void Say(this object that)
        {
            if (null == that)
            {
                throw new ArgumentNullException("that");
            }

            try
            {
                SpeechSynthesizer.Speak(that.ToString());
            }
            catch (Exception exc)
            {

                SpeechSynthesizer.Speak(exc.GetType().Name);
            }
        }


        public static void WhileTrue(this Func<bool> condition, Action block)
        {
            if (null == condition)
            {
                throw new ArgumentNullException("condition");
            }

            while (condition())
            {
                block();
            }
        }


        public static void WhileFalse(this Func<bool> condition, Action thenBlock)
        {
            if (null == condition)
            {
                throw new ArgumentNullException("condition");
            }

            while (!condition())
            {
                thenBlock();
            }
        }


        public static void IfTrue(this Func<bool> condition, Action thenBlock)
        {
            if (null == condition)
            {
                throw new ArgumentNullException("condition");
            }

            if (condition())
            {
                thenBlock();
            }
        }


        public static void IfTrueElse(this Func<bool> condition, Action thenBlock,
            Action elseBlock)
        {
            if (null == condition)
            {
                throw new ArgumentNullException("condition");
            }

            if (condition())
            {
                thenBlock();
            }
            else
            {
                elseBlock();
            }
        }


        public static void IfFalse(this Func<bool> condition, Action thenBlock)
        {
            if (null == condition)
            {
                throw new ArgumentNullException("condition");
            }

            if (!condition())
            {
                thenBlock();
            }
        }


        public static void IfFalseElse(this Func<bool> condition, Action thenBlock,
            Action elseBlock)
        {
            if (null == condition)
            {
                throw new ArgumentNullException("condition");
            }

            if (!condition())
            {
                thenBlock();
            }
            else
            {
                elseBlock();
            }
        }


        public static void TimesRepeat(this int that, Action block)
        {
            for (int i = 0; i < that; ++i)
            {
                block();
            }
        }


        public static void TimesRepeat(this int that, Action<int> block)
        {
            for (int i = 0; i < that; ++i)
            {
                block(i);
            }
        }
    }
#endregion


#region Types for ExtensionMethodEvolution():
    internal static class MyUtilities
    {
        // The type string does not provide methods to work with regular expressions. Therefore a
        // set of utility methods have been written that deal with strings and regular
        // expressions. One example of this set is a method that does simply answer the question,
        // whether the passed string matches against rexToMatch?  
        internal static bool StringMatches(string input, string rexToMatch)
        {
            return Regex.IsMatch(input, rexToMatch);
        }
    }


    /// <summary>
    /// A static class, which defines extension methods is often called their "sponsor class".
    /// Sponsor classes have to be top level classes!
    /// </summary>
    internal static class MyStringExtensions
    {
        // Here we have the same method as MyUtilities.StringMatches, but declared as extension
        // method. The only syntacic modification is the this prefix on the first parameter. The
        // first parameter must be of "extended type". Additionally the name of the method was
        // changed to read more naturally when it is called.
        internal static bool Matches(this string input, string rexToMatch)
        {
            return Regex.IsMatch(input, rexToMatch);
        }
    }
#endregion


#region Types for ExtensionMethodsPervasiveness():
    // The name of the class or of its enclosing namespace does not matter. We can extend multiple
    // different types in the same static class (as it is done here), but this should be considered
    // a bad practice. We should create a static class per type we are about to extend. 
    public static class MyPervasiveExtensions
    {
        // This method is so pervasive that it can be called on instances of _each_ .Net type (all
        // types are derived from object).
        public static bool IsInt(this object o)
        {
            return o is int;
        }


        // Besides: a possibility to optimize IsInt() by providing two overloads.
        // Due to compile time binding to the argument's type, a statically typed version of
        // IsInt() can be expressed with one overload for the static type int w/o having the need
        // to use reflection.
        //public static bool IsInt(this int i)
        //{
        //    return true;
        //}
        // Having the IsInt(int), the remaing types can be handled by the fixed "answer" false
        //public static bool IsInt(this object o)
        //{
        //    return false;
        //}

        // Sidebar: Extension methods with the same signatures should not be defined in different
        // classes in different namespaces (e.g. A and B). - Then only changing from using
        // namespace A to namespace B would change the behavior of that extension method.

        // This method is so pervasive that it can be called on instances of _each_ .Net type that
        // implements the unbound generic interface IList<T>. In this example the parameter list is
        // checked for being null, if so an ArgumentNullException will be thrown. - Throwing
        // ArgumentNullException is very appropriate here, because IsLarge() can accept an IList
        // argument when called with the prefix notation, instead of calling it with the infix
        // notation.
        public static bool IsLarge<T>(this IList<T> list)
        {
            if (null == list)
            {
                throw new ArgumentNullException("list");
            }

            return 100000 <= list.Count;
        }


        // This method is less pervasive than IsLarge of T it can be called for _each_ .Net type
        // that implements the _closed constructed generic_ interface IList<int>.
        public static bool IsLarge(this IList<int> list)
        {
            if (null == list)
            {
                throw new ArgumentNullException("list");
            }

            return 10 <= list.Count;
        }

        // IsLarge() can be expressed alternatively, by making it a generic method and putting a
        // type constraint (for IList<int> as a closed constructed type) on the type parameter.
        public static bool IsLarge2<T>(this T list) where T : IList<int>
        {
            if (null == list)
            {
                throw new ArgumentNullException("list");
            }

            return 100000 <= list.Count;
        }

        // But we can not go so far to make the generic type and the open type of the generic type
        // both open types on our generic method, only mentioning one of the open types in the
        // generic constraints. This method will never be found as an extension method, because the
        // constructed type of T can not be inferred properly. We can only call this method as an
        // ordinary static method incl. the explicit specification of _all_ type arguments.
        public static bool IsLarge3<T, U>(this T list) where T : IList<U>
        {
            if (null == list)
            {
                throw new ArgumentNullException("list");
            }

            return 100000 <= list.Count;
        }


        // Sidebar: Checking against the null reference of the first parameter in extension
        // methods and ignoring nullity is not a good practice, as it changes the semantics of the
        // call. - If the argument in null an ArgumentNullException should be thrown (this is the
        // same behavior as in LINQ's extension methods).
    } 
#endregion


#region Types for ExtensionMethodsNameResolution():
    // Here we have the type Class with some instance methods that may conflict with some
    // extension methods defined in the static class ClassExtensions.
    internal class Class
    {
        internal void Foo(int i)
        {
            Debug.WriteLine("In instance method Foo(int).");
        }


        internal void Bar(int i)
        {
            Debug.WriteLine("In instance method Bar(int).");
        }


        internal void Ram(object o)
        {
            Debug.WriteLine("In instance method Ram(object).");
        }


        internal void Clu<T>(T value)
        {
            Debug.WriteLine("In instance method Clu<T>(T).");
        }


        internal void Tron(bool b)
        {
            Debug.WriteLine("In instance method Tron(bool).");
        }
    }


    // Here we have a static class ClassExtensions with some extension methods on type Class.
    // These methods may "conflict" with instance methods of type Class.
    internal static class ClassExtensions
    {
        internal static void Foo(this Class c, int i)
        {
            Debug.WriteLine("In extension method ClassExtensions.Foo(int).");
        }


        internal static void Bar(this Class c, double d)
        {
            Debug.WriteLine("In extension method ClassExtensions.Bar(double).");
        }

        
        public static void Ram(this Class c, string s)
        {
            Debug.WriteLine("In instance method Ram(object).");
        }


        internal static void Clu(this Class c, bool b)
        {
            Debug.WriteLine("In extension method ClassExtensions.Clu(bool).");
        }


        public static void Tron<T>(this Class c, T value)
        {
            Debug.WriteLine("In extension method ClassExtensions.Tron<T>(T).");
        }
    } 
#endregion

 
    public class Program
    {
        private static void ExtensionMethodEvolution()
        {
            /*-----------------------------------------------------------------------------------*/
            // The Evolution of Extension Methods:

            // A typical static utility method is called via period notation on a static class
            // (prefix notation).
            const string name = "June";
            bool matches = MyUtilities.StringMatches(name, ".*u.*");

            // With extension methods the notation to call utility methods has changed
            // dramatically. The period notation to call the method is applied against the instance
            // of the extended type itself (infix notation).
            matches = name.Matches(".*u.*");
            // Extension methods can be called as ordinary utility methods with the old prefix
            // call notation as well. Compare this to the new, much compacter infix notation.
            matches = MyStringExtensions.Matches(name, ".*u.*");
        }


        private static void ExtensionMethodsPervasiveness()
        {
            /*-----------------------------------------------------------------------------------*/
            // The Pervasiveness of Extension Methods:

            // The type object has been extended with the extension method IsInt(), so it can be
            // called on _every instance_ of sub types of object. It is possible to call IsInt() on
            // sub type int.
            int i = 42;    
            bool isInt = i.IsInt();
            // Also IsInt() can be called on any other descendant of type object:
            isInt = "aString".IsInt();

            // Extension methods can be called on pointers as well.
            unsafe
            {
                int* intPointer = &i;
                isInt = intPointer->IsInt();
            }

            // The unbound generic type IList<T> has been extended with the extension method
            // IsLarge() that returns true if the passed list contains 100000 or more items.
            var list1 = new List<string> { "Samantha", "Frida", "Agatha" };
            bool isLarge = list1.IsLarge();

            // The constructed generic Type IList<int> has also been extended with the extension
            // method IsLarge(). But for IList<int> IsLarge() returns true if the passed list
            // contains ten or more items.
            var list2 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            isLarge = list2.IsLarge();


            /*-----------------------------------------------------------------------------------*/
            // Extension Methods and Reflection:

            // Object's extension method "IsInt" is not discoverable via reflection on the type
            // object:
            Debug.Assert(null == typeof(object).GetMethod("IsInt"));

            // We have to reflect the type MyPersasiveExtensions to find the method "IsInt": 
            Debug.Assert(null != typeof(MyPervasiveExtensions).GetMethod("IsInt"));
        }


        private static void ExtensionMethodsNameResolution()
        {
            /*-----------------------------------------------------------------------------------*/
            // The Resolution of Method Names when Extension Methods come into Play:

            var mc = new Class();
            // Will call the instance method, because the extension method with the _same_
            // signature will not be resolved, instance methods will be resolved first.
            mc.Foo(42);
            // Force the call of the extension method.
            ClassExtensions.Foo(mc, 42);

            // Will call the extension method, because it provides a better matching signature.
            mc.Bar(0.5);
            // Force the call of the instance method.
            mc.Bar((int)0.5);

            // Will call the instance method, although an implicit conversion from string to
            // object is required!
            mc.Ram("some text");
            // Force the call of the extension method.
            ClassExtensions.Ram(mc, "some text");

            // Will call the instance method, because it provides a generic method, which counts
            // _always_ as the better matching signature than the extension method, even if the
            // equally named extension method carries parameters of the exact type. 
            mc.Clu(true);
            // Force the call of the extension method.
            ClassExtensions.Clu(mc, true);

            // Will call the instance method, because the extension method with the generic
            // signature will not be resolved, instance methods will be resolved first. (I.e. an
            // instance method whose signature exactly is a better match than a generic method.)
            mc.Tron(true);
            // Force the call of the extension method.
            ClassExtensions.Tron(mc, true);
        }


        private static void ExtensionMethodFunHouse()
        {
            /*-----------------------------------------------------------------------------------*/
            // Simulating Control Flow:

            // These examples have been inspired by the programming languages Ruby and
            // Smalltalk using blocks.

            // In Ruby and Smalltalk integral type provide methods for looping directly. The
            // similarity to Ruby and Smalltalk are the blocks of code being passed to these
            // methods; in these C# implementations the blocks are of type Action() (i.e. a
            // Delegate type).

            // Simple loop:
            5.TimesRepeat(() => // <- Here the block of code (of type Action).
            {
                Debug.WriteLine("And I say again!");
            });

            // The loop can also be controled by the int-result of an expression
            ("Simon".Length + 2).TimesRepeat(() =>
            {
                Debug.WriteLine("And I say again!");
            });

            // Simple counter loop, with passing the counter variable to the block:
            5.TimesRepeat(counter => // <- Here the block of code (of type Action(int)).
            {
                Debug.WriteLine("Next: " + counter);
            });


            // The next examples have been inspired by the programming language Smalltalk using
            // blocks.

            // The following calls try to simulate control flow statements to some extend. The
            // idea is to extend the Delegate type Func<bool>, which can be used to express
            // predicates that make up the condition of all control flow statements. The
            // programming language Smalltalk uses a similar style. But in Smalltalk the types True
            // and False provide polymorphic methods, rather than these types have been extended.
            // The similarity to Smalltalk are the blocks of code being passed to these methods; in
            // these C# implementations the blocks are of type Action() (i.e. a Delegate type). 
            // In Smalltalk the control flow can only be expressed by these methods, but the syntax
            // is somewhat terser.

            // We have extended the type Func<bool>, but a lambda has _no type_ (as it is a
            // typeless expression). It is required to cast the predicate-lambda to a Func<bool> in
            // order to call the extension method on it (this makes the syntax a little clumpsy):
            ((Func<bool>)(() => 5 == 5))    // <- Here the predicate (of type Func<bool>).
                .IfTrue(() =>               // <- Here the block of code (of type Action).      
                {
                    Debug.WriteLine("True for 5 == 5: " + (5 == 5));
                });

            ((Func<bool>)(() => 5 != 5))
               .IfFalse(() =>
               {
                   Debug.WriteLine("False for 5 != 5: " + (5 != 5));
               });

            ((Func<bool>)(() => 5 == 5))
                .IfTrueElse(() =>
                {
                    Debug.WriteLine("True");
                },
                () =>
                {
                    Debug.WriteLine("False");
                });

            // It also works for loops:
            int i = 0;
            ((Func<bool>)(() => i != 5))
                .WhileTrue(() =>
                {
                    Debug.WriteLine("True for: " + (i++));
                });

            i = 0;
            ((Func<bool>)(() => i > 5))
                .WhileFalse(() =>
                {
                    Debug.WriteLine("False for: " + (i++));
                });


            /*-----------------------------------------------------------------------------------*/
            // Enabling Speech:

            // It is so easy to extend the type object by a new method... As we can see the method
            // Say() can be used like the method ToString(), listen and enjoy:
            42.Say();
            (17 + 3).Say();
            "Completely senseless; but nice!".Say();
            object o = null;
            o.Say();
        }



        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:


            ExtensionMethodEvolution();


            ExtensionMethodsPervasiveness();


            ExtensionMethodsNameResolution();


            ExtensionMethodFunHouse();
        }
    }
}
