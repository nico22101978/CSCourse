using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

namespace NamedArgumentsAndOptionalParameters
{
    public static class Program
    {
        #region Examples for PositionalAndNamedArguments():
        public static void AMethod(int foo)
        {
        }


        public static void AMethod(int foo, int bar)
        {
        }


        public static void AMethod(int foo, int bar, int oog)
        {
        }


        public static void AMethod(int foo, ref int bar)
        {
            bar = 42;
        }


        private static int WithSideEffectA()
        {
            Debug.WriteLine("WithSideEffectA");
            return 42;
        }


        private static int WithSideEffectB()
        {
            Debug.WriteLine("WithSideEffectB");
            return 21;
        }
        #endregion


        #region Examples for Optional Parameters:
        /*---------------------------------------------------------------------------------------*/
        // Enhanced Expressiveness with Optional Parameters:

        public static void AMethodWithOverload(int anInt, string aString)
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOverload({0}, {1})", anInt, aString));
        }


        public static void AMethodWithOverload(string aString, int anInt)
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOverload({0}, {1})", aString, anInt));
        }


        public static void AMethodWithOverload(int anInt)
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOverload({0}, a constant string)", anInt));
        }

        // The two overloads of AMethodWithOverload() can be expressed with optional parameters.
        // Here the value of aString will default to "a constant string" if no argument was
        // provided.
        public static void AMethodWithOptionalParameter(int anInt, string aString = "a constant string")
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter({0}, {1})", anInt, aString));
        }


        /*---------------------------------------------------------------------------------------*/
        // What is possible with Optional Parameters:

        // The default values can be any kind of compile time constant like literals, const members

        public static void AMethodWithOptionalParameter2(int anInt = 42, string aString = "a constant string")
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter2({0}, {1})", anInt, aString));
        }


        // Also ok as default value for the optional parameter: a null literal.
        public static void AMethodWithOptionalParameter3(int anInt = 42, string aString = null)
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter3({0}, {1})", anInt, aString));
        }
        // Also ok as default value for the optional parameter: the default value of a reference
        // type (that compiles to null).
        public static void AMethodWithOptionalParameter4(int anInt = 42, string aString = default(string))
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter4({0}, {1})", anInt, aString));
        }

        // Invalid! Default parameter value for 'aString' must be a compile time constant.
        //public static void AMethodWithOptionalParameter5(int anInt = 42, string aString = string.Empty)
        //{
        //    Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter5({0}, {1})", anInt, aString));
        //}
        //OK! For empty strings you have to use "" rather than string.Empty, because later is not a
        // compile time constant but a static property.
        public static void AMethodWithOptionalParameter5(int anInt = 42, string aString = "")
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter5({0}, {1})", anInt, aString));
        }

        // Or a parameterless ctor of a value type,...
        public static void AMethodWithOptionalParameter5(int anInt = 42, DateTime aDateTime = new DateTime())
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter5({0}, {1})", anInt, aDateTime));
        }
        // ...which is equivalent to using the default value of a value type.
        public static void AMethodWithOptionalParameter6(int anInt = 42, DateTime aDateTime = default(DateTime))
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter6({0}, {1})", anInt, aDateTime));
        }
        // The application of default values of a type is handy when used in generic methods.
        public static void AMethodWithOptionalParameter6<T>(int anInt = 42, T aSomething = default(T))
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter6({0}, {1})", anInt, aSomething));
        }


        // Invalid! Optional parameters must appear after all required parameters.
        //public static void AMethodWithOptionalParameter(int anInt = 42, string aString)
        //{
        //    Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter({0}, {1})", anInt, aString));
        //}
        //  There is one exception: params-arrays must appear after all optional parameters.
        public static void AMethodWithOptionalParameter(int anInt, string aString = "aString", params string[] remainder)
        {
            Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter({0}, {1}, {2})", anInt, aString, string.Join(",", remainder)));
        }


        // Invalid! Default parameter value for 'remainder' must be a compile time constant.
        //public static void AMethodWithOptionalParamsParameter(int anInt, string aString = "aString", params string[] remainder = new []{"2", "3"})
        //{
        //    Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter({0}, {1}, {2})", anInt, aString, string.Join(",", remainder)));
        //}


        // Invalid! Default parameter value for 'aString' must be a compile time constant (this includes string literals as well as null literals).
        //public static void AMethodWithOptionalParameter(int anInt, string aString = Console.ReadLine())
        //{
        //    Debug.WriteLine(string.Format("Calling AMethodWithOptionalParameter({0}, {1})", anInt, aString));
        //}


        // Invalid! A ref or out parameter cannot have a default value. Notice, that this is allowed
        // for generated code for interop assemblies, when being compiled with VS 2010's csc
        // compiler for C#4.
        //public static void AMethodWithOptionalRefParameter(ref string aString = "aString")
        //{
        //    aString = "anotherString";
        //}

        // Invalid! Cannot specify a default value for the 'this' parameter.
        //public static string ConcatWith(this string that = "Hello", string aString = ", World!")
        //{
        //    return that + aString;
        //}


        /*---------------------------------------------------------------------------------------*/
        // Optional Parameters and Constructor Type Constraints of Generics:

        // The parameter t must of a type having a parameterless ctor (as to the constraint).
        private static void AGenericMethod<T>(T t) where T : new()
        {
        }


        public class ClassWithCtorOptionalArgument
        {
            // The only ctor of this class has only optional parameters!
            public ClassWithCtorOptionalArgument(int i = 0)
            {
            }
        }


        private static void OptionalArgumentAndGenericConstraint()
        {
            // Invalid! A ctor having only optional parameters is not accepted as parameterless
            // ctor by the C#4 compiler; the constructor type constraint of AGenericMethod has been
            // violated!
            //AGenericMethod(new ClassWithCtorOptionalArgument());
        }
        #endregion


        #region Examples for CombineNamedArgumentsAndOptionalParameters():
        // A simple method that does nothing but accepts two optional parameters.
        private static void DoSomething(int foo = 0, int bar = 0)
        {
        }
        #endregion


        private static void PositionalAndNamedArguments()
        {
            /*-----------------------------------------------------------------------------------*/
            // The basic Idea:

            // On calling a method you'll pass the arguments in the order their matching parameters
            // appear in the declaration. This is called "positional argument passing":
            AMethod(42);

            // In C#4 you can name the parameters and "associate" to them the matching argument
            // explicitly with following syntax. This is called "named argument passing":
            AMethod(foo: 42);
            // You can of course not only pass literals, but also expressions of any kind:
            AMethod(foo: Environment.GetLogicalDrives().Length);

            // Invalid! The _index operator_ of arrays can not be called with named arguments,
            // because its parameter has no official name.
            // string aDrive = Environment.GetLogicalDrives()[?: 0];

            // But you can use the _indexer_ with named arguments.
            IList<string> drives = Environment.GetLogicalDrives().ToList();
            string aDrive = drives[index: 0];

            // So far this is not spectacular, but if you have more than one argument to pass to
            // your method you have to memorize the order of their matching parameters:
            AMethod(42, 21);

            // With named argument passing you now have the freedom to decide the order of the
            // arguments to be passed.
            // For example you can invoke AMethod(int foo, int bar) like this:
            AMethod(foo: 42, bar: 21);
            // or like that:
            AMethod(bar: 21, foo: 42);

            // If we have two overloads of a method, which only differ in the order of the
            // parameters (i.e. the count, the types and the names of the parameters are equal), we
            // can not call them with the named parameter syntax:
            //AMethodWithOverload(anInt: 42, aString: "bar"); // Invalid! Ambiguous call (Did you mean AMethodWithOverload(int anInt, string aString) or AMethodWithOverload(string aString, int anInt)?)!
            //AMethodWithOverload(aString: "bar", anInt: 42); // Invalid! Ambiguous call (Did you mean AMethodWithOverload(int anInt, string aString) or AMethodWithOverload(string aString, int anInt)?)!
            // Instead the positional syntax must be used:
            AMethodWithOverload(42, "bar"); /*or:*/
            AMethodWithOverload("bar", 42);


            /*-----------------------------------------------------------------------------------*/
            // Mixing positional and named Argument Passing Syntax:

            // In principle the positional and named argument passing syntaxes can be mixed like
            // this:
            AMethod(42, oog: 56, bar: 21);

            // But the positional arguments must be passed as first items in the argument list, so 
            // this call would cause a compilation error:
            //AType.AMethod(oog: 56, bar: 21, 42);// Invalid! Named argument specifications need to appear after all fixed arguments have been specified.

            // The positional arguments will feed "their" matching parameter first and then the
            // named parameters will have to specify the arguments of the remaining parameters in
            // any order. Having this said following call is Invalid, because the argument "foo" 
            // was already specified with the positional argument 21 and can not be specified in a
            // named manner afterwards:
            //AType.AMethod(21, foo: 42, oog: 56);// Invalid! Named argument 'foo' specifies a parameter, for which a positional argument has already been given.


            /*-----------------------------------------------------------------------------------*/
            // Argument Passing by Reference:

            // It is allowed to pass arguments by reference (i.e. with the keywords out and ref) as
            // named arguments:
            int result = 0;
            AMethod(foo: 42, bar: ref result); // It works the same way with out parameters.


            /*-----------------------------------------------------------------------------------*/
            // Named Arguments with param-Arrays:

            // In short you can use named arguments with param-arrays. But you have to pass arrays
            // as arguments for the param-attributed parameters, not a comma separated list of
            // arguments. - This is clear, because you need to specify the "borders" of the 
            // arguments each if you apply the named arguments syntax.
            // Mind, that you have to pass an explicitly object-typed array as value to the parameter
            // arg. If you would pass an array of type int[] (implicitly or explicitly typed) the
            // C# compiler held this argument as being the _first_ element of a passed params
            // array!
            Console.WriteLine(format: "this {0} and that {1}", arg: new object[] { 42, 21 });
            Console.WriteLine(arg: new object[] { 42, 21 }, format: "this {0} and that {1}");


            /*-----------------------------------------------------------------------------------*/
            // Named Arguments to Control the Order of Execution/Evaluation on Method Calls:

            // Sometimes the expressions being used as arguments for methods perform side effects.
            // In such cases it is often relevant, in which order these side effects are being
            // performed. Let's assume that we want to call AMethod() directly with arguments
            // resulting from method calls to WithSideEffectA() and WithSideEffectB(). The methods
            // WithSideEffectB() and WithSideEffectA() both perfom sideeffects, but we want the
            // sideeffect of WithSideEffectB() to be happen firstly and yield the argument for the
            // second parameter and the sideeffect of WithSideEffectA() to be happen secondly and
            // yield the argument for the first parameter. Give it a first try:
            AMethod(WithSideEffectA(), WithSideEffectB());
            // No, this will not perform the side effects in the order we've desired: In C# the
            // side effects of the expressions as arguments of a method call will be always 
            // performed in the order left to right. - So the positional argument style drives the
            // order of the side effects.

            // Sidebar: In C/C++ the order of execution/evaluation of subexpressions is undefined.
            // (At least in between so called "points of execution".) In principle you should avoid
            // the need to control the order of interleaving side effects in method calls; in C++
            // as well as in C#.

            // With named arguments you can control the order of the side effects and the
            // association of the parameters like so:
            AMethod(bar: WithSideEffectB(), foo: WithSideEffectA());

            // Of course you don't need named arguments to control the order of 
            // execution/evaluation of the arguments on calling a method. You can just use local
            // variables:
            int bar = WithSideEffectB(); // First call WithSideEffectB() and assign it to bar,
            int foo = WithSideEffectA(); // then call WithSideEffectA() and assign it to foo.
            AMethod(foo, bar);           // Now AMethod() can be called, the order in which the
                                        // arguments are being passed doesn't matter, because the
            // sideeffects of WithSideEffectA() and WithSideEffectB()
            // ran in the desired order when the variables foo and bar
            // have been assigned.


            /*-----------------------------------------------------------------------------------*/
            // Calling anonymous Functions with Named Arguments:

            // An anonymous function:
            Action<string> doIt = delegate(string s)
            {
                Debug.WriteLine(s);
            };

            // It is possible to pass named arguments to anonymous functions, but you can not use
            // the name of the parameter of your anonymous function like that:
            //doIt(s: "as"); // Invalid! The Delegate type 'Action' does not have a parameter named
                             // 's'.
            // Instead you have to use the name of the parameter of the _Delegate type_ (Action<T>
            // in this case); and the name of Action<T>'s parameter happen to be "obj":
            doIt(obj: "as"); // Fine.
        }


        private static void CallingMethodsHavingOptionalParameters()
        {
            /*-----------------------------------------------------------------------------------*/
            // How Methods with optional Parameters are handled by the Compiler:

            // DoSomething() virtually awaits two integer arguments, but the parameters have been
            // provided with default values of 0 respectively So it is possible to call DoSomething() w/o
            // arguments at all:
            DoSomething();

            // But behind the scenes the compiler will fill in the default values of the method
            // definitions on the _caller side_ of the method! So the compiler will really create
            // this call instead of DoSomething():
            DoSomething(0, 0);            
        }


        private static void CombineNamedArgumentsAndOptionalParameters()
        {
            /*-----------------------------------------------------------------------------------*/
            // Expressiveness of the Combination of Named Argument Passing and Optional Parameters:

            // Many ways to invoke the method DoSomething() (having only one method w/o overloads)
            // are possible in C#4:
            DoSomething();
            DoSomething(42);
            DoSomething(foo: 42);
            DoSomething(bar: 21);
            DoSomething(foo: 42, bar: 21);
            DoSomething(bar: 21, foo: 42);
        }


        #region Code to show Compile Time Binding of optional Parameter's Default Values:
        public interface IVehicle
        {
            void WriteName(string name = "IVehicle");
        }

        public class Car : IVehicle
        {
            public virtual void WriteName(string name = "Car")
            {
                Debug.WriteLine(name);
            }
        }

        public class Bus : Car
        {
            public override void WriteName(string name = "Bus")
            {
                Debug.WriteLine(name);
            }
        }
        #endregion


        #region Code to provoke a Versioning-Problem with optional Parameters:
        // This is a kind of abuse of the optional parameter feature. If we modify the present
        // method AMethodWithOptionalParameter(int anInt, string aString =
        // "a constant string") by introducing another optional parameter just before the present
        // optional parameter, we introduce a versioning problem. E.g. consider this method and see
        // the method OptionalParametersPitfalls().
        public static void AMethodWithOptionalParameter(int anInt,
            string anotherString = "another constant string",
            string aString = "a constant string")
        {
            Debug.WriteLine(
                string.Format("Calling AMethodWithOptionalParameter({0}, {1}, {2})", anInt,
                    anotherString,
                    aString));
        }
        #endregion


        #region Code to be called in an Expression Tree:
        private static int MagicResult(int seed = 0)
        {
            return (seed + 2) * 4;
        }
        #endregion


        private static void OptionalParametersPitfalls()
        {
            /*-----------------------------------------------------------------------------------*/
            // Anonymous Functions can not have optional Parameters:
            
            //Action<string> doIt = delegate(string s = "thing") // Invalid! Default values are not
            //{                                                  // in this context.
            //    Debug.WriteLine(string.Format("{0}", s));
            //};


            /*-----------------------------------------------------------------------------------*/
            // Introducing more Optional Parameters afterwards can cause Versioning Problems:

            // Before we changed AMethodWithOptionalParameter(), this method call:
            AMethodWithOptionalParameter(42, "myString");
            // was translated into following code by the compiler (named syntax):
            /*Generated:*/
            AMethodWithOptionalParameter(anInt: 42, aString: "myString");

            // After we introduced the additional optional parameter, following code will be
            // generated by the compiler (named syntax):
            /*Generated:*/
            AMethodWithOptionalParameter(anInt: 42,
                anotherString: "myString",
                aString: "a constant string");
            // Because the meaning of the second parameter was shifted to the third parameter and
            // both parameters are optional that call will pass the argument "myString" to the
            // wrong parameter.

            // Solution: using named arguments make the code versioning safe:
            AMethodWithOptionalParameter(42, aString: "myString");
            // With this call the parameter aString will associated with the argument "myString"
            // explicitly.

            // Sidebar: If a method of a referenced assembly was modified: Adding new parameters on
            // public methods, even having default arguments, will break at run time, if callers
            // don't recompile! - This means, that only methods that have been modified in the same
            // assembly can go really _silently_ incompatible by using wrong or "shifted" default
            // values.


            /*-----------------------------------------------------------------------------------*/
            // The Values of optional Parameters will be bound at Compile Time: 

            Bus bus = new Bus();
            Car busAsCar = bus;
            IVehicle busAsIVehicle = bus;

            bus.WriteName();            // Will print "Bus".
            busAsCar.WriteName();       // Will print "Car".
            busAsIVehicle.WriteName();  // Will print "IVehicle".
            // This is true, because the values of the optional parameters are taken from the
            // called overloads of the methods at compile time, _not_ from the overridden methods
            // at run time. So the compiler does really create following code:
            // Generated:
            bus.WriteName("Bus");               // Will print "Bus".
            busAsCar.WriteName("Car");          // Will print "Car".
            busAsIVehicle.WriteName("IVehicle");// Will print "IVehicle".


            /*-----------------------------------------------------------------------------------*/
            // Expression Trees:

            // Invalid! An expression tree may not contain a call or invocation that uses optional
            // arguments. This is not allowed as we are trying to call MagicResult() w/o argument.
            //Expression<Func<int, int>> expression = i => MagicResult();
        }


        #region Code for SolvingVersioningProblems():
        // We want to exploit the optional parameter feature by letting this method work with the
        // current DateTime. We can not assign DateTime.Now as a default value for the optional
        // parameter, because it is not a compile time constant. Instead we can assign another
        // compile time constant to the optional parameter, whose value we can reinterpret in the
        // implementation of the method. The versioning problem vanished completely.
        // For most cases we can use the null value for this purpose. Then we have to make the type
        // of the respective optional parameters nullable, if they are value types. Mind that you then
        // loose the ability to pass null as a "legal" value.
        public static void PrintDateTime(DateTime? theTime = null)
        {
            // The null coalescing operator comes in handy:
            DateTime timeToPrint = theTime ?? DateTime.Now;
            Debug.WriteLine(timeToPrint);
        }
        #endregion


        private static void SolvingVersioningProblems()
        {
            /*-----------------------------------------------------------------------------------*/
            // Solving Versioning Problems with Default Value Interpretation:

            // Let's print the current time as a default:
            PrintDateTime();

            // Let's print the time being explicitly passed:
            PrintDateTime(DateTime.Now - TimeSpan.FromSeconds(10));
        }


        #region Code for NameResolutionNamedArgumentsAndOptionalParameters():        
        private static void OverloadedMethod(int x = 21)
        {
            Debug.WriteLine(string.Format("x: {0}", x));
        }


        private static void OverloadedMethod(int x = 21, int y = 42)
        {
            Debug.WriteLine(string.Format("x: {0}, y: {1}", x, y));
        }
        #endregion


        private static void NameResolutionNamedArgumentsAndOptionalParameters()
        {
            /*-----------------------------------------------------------------------------------*/
            // Shows how the Compiler resolves Method Overload when named Arguments and optional
            // parameters come into play:

            //OverloadedMethod();   //AMBIGUOUS! The compiler can not prefer any of the present
            // overloads.
            OverloadedMethod(1);    // Will call OverloadedMethod(1), because in only uses
            // positional argument passing and no further application of a 
            // default argument is needed, which is preferable to any other
            // overload.
            OverloadedMethod(y: 2); // Will call OverloadedMethod(21, 2), because a parameter named
            // y is only present in that overload.
            OverloadedMethod(1, 2); // Will call OverloadedMethod(1, 2), because this is the only 
            // matching overload (having two int parameters).
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            PositionalAndNamedArguments();


            CallingMethodsHavingOptionalParameters();


            OptionalParametersPitfalls();


            SolvingVersioningProblems();


            CombineNamedArgumentsAndOptionalParameters();


            NameResolutionNamedArgumentsAndOptionalParameters();
        }
    }
}