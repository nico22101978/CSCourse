using System;
using System.Linq;
using System.Diagnostics;
// This example shows:
// - How an algorithm operating on data can be expressed in C# with IEnumerable<T> sequences,
//   extension methods and chaining.
namespace ExtensionMethodsAlgorithms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Simply compare this code to the version written with C++ and STL...

            var rnd = new Random(0);
            var list = Enumerable // Get ten iterations.
                .Range(1, 9)
                // Get random (fixed seed) values from one to nine.
                .Select(i => rnd.Next(1, 9))
                // Put only the even values into the result.
                .Where(i => 0 == i % 2)
                // Get only the distinct random values. 
                .Distinct();

            // The chaining notation, which is a feature that came with the introduction of
            // extension methods, allows to write the method calls in the order, in which they are
            // being applied.

            // Mind, that the usage of Where() allows writing simple filtering algorithms w/o the
            // need to write a loop, implement IComparable/<T> or overriding Equals()!

            #region The same Code w/o Chaining:
            // The same code can be expressed with the pre-C#3 way of calling static methods in a
            // cascading manner w/o the ability to chain. Decide yourself, which syntax is more
            // readable?
            var anotherList =
                Enumerable.Distinct(
                    Enumerable.Where(
                        Enumerable.Select(
                            Enumerable.Range(1, 9),
                        i => rnd.Next(1, 9)),
                    i => 0 == i % 2)
                ); 
            #endregion

            // Sidebar: The chaining notation, which is a feature that came with the introduction
            // of extension methods, allows to write the method calls in the order in which they
            // are being applied. Extension methods simplify to call methods on the returned objects
            // the result is what we call "fluent interface". JQuery and LINQ to XML do also exploit
            // fluent interfaces. Extension methods to built fluent interfaces can be a first step
            // to create so-called "Domain Specific Languages (DSLs)". 
            //  * Fluent interfaces in code do also have some downsides: in the chaining notation,
            //    we can often not debug intermediate results, without modifying the code (i.e.
            //    tearing the chained expression apart). A Debugger, which supports REPL and/or
            //    immediate evaluation might help here. -> But be careful, expressions "just tested"
            //    in the debugger may have side effects, which affects the debug-session!
            //    If you will, this downside is allso shared with cascading-style coding, were we use
            //    the result of a call directly as an argument for the next call. The argument, which
            //    is the immediate result, is gone.

            // This (chaining notation) code does explicitly describe what it does, it is really
            // declarative code. Because the calls are chained, there is no chance that any
            // independent information (as on C++'s iterators) can be lost. The chaining syntax
            // feels uniform. Finally we'll just print the generated data to the console:
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }
        }
    }
}
