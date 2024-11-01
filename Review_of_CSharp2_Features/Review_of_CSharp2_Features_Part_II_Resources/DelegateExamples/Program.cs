using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

// These examples show:
// - How we used to work with Delegates in C#1 and the simplifications made in C#2.
// - Delegate Instances and Events.
// - How to provide events in user defined types.
// - How method group conversions work.
// - How anonymous methods work and how they be used in your everyday work.
// - How closures work.
// - How Delegate variance work.
namespace DelegateExamples
{
    // The Delegate type. A Delegate type describes a method signature. It can be compared to
    // the type of a function pointer in the C programming language. Delegate types are first class
    // types in the .Net types system.
    public delegate void AcceptString(string aString);


    #region Providing Event in a user defined Type:
    /// <summary>
    /// This type provides a publicly accessible event and implements the code that cares for the
    /// event in the by-best-practices-proposed manner.
    /// </summary>
    internal class ATypeWithEvents
    {
        /// <summary>
        /// The field-like event, on which external code can advise event handlers.
        /// </summary>
        public event EventHandler MyEvent;


        /// <summary>
        /// The event-wrapping method wraps the MyEvent field and calls this event. The event is 
        /// only called, if the event field is not null, i.e. any event handlers have been advised
        /// (read: the event Delegate's invocation list is not empty). Also does this method accept
        /// and pass event argument to the wrapped event field. Notice, that the fixed argument
        /// "this" is passed as sender to the event. The event-wrapping method is protected, so it
        /// can be called only by derived types (not publicly). This is exactly the point in
        /// defining event-wrapping method: to allow derived classes to raise this event! - The
        /// event can not be raised publicly.
        /// 
        /// This method is virtual in order to allow inheriting types to override it to provide 
        /// event handler code. - That is a less-coupling alternative to advising to MyEvent
        /// directly.
        /// </summary>
        /// <param name="ea">The EventArgs for the MyEvent.</param>
        protected virtual void OnMyEvent(EventArgs ea)
        {
            // Sidebar: Assigning MyEvent to a local variable and checking and operating against
            // this local variable, is a way to provide threadsafety. The idea is that Delegates 
            // (which are implemented by events underneath) are immutable, and after each advising
            // and unadvising of event handlers a _new_ Delegate object is generated. If you assign
            // the event field to a local variable and only work against this local variable in
            // your code, it is guaranteed that this variable can't be modified by different
            // threads. - All without locks!
            EventHandler myEvent = MyEvent;
            if (null != myEvent)
            {
                myEvent(this, ea);
            }
        }


        /// <summary>
        /// This method allows raising MyEvent from other code.
        /// </summary>
        public void RaiseMyEvent()
        {
            OnMyEvent(EventArgs.Empty);
        }
    } 
    #endregion


    public class Program
    {
        #region Methods for WorkingWithDelegatesInCSharp1:
        // An instance of the Delegate. As the Delegate type is a first class .Net type we can
        // create Delegate variables that can hold Delegate instances.
        public static AcceptString _aDelegateInstance;


        // The method Foo(). Foo(string) is an example of the method group Foo.
        public static void Foo(string toConsole)
        {
            Debug.WriteLine(toConsole);
        }


        // An event handler compatible to the Delegate EventHandler. MyForm_Load(object, EventArgs)
        // is a member of the method group MyForm_Load.
        private static void MyForm_Load(object sender, EventArgs e)
        {
            /*pass*/
        }
        #endregion


