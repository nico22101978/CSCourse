using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;  
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;

using Microsoft.CSharp.RuntimeBinder;


// These examples show:
// - Home brew dynamic dispatch with the CLR type DynamicObject.
namespace DynamicObjects
{
    public class Program
    {
        #region Types for DynamicObjectExamples():
        /// <summary>
        /// This type implements a dynamic property bag. In the accompanying source code, one can
        /// find richly commented Ruby, Python, Smalltalk and Obj-C implementations of a property
        /// bag. (Please have a look, it is really not difficult to understand it in that
        /// languages!)
        /// </summary>
        public class MyPropertyBag : DynamicObject
        {
            // In opposite to ExpandoObject we have to derive a new class from DynamicObject to put
            // it into effect (the class Expandoobject is sealed and can not act as base class).
            // In that new class we can implement the desired dynamic behavior. DynamicObject
            // provides a set of overrideable methods, which will be called during dynamic
            // dispatch. These TryXXX() methods will handle the dynamic calls to an instance of
            // MyPropertyBag.


            private readonly IDictionary<string, object> _properties =
                new Dictionary<string, object>();


            // In this example we'll only override the behavior of dynamically getting, setting
            // and creating properties (TryGetMember() and TrySetMember()), to mimic the behavior
            // of the type ExpandoObject. We could, however, define dynamic behavior for other ten
            // (in sum: tweleve) dynamic operations such as "InvokeMember" or "BinaryOperation"! -
            // So deriving from DynamicObject will give to us very much control over dynamic
            // behavior!
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                // The binder's property IgnoreCase indicates, whether the calling language binds
                // to case sensitive or case insensitive symbols (e.g. the language binder of VB
                // sets IgnoreCase to true). Here we handle this case in a very simple manner:
                string searchName = binder.IgnoreCase ? binder.Name.ToUpper() : binder.Name;

                // If the searchName is present in _properties, true will be returned, which means,
                // that the binding was successful. Otherwise false will be returned and the
                // binding fails (a RuntimeBinderException will be thrown by the binder in this
                // case). 
                return _properties.TryGetValue(searchName, out result);
            }


            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                // Please see TryGetMember()!
                string putName = binder.IgnoreCase ? binder.Name.ToUpper() : binder.Name;

                if (_properties.ContainsKey(putName))
                {
                    _properties[putName] = value;
                }
                else
                {
                    _properties.Add(putName, value);
                }
                // This will always succeed.
                return true;
            }


