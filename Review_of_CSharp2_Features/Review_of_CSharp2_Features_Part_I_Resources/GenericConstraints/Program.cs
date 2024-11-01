using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// These examples show:
// - How to constrain generic types.
// - Restrictions to the definitions of generics.
namespace GenericConstraints
{
    #region Types for ConstrainingAType:
    namespace Unconstrained
    {
        #region Unconstrained:
        public class Person
        {
            private readonly int _age;

            public Person(int age)
            {
                _age = age;
            }
        }


        public class SortedList<T>
        {
            private readonly List<T> _list = new List<T>();


            public void Add(T item)
            {
                // A very simple way to retain a sorted list after an item has been added. Not
                // optimal, but Ok to show the idea behind generic constraints.
                _list.Add(item);
                _list.Sort();
            }
        }
        #endregion
    }


    namespace Constrained
    {
        #region Constrained:
        public class Person : IComparable<Person>
        {
            private int _age;

            public Person(int age)
            {
                _age = age;
            }


            #region IComparable<Person> Member
            public int CompareTo(Person other)
            {
                return _age.CompareTo(other._age);
            }
            #endregion
        }


        // Here we've established a conversion type constraint, constraining T to implement
        // IComparable<T>
        public class SortedList<T> where T : IComparable<T>
        {
            private readonly List<T> _list = new List<T>();


            public void Add(T item)
            {
                // A very simple way to retain a sorted list after an item has been added. Not
                // optimal, but Ok to show the idea behind generic constraints.
                _list.Add(item);
                _list.Sort();
            }
        }
        #endregion
    }


    #endregion


    public class Program
    {
        public static void ConstrainingAType()
        {
            /*-----------------------------------------------------------------------------------*/
            // Shows the Rationale behind Constraining a generic Type:

            // First, let's try to use the unconstrainted SortedList; we'd like to add some Persons
            // to it. The Persons will be sorted in the list while adding them.
            Unconstrained.SortedList<Unconstrained.Person> myList1 =
                new Unconstrained.SortedList<Unconstrained.Person>();
            myList1.Add(new Unconstrained.Person(42));
            //myList1.Add(new Unconstrainted.Person(24));
            // This last call to Add() will fail with an InvaidOperationException: "Failed to 
            // compare two elements in the array. At least one object must implement IComparable."
            // The operation List<T>.Sort() is only able to sort objects that implement the
            // interface IComparable.

            // By constraining SortedList<T> to accept only types that implement IComparable<T>, we
            // transform this run time error into a compile time error as a first step. The 
            // constrainted SortedList<T> is contained in the namespace "Constrainted":
            //Constrainted.SortedList<Unconstrainted.Person> myList2 = 
            //    new Constrainted.SortedList<Unconstrainted.Person>();
            // Invalid! This statement won't compile, because we can not use the present
            // implementation of Person, as it does not implement IComparable<T>.

            // So in the next step we've to provide an implementation of Person, which implements
            // the interface IComparable<T>. This new type can be found in the namespace
            // "Constrainted":
            Constrained.SortedList<Constrained.Person> myList3 =  // Fine.
                new Constrained.SortedList<Constrained.Person>();
            myList3.Add(new Constrained.Person(42));
            myList3.Add(new Constrained.Person(24));
        }


        #region Methods for ImpossibleGenerics():
        public static U SumAllItems<T, U>(T data) where T : IEnumerable<U>
        {
            U sum = default(U); // Fine, default expression used.

            foreach (U item in data)
            {
                //sum += item; // Invalid! Operator '+=' can't be applied on U.

                // Only the interface of type arguments can be checked within generic code. In .Net
                // static methods/properties/fields/delegates don't belong to this interface.
                // Consequence: you can't use (mathematical) operators on “open” types!

                // Sidebar: The operators != and == can be used when the type is constrainted to be
                // a reference type. But then only the references will be compared. If the types
                // are further constrainted to derive from a special reference type the overloads
                // of == and != of exactly that derived type will be called. It is possible to use
                // various operators with the advent of .Net 4/C#4 and dynamic typing.
            }
            return sum;
        }
        #endregion


        private static void ImpossibleGenerics()
        {
            /*-----------------------------------------------------------------------------------*/
            // Impossible to call Operators on an open Type:

            int sum = SumAllItems<IEnumerable<int>, int>(new int[] { 1, 2, 3, 4, 5, 6 });
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            ConstrainingAType();

            ImpossibleGenerics();
        }
    }
}