        public static void WorkingWithDelegatesInCSharp1()
        {
            /*-----------------------------------------------------------------------------------*/
            // Presents how we work with Delegates in C#1:

            // Delegate instantiation. Because the method Foo(string) is compatible to the Delegate
            // type AcceptString we can create a new instance of AcceptString _of Foo_ and
            // initialize the field _aDelegateInstance with it.
            _aDelegateInstance = new AcceptString(Foo);

            // Then we can _invoke the Delegate instance_, which delegates the invocation to the
            // method Foo(string):
            _aDelegateInstance.Invoke("Hello World");
            // ... or with the C# method call syntax:
            _aDelegateInstance("Hello World");
            // The interesting point is that the Invoke() method and the method call syntax have
            // exactly the same signature (accepting string in this case), which the compiler
            // derived from the Delegate (AcceptString).

            // We can also chain more methods (each matching the Delegate's type) to the Delegate
            // instance with the utility method Combine() to build an _invocation list_ of methods.
            // The invocation list is a linked list, whose last item is the value null.
            // !! Note: all operations on Delegate instance don't modify these Delegate instances,
            // rather new Delegate instances will be created! This is exactly the same behavior
            // like with working with strings, as e.g. concatenation will never modify, but rather
            // create new strings. Side effect: all basic operations on Delegate instances are
            // threadsafe! !!
            _aDelegateInstance =
                (AcceptString)Delegate.Combine(_aDelegateInstance, new AcceptString(Foo));
            // ... or with the +=-operator:
            _aDelegateInstance += new AcceptString(Foo);
            // And you can also use the +-operator to "concat" Delegate instances to the invocation
            // list:
            _aDelegateInstance += new AcceptString(Foo) + new AcceptString(Foo);

            // Calling _aDelegateInstance will now delegate this call to the method Foo for _five_
            // times (this feature is called multicasting):
            _aDelegateInstance("Hey!");

            // We can also remove Delegate instances from the invocation list (the last occurance
            // of Foo will be removed from the invocation list):
            _aDelegateInstance =
                (AcceptString)Delegate.Remove(_aDelegateInstance, new AcceptString(Foo));
            // ... or with the -=-operator:
            _aDelegateInstance -= new AcceptString(Foo);
            // And you can also use the --operator to remove Delegate instances from the
            // invocation list:
            _aDelegateInstance = _aDelegateInstance - new AcceptString(Foo);

            // Calling _aDelegateInstance will now delegate this call to the method Foo for _two_
            // times, because we've removed three invocations:
            _aDelegateInstance("Hey!");

            // After removing the remaining two invocations the _aDelegateInstance will evaluate to
            // the null reference! Yes, really, because it again marks the end of the invocation
            // list (which is a linked list) and the invocation list is empty!
            _aDelegateInstance -= new AcceptString(Foo);
            _aDelegateInstance -= new AcceptString(Foo);
            Debug.Assert(null == _aDelegateInstance);

            // If you want to remove all Delegate instances from the invocation list, you can
            // directly assign null to the Delegate instance holding the invocation list:
            _aDelegateInstance += new AcceptString(Foo);
            _aDelegateInstance = null;
            Debug.Assert(null == _aDelegateInstance);

            // Sidebar: All methods listed in the invocation list of a Delegate instance will be
            // called sequentially. If an Exception escapes one of that calls, the remaining
            // methods in the invocation list won't be called and the Exception must be handled by
            // the caller of the Delegate instance holding the invocation list. There is another
            // problem: if the methods in the invocation list return values, you can only get the
            // value of the _very last_ method being called in the invocation list. If you want to
            // work with all results of the methods in the invocation list, you have to call the
            // individual methods in the Delegate instance's invocation list explicitly in a loop.
            // Finally we have to consider that all the methods of the invocation list are called
            // in a synchronous manner. I.e. if any of the invocation list's methods blocks the
            // execution, the execution of the complete invocation list will block.

            // If you call Remove() or the operator -= on a Delegate instance with an empty
            // invocation list (_aDelegateInstance will evaluate to null) nothing will happen:
            _aDelegateInstance =
                (AcceptString)Delegate.Remove(_aDelegateInstance, new AcceptString(Foo));
            // ... or with the -=-operator:
            _aDelegateInstance -= new AcceptString(Foo);
            // Also if you try to remove a Delegate instance, which never was enlisted in the 
            // invocation list nothing will happen.


            /*-----------------------------------------------------------------------------------*/
            // Delegate Instances and Events:

            // Sidebar: What is the difference between Delegate instances and events?
            // Primary, events are field-like items in any .Net type that are of a Delegate type
            // (e.g. EventHandler). So, virtually you can use a simple (public) field of a Delegate
            // type (e.g. EventHandler Load) instead of an event-"field"
            // (e.g. event EventHandler Load), but events put restrictions on how you can interact
            // with the "encapsulated" Delegate instance from outside of the declaring type:
            // - You can only use += and -= to advise/add and unadvise/remove event handlers from
            //   the event field. - You can _not call_ the methods Add() or Remove().
            // - _Only from within the declaring type_ you can call/invoke the event, directly
            //   access the invocation list, check the event for nullity or set the event to null.
            // (- These restrictions can be circumvented with reflection.)

            // One of the most import uses of Delegate instances is to advise them to an event
            // as event handler (i.e. code that will be executed, when the event has been raised).
            Form form = new Form();
            // The Delegate is EventHandler, MyForm_Load is the method to be advised and Load is
            // the event, in which we are interested in:
            form.Load += new EventHandler(MyForm_Load);

            // Please mind, that these examples just presented the basic operations on Delegate
            // instances. E.g. we did not discuss the asynchronous invocation of Delegate
            // instances!
        }


