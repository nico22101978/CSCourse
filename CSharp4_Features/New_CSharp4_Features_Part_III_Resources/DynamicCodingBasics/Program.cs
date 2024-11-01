using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

// These examples show:
// - The basic syntax (in local and type-definition scopes) using dynamic types.
// - Restrictions on the dynamic keyword.
// - Dynamic dispatching.
namespace DynamicCodingBasics
{
    public static class IntExtensions
    {
        public static void MagicMethod(this int that)
        {
            Debug.WriteLine("Congratulations, you've found this extension method!"+
                " MagicMethod(int)");
        }


        public static void MagicMethod(this int that, int aParameter)
        {
            Debug.WriteLine("Congratulations, you've found this extension method!"+
                " MagicMethod(int, int)");
        }
    }


    public static class DynamicExtensions
    {
        // Invalid! The type dynamic can not be extended!
        //public static void MagicMethod(this dynamic that)
        //{
        //    Debug.WriteLine("Congratulations, you've found this extension method!");
        //}
    }


    public class Program
    {
        #region Types for BasicSyntax():
        public class NestedStuff
        {
            public int AnInt { get; set; }
        }


        public class Limitations
        {
            public void Accept<T, TReturn>(Func<T, TReturn> func)
            {
                func(default(T));
            }
        }
        
        
        #region Non-local constructs w/ dynamic:
        // Fields and properties of types can be of type dynamic as well.
        public class Tuple
        {
            private dynamic _aField;


            public dynamic AProperty { get; set; }


            public dynamic AProperty2 { get; set; }
        }


        // The type dynamic can legally replace each occurance of type object. For the compiler
        // both types are equivalent.
        public class ClassWithEquals
        {
            public override int GetHashCode()
            {
                return 0;
            }

            public override bool Equals(dynamic obj)
            {
                return true;
            }
        }


        // A method can accept parameters of type dynamic as well as return objects of type
        // dynamic.
        // Notice, how the dynamic keyword reads at least strange together with the static keyword.
        public static dynamic AMethodDealingWithDynamics(dynamic d)
        {
            return d.ToString();
        }


        // Invalid! AMethodDealingWithDynamics(dynamic) and AMethodDealingWithDynamics(object) have
        // the same syntax for the compiler (both AMethodDealingWithDynamics(object)).
        //public static dynamic AMethodDealingWithDynamics(object d)
        //{
        //    return d.ToString();
        //} 
        #endregion


        #region LimitationsBaseClassAndConstraints
        // Sidebar: Most of these limitations (esp. on generics) arise, because dynamic is not a
        // "real" .Net type.

        // Invalid! The type dynamic can not be used as a base class! 
        //public class InvalidBase1 : dynamic { }

        // Invalid! A generic interface type used as an interface to implement _can not_ have
        // dynamic as type argument! 
        //public class InvalidBase2 : IList<dynamic> { }

        // OK! A generic non-interface type used as a base class can have dynamic as type argument! 
        public class ValidBaseCollection : List<dynamic> { }

        // Invalid! A type constraint can not be dynamic! 
        //public class InvalidTypeConstraint<T> where T : dynamic { }

        // Invalid! A type constraint can not be a generic type with the type argument dynamic! 
        //public class InvalidTypeConstraint2<T> where T : IList<dynamic> { }

        // Invalid! A type constraint can not be a generic type with the type argument dynamic! 
        //public class InvalidTypeConstraint3<T> where T : List<dynamic> { } 
        #endregion
        #endregion


