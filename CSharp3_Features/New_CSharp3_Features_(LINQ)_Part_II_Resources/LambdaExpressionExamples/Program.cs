using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq.Expressions;
using System.Diagnostics;

// These examples show:
// - How lambda expressions can express Delegate instances.
// - An implementation of the logical-or-operator with deferred execution.
// - Some everyday usage examples of lambda expressions.
// - The basic usage of expression trees.
namespace LambdaExpressionExamples
{
    public class Program
    {
        #region Types for LambdaDefinition():
        delegate void WithRefParameter(ref int anInt);
        #endregion


        private static void LambdaDefinition()
        {
            /*-----------------------------------------------------------------------------------*/
            // Lambda Expressions to express Delegate Instances:

            // Here we have a simple Delegate instance of type Predicate<int>.
            Predicate<int> isEven = delegate(int anInt)
            {
                return 0 == (anInt % 2);
            };
            bool b = isEven(67);

            // In the first phase of lambda expression application we express the same Delegate
            // instance as in isEven, but as lambda expression. In principle we got rid of the
            // delegate keyword and added the lambda operator =>, called "big arrow" or "rocket operator",
            // which resembles the mathematical functional "goes to" arrow.
            Predicate<int> isEven2 = (int anInt) =>
            {
                return 0 == (anInt % 2);
            };

            // In the next phase we remove the type name of the parameter anInt. The C#3 compiler
            // is able to infer the parameter type from the Delegate type. This is an all-or-none
            // decision, either all parameters must be explicitly typed or none. When using out or
            // ref parameters we're forced to use explicit typing! If there is only one
            // parameter the parameter parentheses can be removed as well.
            Predicate<int> isEven3 = anInt =>
            {
                return 0 == (anInt % 2);
            };

            // The final phase of this lambda expression gets rid of the return statement and gets
            // rid of the braces. This syntax (no return statement, no semicolon and no braces)
            // is required, when the body of the lambda expression consists of just one expression
            // making up the result of the lambda expression.
            // Mind, that the lambda expression pursues one main target: compactness but
            // readability.
            // In this example the mathematical notation of a function becomes very obvious: 
            // n -> 0 = n rem 2. When a lambda expression is made up from just one expression it is
            // called "expression lambda".
            Predicate<int> isEven4 = anInt => 0 == (anInt % 2);

            // Occasionally a further set of parentheses around the whole lambda expression
            // enhances the readability somewhat.
            Predicate<int> isEven4a = (anInt => 0 == (anInt % 2));
            

            /*-----------------------------------------------------------------------------------*/
            // Sometimes the Types of the Parameters must be specified explicitly:

            // As can be seen if ref (or out) parameters are used in a lambda expression we
            // have to apply explicit typing for _all_ parameters. Furthermore if we want to use
            // ref or out types we have to create an explicit Delegate type (WithRefParameter in
            // this case), because we can not instantiate generics with ref or out types (e.g. we
            // can not write Action<ref int>).
            WithRefParameter makeEven = (ref int anInt) =>
            {
                if (0 != anInt % 2)
                {
                    ++anInt;
                }
            };


            /*-----------------------------------------------------------------------------------*/
            // Recursive Lambdas:

            // There is no direct way to express recursive calls of a lambda function. But we can
            // simulate this by the usage of a Delegate reference and the separation of declaration
            // and assignment:

            Func<int, int> fac = null;
            fac = n => (n <= 1) ? 1 : n * fac(n - 1);

            // Sidebar: In JavaScript we can call an anonymous function recursively by calling
			// arguments.callee within the anonymous function. Btw.: arguments.callee is deprecated.


            /*-----------------------------------------------------------------------------------*/
            // The Type of Lambda Expressions:

            // Lambda expressions (and anonymous functions in general) have no type, so implicit
            // typing with the var keyword is _not_ allowed.

            // Invalid! A lambda expression can not be implicitly typed.
            //var fun = anInt => 0 == (anInt % 2); 
            // Invalid! An anonymous method can not be implicitly typed.
            //var fun = delegate(int anInt) { return 0 == (anInt % 2); }; 

            // We have always to use a Delegate types as type for a symbol displaying an anonymous
            // function.
            Predicate<int> isEven5 = anInt => 0 == (anInt % 2); // Fine
            Predicate<int> isEven5b = delegate(int anInt) { return 0 == (anInt % 2); }; // Fine

            // Additionally lambda expression do comply to the type LambdaExpression. But this
            // compliance is only present to use lambda expressions as building blocks in
            // Expression trees. -> See below!
        }