        private static void SimplifiedDelegateInstantiation()
        {
            /*-----------------------------------------------------------------------------------*/
            // Presents simplified Delegate Instantiation in C#2:

            // This code mirrors WorkingWithDelegatesInCSharp1(), but expressed with C#2 syntax 
            // (simplified delegate instantiation).
            _aDelegateInstance = null;

            // Delegate instantiation. Because the method Foo(string) is compatible to the Delegate
            // type AcceptString we can create a new instance of AcceptString _with Foo_ and
            // initialize the field _aDelegateInstance with it. - This is called _method group
            // conversion_. In this example the an overload of the method group Foo() (exactly the
            // one accepting a string) can be converted to a Delegate instance of AcceptString.
            _aDelegateInstance = Foo;

            // Then we can _call the Delegate instance_, which delegates the call to the method
            // Foo(string):
            _aDelegateInstance("Hello World");

            // Since every Delegate instance provides a method Invoke() that carries the signature
            // of the Delegate, Invoke()'s method group can also be used work with Delegate
            // instances like so:
            AcceptString anotherDelegateInstance = _aDelegateInstance.Invoke;
            
            // You _can not_ chain methods by application of the Combine() method and method
            // groups! Reason: the compiler can not convert from a method group to the .Net type
            // Delegate, which is Combine's parameters' type. You have to switch back to the C#1
            // syntax and create an instance of the Delegate explicitly (the same is true for the
            // application of Remove() and the operators -= and -):
            //_aDelegateInstance =
            //    (AcceptString)Delegate.Combine(_aDelegateInstance, Foo); // Invalid!
            // But you can use the C#1 syntax to create a Delegate instance:
            _aDelegateInstance =
                (AcceptString)Delegate.Combine(_aDelegateInstance, new AcceptString(Foo)); // OK!
            // ... in C#2 you can alternatively cast the method group to the Delegate type:
            _aDelegateInstance =
                (AcceptString)Delegate.Combine(_aDelegateInstance, (AcceptString)Foo); // OK!
            // ... or with the += operator, which needs no explicit conversion:
            _aDelegateInstance += Foo;
            // But the +-operator can not be used with method group conversion:
            //_aDelegateInstance += Foo + Foo; // Invalid!
            // But that is ok, a combination of both explicit conversion syntaxes:
            _aDelegateInstance += (AcceptString)Foo + new AcceptString(Foo); // OK!

            // Calling _aDelegateInstance will now delegate this call to the method Foo for _six_
            // times (this feature is called multicasting):
            _aDelegateInstance("Hey!");

            // One of the most import uses of Delegate instances is to advise them to an event
            // as event handler (i.e. code that will be executed, when the event has been raised):
            Form form = new Form();
            // MyForm_Load is the method to be advised and Load is the event, in which we are
            // interested in (the Delegate EventHandler is never mentioned in this code):
            form.Load += MyForm_Load;
            // Unadvising the event handler:
            form.Load -= MyForm_Load;

            // You can also advise anonymous methods as event handlers like this:
            form.Load += delegate(object sender, EventArgs args)
            {
                Debug.WriteLine("Form loaded");
            };
            // But, how can you unadvise an anonymous method as event handler? There is no "named
            // variable", and for events of other objects than this you can not modify that event's
            // invocation list. You can only use .Net reflection to archive this.
            // If you need to unadvise event handler that are backed by an anonymous method, you 
            // have to assign the anonymous method to another (maybe local) variable to have a kind
            // of "handle" to the anonymous method:
            EventHandler eventHandler = delegate(object sender, EventArgs args)
            {
                Debug.WriteLine("Form loaded");
            };
            form.Load += eventHandler;
            // Done with the event handler? - Undavise the handle:
            form.Load -= eventHandler;
        }


