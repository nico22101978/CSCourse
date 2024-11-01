using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

// This example shows: 
// - How LINQ maps to IEnumerable<T>'s extension methods as query operators.
namespace LINQExtensionMethods
{
    public class Program
    {
        /*---------------------------------------------------------------------------------------*/
        // This Section shows the Compiler generated Code of Queries that do neither access local
        // nor Instance Variables:


        #region Compiler generated Stuff for resultSequence1's Query:
        // Because to get resultSequence1 no local or instance variables have been accessed, a
        // static method is sufficient to express the lambda expression of the select clause.
        private static int HiddenFunc(int n)
        {
            return n * n;
        }

        private static Func<int, int> HiddenDelegateDefinition;
        #endregion


        private static void SimpleProjection(IEnumerable<int> sequence)
        {
            // In the simplest case, a LINQ expression just reads from the input sequence and the
            // lambda expression in the select clause does only operate on variables being defined
            // in that query:

            // The implicitly typed variable n is called "range variable". Esp. because the from
            // clause comes first, VS' IntelliSense is fully functional on the introduced range
            // variables and can be used immediately in the where and select clauses:
            var resultSequence1 = from n in sequence
                                  select n * n;
            #region Compiler generated Stuff for resultSequence1's Query:
            // The static Delegate instance that contains the lambda expression's code is buffered.
            if (null == HiddenDelegateDefinition)
            {
                HiddenDelegateDefinition = new Func<int, int>(HiddenFunc);
            }
            // The LINQ query boils down to a call to IEnumerable<T>'s extension method Select().
            // As can be seen the from clause does not translate to any special query operator
            // call.
            IEnumerable<int> resultSequence1a =
                Enumerable.Select<int, int>(sequence, HiddenDelegateDefinition);
            #endregion

            // Sidebar: The expression being used in the select clause must be a non-void
            // expression, i.e. it needs to produce a value as result of the projection! - It means
            // you can not just call a void method in a select clause. You can find projections
            // like the select clause in virtually all functional programming languages; often this
            // function is called "map".
            // So this is invalid:
            //var resultSequence =
            //    from n in sequence
            //    select Console.WriteLine(n);
            // What projection should be created and to which type should the result be inferred as
            // WriteLine returns void?
        }


        /*---------------------------------------------------------------------------------------*/
        // This Section shows the Compiler generated Code of Queries that do access Instance
        // Variables:


        #region Compiler generated Stuff for resultSequence2's Query:
        // Because to express the select clause no local or instance variables has been accessed,
        // a static method is sufficient to build the lambda expression of the select clause.
        private static int SelectClause2(int n)
        {
            return n * n;
        }

        // Because to express the where clause an instance variable has been accessed, an instance
        // method must be used to build the lambda expression of the where clause. -> The field
        // remainder must be accessed.
        private bool WhereClause2(int n)
        {
            return 0 == (n % this.remainder);
        }

        private static Func<int, int> SelectDelegate;
        #endregion


        // Instance variable remainder.
        private readonly int remainder = 2;

        public void SelectionWithInstanceVariable(IEnumerable<int> sequence)
        {
            // This query expression reads from the input sequence, the lambda expression in the
            // select clause does only operate on variables being defined in that query, but the
            // where clause accesses an instance variable (remainder):
            var resultSequence2 = from n in sequence
                                  where n % remainder == 0
                                  select n * n;
            #region Compiler generated Stuff for resultSequence2's Query:
            if (null == SelectDelegate)
            {
                SelectDelegate = new Func<int, int>(SelectClause2);
            }
            // The LINQ query boils down to a call to IEnumerable<T>'s extension methods Select()
            // and Where().
            IEnumerable<int> resultSequence2a = 
                Enumerable.Select<int, int>(
                    Enumerable.Where<int>(sequence, new Func<int, bool>(this.WhereClause2)),
                        SelectDelegate);
            #endregion
        }


        /*---------------------------------------------------------------------------------------*/
        // This Section shows the Compiler generated Code of Queries that apply degenerate Select:


        #region Compiler generated Stuff for resultSequence3's Query:
        // The same code as for the where clause of the previous example must be generated.
        private bool WhereClause3(int n)
        {
            return 0 == (n % this.remainder);
        }
        #endregion


        #region Compiler generated Stuff for resultSequence4's Query:
        private int SelectClause4(int n)
        {
            return n;
        }
        #endregion


