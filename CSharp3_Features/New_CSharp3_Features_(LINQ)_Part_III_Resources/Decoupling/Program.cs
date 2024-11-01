using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

// These examples show:
// - The extended type inference abilities of generic methods when used with anonymous methods.
// - How to gradually decouple functionality to improve reusability of methods.
// - How injected code can disturb deferred execution when it throws exceptions.
namespace Decoupling
{
    internal static class Utilities
    {
#region Methods for GraduallayDecoupling():
        /// <summary>
        /// Pass a List of int to this method and the even values of that List will be printed to
        /// the console.
        /// </summary>
        /// <param name="values">Represents the List of int values.</param>
        internal static void FilterOdds(List<int> values)
        {
            foreach (var item in values)
            {
                if (0 == (item % 2))
                {
                    Debug.WriteLine(item);
                }
            }
        }
        // The problem of FilterOdds is that it's not reusable. It contains three responsibilities:
        // iterating over the values, filter only the even values and print them on the console. By
        // the way this algorithm is cast in stone to work with ints only. It can be improved.


        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">An IEnumerable of T to filter.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An IEnumerable of T that contains elements from the input sequence that
        /// satisfy the condition.</returns>
        internal static IEnumerable<T> FilterAll<T>(this IEnumerable<T> source,
            Predicate<T> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }
        // In this function much things have been improved:
        // - The iteration have been separated from the filter activity by delegation to predicate.
        //   The implementation of the filter has to be provided by the caller.
        // - The type of the sequence's items is no longer frozen to type int, because FilterAll()
        //   is a generic method.
        // - The filtered sequence is generated in a deferred manner.
        // - It is an extension method that can be called on each object whose type implements
        //   IEnumerable of T.
        // As can be seen in this implementation the possible side effect was somewhat removed (you
        // could execute side effects in predicate). That means that the caller of this method
        // would have to call another method creating a side effect on each item of the returned
        // filtered sequence. -> Possibly by chaining the result to the next extension method
        // directly.


        /// <summary>
        /// Performs the specified action on each element of the sequence that fulfills the
        /// predicate.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">An IEnumerable of T to filter.</param>
        /// <param name="action">A function to perform on each element. If this parameter is null,
        /// no action will be performed.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        internal static void ForEachIf<T>(this IEnumerable<T> source, Action<T> action,
            Predicate<T> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    if (null != action)
                    {
                        action(item);
                    }
                }
            }
        }
        // This method does just serve some demonstration purposes. Here we didn't really merge the
        // responsibilities again, but we left the "door open" for filtering and side effects. Note
        // that to the parameter action the value null can be passed, in which case no side effect
        // will be performed (read: action will not be called)! 