        #region The Method Group "PrintSomethingToConsole":
        // An example for a method group:
        public static void PrintSomethingToConsole(int theInt)
        {
            Debug.WriteLine(theInt);
        }


        public static void PrintSomethingToConsole(string theString)
        {
            Debug.WriteLine(theString);
        }
        #endregion


        private static void MethodGroupConversionExamples()
        {
            /*-----------------------------------------------------------------------------------*/
            // Method Group Conversion Examples:

            // A method group is the set of all methods found by a member lookup of a certain
            // method name in a certain context. E.g. all overloads of a method being effectively
            // present due to inheritance from multiple types are part of that method's method
            // group.

            // This is the conventional way to create a new instance of a Delegate:
            AcceptString acceptString = new AcceptString(PrintSomethingToConsole);

            // The method group conversion to a Delegate instance looks like this:
            AcceptString acceptString2 = PrintSomethingToConsole;
            // The method group PrintSomethingToConsole is the result of a member lookup and will
            // yield two methods PrintSomethingToConsole(int) and PrintSomethingToConsole(string).
            // But the delegate AcceptString is only compatible to PrintSomethingToConsole(string),
            // so this instance of the method group will be assigned.

            // This time we use the generic Delegate type Action<T> to construct the Delegate type
            // Action<int>. Then the method group's instance PrintSomethingToConsole(int) is
            // compatible and can be assigned to the Delegate instance like this:
            Action<int> acceptInt = PrintSomethingToConsole;
        }


        #region Types for AnonymousMethodsUnderneath:
        public class MyForm
        {
            private List<string> GetFileNames()
            {
                // Some wierd code...
                return null;
            }


            private void Display(List<string> fileNames)
            {
                // Some wierd code...
            }


            public Form SetupEvents()
            {
                Form form = new Form();

                // Getting a big chunk of data.
                List<string> fileNames = GetFileNames();

                // Advise an event handler to the Loaded event. This anonymous method captures the
                // outer local fileNames and accesses the instance method Display(), which makes it
                // a closure.
                form.Load += delegate
                {
                    Display(fileNames);
                };
                // The compiler generated class for the anonymous method as a closure will contain
                // a field of type List<string> to hold a reference to the captured outer local
                // fileNames, as well a reference to this in order to call the method Display().

                #region Compiler generated stuff for setting up the closure (MS specific):
                Closure c = new Closure();
                c.that = this;          // Set the captured this reference.
                c.fileNames = fileNames;// Set the captured local fileNames.
                EventHandler eventHandler = new EventHandler(c.Form_Load);
                //eventHandler.Target = this;   // Additionally the compiler will set the Target
                // property to this, because the code of the
                // anonymous method made use of the this reference.
                // In other cases the Target is null.
                form.Load += eventHandler;
                // -- end MS specific
                //
                // Attention, there is a source of memory leaks:
                // As can seen by this code, references to this and the local fileNames are also
                // used by the Closure's instance. And the captured references are used by the code
                // of the method c.Form_Load, which is advised to form's Load event.
                // The effect of this code is that as long as the form object (the event provider 
                // or publisher) was not gc'd, c (as event consumer) can't be gc'd and thus none of
                // the captured references can be gc'd! Since the form object will be returned from
                // this method, the locals virtually "survive" the scope of this method. - In this
                // case we have prolonged the lifetime of the captured variables to the outside of
                // this method, because we return the form object that holds a reference to the
                // Delegate instance. - The lifetime of the locals is probably as long as the
                // form's lifetime
                // And the Delegate instance's Target object can not be gc'd as long as the object,
                // to which the Delegate instance was advised as event handler wasn't gc'd!

                // Sidebar: In Cocoa before every event is handled an autorelease-pool will be
                // generated, which will contain all the temporary objects of the executed event
                // handlers. After the event has been handled this autorelease-pool will be
                // drained.
                #endregion

                return form;
            }