        private void DegenerateSelect(IEnumerable<int> sequence)
        {
            // This query expression reads from the input sequence, filters the read values and
            // selects the filtered values. No new special projection (as n * n in the other
            // examples) is used here.
            var resultSequence3 = from n in sequence
                                  where n % remainder == 0
                                  select n;
            #region Compiler generated Stuff for resultSequence3's Query:
            // The where clause is transformed as in the previous example, but the call to the
            // extension method Select() is optimized away! (Yes, also if you've provided a
            // user-defined implementation of Select()!) The call to Select() would just select
            // the results produced by Where() and this result is different from the input
            // sequence. This is called a _degenerate select_ and degenerate selects can be
            // optimized away.
            IEnumerable<int> resultSequence3a =
                Enumerable.Where<int>(sequence, new Func<int, bool>(this.WhereClause3));
            #endregion

            // Notice, that for mere select clauses there is _no_ degenerate select! The input
            // sequence and the result sequence would be equivalent and this is not allowed in
            // query expressions. Therefor a call to Select() _will_ be applied in order to make
            // the input sequence different from the result sequence. In other words: a "no-op"
            // projection clause at the end of the query will only be optimized away if there are
            // other query operators different from Cast() before that query operator.
            var resultSequence4 = from n in sequence
                                  select n;
            #region Compiler generated Stuff for resultSequence4's Query:
            // There is also another need, not to optimize Select() away in this case: you could
            // have provided a customized overload of Select(), which has to be called.
            IEnumerable<int> resultSequence4a =
                Enumerable.Select<int, int>(sequence, new Func<int, int>(this.SelectClause4));
            // In other words: resultSequence4a must be different from sequence (i.e. it will not
            // degenerate to a statement like "var resultSequence4a = sequence;")!
            // !!The result of a query expression is never the same object as the source data!!
            #endregion
        }


        /*---------------------------------------------------------------------------------------*/
        // This Section shows the Compiler generated Code of Queries that apply local Closure:

        #region Compiler generated Types for resultSequence4's Query:
        // This closure is created to hold all the local variables during the execution of the LINQ
        // expressions being defined in another "surrounding" method. If the "surrounding" method
        // was an instance method an additional public field would be added, which contained the
        // this reference. The local variables are managed as public fields in the closure type 
        // (e.g. numValues).
        private sealed class Closure
        {
            public int numValues;
            public int SelectClause(int n)
            {
                return ((n * n) / ++this.numValues);
            }
        }
        #endregion


        private static void QueryWithLocalClosure(IEnumerable<int> sequence)
        {
            int numValues = 0;
            // This LINQ expression reads from the input sequence, the lambda expression in the
            // select clause does operate on variables being defined in that query and on the local
            // variable numValues:
            var resultSequence4 = from n in sequence
                                  select n * n / ++numValues;
            #region Compiler generated Stuff for resultSequence4's Query (MS specific):
            // The local variable is completely expressed by the closure's public field numValues,
            // that means that the method's code and the lambda's code can access and modifiy the
            // same shared variable.
            Closure c = new Closure();
            c.numValues = 0;
            IEnumerable<int> resultSequence4a =
                Enumerable.Select<int, int>(sequence, new Func<int, int>(c.SelectClause));
            //
            // Attention, there is a source of memory leaks:
            // As can seen by this code, the local numValues will be stored in the Closure's
            // instance. If we returned the result of the query expression from this method, we 
            // prolonged the lifetime of the value of numValues to the outside of this method. This
            // could be a problem if you capture locals of reference type that hold big resources,
            // as you may loose control, when they are getting gc'd. If such a big resource needed
            // to be disposed off, you would have to code very carefully, or you are even better
            // off not capturing such a resource.
            #endregion

            // But it should be said that unwanted capture to modified variables and multiple
            // (apparently independent) query calls can be a nasty source of bugs!
        }


        #region Types for ProjectionAsConversion():
        // Our toy: class Person with some properties and ctors.
        public class Person : IComparable<Person>, IEquatable<Person>
        {
            // Dctor.
            public Person()
            { }


            // Another ctor awaiting a string (the name).
            public Person(string name)
            {
                Name = name;
            }


            public int Age { get; set; }


            public string Name { get; set; }


            public override string ToString()
            {
                return string.Format("Name: {0}, Age: {1}", Name, Age);
            }


            public int CompareTo(Person other)
            {
                return Age.CompareTo(other.Age);
            }