        public static void BasicSyntax(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Dynamic as local Variable Type:

            // Sidebar: The feature, which we discuss here as dynamic typing is also known as
            // loosely typing. - The main idea is that a dynamically typed variable is just a named
            // placeholder for a value. In C# you still have to declare variables explicitly to be 
            // dynamic.

            // Simple initialization and assignment:
            // Almost every type can be converted into the type dynamic.
            // The compile time type of d is dynamic, but its run time type is string.
            dynamic d = "Hello dynamic";
            // Yes, it is really dynamic, now d's run time type is int. - The int was also boxed
            // into object, because a local of compile time type dynamic is really of compile time
            // type object.    
            d = 1;
            // So virtually following assertion is valid:
            //Debug.Assert(typeof(dynamic) == typeof(object));
            // But the expression "typeof(dynamic)" is Invalid, because the typeof operator cannot
            // be used on the dynamic type.
            // Since dynamic is object-based you can also use the is operator to test the run time
            // type like this:
            bool isInt = d is int;
            // Or the as operator for conversions:
            int? containedInt = d as int?;

            // Conditional initialization and assignment:
            // But following construct is Invalid, because the C# compiler counts this as being
            // an Invalid expression, as int and string can not be implicitly converted to the same
            // type.
            // dynamic ofAnyType = (0 == DateTime.Now.Millisecond % 2) // Invalid!
            //    ? 42
            //    : "Babelfish";
            // But this is OK for the C# compiler:
            dynamic ofAnyType;
            if (0 == DateTime.Now.Millisecond % 2)
            {
                ofAnyType = 42;
            }
            else
            {
                ofAnyType = "Babelfish";
            }

            // Other conversions:
            // Virtually you can not convert dynamic expressions to any other type, period. But
            // there are special situations, in which a conversion is allowed: overload resolution,
            // assignment conversion, if clauses, using clauses and foreach loops.
            // Special situation: assignment conversion, so back conversion from dynamic to int is
            // allowed.
            int theInt = d;
            // will be default initialized to null
            dynamic d1;
            dynamic d2 = default(dynamic);

            // will be default initialized to null and inferred by the compiler to the type
            // dynamic.
            var d3 = default(dynamic);

            // dynamic Invalid1 = delegate(int anInt) { Debug.WriteLine(anInt); }; // Invalid! Cannot convert lambda expression to type 'dynamic' because it is not a delegate type.
            dynamic better1 = (Action<int>)delegate(int anInt) { Debug.WriteLine(anInt); }; // OK with cast: .
            better1(42);

            //dynamic Invalid2 = Main; // Invalid! Cannot convert method group 'Main' to non-delegate type 'dynamic'.
            dynamic better2 = (Action<string[]>)Main; // Ok with cast: method groups can be converted successfully.

            //Invalid! Conversion from pointer types to and from dynamic.
            int value = 42;
            dynamic anotherIntValue = 42;
            unsafe
            {
                //dynamic intPointer = &value; // Invalid! Cannot implicitly convert type 'int*' to 'dynamic'.
                //int* intPointer2 = &anotherIntValue; // Invalid! Cannot implicitly convert type 'dynamic*' to 'int*'. An explicit conversion exists (are you missing a cast?) Cannot take the address of, get the size of, or declare a pointer to a managed type ('dynamic').
            }

            // Dynamic expressions:
            // If a dynamic is used in an expression, the whole expression turns into a so called
            // _dynamic expression_.
            dynamic nestedStuff = new NestedStuff();
            // Notice, that anInt will be inferred to be of type dynamic, not of type int. Because a
            // dynamic was used in the expression the whole expression turned into a dynamic
            // expression, where the return types are assumed to be of type dynamic as well.
            var anInt2 = nestedStuff.AnInt;


            /*-----------------------------------------------------------------------------------*/
            // Examples of dynamic Typing in Control Structures and Scopes:

            // Dynamics can be used in if clauses:
            dynamic b = true;
            // Special situation: if clause, so back conversion from dynamic to bool is allowed.
            if (b)
            {
                Debug.WriteLine("You've made it here ...");
            }

            // Dynamic typing can be used for the counter variable in for loops:
            for (dynamic i = 0; i < 10; ++i)
            {
                // Pitfall!
                // In order to call Debug.WriteLine we have to get rid of dynamic dispatch. -  
                // Debug.WriteLine has a compile time custom attribute of type Conditional, it can
                // not be called with a dynamic variable, because the expression tree could be
                // broken due to the condition... Alas this problem will only show up as a warning
                // on compilation (warning CS1974) and during run time a RuntimeBinderException
                // will be thrown: Cannot dynamically invoke method 'WriteLine' because it has a
                // Conditional attribute.
                //Debug.WriteLine("Item: {0}", i);
                object obj = i;
                Debug.WriteLine("Item: {0}", obj);
            }

            // Dynamic typing can be used for the counter variable in foreach loops.
            foreach (dynamic name in new[] { "Karen", "Minnie", "Trudy" })
            {
                object obj = name;
                Debug.WriteLine("Item: {0}", obj);
            }

            // A Dynamic can also be used to be looped upon.
            dynamic listOfNames = new[] { "Karen", "Minnie", "Trudy" };
            // Special situation: foreach loop, so back conversion from dynamic to string is
            // allowed.
            foreach (string name in listOfNames)
            {
                Debug.WriteLine("Item: {0}", name);
            }

            // Dynamic typing within using statements.
            using (dynamic reader = File.OpenText("test.txt"))
            {
                int firstCharacter = reader.Peek();
            }

            // Dynamics can also be locked upon.
            lock (d)
            {
                Debug.WriteLine("In the lock");
            }

            // Creating a collection that may contain instances of either type (like a fallback to
            // object based colections):
            IList<dynamic> list = new List<dynamic>
            {
                "hello",                // Put in a string.
                new[] { "hi", "there" } // Put in a string array.
            };

            // Iterate over the list and call the Property Length. It is present in both run time
            // types but not accessible via a common interface (string and string array are not
            // related): it will be found via dynamic dispatching.
            foreach (var item in list)
            {
                Console.WriteLine(item.Length);
            }

            // This is valid, because for the compiler dynamic is of type object virtually:
            IList<dynamic> objectList = new List<object>();

            // Invalid! The conversion from a type to dynamic doesn't work on generic types: 
            //IList<dynamic> stringList = new List<string>();
        }