        #region Methods for TheOrFunction:
        private static bool CheapMethod()
        {
            return true;
        }

        private static bool ExpensiveMethod()
        {
            return true;
        }

        private static bool OrElse(bool lhCondition, Func<bool> alternativeFunction)
        {
            // If the left hand condition is met, return true (i.e. perform the shortcut):
            if (lhCondition)
            {
                return true;
            }

            // If and only if the left hand condition was _not_ met, _evaluate_ the right hand
            // condition and return the result of right hand condition:
            return alternativeFunction();
        }
        #endregion


        private static void TheOrFunction()
        {
            /*-----------------------------------------------------------------------------------*/
            // Implementation of the logical-or-Operator with deferred Execution:

            // Inspired by the implementation of the method or: in Smalltalk using blocks. 

            // Imagine we want to program a method, which simulates the logical-or-operator of C#.
            // It can not be done with an ordinary method that accepts two bool arguments like so:
            //Assume: bool OrElse(bool lhCondition, bool rhCondition);
            //bool result = OrElse(CheapMethod(), ExpensiveMethod()); // NO!
            // The problem: both methods CheapMethod() and ExpensiveMethod() need to be called, as
            // both arguments need to be evaluated _before_ the method OrElse() is called. Net
            // effect: we can not make use of the _short circuit evaluation_ of the
            // logical-or-operator, because ExpensiveMethod() will always be called, it doesn't
            // matter, if CheapMethod() evaluates to true or false!

            // The solution: deferred execution! Rewrite OrElse() to accept a _Delegate instance_
            // as second parameter! The second parameter will then only be called _within the
            // implementation of OrElse()_ if the first parameter happens to evaluate to true.
            // OrElse() can be called with a lambda expression as second parameter to retain a terse
            // syntax like so:
            bool result = OrElse(CheapMethod(), () => ExpensiveMethod()); // Fine!
            // or so, by exploiting the method group conversion:
            result = OrElse(CheapMethod(), ExpensiveMethod);
            // It is crucial to understand, that ExpensiveMethod() is _not_ executed in the _call_ expression
            // of OrElse() in place! Instead a lambda, which calls ExpensiveMethod() is passed to OrElse(). And
            // OrElse() _maybe_ decides in its implementation to call this lambda (accessible via its
            // second parameter alternativeFunction), which then effectively calls ExpensiveMethod().

            // Sidebar: What we have here is called the "execute-around pattern", in which a yet
            // to defined block of code is executed after a "head-code" and before a "tail-code" is
            // executed. The expected block of code is accepted as parameter of a function (e.g OrElse())
            // and this parameter is of a delegate type. The caller can just pass a lambda-argument to
            // satisfy the block-parameter to be executed in the midst of the surrounding code.
            // The "execute-around pattern" also underscores the usage of lambdas for (simple)
            // _behavior parametrization_. - I.e. we use lambdas as arguments for methods to control their
            // behavior, in that the method being called just directly calls the passed function in an
            // appropriate place in its algorithm. The code being called is just passed as argument and is
            // executed somewhere in the method (but not in the moment it is passed).

            // Sidebar: The presented method OrElse() is exactly the way the Boolean>>or:-method works in Smalltalk
            // with blocks (a block is Smalltalk's equivalent of lambdas).
        }