            public override int GetHashCode()
            {
                return Age.GetHashCode() ^
                        (null != Name
                            ? Name.GetHashCode()
                            : 0);
            }


            public override bool Equals(object obj)
            {
                return null != obj && GetType().Equals(obj.GetType()) && Equals((Person)obj);
            }


            public bool Equals(Person other)
            {
                return null != other && Age.Equals(other.Age) && object.Equals(Name, other.Name);
            }
        }
        #endregion


        #region Compiler generated Stuff for resultSequence5's Query:
        // See HiddenFunc1()!
        private static Person HiddenFunc5(int n)
        {
            Person p = new Person();
            p.Name = (n <= 35)
                        ? "Jane Doe"
                        : "Jon Doe";
            p.Age = n;

            return p;
        }

        private static Func<int, Person> HiddenDelegateDefinition5;
        #endregion


        private static void ProjectionAsConversion(IEnumerable<int> sequence)
        {
            // This example shows a projection by converting the incoming int sequence to a
            // sequence of Person objects. Notice how the new C#3 initialization syntax
            // comes in handy!
            var persons = from age in sequence
                          select new Person
                          {
                              Name = (age <= 35)
                                        ? "Jane Doe"
                                        : "Jon Doe",
                              Age = age
                          };

            foreach (var item in persons)
            {
                Debug.WriteLine(item);
            }

            #region Compiler generated Stuff for resultSequence5's Query:
            // Buffering.
            if (null == HiddenDelegateDefinition5)
            {
                HiddenDelegateDefinition5 = new Func<int, Person>(HiddenFunc5);
            }
            // The LINQ query boils down to a call to IEnumerable<T>'s extension method Select().
            IEnumerable<Person> personsa =
                Enumerable.Select<int, Person>(sequence, HiddenDelegateDefinition5);
            #endregion
        }


        #region Compiler generated Stuff for CrossJoin's Query:
        internal class AnonymousType_intint : IEquatable<AnonymousType_intint>
        {
			// Mind, that the generated properties are readonly! They will be "automagically"
			// initialized with the initializer syntax.
            public int i { get; }
			public int j { get; }

            public bool Equals(AnonymousType_intint other)
            {
                return null != other && i == other.i && j == other.j;
            }


            public override bool Equals(object obj)
            {
                return null != obj 
                        && GetType().Equals(obj.GetType())
                        && Equals((AnonymousType_intint)obj);
            }


            public override int GetHashCode()
            {
				// Something like this:
                return i.GetHashCode() ^ j.GetHashCode();
            }


            public override string ToString()
            {
                return string.Format("{{ i = {0}, j = {1} }}", i, j);
            }
        }
        #endregion


        private static void CrossJoin(IEnumerable<int> sequence)
        {
            /*-----------------------------------------------------------------------------------*/
            // Projection of Cross Joins:

            // This example shows how unrelated sequences combine to a projection of the cartesian
            // product, i.e. all combinations of the items in each sequence. The resulting
            // "cross join" has a count of sequence1.Count() * sequence2.Count() * ... 
            // sequencen.Count(). The resulting projection is put into an anonymous type. Cross 
            // joins are well suited to generate test data!
            var crossJoin = from i in sequence
                            from j in sequence
                            select new { i, j };
            foreach (var item in crossJoin)
            {
                Debug.WriteLine(item);
            }

            #region Compiler generated Stuff for crossJoin's Query:
            // This LINQ expression is translated into a call of IEnumerable<T>'s extension method
            // SelectMany(). In the previous examples "introductionary" from clauses were never
            // translated into calls to query operators, but multiple from clauses will be
            // translated into cascaded calls to SelectMany() (for each alinged from clause one
            // cascade) in order to combine all the sources of the cartesian product. (Generated
            // types and methods and Enumerable.SelectMany-cascades have been elided.)
            var crossJoina = sequence.SelectMany(i => sequence.Select(j => new { i, j }));
            #endregion
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Our Test Data to play with:

            var someNumbers = Enumerable.Range(0, 10);


            /*-----------------------------------------------------------------------------------*/
            // Calling the Test Methods:

            SimpleProjection(someNumbers);


            var program = new Program();
            program.SelectionWithInstanceVariable(someNumbers);


            program.DegenerateSelect(someNumbers);


            QueryWithLocalClosure(someNumbers);


            ProjectionAsConversion(Enumerable.Range(25, 20));


            CrossJoin(someNumbers);
        }
    }
}