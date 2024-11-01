using System;
using System.IO;
using System.Diagnostics;

// These examples show:
// - The type inference of local variables.
namespace TypeInferenceExample
{
    public class Program
    {
        #region Methods for ImplicitTyping():
        /// <summary>
        /// Gets a magic number.
        /// </summary>
        /// <returns>A magic number.</returns>
        private static int GetMagicNumber()
        {
            return 42;
        } 
        #endregion


        private static void ImplicitTyping(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Implicit Typing:

            // Declaring a string in C#1/2:
            string myString = "some text";

            // In C#3 the var keyword can be used instead of an explicit type name. The static type
            // of myStrings is automatically inferred by the compiler by analyzing the expression
            // on the right side of the assignment (the initializer expression).
            // This means: implicitly typed variables must always be initialized! 
            var myString2 = "some text";

            // Also arrays can be implicitly typed:
            var someValues = new [] { 1, 2, 3, 4, 5 };

            // The initializer expression needs not to be a constant or literal! The result of
            // following initializer expression is definitively not a constant!
            // But as can be seen in this call it is rather difficult for the _developer_ to infer
            // the correct type, because the initializer expression is not trivial. 
            // The variable commandLineItems is inferred to be a string[], because string[] is the
            // concrete return type of Array.FindAll<string>().
            var commandLineItems = Array.FindAll<string>(args, delegate(string item)
            {
                return item.StartsWith("A");
            });

            // Keep in mind, that the type of myString2 is still static! I.e. only the interface of
            // type string can be used on myString2; only methods and properties of type string can
            // be called.
            // - In Visual Studio the programmer gets supported by editor tooltips on the symbols
            //   (so myString2 is unveiled to be of static type string in the tooltip).
            // - Additionally Visual Studio's editor presents only applicable items when
            //   IntelliSense is used.
            int nChars = myString2.Length;

            // Once again: implicitly typed variables are statically and immutably typed after
            // initialization.
            var anotherValue = "yet another string";
            // Invalid! The type of anotherValue was inferred to be string, no differently typed
            // expression can be used to assign to it.
            //anotherValue = 25;

            // It can be difficult to get the type of total for the developer. It depends on the
            // return type of GetMagicNumber(), it determines the applied implicit conversions.
            // If GetMagicNumber() returns an int, total will be of type int.
            // If GetMagicNumber() returns a double, total will be of type double.
            var total = 100 * GetMagicNumber() / 6;
            // The type of total can be controlled with explicit conversions
            var total2 = (decimal)(100 * GetMagicNumber() / 6);
            // or with explicit typing, then implicit conversions may apply.
            decimal total3 = 100 * GetMagicNumber() / 6;


            /*-----------------------------------------------------------------------------------*/
            // Not all Types can be inferred:

            // Invalid! Which type should be inferred from a null literal?
            //var invalid1 = null; 
            var better1 = (string)null; // With cast: OK: the type is string!
            
            // Invalid! Anonymous methods can't be inferred, because they are
            // "typeless expressions" after the C# standard.
            //var invalid2 = delegate(int anInt) { Debug.WriteLine(anInt); };
            // With cast: anonymous methods can be inferred successfully.
            var better2 = (Action<int>)delegate(int anInt) { Debug.WriteLine(anInt); };
            
            // Invalid! Method groups can't be inferred, because they are "typeless expressions"
            // after the C# standard. 
            //var invalid3 = Main;
            // With cast: method groups can be inferred successfully.
            var better3 = (Action<string[]>)Main;
            
            // Invalid! The type can not be inferred form the array initializer alone.
            //var invalid4 = { 1, 2, 3 };
            // Fine. Just add the new[] (implicitly typed array) keyword and inference will
            // succeed. 
            var better4 = new[] { 1, 2, 3 };
            // Invalid! Cannot use a local variable before it is declared. Such expressions don't
            // work in order to forbid constructs like this:
            //var s = (s = 10);                     // What should s' type be?
            //var t = (Foo() ? t = "test": t = 15); // What should t's type be?
            

            /*-----------------------------------------------------------------------------------*/
            // Examples of implicit Typing in Control Structures and Scopes:

            // Implicit typing can be used for the counter variable in for loops.
            for (var i = 0; i < 10; ++i)
            {
                Debug.WriteLine(string.Format("Item: {0}", i));
            }

            // But at least when used with LINQ, implicit typing becomes valueable with foreach
            // loops. - Here a simple string array is used w/o LINQ in foreach yet.
            foreach (var name in new[] { "Karen", "Minnie", "Trudy" })
            {
                Debug.WriteLine(string.Format("Name: {0}", name));
            }

            // Implicit typing is handy within using statements.
            using (var reader = File.OpenText("test.txt"))
            {
                int firstCharacter = reader.Peek();
            }

            // Implicit typing works with pointer types as well.
            int value = 42;
            unsafe
            {
                // The variable intPointer will be inferred to be of type int*.
                var intPointer = &value;

                int[] values = new int[5];
                // Invalid! Implicitly typed variables cannot be fixed.
                //fixed (var anotherIntPointer = &values[2])
                //{
                //}
            }
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            ImplicitTyping(args);
        }
    }
}