        private static void LambdaEverydayUsage()
        {
            /*-----------------------------------------------------------------------------------*/
            // New Delegate Types to simplify Usage of Delegates and Lambda Expressions
            // Functionality and Lambda Expressions w/o Parameters:

            // In order to make programming with functional paradigms more concise, the Delegate
            // type Func, a Delegate with a generically typed return value and five generic
            // function parameter overloads, was introduced in .Net 3.5. The Delegate type
            // Predicate<int> can be expressed as Func<int, bool>.
            Predicate<int> isEven1 = (anInt => 0 == (anInt % 2));// Can be expressed as:
            Func<int, bool> isEven2 = (anInt => 0 == (anInt % 2));
            // I.e. the same lamdba expression on the right side can be hold by variables of matching
            // Delegate type. - This is sometimes called "target typing".

            // Here is an example of a lambda expression that defines a Delegate w/o parameters.
            // Notice, that we have to write an empty set of parentheses instead of the parameters
            // respectively.
            Func<int> returns42 = () => 42;


            /*-----------------------------------------------------------------------------------*/
            // We can not leave the Parameter List away on Lambdas:

            // Even if we don't use the parameter(s) being passed to a lambda, we have to declare
            // them in the parameter list, so this is invalid:
            //Action<int> actionOnDoesntMatter = => Console.WriteLine("Bang!"); 
            // Instead of using a lambda we have to switch back to the anonymous methods' syntax
            // to get rid of the unused parameter list:
            Action<int> actionOnDoesntMatterBetter = delegate { Console.WriteLine("Bang!"); };


            /*-----------------------------------------------------------------------------------*/
            // Partial Application:

            // In functional programming languages it is possible to bind variables or constants to
            // a parameter of a present function. The result of this bind operation is a new
            // function. This way to create new functions by binding parameters is often called
            // "partial application".

            // C# does not support a dedicated syntax for partial application. It can be simulated by
            // creating a new Delegate instance from a call of the bound function with a fixed
            // argument variable or constant.

            // 1. Partial application: Bind a constant to a parameter.

            // The function to apply partially:
            Func<int, int, int> canAddTwoInts = (a, b) => a + b;
            // The function called with a constant four: canAddTwoInts() will be partially applied
            // and the result is a new function addFour() having only one parameter:
            Func<int, int> addFour = (a) => canAddTwoInts(a, 4);
            int five = addFour(1);


            // 2. Partial application: We can also bind a variable to a parameter.

            // The argument as variable:
            int variableArgument = 3;
            // The function called with a variable: canAddTwoInts() will be partially applied and
            // the result is a new function addX() having only one parameter:
            Func<int, int> addX = (a) => canAddTwoInts(a, variableArgument);
            int seven = addX(4);
            // The argument can be "rebound" by simply setting variableArgument to another value.
            // This is possible, because addX() binds to the variables used in its implementation.
            // A function that binds to its contextual symbols is called "closure" (see below).
            variableArgument = 4;
            int eight = addX(4);    // Yes, the result is eight, because the context of addX() has
                                    // been modified. Decide yourself: bug or feature?


            /*-----------------------------------------------------------------------------------*/
            // Currying:

            // Also do functional programming languages provide means to slash a function having
            // multiple parameters into a function having e.g. only one parameter. - This returned
            // function does then accept one parameter (the first parameter of the original
            // function) and returns another function accepting the second parameter of the
            // original function and so forth...
            // The process of turning a function having multiple parameters into a function
            // accepting only one parameter, but being chained with other functions accepting the 
            // remaining parameters is called "currying" (named after the American mathematician
            // Haskell Brooks Curry) or "Schönfinkeln" (named after the Russian mathematician Moses
            // Ilyich Schönfinkel).
            // In mathematical theory and functional programming practice the term currying is
            // strongly tied with the notion of lambdas. - It is easier to be understood if we
            // read the following example.

            // C# does not support a dedicated syntax for currying. It can be simulated by
            // creating a new Delegate instance from a call of the bound function with a fixed
            // argument variable or constant.

            // The function to curry:
            Func<int, int, int> add = (x, y) => x + y;

            // Perform the currying and slash add() to a chain of two functions:
            Func<int, Func<int, int>> curriedAdd = x => y => add(x, y);

            // Create a function that adds one to its sole parameter:
            Func<int, int> increment = curriedAdd(1); // The function y => add(4, y) is created.
            int alsoFive = increment(4); // Here add(1, 4) is finally executed. Isn't that nice?


            /*-----------------------------------------------------------------------------------*/
            // Statement Lambdas: Lambda Expressions w/o return Values and EventHandlers:

            // Instead of functions that return values, also code fragments that produce side
            // effects can be used in lambda expressions. A lambda expression's code block can
            // contain none, one or multiple statements (none and multiple statements must be
            // written within a pair of braces). We call them "statement lambdas". (The lambda
            // isEven2 above was also a statement lambda, with a single return statement but it
            // could be written as expression lambda as well.) In order to make programming with
            // functional paradigms more concise the Delegate type Action, a Delegate w/o a return
            // value to express side effects, was extended from a single Delegate type to a
            // Delegate type with five generic function parameter overloads in .Net 3.5.
            Action<string> printStringAndLength = aString =>
            {
                string resultString = string.Format("{0} {1}", aString, aString.Length);
                Debug.WriteLine(resultString);
            };
            printStringAndLength("Acme");

            // So it's also possible to define an event handler as a lambda expression! In this
            // case there are two parameters in the lambda expression. - They need to be written
            // comma separated and within a pair of parentheses. But in opposite to methods the
            // types of the parameters need not to be explicitly written. The types of sender and
            // eventArgs are automatically inferred to be object and MouseEventArgs. 
            EventHandler<MouseEventArgs> anEventHandler = (sender, eventArgs) =>
            {
                // Even VS' IntelliSense is able to figure out the type of eventArgs, so the
                // interface of MouseEventArgs can be seen in the editor.
                Debug.WriteLine("Mouse at position: " + eventArgs.Location);
            };
            // The same event handler can be expressed as a statement lambda w/o braces, because
            // it's code block contains only one statement.
            EventHandler<MouseEventArgs> a2ndEventHandler =
                (sender, eventArgs) =>
                    Debug.WriteLine("Mouse at position: " + eventArgs.Location);

            // With statement lambdas we can also express empty lambda bodies:
            Action<Form> emptyLambda = (Form form) => { };


            /*-----------------------------------------------------------------------------------*/
            // Practical Usage of Lambda Expression in Higher-Order Functions to filter some
            // Integers:

            // Let's define a list of integers:
            var someInts = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // With one of the previously defined predicates we can filter all the even integers.
            // Since C#1 it is possible to pass functions or get functions back as return value
            // from other functions in form of Delegate instances. Functions accepting other
            // functions as parameters or returning other function are called higher-order
            // functions in maths.
            foreach (var item in someInts.FindAll(isEven1))
            {
                Debug.WriteLine("Item: " + item);
            }

            // We can also apply a lambda expression directly as predicate to filter the even
            // integers. In C#2 and C#3 it is easy to create passed functions inline as anonymous
            // method or lambda expression.
            foreach (var item in someInts.FindAll(anInt => 0 == (anInt % 2)))
            {
                Debug.WriteLine("Item: " + item);
            }
			// What we see here shows the benefit using lambdas (and the benefit of anonymous
			// functions at all) compared to named functions: The code (of the lambda) is written
			// near the location where it is required, instead of passing the name of a function,
			// where you would need to look in to read the code.


            /*-----------------------------------------------------------------------------------*/
            // Eta-Reduction Style Syntax:

            // We could apply the Delegate instance isEven1 as a higher-order filter function like
            // that: 
            /* A */
            var result = someInts.FindAll(x => isEven1(x));
            // Or we could use it like that:
            /* B */
            var result2 = someInts.FindAll(isEven1);

            // In the lambda calculus we'd call B the _eta-reduced_ (eta, for the greek letter η)
            // form of A. The eta-reduction allows to write "λx.isEven x" as "isEven".


            /*-----------------------------------------------------------------------------------*/
            // Capturing local Variables as Closures:

            // From within anonymous functions (lambdas and anonymous methods) we can refer the
            // enclosing scope's locals and the "this" reference (only in instance methods of
            // reference types). So within anonymous functions locals and the "this" reference are
            // bound lexically (lexical binding/scoping) and virtually only _this link_ to the outer
            // context is called closure.


            int value = 0;
            // Concerning access to modified closure: see below.
            Action action = () => ++value;

            for (int i = 0; i < 10; ++i)
            {
                action();
            }
            // This assertion is true, the local variable has been captured and modified, when
            // action was called.

			// An interesting point is that the ability to capture objects of value type in an 
			// anonymous function, enables anonymous functions to "promote" value type objects to
			// reference type objects!

            Debug.Assert(10 == value);

            // We've just discussed higher-order functions. Since we don't know how often a passed
            // function will be executed or whether it will ever be executed, capturing of local variables
            // can be a problem! - It can also be a desired feature (see the section "Currying"
            // above).

            value = 0;
            for (int i = 0; i < 10; ++i)
            {
                action();
                ++value; // Modifies value exterior from action().
            }
            // This assertion is true, the local variable has been captured and modified, when
            // action was called _and_ in the exterior code of the closure.
            Debug.Assert(20 == value);

            // Since captured variables can be modified from exterior code, some attention is
            // required! The problem is that in the line:
            //Action action = () => ++value;
            // we access and even modify the variable value. Why is this a problem? Well we are
            // about to access the local variable value, which will create a closure referring to
            // this variable. But we can _execute_ the Delegate instance almost everywhere in the
            // code, e.g. we could even return action, not knowing when a potential caller executes
            // action! -> The Delegate function has access to scope of outer function, even if the
			// outer function has run to its end. And between creating the closure and executing the
			// Delegate instance the (still local) variable value could have been modified multiple
			// times! In such a case the tool Resharper issues the message "Access to modified closure"
			// in order to warn us as developers. This message often appears within a loop with calls to LINQ's
            // query operators (using the iteration variable in an implementation of a lambda
            // building a closure); but it is not a problem, if that query operator is _executed_
            // within that loop (the modification of the closure to the iteration variable can not
            // happen, though).
			// Keep in mind, that we can pass anonymous functions around as if they were values. Capturing
			// means, that wherever the function goes the captured values will go with it. 

            // Sidebar: JavaScript. In JavaScript, we have so-called dynamic binding/scoping in
            // functions (in opposite to C#'s lexical binding/scoping). This means, that, e.g., the
            // "this" reference in JavaScript is not bound where a function is declared, but where
            // the function is called. - The same function can be called on different contexts (the
            // context is defined by the this reference).
            // A function in JavaScript can reference each variable that is in its context at the
            // time of its declaration. A closure is a function that keeps referencing variables
            // even after the declaration of the function went out of scope. The "this" reference
            // is never captured in a closure, because each JavaScript function has its own
            // context.
            // When we have multiple cascaded JavaScript functions, the "this" reference refers to
            // different contexts on _each_ cascaded level. - This is _not_ the case in C#! In C#
            // the "this" reference in a lambda (and also cascaded lambdas) refers to the enclosing
            // instance and not the lambda's Delegate instance!


            /*-----------------------------------------------------------------------------------*/
            // Not supported - Anonymous Iterator Blocks:

            // The compiler does not support this. There is no technical reason, but the compiler
            // procedure to create a class for the Delegate as well as the compiler procedure to
            // rewrite the code for the iterator block was considered to be too much effort at the
            // C# team compared to the gain. - So iterator blocks must be top level methods in C#!
            //Func<IEnumerable<int>> firstTen = () => // NOT SUPPORTED!
            //{
            //    for (int i = 1; i < 11; ++i)
            //    {
            //        yield return i;
            //    }
            //};

            // However, it is planned that VB11 will have support for anonymous iterator blocks!
        }