            private sealed class Closure
            {
                public MyForm that;
                public List<string> fileNames;

                // Here the body of the anonymous method (making up the closure) can be found. As
                // you it makes use of the this reference (stored in that) in order to access the 
                // method Display. 
                public void Form_Load(object sender, EventArgs e)
                {
                    // As Closure is a nested class it can access the private members of its outer
                    // class, e.g. the method Display():
                    that.Display(fileNames);
                }
            }
        }
        #endregion


        public static void AnonymousMethodsUnderneath()
        {
            /*-----------------------------------------------------------------------------------*/
            // This Code shows, how the Compiler generated Code for anonymous Methods looks like:

            // Please see code in SetupEvents().
            Form form = new MyForm().SetupEvents();
        }


        #region Types for EverydayAnonymousMethods:
        public delegate void WithRefParameter(ref int anInt);
        #endregion


        public static void EverydayAnonymousMethods()
        {
            /*-----------------------------------------------------------------------------------*/
            // Anonymous Methods in Everyday Use:

            // This set of code examples shows the basics of anonymous methods as well as the usage
            // of anonymous methods as means to replace foreach loops and to reduce code.
            // Additionally the generic Delegate types Action<T> and Predicate<T> will be
            // discussed, as they help to reduce the need for the programmer to create user defined
            // Delegate types.


            /*-----------------------------------------------------------------------------------*/
            // First Example with Anonymous Methods:

            // This is the conventional way to create a new instance of a Delegate:
            AcceptString acceptString = new AcceptString(PrintSomethingToConsole);

            // Alternatively we can create a block of code with an anonymous method, which prints
            // the passed string to console. The type of this anonymous method is the type of the 
            // Delegate AcceptString:
            AcceptString printToConsole =
                delegate(string s)
                {
                    Debug.WriteLine(s);
                };
            // Having AcceptString in avail we can call this Delegate directly:
            printToConsole("Hey!");

            // As an important convenience The .Net 2 framework introduced the new generic Delegate
            // type Action<T>, which can be used as type of your anonymous methods instead of
            // introducing user defined Delegate Types. Action<T> is a Delegate type describing
            // methods acting on item, i.e. for side effects. So we can get rid of AcceptString and
            // replace it with the Delegate type Action<string>, which has the same signature:
            Action<string> printStringToConsole =
                delegate(string s)
                {
                    Debug.WriteLine(s);
                };
            // Let's call it:
            printStringToConsole("Hey!");

            // There is a restriction on the generic Delegate types (Action<T> and Predicate<T>
            // (the later one will be discussed below)): if you want to use ref or out types on the
            // parameters of the anonymous method, you have to create an explicit Delegate type.
            // This does not compute:
            //Action<ref int> makeEven = // Invalid! You can not instantiate generics with ref or out types.
            //    delegate(ref int anInt)
            //    {
            //        if (0 != anInt % 2)
            //        {
            //            ++anInt;
            //        }
            //    };
            // Instead you need to create an explicit Delegate type (WithRefParameter in this
            // case):
            WithRefParameter makeEven =
                delegate(ref int anInt)
                {
                    if (0 != anInt % 2)
                    {
                        ++anInt;
                    }
                };
            int value = 5;
            makeEven(ref value);


            /*-----------------------------------------------------------------------------------*/
            // ForEach Examples (compacting Code):

            // Next we create a List<int> as data for following examples:
            List<int> values = new List<int>(new int[] { 7, 9, 1, 5, 8, 10, 4, 6, 3, 2 });

            // Then we also need a Delegate instance, here we use an anonymous method and the
            // Delegate type Action<T> we've just discussed.
            Action<int> printIntToConsole =
                delegate(int i)
                {
                    Debug.WriteLine(i);
                };


            // There exist following ways to print all items of values to the console:

            // (1) We can print all items of values to console via a bare foreach loop:
            foreach (int item in values)
            {
                Debug.WriteLine(item);
            }

            // (2) We can also _call_ printIntToConsole to print them to console:
            foreach (int item in values)
            {
                printIntToConsole(item);
            }

            // (3) We can use the ForEach method in class List, it accepts arguments of Delegate
            // Action<T>, so we can _pass_ printIntToConsole directly:
            values.ForEach(printIntToConsole);
            // Sidebar: In the lambda calculus we'd call (3) the _eta-reduced_ (eta, for the Greek
            // letter η) form of (2).
            // The eta-reduction allows to write "λitem.printIntToConsole item" as 
            // "printIntToConsole".
            // The eta-reduction even allows to write "λitem.Console.WriteLine item" directly as
            // "Console.WriteLine".
            values.ForEach(Console.WriteLine);

            // (4) In C#2 we are able to do exactly the same within one statement! We use List's
            // method ForEach and an anonymous method of Delegate Action<int> performing the side
            // effect (printing to the console):
            values.ForEach(delegate(int i)
                            {
                                Debug.WriteLine(i);
                            });


            /*-----------------------------------------------------------------------------------*/
            // Anonymous Methods even shorter:

            // If you make no use of the parameter being passed to your anonymous method you can
            // just leave the signature of the anonymous method away, incl. the parameter
            // parentheses.

            // Instead of this:
            Action doSomething =
                delegate()
                {
                    Debug.WriteLine("Doing something");
                };
            //, you could write it like this:
            Action doTheSame =
                delegate
                {
                    Debug.WriteLine("Doing the same");
                };

            // Even if you have this code (making not use of the present parameter):
            Action<int> doMore =
                delegate(int i)
                {
                    Debug.WriteLine("Doing more");
                };
            //, you could write it like this:
            Action<int> doEvenMore =
                delegate
                {
                    Debug.WriteLine("Doing even more");
                };


            /*-----------------------------------------------------------------------------------*/
            // FindAll Examples (compacting Code):

            // Now we'd like to get all even numbers in values and store them into another List.
            // There exist following ways to do this:

            // (1) Filter the values with a bare foreach loop. Then print the contents of the new
            // List to console:
            List<int> evenValues = new List<int>();
            foreach (int item in values)
            {
                if (0 == item % 2)
                {
                    evenValues.Add(item);
                }
            }
            evenValues.ForEach(printIntToConsole);

            // As an important convenience The .Net 2 framework introduced the new generic Delegate
            // type Predicate<T>, which can be used as type of your anonymous methods instead of
            // introducing user defined Delegate Types. Predicate<T> is a Delegate type describing
            // methods that check a predicate on the passed item. So we can define code to filter
            // the odd numbers as a Predicate<int> like so:
            Predicate<int> isEven =
                delegate(int item)
                {
                    return 0 == item % 2;
                };

            // (2) We can use the FindAll method in class List, it accepts arguments of Delegate
            // Predicate<T>, so we can _pass_ isEven directly:
            List<int> evenValues2 = values.FindAll(isEven);

            // (3) In C#2 we are able to do exactly the same within one statement! We use List's
            // method FindAll and an anonymous method of Delegate Predicate<int>. This time we need
            // an anonymous method returning a value instead of a side effect:
            List<int> evenValues3 =
                values.FindAll(delegate(int item)
                {/*Anonymous method of type Predicate.*/
                    return 0 == item % 2;
                });
            evenValues3.ForEach(printIntToConsole);


            /*-----------------------------------------------------------------------------------*/
            // Closure Example (Binding to Locals):

            // Next we create a new List<int> of values that should be filtered out. Therefor we
            // use List's method FindAll and use an anonymous method to filter those values, which
            // are contained in the local List valuesToFilter. Then we print it to console.

            List<int> valuesToFilter = new List<int>(new int[] { 9, 5, 10, 6, 2 });

            List<int> filteredValues = values.FindAll(delegate(int item)
            {
                // Anonymous method of type Predicate<int>, which captures the local valuesToFilter:
                return !valuesToFilter.Contains(item);
            });
            filteredValues.ForEach(printIntToConsole);


            /*-----------------------------------------------------------------------------------*/
            // Recursive Anonymous Methods:

            // There is no direct way to express recursive calls of an anonymous method. But you
            // can simulate this by the usage of a Delegate reference and the separation of
            // declaration and assignment:

            Action<int> countDown = null;
            countDown = delegate(int n)
                  {
                      Debug.WriteLine(n);
                      if (n > 0)
                      {
                          countDown(n - 1);
                      }
                  };
            countDown(4);

            // Sidebar: In JavaScript you can call an anonymous function recursively by calling
            // arguments.callee within the anonymous function.
        }