            // The dynamic operations "CreateInstance", "DeleteIndex" and "DeleteMember" can't be
            // expressed in C# syntax. No run time binders required for these methods are even
            // existent in C#:
            public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args,
                out object result)
            {
                return base.TryCreateInstance(binder, args, out result);
            }


            public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
            {
                return base.TryDeleteIndex(binder, indexes);
            }


            public override bool TryDeleteMember(DeleteMemberBinder binder)
            {
                return base.TryDeleteMember(binder);
            }


            // Definition of a new member:
            // The property MemberCount will _also be bound at run time_, if MemberCount was called
            // on an instance of MyPropertyBag bound to a dynamic reference. I.e. if there are
            // methods directly defined in your DynamicObject subclass, they will be called
            // _w/o dynamic dispatching_ (i.e. via the TryXXX-methods). - The language binder can
            // handle ("shortcut") this case. All other accesses to members will go their way
            // "through" dynamic dispatch. 
            public int MemberCount
            {
                get
                {
                    return _properties.Count;
                }
            }


            // Definition of an override:
            // As for the property MemberCount, the method GetDynamicMemberNames() will be called
            // w/o dynamic dispatch, because it is directly defined in the class MyPropertyBag,
            // even as it is an override (The language binder will handle this case as well.).
            // The method GetDynamicMemberNames() can be overridden if we want to issue the
            // information about the members' names. It was mainly designed for debugging purposes,
            // because .Net reflection is _not functional_ for dynamic objects. (It is similar to
            // Python's function "dir".)
            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _properties.Keys;
            }


            // Now we can also overridde ToString() and all the other virtual methods inherited
            // from Object (this was not possible with ExpandObject):
            public override string ToString()
            {
                return _properties.ToString();
            }
        }
        #endregion


        private static void DynamicObjectExamples()
        {
            /*-----------------------------------------------------------------------------------*/
            // Basic Usage of DynamicObjects:

            // See the implementation of MyPropertyBag above!

            // Mind, that we have to use the dynamic keyword for a dynamic Object (as DynamicObject)
            // to enable dynamic dispatching!
            dynamic myPropertyBag = new MyPropertyBag();

            // We can now use a MyPropertyBag instance like an ExpandoObject instance, because we
            // redefined the behavior on GetMember/SetMember accordingly:
            myPropertyBag.Name = "Heidi"; // When we type the . operator, IntelliSense won't help!
            // After the action above the member Name is really present:
            string name = myPropertyBag.Name;
            Debug.Assert("Heidi".Equals(name));
            // We can of course change the contents of the property Name afterwards:
            myPropertyBag.Name = "Brutus";
            string otherName = myPropertyBag.Name;
            Debug.Assert("Brutus".Equals(otherName));


            /*-----------------------------------------------------------------------------------*/
            // Calling Methods being present during Compile Time (directly implemented in the
            // Type we're going to use with dynamic Dispatch):

            // Calling a member being present in type MyPropertyBag at compile time (MemberCount in
            // this example) will not issue a dynamic dispatch (TryGetMember() won't be called)!
            // Rather MemberCount will be bound by the language binder and that allows to call
            // MyPropertyBag.MemberCount directly.
            int nMembers = myPropertyBag.MemberCount;
            // The same is valid for methods being overridden in MyPropertyBag.
            IEnumerable<string> memberNames = myPropertyBag.GetDynamicMemberNames();


            /*-----------------------------------------------------------------------------------*/
            // Dealing with dynamic Operations not being defined:

            try
            {
                // If we try to call a method on an instance of MyPropertyBag, the _inherited_
                // default implementation of TryInvokeMember() will be called (as this is a dynamic
                // "InvokeMember" operation), because we _didn't override_ the dynamic behavior of
                // the "InvokeMember" operation (i.e. we didn't override TryInvokeMember()). The 
                // inherited default implementation of TryInvokeMember() will return false to the
                // calling binder. Then the binder will call TryGetMember(), because DoIt() could
                // also be a member, which stores a callable object like a Delegate instance. - But
                // there is no member named "DoIt" present in the property dictionary, so
                // TryGetMember() will return false and because of this the binder will finally
                // "surrender" and throw a RuntimeBinderException (see the handler below).
                int age = myPropertyBag.DoIt(42);
            }
            catch (RuntimeBinderException exc)
            {
                // RuntimeBinderException with the message "'MyPropertyBag' does not contain a
                // definition for 'DoIt'".
            }

            // Of course it is better to check for the presence of a member, _before_ we try to
            // access it. You can use GetDynamicMemberNames() to check for this. - If we want to
            // use LINQ to perform the check, we'll need to cast down to IEnumerable<string>, in
            // order to _not_ to dynamically dispatch the call to LINQ methods:
            Debug.Assert(
                !((IEnumerable<string>)myPropertyBag.GetDynamicMemberNames())
                    .Any(p => p == "Age"));
            Debug.Assert(
                ((IEnumerable<string>)myPropertyBag.GetDynamicMemberNames())
                    .Any(p => p == "Name"));


            /*-----------------------------------------------------------------------------------*/
            // DynamicObject's Restrictions:

            // We can not dynamically dispatch following operations in C#:
            // - Delete a property.
            // - Create or delete an indexer.
            // - Create an instance.

            // But these operations are available in other dynamically typed languages (e.g. in
            // IronPython and IronRuby).

            // DynamicObject does not allow to dynamically dispatch calls to static methods.
        }


        private static void PracticalWorkingWithDynamicObjects()
        {
            /*-----------------------------------------------------------------------------------*/
            // How to tell dynamic Objects from non-dynamic Objects at Run Time under the Cover of
            // the Keyword dynamic:

            dynamic myExpando = new ExpandoObject();
            myExpando.Name = "Heidi";

            // Reflection will not work with the dynamically added property "Name", because the
            // dynamic type information of the object is part of the object's _value_:
            PropertyInfo nameProperty = myExpando.GetType().GetProperty("Name");
            Debug.Assert(null == nameProperty);
            // Instead we can check, whether myExpando is a type provided by the DLR and implements
            // the interface IDynamicMetaObjectProvider) like so:
            bool isDlrType = myExpando is IDynamicMetaObjectProvider;
            Debug.Assert(isDlrType);
            // Whereas it will not work for other types (even if "hidden" by a dynamic reference):
            dynamic foo = "Janet";
            isDlrType = foo is IDynamicMetaObjectProvider;
            Debug.Assert(!isDlrType);
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            DynamicObjectExamples();


            PracticalWorkingWithDynamicObjects();
        }
    }
}