        private static void LambdaExpressionTrees()
        {
            /*-----------------------------------------------------------------------------------*/
            // Working with Expression Trees:

            // We'll find the needed types in the namespace System.Linq.Expressions.

            // This example expresses the idea that code can be handled as data, which can be
            // combined to build an expression completely at run time. The compiled expression is
            // accessible via the Delegate instance "compiled".
            {
                ParameterExpression parameter = Expression.Parameter(typeof(int));
                Expression secondArgument = Expression.Constant(3);
                Expression add3 = Expression.Add(parameter, secondArgument);
                Func<int, int> compiled =
                    Expression.Lambda<Func<int, int>>(
                        add3,
                        new ParameterExpression[] { parameter }).Compile();
                
                // A very contrived way to print five to console:
                int argumentValue = 2;
                Debug.WriteLine(compiled(argumentValue));
            }

            // Expression trees can be generated from lambda expressions as well:
            {
                Expression<Func<int, int>> add3 = parameter => parameter + 3;
                Func<int, int> compiled = add3.Compile();
                
                int argumentValue = 2;
                Debug.WriteLine(compiled(argumentValue));
            }

            // Sidebar: Why didn't this example discuss an expression like
            // Expression<Func<int>> add2And3 = () => 2 + 3;
            // ?
            // The explanation is simple as well as comforting. The expression 2 + 3 would not be
            // translated into an Add expression, rather it would be directly translated into a
            // Constant expression of value five. So an expression being made up only of constants
            // will be converted into its result.


            /*-----------------------------------------------------------------------------------*/
            // Following Expression Trees are Invalid: 

            // Note that we can not create Expression Trees from anonymous methods being
            // defined with the delegate keyword. The right hand type must be derived from
            // Expression, lambdas are compatible, because they comply to the type
            // LambdaExpression.

            // Invalid! Won't compile.
            //Expression<Func<int>> invalid1 = delegate { return 2 + 3; }; 

            // We cannot create an Expression Tree from an assignment expression.
            // int someInt = 0;
            //Expression<Action> invalid2 = () => someInt = 0;
            //Expression<Action> invalid3 = () => ++someInt; // Mind, that this is also an assignment!


            // We can only create trees of single expressions, not of a group of statements.
            //Expression<Func<int>> invalid4 = () => 
            //{ 
            //   int result = 2 + 3;
            //   return result; 
            //};

            // We can also not construct circular Expression Trees.
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            LambdaDefinition();


            TheOrFunction();


            LambdaEverydayUsage();


            LambdaExpressionTrees();
        }
    }
}