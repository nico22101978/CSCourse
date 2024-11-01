using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;

// Closed generic type alias:
using TelephoneDirectory = System.Collections.Generic.Dictionary<string, int>;

// These examples show:
// - The lack in the type system of .Net 1 and how to solve it with generic types in .Net 2.
// - The terms unbound generic type, closed/open constructed type and open type.
namespace GenericTypes
{
    public class Program
    {
        #region Methods for ObjectBasedVsGenericCollections:
        public static void PrepareListWithObjectBasedCollection()
        {
            // ArrayList is an Object type collection.
            ArrayList listOfStrings = new ArrayList();
            listOfStrings.Add("Mainform");
            listOfStrings.Add("Document1");

            // Let's pass the created string list with Form titles to the next method:
            HandleFormsWithObjectBasedCollection(listOfStrings);
        }


        public static void HandleFormsWithObjectBasedCollection(ArrayList listOfForms)
        {
            // The Form list did arrive, we have to focus the first item! 
            if (0 != listOfForms.Count)
            {
                // But, these are not the awaited types, we can't deal with strings, we awaited
                // Forms!
                // This cast will fail! A InvalidCastException will be thrown!
                ((Form)listOfForms[0]).Focus();
            }
        }


        public static void PrepareListWithGenericCollection()
        {
            // This time List<string> is a generic Collection.
            List<string> listOfStrings = new List<string>(); 
            listOfStrings.Add("Mainform");
            listOfStrings.Add("Document1");
            
            // Let's pass the created string list with Form titles to the next method. This code
            // won't compile, because C# cannot convert from 'List<string>' to 'List<Form>'.
            //HandleFormsWithGenericCollection(listOfStrings);
        }


        public static void HandleFormsWithGenericCollection(List<Form> listOfForms)
        {
            // The Form list did arrive, we have to focus the first item! 
            if (0 != listOfForms.Count)
            {
                // This code is type safe, no downcast from the object-item at index 0 of the
                // collection to Form is required.
                listOfForms[0].Focus();
            }
        }
        #endregion


        public static void ObjectBasedVsGenericCollections()
        {
            /*-----------------------------------------------------------------------------------*/
            // This Examples show the Lack in the Type System of .Net 1/C#1 and how to solve it 
            // with Generics:

            // 1. Provoking the lack in the type system:
            try
            {
                // Will throw InvalidCastException:
                PrepareListWithObjectBasedCollection();
            }
            catch (InvalidCastException exc)
            {
                Debug.WriteLine("A type problem at run time: "+exc.Message);
            }

            // 2. Closing the lack in the type system with generics:
            // Now we'll call equivalent code expressed with generics. - As you see in the code of
            // the method PrepareListWithGenericCollection() a part of that method could not be
            // compiled! This is because the run time errors we found with object based collections
            // have been transformed into compile time errors with generics. 
            PrepareListWithGenericCollection();
        }


        #region Types for UnboundAndClosedGenericTypes:
        // ADelegate is a C# generic type definition and a .Net _unbound generic type_. (This term
        // is not official for type declarations, see UnboundAndClosedGenericTypes()).
        public delegate void ADelegate<T>(T t);


        // AType is a C# generic type definition and a .Net _unbound generic type_. (This term is
        // not official for type declarations, see UnboundAndClosedGenericTypes()).
        public class AType<T>
        {
            // The type T is a .Net _open type_, the type will be filled, as soon as a type
            // argument will be passed. 
            private T _t;


            // The same is valid for arrays: The type T is a .Net _open type_, the type will be
            // filled, as soon as a type argument will be passed. 
            private T[] _tt;


            // The type List<T> is a .Net _open constructed type_. The construction of the type
            // List<T> depends on the open type T.
            private List<T> _list = new List<T>();


            // The type List<int> is a .Net _closed constructed type_.
            private List<int> _intList = new List<int>();


			// Static fields are created per constructed type. So Count is _not_ shared among
			// different constructed types, i.e. AType<int>.Count is a field different from
			// AType<string>.Count!
			public readonly static int Count = 20;


            // The type KeyValuePair<T, string> is still a .Net _open constructed type_, because
            // the construction of the type KeyValuePair<T, string> depends on the open type T.
            private KeyValuePair<T, string> _pair = new KeyValuePair<T, string>();
        }
        #endregion


        public static void UnboundAndClosedGenericTypes()
        {
            /*-----------------------------------------------------------------------------------*/
            // Unbound Generic Types must also be used to get the respective CLR Type:

            // In fact unbound generic types can only be used in typeof expressions, the
            // declaration of e.g. class AType<T>{} is not officially called unbound generic types.

            // Just use the generic type name with a empty pair or angle brackets to express that
            // you mean the unbound generic type:
            Type unboundGenericTypeWithOneTypeParameter = typeof(List<>);
            Debug.WriteLine(unboundGenericTypeWithOneTypeParameter);
            // This will print "System.Collections.Generic.List`1[T]", whereby 1 means the count of
            // type parameters.

            // If you need the CLR type of an unbound generic type having two or more type
            // parameters, you have to place commas to separate the "unwritten" type parameters
            // with a pair of angle braces like this:
            Type unboundGenericTypeWithTwoTypeParameters = typeof(KeyValuePair<,>);
            Debug.WriteLine(unboundGenericTypeWithTwoTypeParameters);
            // This will print "System.Collections.Generic.KeyValuePair`2[TKey,TValue]", whereby
            // 2 means the count of type parameters.

            // In opposite to the examples above this expression yield the CLR of a constructed
            // generic type (the CLR type of List<int>):
            Type constructedGenericType = typeof(List<int>);
            Debug.WriteLine(constructedGenericType);
            // This will print "System.Collections.Generic.List`1[System.Int32]", whereby 1 means
            // the count of type parameters.


            /*-----------------------------------------------------------------------------------*/
            // Reflection on generic Types:

            // Do you want to know, whether a type is a generic type at all? - Just use Type's
            // property IsGenericType.
            Debug.Assert(unboundGenericTypeWithOneTypeParameter.IsGenericType);
            Debug.Assert(constructedGenericType.IsGenericType);
            // If you need to know, whether the type in your hands is an unbound generic type or a
            // constructed generic type you can use Type's property IsGenericTypeDefinition.
            Debug.Assert(unboundGenericTypeWithOneTypeParameter.IsGenericTypeDefinition);
            Debug.Assert(!constructedGenericType.IsGenericTypeDefinition);
        }

        
        private static void ClosedGenericTypeAlias()
        {
            /*-----------------------------------------------------------------------------------*/
            // Closed Generic Type Aliases in Action:

            // You can create an instance of a closed generic type like this:
            Dictionary<string, int> directory1 = new Dictionary<string, int>();

            // Or like this using the generic type alias for Dictionary<string, int> (as defined
            // above):
            TelephoneDirectory directory2 = new TelephoneDirectory();
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            ObjectBasedVsGenericCollections();

            UnboundAndClosedGenericTypes();

            ClosedGenericTypeAlias();
        }
    }
}