        #region Types for DynamicsAndDelegates():
        // A delegate can accept parameters of type dynamic as well as return objects of type
        // dynamic.
        public delegate dynamic DynamicDelegate(dynamic d); 
        #endregion


        private static void DynamicsAndDelegates()
        {
            /*-----------------------------------------------------------------------------------*/
            // Dynamics and Delegates and Consistency:

            // Let's define an instance of a Delegate accepting a dynamic and returning a dynamic:
            DynamicDelegate del = (DynamicDelegate)((dynamic x) => x * x);
            // The Delegate instance is consistent to every type having a binary * operator,
            // like int,
            int i = del(2);
            // and like double.
            double d = del(4.6);

            // DynamicDelegate is in effect the same as Func<dynamic, dynamic>:
            Func<dynamic, dynamic> del2 = (dynamic x) => x + x;
            int i2 = del2(2);
            double d2 = del2(4.6);
        }


        #region Methods and Types for Dispatching():
        private static void PrintIt(dynamic d)
        {
            string str = d.ToString();
            Debug.WriteLine(str);
        }


        // Invalid! PrintIt(dynamic) and PrintIt(object) have the same syntax for the compiler
        // (both PrintIt(object)).
        //private static void PrintIt(object d)
        //{
        //    Debug.WriteLine(d);
        //}


        private static void PrintIt(Form f)
        {
            Debug.WriteLine(f);
        }


        private static void PrintIt(Control c)
        {
            Debug.WriteLine(c);
        }


        private static void PrintIt(string s)
        {
            Debug.WriteLine(s);
        }


        private static void PrintIt(int i)
        {
            Debug.WriteLine(i);
        }


        public interface IAny
        {
            void Accept(int i);
        }


        public class Bar : IAny
        {
            void IAny.Accept(int i)
            {
                Debug.WriteLine("In Bar.IAny.Accept(int)");
            }
        }


        public class PrivateStuff
        {
            private void PrivateAccept(int i)
            {
                Debug.WriteLine("In PrivateStuff.PrivateAccept(int)");
            }
        }


        private void PrivateAccept(int i)
        {
            Debug.WriteLine("In PrivateAccept(int)");
        }


        public class C
        {
            public void Accept(decimal d)
            {
                Debug.WriteLine("In C.Accept(decimal)");
            }
        }


        public class D : C
        {
            public void Accept(int i)
            {
                Debug.WriteLine("In D.Accept(int)");
            }
        }


        public struct S
        {
            public int i;
        }


        public class K
        {
            public S s;
        }
        #endregion


        private static void Dispatching()
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling Methods and Properties/Fields w/ dynamic Dispatching and w/ a dynamic
            // Receiver (Duck Typing):
            
            // Sidebar: If you need to do more with a dynamic object than, say, assigning values to
            // it, e.g. calling methods, the DLR starts its work to dispatch the resulting dynamic
            // expressions. In C#, the C# language binder will come into play in that situation,
            // and if you apply dynamic dispatch in your code, you will need to add a reference to
            // the assembly Microsoft.CSharp.dll in VS 2010. - The good news is that this assembly
            // will be referenced by default, when you setup a new C# project in VS 2010.

            dynamic d = "I am a string!";
            // Following calls to methods and operators will crash at run time (a
            // RuntimeBinderException will be thrown), because the type string (i.e. the run time
            // type "behind" the dynamic d) does not offer this operator. - But the compiler
            // has no problem with these dynamic expressions, because we have turned off
            // compile time checking with the dynamic keyword and all these expressions turn into
            // dynamic expressions, because at least one of the arguments is dynamic...
            