        #region Types and methods for DelegateVariance:

        #region Stuff for Covariance:
        // Base class Car.
        public class Car
        {

        }

        // Derived class Bus.
        public class Bus : Car
        {

        }

        // A Delegate type returning Car.
        public delegate Car CarDelegate();

        // Method returning Car. Its method group is compatible to the Delegate type CarDelegate.
        public static Car CarMethod()
        {
            return new Car();
        }

        // Method returning Bus, which is derived from Car. Its method group is _also_ compatible
        // to the Delegate type CarDelegate, due to Delegate covariance (the return type Bus is
        // more derived than Car).
        public static Bus BusMethod()
        {
            return new Bus();
        }
        #endregion


        #region Stuff for Contravariance:
        // Method awaiting object and KeyEventArgs as parameter types. Its method group is
        // compatible to the .Net Delegate type KeyEventHandler, so it can be advised to a KeyDown-
        // event.
        public static void myForm_KeyDown(object sender, KeyEventArgs e)
        {
            /*pass*/
        }


        // Method awaiting object and EventArgs as parameter types. Its method group is _also_
        // compatible to the .Net Delegate type KeyEventHandler, due to Delegate contravariance
        // (the parameter type EventArgs is less derived than KeyEventArgs), so it can be advised
        // to _any event_ whose Delegate type is "contravariantly" compatible (i.e. pretty all!).
        public static void AnyEventHandler(object sender, EventArgs e)
        {
            /*pass*/
        }
        #endregion