#endregion
    }


    public class Program
    {
#region Methods and Types for ExtendedTypeInference():
        // Delegate usable for any non-parameter function returning a generic type.
        private delegate T MyFunc<T>();


        // This generic method accepts a Delegate instance of type MyFunc<T> and prints the result
        // of the Delegate instance's invocation to the console.
        private static void WriteToConsole<T>(MyFunc<T> function)
        {
            Debug.WriteLine(function());
        } 
#endregion


        private static void ExtendedTypeInference()
        {
            /*-----------------------------------------------------------------------------------*/
            // Extended Type Inference Abilities for Generic Methods accepting Delegate Instances
            // as parameters are the Basis for the Usefulness of Lambda Expressions:

            // This call is ok in C#3, because WriteToConsole's type parameter T can be inferred to
            // be an int out of the type of the returned value of the passed anonymous method. This
            // type inference does also work for parameter types. In C#2 the type of T could _not_
            // have been inferred from its usage!
            WriteToConsole(() => 42); // which boils down to this:
            WriteToConsole(delegate { return 42; });

            // In C#2 you would have to write this:
            WriteToConsole<int>(delegate { return 42; });
            // or this to provide the return type explicitly:
            WriteToConsole((MyFunc<int>)delegate { return 42; });
        }


#region Methods and Types for MoreExtendedTypeInference():
        // An example function, whose parameter types should be inferred by the compiler.
        private static void PrintConvertedValue<T, U>(T input, Converter<T, U> converter)
        {
            Debug.WriteLine(converter(input));
        }

        // An example function, whose Delegate-parameter's return type should be inferred by the
        // compiler.
        private static void WriteResult<T>(MyFunc<T> function)
        {
            Debug.WriteLine(function());
        }
#endregion


        private static void MoreExtendedTypeInference()
        {// Examples and explanation taken from "CSharp in Depth" by Jon Skeet.
            /*-----------------------------------------------------------------------------------*/
            // Shows more of the extended Type Inference present in C#3:


            /*-----------------------------------------------------------------------------------*/
            // Combined Type Inference of Parameter Types and Return Types:

            // In fact the rules to allow this extended C#3 type inference are far more complex.
            // E.g. all the used type arguments are taken into consideration by the C#3 compiler.
            // The compiler performs a repeated two-phase type inference process to conclude the
            // fixed types as it analyzes their uses in all the parameters of a method together as
            // well as all the return statements.

            // For example for this call:
            PrintConvertedValue("A string", s => s.Length);
            // A C#3 compiler would infer T to be of type string and U to be of type int. This
            // inference would fail with a C#2 compiler, because the C#2 compiler analyzed the type
            // arguments _individually_ and couldn't transfer a fixed type of one type argument to
            // the next.


            /*-----------------------------------------------------------------------------------*/
            // Conclusive Type Inference of Return Types:

            // For example for this call:
            WriteResult(delegate
            {// Code returning an integer or an object depending on the time of day:
                if (DateTime.Now.Hour < 12)
                {
                    return 10;
                }
                else
                {
                    return new object();
                }
            });
            // The compiler uses the same logic to determine the return type in this situation as
            // it does for implicitly typed arrays. It creates a set of all the types from the
            // return statements in the body of the anonymous method (in this case int and object)
            // and checks to see if exactly one of the types can be implicitly converted to from
            // all the others. There’s an implicit conversion from int to object (via boxing) but
            // not from object to int, so the inference succeeds with object as the inferred return
            // type. If there are no types matching that criterion (or more than one), no return
            // type can be inferred and you’ll get a compilation error.
        }


        private static void GraduallayDecoupling(List<int> values)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling FilterOdds: 

            // The call of the method FilterOdds() does, well, everything. It iterates the values,
            // filters them and prints them on console on its sole behalf. In effect we as calles
            // can not control any of this responsibilties.
            Debug.WriteLine("Result of Utilities.FilterOdds():");
            Utilities.FilterOdds(values);


            /*-----------------------------------------------------------------------------------*/
            // Calling FilterAll:

            // On calling FilterAll() we can control the filter criterion on our behalf and we have
            // to do the printing to the console ourselfs as well. - The responsibilities are
            // clearly manifested: FilterAll() does only provide the filtered values and the caller
            // does the rest. The implementation of the Predicate is passed as lamdba expression on
            // the fly.
            Debug.WriteLine("Result of Utilities.FilterAll():");
            foreach (var item in Utilities.FilterAll(values, i => 0 == (i % 2)))
            {
                Debug.WriteLine(item);
            }

            // FilterAll() was implemented as extension method of IEnumerable of T. So we can call
            // it _on_ values, because List of T implements IEnumerable of T and values is extended
            // as well.
            Debug.WriteLine("Result of IEnumerable<T>.FilterAll():");
            foreach (var item in values.FilterAll(i => 0 == (i % 2)))
            {
                Debug.WriteLine(item);
            }

            // To be frank the functionality of method FilterAll() is already present as extension
            // method of IEnumerable of T in the .Net framework, it is called Where().
            Debug.WriteLine("Result of IEnumerable<T>.Where():");
            foreach (var item in values.Where(i => 0 == (i % 2)))
            {
                Debug.WriteLine(item);
            }


            /*-----------------------------------------------------------------------------------*/
            // Calling ForEachIf:

            // As can be seen this call resembles much the call of FilterOdds(), but here the
            // caller can control each responsibility by delivering code in Delegates. - In this
            // call we use lambdas to implement the Delegates on the fly. Note that this method
            // is not an iterator function, so it will be executed eagerly.
            Debug.WriteLine("Result of IEnumerable<T>.ForEachIf():");
            values.ForEachIf(i => Debug.WriteLine(i),
                             i => 0 == (i % 2));
        }


        private static void CodeInjectionExceptions(IEnumerable<int> values)
        {
            /*-----------------------------------------------------------------------------------*/
            // Injected Code can disturb deferred Execution, when it throws Exceptions:

            // Code being injected (e.g. as lambdas) into the decoupled methods can throw 
            // Exceptions itself! The result is generated in a deferred manner, so the Exception
            // will not throw, when it is generated and assigned to the variable respectively.
            var result = values.Where(i =>
            {
                if (0 == (i % 2))
                {
                    return true;
                }
                throw new Exception("Pardon? There are odd numbers contained?");
            });
            Debug.WriteLine("Result of IEnumerable<T>.Where():");
            // On iteration of the result the execution will crash on the 2nd item, the number 1,
            // because it is odd. The injected code can't cope with odd numbers.
            foreach (var item in result)
            {
                Debug.WriteLine(item);
            }

            // Sidebar: This is a downside of decoupling: establishing a proper error handling can
            // be hard as you don't know about the side effects of injected code.
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Our Test Data to play with:

            var values = new List<int>{ 4, 5, 6, 2, 5, 7, 6, 8, 3, 8, 3,
                                        4, 6, 4, 6, 5, 3, 2, 1, 4, 3, 8, 
                                        7, 6, 5, 3, 2, 6, 5, 4, 3, 2, 1 };


            /*-----------------------------------------------------------------------------------*/
            // Calling the Test Methods:

            ExtendedTypeInference();

            GraduallayDecoupling(values);

            CodeInjectionExceptions(values);

            MoreExtendedTypeInference();
        }      
    }
}