            // During run time the DLR recognizes d as dynamic receiver and will try to bind to a
            // method named "Foo" and apply the arguments 1 and 2 to it.
            //d.Foo(1, 2); Invalid, will fail with a RuntimeBinderException: 
            // 'string' does not contain a definition for 'Foo'     
            
            // The DLR recognizes d as dynamic receiver and will try to bind to a
            // property-looking-item (it could be a property or field) named "Bar" and tries to set
            // the value 23 to it. - The return type of a property-looking-item is always dynamic
            // at compile time.
            //d.Bar = 23; Invalid, will fail with a RuntimeBinderException: 
            // 'string' does not contain a definition for 'Bar' 

            // During run time it is tried to perform a normal operator overload resolution, and it
            // would find any (incl. user-defined ones) unary "++"-operator on the run time type of 
            // d.
            //++d; Invalid, will fail with a RuntimeBinderException: 
            // Operator '++' cannot be applied to operand of type 'string'  

            // During run time it is tried to perform a normal operator overload resolution, and it
            // would find any (incl. user-defined ones) binary "+"-operator on the run time type of 
            // d.
            //dynamic f = d * d; Invalid, will fail with a RuntimeBinderException:
            // Operator '*' cannot be applied to operands of type 'string' and 'string'
            
            // During run time it is tried to perform an implicit conversion from d's run time type
            // to int.
            //int i = d; Invalid, will fail with a RuntimeBinderException: 
            // Cannot implicitly convert type 'string' to 'int'
            
            // During run time it is tried to perform an explicit conversion from d's run time type
            // to string. - This will work of course! This is the only operation, for which the
            // compile time return type is not dynamic.
            string s = (string)d;   
            
            // As this is still a dynamic expression, because d is dynamic, str will also be
            // inferred to be dynamic, not string! 
            var str = d.ToString(); 


            /*-----------------------------------------------------------------------------------*/
            // Exploiting Polymorphism w/ dynamic Dispatching:

            // The static type of control is Control, the dynamic/polymorphic type is Form. - We
            // can not access the interface of Form w/o downcasting to Form. But dynamic opens a
            // gate here.
            Control control = new Form();               
            dynamic dynamicControl = control;
            // We can access Form's property AcceptButton "through" the static type by using the
            // type dynamic and dynamic dispatching.
            dynamicControl.AcceptButton = new Button(); 

            // You can also call overloads of methods being only defined in the dynamic type like
            // this: 
            C c = new D();
            // "Open" the dynamic gate.
            dynamic receiver = c;           
            dynamic iamReallyAnInt = 10;
            // Call on a dynamically typed variable w/ dynamic argument:
            // This will call D.Accept(int), because it is the best match of the dynamically typed
            // int "within" iamReallyAnInt, together with the dynamically typed receiver.
            receiver.Accept(iamReallyAnInt);
            // Call on a statically typed variable w/ dynamic argument:
            // This will call C.Accept(double), because the receiver is of static type C (where
            // Accept(int) is not present). Thus iamReallyAnInt will be converted to double
            // implicitly.
            c.Accept(iamReallyAnInt);   


            /*-----------------------------------------------------------------------------------*/
            // Dynamic Dispatching as a Way to implement multiple Dispatching:

            // Although we're calling a statically bound method, the call to PrintIt() will be
            // dispatched dynamically, because we're calling it with a dynamic argument. - The
            // _run time type_ of d will then control the DLR's decision, which overload of
            // PrintIt() to call! In this case PrintIt(string) will be called, because the run time
            // type of d is string. - Mind, that this is _very_ different from static binding, where
            // only the _compile time type_ determines a method's overload to call! - If we wanted
            // to implement such a thing ourselves (w/o dynamic), we would end up with code
            // fragment testing the run time type of the parameter(s) (a bunch of if-statements
            // using the "is"-operator) and call the appropriate method manually. That is an
            // incarnation of the GoF pattern "visitor". This pattern is known to be extended only
            // with great pains, because when a new type is introduced you need to modify the
            // dispatching code (the bunch of if-statements) and _all_ the acting visitor-types in
            // order to handle the new type. The ability to select a method's implementation on the
            // information of the run time type of one or many arguments when it is called, is a
            // gateway to so-called "multiple dispatching".
            PrintIt(d);

            // In this case PrintIt(int) will be called, because the compile time type of the
            // argument is int.
            PrintIt(42);