        #endregion


        public static void DelegateVariance()
        {
            // Variance expresses compatibility of Delegate instances to a Delegate type, if:
            // - the return types of instance and type are related as covariance, and if
            // - the parameter types of instance and type are related as contravariance.


            /*-----------------------------------------------------------------------------------*/
            // Showing Delegate Covariance:

            // Covariant return type:
            // The instance's return type can be more derived than the Delegate type's, but the
            // instance is still being considered compatible for creation from the method group:

            CarDelegate carDelegate = CarMethod;   // OK, Delegate and instance have the same type. 
            carDelegate = BusMethod;		       // _Legal_ in C#2; Bus is more derived than Car
            // and covariance comes into play.


            /*-----------------------------------------------------------------------------------*/
            // Showing Delegate Contravariance:

            // Contravariant parameter type:
            // An instance's parameter types can be less derived than the Delegate types'.

            Form myForm = new Form();
            myForm.KeyDown += myForm_KeyDown;   // OK, Delegate and instance have the same type. 
            myForm.KeyDown += AnyEventHandler;  // _Legal_ in C#2; KeyEventArgs is more derived 
            // than EventArgs and contravariance comes into
            // play!
            myForm.LocationChanged += AnyEventHandler;  // We can really advise AnyEventHandler to
            // any event!
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            WorkingWithDelegatesInCSharp1();


            SimplifiedDelegateInstantiation();


            MethodGroupConversionExamples();


            EverydayAnonymousMethods();


            AnonymousMethodsUnderneath();
        }
    }
}