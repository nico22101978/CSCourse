using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;  
using System.Reflection;
using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder;
using System.ComponentModel;


// These examples show:
// - The basic usage of ExpandoObjects.
namespace DynamicObjects
{
    public class Program
    {
        private static void ExpandoObjectsExamples()
        {
            /*-----------------------------------------------------------------------------------*/
            // Basic Usage of ExpandoObject:

            // The type ExpandoObject looks very sad, as it introduces no new members, it only has
            // a dctor and it can not act as a base class (i.e. it is sealed). On the other hand it
            // implements a bunch of interfaces explicitly (incl. IDictionary<string, object>). So
            // how should we use this strange type? Answer: an object of type ExpandoObject learns
            // about its members while we are using it! - That sounds weird, but it is simple to
            // handle!

            // Let's create an ExpandoObject: Mind, that we have to use the dynamic keyword for a
            // dynamic reference (to an ExpandoObject) to enable dynamic dispatching!
            dynamic myExpando = new ExpandoObject();

            // Here we implicitly add a member to the ExpandoObject by _using_ (e.g. assigning) it:
            myExpando.Name = "Heidi"; // When you type the . operator, IntelliSense won't help!
            // After the action above the member Name is really present:
            string name = myExpando.Name;
            Debug.Assert("Heidi".Equals(name));
            // We can of course change the contents of the property Name afterwards:
            myExpando.Name = "Brutus";
            string otherName = myExpando.Name;
            Debug.Assert("Brutus".Equals(otherName));
            // -> This is our first example of a data-driven API! Besides: also the debugger is
            // able to view the dynamic properties of the object myExpando with the "Dynamic View".

            // Invalid! However, it is not allowed to use C#'s object initializer syntax (which is
            // similar to JavaScript's object literals) to initialize an ExpandoObject with
            // dynamically-to-be-added properties, because the properties are not visible to the
            // compiler:
            //dynamic anotherExpando = new ExpandoObject()
            //{
            //    Name = "Taylor",
            //    data.Age = 38
            //};

            // Having ExpandoObject we can also create ExpandoObject instances, which contain other
            // ExpandoObject instances as properties, in order to gain even more complex data
            // driven models:
            myExpando.Person = new ExpandoObject();
            myExpando.Person.Age = 34;
            myExpando.Person.Name = "Nico";
            // This is possible, because ExpandoObject stores all its properties as elements of
            // type object, but exposes them as type dynamic, making dynamic dispatch possible.


            /*-----------------------------------------------------------------------------------*/
            // Change the Type of a Property:

            // We are also allowed to change the type of a property:
            myExpando.Name = new Uri("http://brutus.com");
            // Here is the prove that it works: we can access the interface of the type Uri
            // directly on the property Name, because Name is no longer of type string, but of type
            // Uri!
            string absoluteUri = myExpando.Name.AbsoluteUri;
            Debug.Assert("http://brutus.com/".Equals(absoluteUri));
            myExpando.Name = "Brutus"; // Change it back to a string.
            // Again, this is possible, because ExpandoObject stores all its properties as elements
            // of type object, but exposes them as type dynamic, making dynamic dispatch possible.


            /*-----------------------------------------------------------------------------------*/
            // Adding Methods (kind of):

            // Anonymous functions allow to mimic methods for ExpandoObjects. Like for the other
            // properties you have just to assign a value (a lambda expression in this case) to a
            // property access. A cast is needed here, because anonymous functions are typeless: 
            myExpando.TwoTimes = (Func<int, int>)(x => x * x);

            // Here we call it. As you see this example technically mimics a static method, which
            // does not access the state of the object it will be called on:
            int result = myExpando.TwoTimes(8);
            Debug.Assert(result == 64);

            // It is also possible to mimic instance methods, by creation of a closure to the
            // ExpandoObject instance, on which we want to call the method:
            myExpando.LowerName = (Func<string>)(() => myExpando.Name.ToLower());
            string lowerName = myExpando.LowerName();
            Debug.Assert("brutus".Equals(lowerName));

            // You can also kind of mimic events on ExpandoObjects:
            myExpando.AnEvent = null; // Read: no event handler advised.
            myExpando.AnEvent += new EventHandler((o, ea) => Console.WriteLine("Event fired"));
            // Firing the event by calling the delegate:
            myExpando.AnEvent(myExpando, EventArgs.Empty);

            // Restriction: You can not override the methods derived from Object by method-like
            // properties (e.g. ToString())! The inherited implementations of Object will be called
            // always.
            myExpando.ToString =
                (Func<string>)(() => ((IDictionary<string, object>)myExpando).ToString());
            string s = myExpando.ToString(); // Will still call Object.ToString()!

            // Sidebar: When we call methods on an ExpandoObject, we really access properties
            // containing Delegate instances and then call Invoke() on those instances. But the 
            // compiler-generated DLR expression does indeed emit a single "InvokeMember"
            // operation, which get handled by ExpandoObject correctly (i.e. it accesses the member
            // and invokes the Delegate instance).


            /*-----------------------------------------------------------------------------------*/
            // Dealing with Properties not being present:

            try
            {
                // We could also access a property that is _not yet present_ in the ExpandoObject.
                // The compiler will let us do it, because myExpando is a dynamic object, where the
                // members are getting bound during run time. Instead we'll get a
                // RunTimeBinderException during run time.
                int age = myExpando.Age;
            }
            catch (RuntimeBinderException exc)
            {
                // RuntimeBinderException with the message "'System.Dynamic.ExpandoObject' does not
                // contain a definition for 'Age'".
            }


            /*-----------------------------------------------------------------*/
            // ExpandoObject as Dictionary:

            // There is more: ExpandoObjects can also be handled as a collection of
            // key-value-pairs. This is very similar to how _all_ objects in Python and ECMAScript
            // are represented as dictionaries as well, so it is a common thing among dynamic
            // languages. ExpandoObjects implement IDictionary<string, object> explicitly, so lets
            // iterate myExpando:
            foreach (KeyValuePair<string, object> item in myExpando)
            {
                // Each KeyValuePair-item of an ExpandoObject's implementation of
                // IDictionary<string, object> contains the name of each of the members along with
                // its value.
                Debug.WriteLine("Membername: {0}; Value: {1}", item.Key, item.Value);
            }

            // We can also access and modify members with the indexer _provided by IDictionary_:
            IDictionary<string, object> myEypandosDictionaryNature = myExpando;
            name = (string)myEypandosDictionaryNature["Name"];
            Func<int, int> theMethod = (Func<int, int>)myEypandosDictionaryNature["TwoTimes"];
            // Adding of new properties is possible either:
            myEypandosDictionaryNature["Age"] = 42;
            // We can also add members, which have an invalid C# symbol name! I.e. we can only
            // access such members with IDictionary<string, object>'s indexer, e.g. a property
            // having a whitespace in its name like so:
            myEypandosDictionaryNature["Last Name"] = "Nobody";
            // Hint: Sometimes the indexer can be used to access and modify members that have an
            // automatically generated name.

            // Sidebar: In effect we can use a dot-notation or the indexer (the later requires a
            // cast to IDictionary<string, object>) to access or modify properties similar to
            // ECMAScript. In Python you can use the special method "__dict__" to access and modify
            // a dynamic object as dictionary.

            // Of course it is better to check for the presence of a member, _before_ you try to
            // access it. You can use ExpandoObjects Dictionary nature to check for this.
            // Checking the existence of a member with ExpandoObject's Dictionary nature. - This
            // feature is required, because in a dynamically typed language you need a way to
            // "re-discover" possibly present properties.

            // A member named "Yada" _does not_ exist in myExpando:
            Debug.Assert(!((IDictionary<string, object>)myExpando).ContainsKey("Yada"));

            // A member named "Name" _does_ exist in myExpando:
            Debug.Assert(((IDictionary<string, object>)myExpando).ContainsKey("Name"));


            /*-----------------------------------------------------------------------------------*/
            // Removing Members from an ExpandoObject:

            // Alas C# does not provide an elegant way to remove members from a dynamic object.
            // But as far as ExpandoObjects are concerned, we can exploit ExpandoObject's nature of
            // being a Dictionary to remove members:
            
            // Remove the property Age:
            ((IDictionary<string, object>)myExpando).Remove("Age");
            Debug.Assert(!((IDictionary<string, object>)myExpando).ContainsKey("Age"));

            // Add the property Age again:
            ((IDictionary<string, object>)myExpando)["Age"] = 42;
            Debug.Assert(((IDictionary<string, object>)myExpando)["Age"].Equals(42));


            /*-----------------------------------------------------------------------------------*/
            // ExpandoObject implements INotifyPropertyChanged (e.g. to support Databinding):

            // ExpandoObject explicitly implements INotifyPropertyChanged. We can exploit this
            // feature to get a notification (i.e. an event), if a property has been added, or the
            // value of a dynamically added property has changed. -> .Net's Dictionary-
            // implementations can not do this trick! It allows us to use ExpandoObjects in
            // databinding scenarios (e.g. in WPF applications, please see example project
            // ExpandoDataBindingWpf in this solution).

            // Ok, so let's advise a handler to the event PropertyChanged:
            ((INotifyPropertyChanged)myExpando).PropertyChanged += 
                (object sender, PropertyChangedEventArgs e) =>
                {
                    Console.WriteLine("Property {0} has been changed to value: {1}",
                        e.PropertyName, ((IDictionary<string, object>)myExpando)[e.PropertyName]);
                };

            // After having the handler advised to the event PropertyChanged, the following
            // statements will raise PropertyChanged and will execute the handler:
            myExpando.Name = "Gertude"; // Modifies the value of a present property.
            myExpando.Sex = "Female";   // Adds a new property.


            /*-----------------------------------------------------------------------------------*/
            // ExpandoObject's Restrictions when used from C#:

            // You can not:
            // - Easily Delete a property.
            // - Create or delete an indexer.
            // - Define operators (i.e. operator methods).
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            ExpandoObjectsExamples();
        }
    }
}