            // In this case PrintIt(int) will be called, because the run time type of d is int.
            d = 42;
            PrintIt(d);

            // In this case PrintIt(dynamic) will be called, because the run time type of d is
            // double and no overload of PrintIt() for double is exitent. But double can be
            // implicitly converted to dynamic. 
            d = .5;
            PrintIt(d); 
            
            // In this case PrintIt(dynamic) will be called for the same causes as when d was a
            // double.
            PrintIt(new List<int> { 1, 2, 3 });


            /*-----------------------------------------------------------------------------------*/
            // Limitations of dynamic Dispatching:

            // Extension methods:
            // The dynamic type of d is int now.
            d = 42; 
            // Invalid! Here we try to call an extension method on int, but the C# run time binder
            // will not find this method, because it will not analyze all the using statements and
            // referenced assemblies to get this information. 
            // -> This call would throw a RuntimeBinderException.
            //d.MagicMethod();  
            // A solution is calling the extension method as a static method.
            // -> OK, if called as static method.
            IntExtensions.MagicMethod(d); 
            
            // Extension methods can also not be called with a dynamic argument:
            // A variable of static type int.
            int ii = 42; 
            // Invalid! Here we try to call an extension method on int, but the C# compiler is not
            // able to produce code to dynamically dispatch extension methods. 
            // -> This call would yield a compile time error CS1973. A solution is calling the
            // extension method as a static method or converting (unboxing) the dynamic parameter
            // to int.
            //i.MagicMethod(d);
            // OK, if the dynamic argument is converted to the awaited static type of the method.
            ii.MagicMethod((int)d);
            // OK, if called as static method.       
            IntExtensions.MagicMethod(ii, d);

            // Passing lambdas:
            // The dynamic type of d is now "Limitations".
            d = new Limitations();  
            // Invalid! There is not enough context to give the lambda a meaning, i.e. in simple
            // words the full type of the lambda is not known and such dynamic expressions can not
            // be represented as expression trees by the C# run time binder.
            // -> This call would yield a compile time error CS1977.
            //d.Accept(x => x);
            // OK with cast.
            d.Accept((Func<int, int>)(x => x));
            // OK with a constructed Delegate.
            d.Accept(new Func<int, int>(x => x)); 

            // Explicitly implemented interfaces:
            d = new Bar();  // The dynamic type of d is now Bar.
            //d.Accept(42); // Invalid! Here we try to call a method being provided by an
            // explicitly implemented interface. That method is not discoverable by
            // by the C# run time binder, because the name of the method has been 
            // removed by the compiler and interfaces are only a compile time
            // construct, so the method Accept(int) does not exist on run time.
            // -> This call would throw a RuntimeBinderException.
            ((IAny)d).Accept(42);   // As a solution you could explicitly convert to the static
            // type of the explicitly implemented interface and call the
            // method you like with "static dispatch".

            // Private methods:
            // The dynamic type of d is now PrivateStuff.
            d = new PrivateStuff();
            // Invalid! You can not call a private method of another type. This exactly mirrors the
            // rules in C#, and you'd get a RuntimeBinderException explaining that this method
            // couldn't be called due to its protection level. - A message that you would normally
            // get from the compiler directly.
            //d.PrivateAccept(42);  
            d = new Program();  // The dynamic type of d is now Program.
            d.PrivateAccept(42);// But it is ok to call a private method from within the same type,
            // this also mirrors the C# rules exactly.

            // Sidebar: You can neither call constructors or static methods via dynamic dispatch.
            // You have to stick to instance methods or you have to use polymorphic static typing
            // and factories. 

            // Nested value types:
            // The dynamic type of d is now a class (K) that contains an instance of a value type
            // (s), and that instance holds an int (i).
            dynamic k = new K();    
            k.s = default(S);
            // Assign a value to i with nested-dotted syntax.
            k.s.i = 10;             
            int result = k.s.i;
            // Alas, the former assignment of k.s.i to ten didn't work! The problem is that s is an
            // instance of a value type, and within a dynamic expression that instance will be
            // converted to dynamic and thus boxed into an object. As a a boxed instance of a value
            // type is a copy of the original one, the next dot would access the i of a copy of s.
            // After the assignment completed, the copy will be thrown away and the original value
            // of i is still the unmodified 0.
            Debug.Assert(0 == result); 
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:


            BasicSyntax(args);


            DynamicsAndDelegates();


            Dispatching();
        }
    }
}