using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;

using CommonTypes;

// These examples show:
// - The nuts and bolts of LINQ to Objects.
// - The usage of order by, group by and having clauses.
// - The usage of aggregate functions, additional variables, transparent identifiers,
//   distinct results and the EqualityComparer mechanism.
// - The usage of joins.
// - Set operations with LINQ.
// - The usage of LINQ with non-generic collections and result materialization.
// - How to circumvent dangerous query operators.
// - The usage of LINQ to XML to show functional construction.
namespace MoreLINQ
{
    #region Utilities:
    internal static class Utilities
    {
        /// <summary>
        /// This method splits the passed string at its newlines. The execution of the method
        /// (i.e. the fetching of the resulting strings) is deferred.
        /// </summary>
        /// <param name="contents">A string that contains newlines to be split. </param>
        /// <returns>The resulting strings in between newlines of the input string.</returns>
        public static IEnumerable<string> ToLines(this string contents)
        {
            using (var sr = new StringReader(contents))
            {
                string line;
                while (null != (line = sr.ReadLine()))
                {
                    yield return line;
                }
            }
        }


        /// <summary>
        /// This method will produce the sequence of integers with increasing values starting by
        /// zero.
        /// </summary>
        /// <returns>The sequence of integers with increasing values.</returns>
        public static IEnumerable<int> AllIntegers()
        {
            for (int i = 0; i <= int.MaxValue; ++i)
            {
				Console.WriteLine("AllIntegers: "+i);
                yield return i;
            }
        }


        /// <summary>
        /// Officially:
        /// A deferred filter function (like the query operator Where()).
        /// Inofficially:
        /// The problem with that implementation of query operator Where() is that the
        /// preconditions null != input and null != predicate are checked when the result will be
        /// iterated, i.e. in a deferred manner. This is too late, the arguments have to be
        /// immediately checked when MyWhere() _is called_.
        /// </summary>
        /// <exception cref="ArgumentNullException">An ArgumentNullException (instead of a
        /// NullReferenceException) is thrown if either <paramref name="input" /> or
        /// <paramref name="predicate" /> is null. Esp. for <paramref name="input" /> this makes
        /// sense! - Because even if this method could be called like a member of IEnumerable<T>,
        /// whereby the "argument character" of input is invisible, we could also call this method
        /// with input as visible argument. As we see the ArgumentNullException is exactly
        /// correct. - We should also follow this pattern in our extension methods for the "this"
        /// parameter.
        /// </exception>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="input">An IEnumerable of T to filter.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An IEnumerable of T that contains elements from the input sequence that
        /// satisfy the condition.</returns>
        public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> input,
            Func<T, bool> predicate)
        {
            if (null == input)
            {
                throw new ArgumentNullException("input");
            }

            if (null == predicate)
            {
                throw new ArgumentNullException("predicate");
            }

            foreach (T item in input)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }


        /// <summary>
        /// Officially:
        /// A deferred filter function (like the query operator Where()).
        /// Inofficially:
        /// Better than MyWhere()! Now the arguments input and predicate will be checked when
        /// MyWhere2() is called immediately, not when the result is iterated. The deferred part
        /// of the execution (i.e. using the yield return statement) is done in MyWhere2Impl().
        /// Btw, the default implementation of the query operator Where() is implemented like that.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="input">An IEnumerable of T to filter.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An IEnumerable of T that contains elements from the input sequence that
        /// satisfy the condition.</returns>
        public static IEnumerable<T> MyWhere2<T>(this IEnumerable<T> input,
            Func<T, bool> predicate)
        {
            if (null == input)
            {
                throw new ArgumentNullException("input");
            }

            if (null == predicate)
            {
                throw new ArgumentNullException("predicate");
            }

            return MyWhere2Impl(input, predicate);
        }


        /// <summary>
        /// A deferred filter function.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="input">An IEnumerable of T to filter.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An IEnumerable of T that contains elements from the input sequence that
        /// satisfy the condition.</returns>
        private static IEnumerable<T> MyWhere2Impl<T>(this IEnumerable<T> input,
            Func<T, bool> predicate)
        {
            foreach (T item in input)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }
    }
    #endregion


    public class Program
    {
        private static void OrderByExamples(IEnumerable<Person> persons)
        {
            /*-----------------------------------------------------------------------------------*/
            // Order by:

            // This query expression selects all persons from the passed sequence and orders them
            // by the Age property in an ascending (this is the default) manner. Alternatively
            // the keyword "ascending" can be written explicitly to make the ascending order clear
            // to the reader. The type of the property that is used in the order by clause must 
            // implement the interfaces IComparable or IComparable<T>. 
            // The orderby operator in LINQ to Objects is stable, i.e two equal items will be
            // returned in the original order. Other .Net sorts algorithms are not stable.
            var personsByAge = from person in persons
                               orderby person.Age //ascending
                               select person;
            // Orderby buffers the input sequence right when the result is iterated.
            foreach (var person in personsByAge)
            {
                Debug.WriteLine(person);
            }

            // This query expression also selects all persons from the passed sequence, but orders
            // them by the person itself. In this case the method CompareTo() of Person's 
            // implementation of the interface IComparable<T> is called by the order by clause.
            var personsByPerson = from person in persons
                                  orderby person
                                  select person;
            foreach (var person in personsByPerson)
            {
                Debug.WriteLine(person);
            }


            /*-----------------------------------------------------------------------------------*/
            // Order by with Then by:

            // This query expression also selects all persons from the passed sequence, but orders
            // them by the property Age and then by the property Name in an ascending manner.
            var personsByAgeAndName = from person in persons
                                      orderby person.Age, person.Name
                                      select person;
            // The compiler will translate this query expression to following chain of method
            // calls:
            var personsByAgeAndName2 =
                persons
                    .OrderBy(person => person.Age)
                    .ThenBy(person => person.Name);
            // As can be seen the second orderby criterion "Name" will be translated into an extra
            // call to the extension method ThenBy(). ThenBy() is not a "first-class" query
            // operator, it is not available as extension method of IEnumerable<T>, but it is an 
            // extension method of the interface IOrderedEnumerable<T>. So we come to the
            // conclusion that OrderBy() returns a IOrderedEnumerable<T>,

            // Above I explained that orderby buffers the input sequence when the iteration of the
            // result is started. But when multiple values to order by have been specified (i.e.
            // input.OrderBy(...).ThenBy(...)) the last ThenBy() call will initiate the buffering
            // of the OrderBy()'s input sequence, rather than sorting separately for each key. The
            // special handling/implementation of IOrderedEnumerable<T> makes this possible.
            foreach (var person in personsByAge)
            {
                Debug.WriteLine(person);
            }

            // This query expression also selects all persons from the passed sequence, but orders
            // them by the property Age a descending manner.
            var personsByAgeDesc = from person in persons
                                   orderby person.Age descending
                                   select person;
            // The compiler will translate this query expression to following chain of method
            // calls:
            var personsByAgeDesc2 = persons.OrderByDescending(person => person.Age);
            foreach (var person in personsByAgeDesc)
            {
                Debug.WriteLine(person);
            }


            /*-----------------------------------------------------------------------------------*/
            // Order by/Then by vs. multiple Order Bys:

            // This query expression also selects all persons from the passed sequence, but orders
            // them by the property Age in a descending manner and then by the property Name in an
            // ascending manner.
            var personsByAgeDescAndNameA =
                from person in persons
                orderby person.Age descending, person.Name
                select person;
            // The compiler will translate this query expression to following chain of method
            // calls:
            var personsByAgeDescAndNameA2 =
                persons
                    .OrderByDescending(person => person.Age)
                    .ThenBy(person => person.Name);
            foreach (var person in personsByAgeDescAndNameA)
            {
                Debug.WriteLine(person);
            }

            // If we have more then one criterion after which we want to order the sequence, we can use
            // either the OrderBy(...).ThenBy(...) pattern explained above, or we can use
            // multiple OrderBy(...)/orderby clauses.
            var personsByAgeDescAndNameB =
                from person in persons
                orderby person.Age descending
                orderby person.Name
                select person;

            // The compiler will translate this query expression to following chain of method
            // calls:
            var personsByAgeDescAndNameB2 =
                persons
                    .OrderByDescending(person => person.Age)
                    .OrderBy(person => person.Name);

            // However you should prefer the OrderBy(...).ThenBy(...) pattern over the usage of
            // multiple of OrderBy(...)/orderby clauses. OrderBy(...).ThenBy(...) can be executed
            // in an optimized way, because only the last ThenBy() call will initiate the buffering
            // of the OrderBy()'s input sequence, rather than sorting separately for each key as
            // explained above. But multiple OrderBy(...)/orderby clauses will order the input
            // sequence for multiple times (once for the Age property (descending) and once for the
            // Name property), incl. multiple buffering.

            // Sidebar: LINQ does not support a form of binary search. If you require this, you
            // should resort to IList<T>.
        }


        private static void GroupByQueryContinuationAndLookupExamples(IEnumerable<Person> persons)
        {
            /*-----------------------------------------------------------------------------------*/
            // Group by, Query Continuation and Lookups:

            // This query expression selects all persons from the passed sequence grouped by the
            // property Age. The group by clause produces an object of type IGrouping<int, Person>.
            // In effect all Persons in an IGrouping object are of the same Age. The type of the
            // property to be grouped for, the key, should implement the interface IEquateable<T>
            // in order to provide implementation of Equals() and GetHashCode(). There is an
            // implicit select clause that just selects each created IGrouping object into an
            // IEnumerable. - So the resulting sequence is of type
            // IEnumerable<IGrouping<int, Person>>; in this example the result types have been
            // written explicitly to make this somewhat clearer.
            IEnumerable<IGrouping<int, Person>> personsGroupedByAge = from person in persons
                                                                      group person by person.Age;
            // The outer loop iterates all the selected groups. The group by clause returns the 
            // groups in the order, in which their keys appear in the input sequence. The items in 
            // each group will be returned in the order they appear in the input sequence. 
            foreach (IGrouping<int, Person> ageGroup in personsGroupedByAge)
            {
                // For each group we'll print out the key (here the common Age) of that group.
                Debug.WriteLine("Persons of Age: " + ageGroup.Key);
                // The inner loop iterates all the items (or members) of a group.
                foreach (Person person in ageGroup)
                {
                    Debug.WriteLine("\t\t" + person);
                }
            }

            // Sidebar: The expression being used in the group clause (in front of by) must be a
            // non-void expression, i.e. it needs to produce a value as result of the projection!
            // - It means you can not just call a void method in a group clause.
            // So this is invalid:
            //var resultSequence =
            //    from person in persons
            //    group Console.WriteLine(person) by person.Age;
            // What projection should be created and to which type should the result be inferred as
            // WriteLine returns void?

            // In order to group by multiple data you have to use an anonymous type that plays the
            // role of a combined (i.e. multi-column) key. Mind that anonymous types provide an
            // implementation of Equals() and GetHashCode() (i.e. they implement IEquateable<T>),
            // which compares all of its property values like a value type.
            var personsGroupedByAgeAndState = from person in persons
                                              group person by new { person.Age, person.State };
            foreach (var ageAndStateGroup in personsGroupedByAgeAndState)
            {
                Debug.WriteLine("Grouped by: " + ageAndStateGroup.Key);
                foreach (var person in ageAndStateGroup)
                {
                    Debug.WriteLine("\t\t" + person);
                }
            }

            // The projection can also be controlled by specifying the data to group more
            // precisely. E.g. you can use an anonymous type like you'd do in select clauses:
            var personsGroupedByAgeAndState2 = from person in persons
                                               group new { person.Name, person.State }
                                               by new { person.Age, person.State };
            foreach (var ageAndStateGroup in personsGroupedByAgeAndState2)
            {
                Debug.WriteLine("Grouped by: " + ageAndStateGroup.Key);
                foreach (var item in ageAndStateGroup)
                {
                    Debug.WriteLine("\t\t" + item);
                }
            }


            /*-----------------------------------------------------------------------------------*/
            // Query Continuation:

            // Often it is required to post-process the results of a group by clause. This can be
            // done with the into keyword; into allows to collect the results of a group into an
            // intermediate result, which can be processed further. This is called
            // "query continuation".
            // 
            // The query continuation introduces one restriction: you can not access the range
            // variable being declared in the from clause just before the into clause in any
            // clauses after the into clause. This sounds cumbersome, but in this example this
            // means that you can not access person in the select clause. This restriction exists,
            // because the from clause is seen by the compiler as sub query, where person is out of
            // scope. (But the join into clause does _not_ introduce a query continuation, see
            // "JoinExamples()".)

            // This shows a very simple way of query continuation of a select clause:
            var allFriends =
                from person in persons
                select person.Friends into friends
                from friend in friends
                select friend;
            // Query continuations are much more useful when used with the group by clause.

            // Query Continuation: Order by group key
            var orderedGroups = from person in persons
                                group person by person.Age into ageGroup
                                orderby ageGroup.Key
                                select ageGroup;
            foreach (var ageGroup in orderedGroups)
            {
                Debug.WriteLine("Persons of Age: " + ageGroup.Key);
                foreach (var person in ageGroup)
                {
                    Debug.WriteLine("Person: " + person);
                }
            }

            // Query Continuation: Order by group key _without_ the into keyword:
            // The query written for the result orderedGroups can be expressed w/o the into keyword
            // to get query continuation. It looks like this (the query being continued is just
            // used as subquery). As explained above this syntax should make very clear, why the
            // range variable person can not be accessed in the select clause (read: after the 
            // into keyword).
            var orderedGroups2 = from ageGroup in
                                     from person in persons
                                     group person by person.Age
                                 orderby ageGroup.Key
                                 select ageGroup;
            foreach (var ageGroup in orderedGroups2)
            {
                Debug.WriteLine("Persons of Age: " + ageGroup.Key);
                foreach (var person in ageGroup)
                {
                    Debug.WriteLine("Person: " + person);
                }
            }


            var orderedGroups2_2 = from ageGroup in
                                     from person in persons
                                     orderby person.Name
                                     group person by person.Age
                                 orderby ageGroup.Key
                                 select ageGroup;
            foreach (var ageGroup in orderedGroups2_2)
            {
                Debug.WriteLine("Persons of Age: " + ageGroup.Key);
                foreach (var person in ageGroup)
                {
                    Debug.WriteLine("Person: " + person);
                }
            }


            // Query Continuation: Groups of groups
            var groupsOfGroups = from person in persons
                               group person by person.Age into ageGroup2
                                 select new
                                 {
                                     Key = ageGroup2.Key,
                                     StateGroup = from person in ageGroup2
                                                  group person by person.State
                                 };
            foreach (var ageGroup in groupsOfGroups)
            {
                Debug.WriteLine("Persons of Age: " + ageGroup.Key);
                foreach (var stateGroup in ageGroup.StateGroup)
                {
                    Debug.WriteLine("\tPersons from State: " + stateGroup.Key);

                    Debug.WriteLine(stateGroup.Key);
                    foreach (var person in stateGroup)
                    {
                        Debug.WriteLine("\t\t" + person);
                    }
                }
            }

            // Query Continuation: Queries with Having (as known from SQL)
            // This query selects all groups of persons from the passed sequence grouped by the
            // property Age, having more than one Person in a group. The where clause is targeted
            // against the groups, not the persons each, so it acts like a having clause known
            // from SQL. Notice that you can use any other predicates in the where clause. Also
            // all the extension methods from IEnumerable<T> can be used with the selected group
            // (here Count() is used).
            var personsGroupedByAgeHaving = from person in persons
                                            group person by person.Age into ageGroup
                                            where ageGroup.Count() > 1 // Having clause.
                                            select ageGroup;
            foreach (var ageGroup in personsGroupedByAgeHaving)
            {
                Debug.WriteLine(
                    string.Format("Persons of Age: {0} ({1})", ageGroup.Key, ageGroup.Count()));
                foreach (var person in ageGroup)
                {
                    Debug.WriteLine(person);
                }
            }


            /*-----------------------------------------------------------------------------------*/
            // Lookups:

            // There exists a handy type to represent a flat interpretation of grouped results. The
            // method ToLookup() creates an object of type ILookup. ILookup is similar to the type
            // IDictionary, but it is designed to store multiple values for one key (i.e. like
            // groups) similar to a multimap in C++. So ILookup and its default implementation
            // Lookup make up a new set of Collection types in the .NET framework. As argument for
            // ToLookup() you have to pass the lookup keying (e.g. as lambda expression). ILookup
            // is so handy, because it provides an indexer to access the groups. Notice that
            // ToLookup() is a conversion method, which evaluates all items of the input sequence
            // (persons in this case) _immediately_, so that the indexer (i.e. the operator []) and
            // the method Contains() can be executed as fast as possible. - Its behavior proves the
            // rule that query operators not returning IEnumerable/<T> cause immediate execution.
            // If you need deferred execution on the enumeration of the values, you have to use the
            // query operator GroupBy().

            // Get a lookup over all ages of the persons (immediately).
            ILookup<int, Person> agePersonLookup = persons.ToLookup(person => person.Age);
            // Are there any Persons of Age 40? In opposite to the group by clause the order of the
            // items in the resulting lookup is not defined.
            if (agePersonLookup.Contains(40))
            {
                // Iterate the Persons of Age 40.
                foreach (var person in agePersonLookup[40])
                {
                    Debug.WriteLine(person);
                }
            }


            // In opposite to ToLookup() the query operator GroupBy() provides its values as
            // _deferred_ enumeration on iteration. As argument you have to pass the group keying
            // (e.g. as lambda expression). Because you have no indexer on the result type, you
            // have to use another query to filter out the group you want to iterate like this:

            // Get an enumeration of groups over all ages of the persons (deferred).
            IEnumerable<IGrouping<int, Person>> agePersonGroups = persons.GroupBy(person =>
                person.Age);

            // Iterate the Persons of Age 40. But this time the order of the returned items in the
            // group "40" is strictly defined (see above).
            foreach (var person in from ageGroup in agePersonGroups
                                   from person in ageGroup
                                   where 40 == ageGroup.Key
                                   select person)
            {
                Debug.WriteLine(person);
            }
        }


        private static void AggregateLetClauseAndTransparentIdentifierExamples(
            IEnumerable<Person> persons)
        {
            /*-----------------------------------------------------------------------------------*/
            // Aggregate Functions, Let Clauses and Transparent Identifiers:

            // Notice that the execution of these aggregate queries is always immediate (query
            // operators returning non-IEnumerable/<T> cause immediate execution). So for
            // LINQ underneath it is needed to access (or enumerate) all items somehow to evaluate
            // the queries. The standard query operator Aggregate() can be used to express any of
            // the presented aggregate query operators in this method, incl. others (like Sum()).
            // The query operator Aggregate() does somewhat play the "scalar" counterpart of the
            // query operator SelectMany(), which can be used to express any other non-scalar query
            // operators. In functional programming aggregate operations are often called reduce
			// operations.

            // This query selects all persons of the passed sequence being younger than 40 and
            // returns the count. As can be seen this must be done by calling the query operator
            // Count() on the inner query expression (whose result sequence is of type
            // IEnumerable<Person>).
            int nYounger40 = (from person in persons
                              where person.Age < 40
                              select person).Count();
            // The same query w/o query expression, just calling the query operators Where() and
            // Count():
            int nYounger40a = persons.Where(p => p.Age < 40).Count();

            // You get the oldest Person by calling the query operator Max() on the returned
            // IEnumerable<Person>. - This is only possible if the type Person implements the
            // interface IComparable or IComparable<Person>.
            Person oldestPerson = (from person in persons
                                   select person).Max();
            // The same query w/o query expression, just calling the query operator Max():
            Person oldestPersona = persons.Max();

            // If, however, a type doesn't implement IComparable, a function can be passed to
            // Min() that contains the selector code, which selects the property, of which we
            // search the minimal value (the type of this property must implement IComparable as
            // well). Here this property is the Age of the persons.
            int minAge = (from person in persons
                          select person).Min(p => p.Age);
            // The same query w/o query expression:
            int minAgea = persons.Min(p => p.Age);

            // If the implementation of IComparable does not compare the appropriate property you
            // want to compare, but you'd like to get the object with a min/max property rather
            // than only the property's value, you can resort to the query operator Aggregate():
            Person personWithMinName =
                persons.Aggregate((lhs, rhs) => 0 > lhs.Name.CompareTo(rhs.Name) ? lhs : rhs);

			// Let's sum up all the ages of the persons:
			int ageSum = persons.Aggregate(0, (sum, next) => sum + next.Age);


            /*-----------------------------------------------------------------------------------*/
            // Let Clauses and Transparent Identifiers:

            // Let clauses allow defining intermediate values that can be used in different places
            // in the query expression. Notice that the values used in let clauses are no variables
            // in the C# sense; they can neither be modified after they have been initialized, nor
            // can they be explicitly typed on "declaration". The value in the let clause will be
            // filled on _each_ iteration of the query (i.e. in the query for
            // personsOlderThanYougest, the expression "persons.Min(p => p.Age)" for the
            // initialization of youngestAge must be executed for _each person_ in the input
            // sequence)! The C# language calls the symbols defined with let clauses as "readonly
            // range variables". - This indicates that they are being filled on each iteration of
            // the query as the range variables in the from clauses are. - A let clause introduces
            // a new range variable.
            // In functional programming (fp) the term "value" is used for such items being
            // non-modifiable after initialization; the term variable would be quite inadequate!
            // In most fp languages values are the common case when you define symbols. To define
            // "real" variables, which allow assignment and all other sorts of side effects being
            // performed on them, more syntactical fluff is required typically (i.e. as in F#).
            var personsOlderThanYougestNotGood =
                from person in persons
                // Bad! youngestAge will be calculated for each person, but that value will always
                // be the same!
                let youngestAge = persons.Min(p => p.Age)
                where person.Age > youngestAge
                select string.Format("'{0}' is younger than {1}.", person, youngestAge);

            // Such a query is Invalid:    
            //var someResult =
            //    from person in persons
            //    let divisor = 0
            // Invalid! Can't write the range variable divisor!
            //    select person.Age / ++divisor; 

            // So far so good. As can be seen within the select clause we use both range variables
            // person and youngestAge. But the query operator Select() does only accept one
            // argument in its override used for query expressions, so how does the lambda used
            // in the select clause cope with two (or more) range variable in the query? The 
            // answer are "transparent identifiers" being created underneath by the compiler during
            // the translation of the query expression to a chain of calls to query operators. The
            // C# compiler will introduce anonymous types to get things done; the example above
            // will be translated to something like this in order to satisfy the underlying query
            // operators:
            var personsOlderThanYougestNotGood2 =
                from p in persons
                // Introduces the transparent identifier t:
                select new { person = p, youngestAge = persons.Min(innerP => innerP.Age) } into t
                where t.person.Age > t.youngestAge
                select string.Format("'{0}' is younger than {1}.", t.person, t.youngestAge);

            // The let clauses make only sense, when applied on the current item of the input
            // sequence. Then this intermediate value can be processed comfortably in subsequent
            // clauses of the LINQ expression, like it is done in this query:
            var formatedPersonsReport =
               from person in persons
               where person.Age > 40
               // Good! Format only the information for the currently queried person.
               let formatedInformation = string.Format("'{0}' is in the age of {1}.",
                                                       person.Name, person.Age)
               // Good! Process the intermediate value. -> If formatedInformation is too long,
               // exclude it from the report!
               where formatedInformation.Length < 50
               select formatedInformation;

            // When there exist intermediate results that are independent from the currently
            // queried item, you're better off introducing a local variable (alas you can not
            // define non-modifiable locals for this in C#). - Then you can exploit the feature of
            // capturing the local in your LINQ expression.
            var youngestAgea = persons.Min(p => p.Age);
            var personsOlderThanYougestEvenBetter =
                from person in persons
                where person.Age > youngestAgea
                select string.Format("'{0}' is younger than {1}.", person, youngestAgea);

            // The same query w/o query expression:
            var personsOlderThanYougestDegeneratedSelect =
                persons.Where(p => p.Age > youngestAgea)
                        .Select(p => string.Format("'{0}' is younger than {1}.", p, youngestAgea));

            // To get the average Age of the passed Person sequence, the query operator Average()
            // must be called. To the Average()-call the function containing the selector code,
            // which selects the property, of which we'd like to know the average value of, must be
            // passed. Notice that the result of Average() is _always_ of type double.
            double averageAge = (from person in persons
                                 select person).Average(person => person.Age);
            // The same query w/o query expression, just calling the query operator Average():
            double averageAgea = persons.Average(p => p.Age);

            // Sidebar: For any thinkable aggegate operation you can also use the Aggregate() query
            // operator. VB has an extra query keyword for Aggregate().

        }


        #region Methods for DistinctExample():
        private sealed class PersonByAgeEqualityComparer : IEqualityComparer<Person>
        {
            public bool Equals(Person x, Person y)
            {
                return x.Age == y.Age;
            }

            public int GetHashCode(Person obj)
            {
                return obj.Age.GetHashCode();
            }
        }
        #endregion


        private static void DistinctExample(IEnumerable<Person> persons)
        {
            /*-----------------------------------------------------------------------------------*/
            // Distinct:

            // This query selects all distinct persons of the passed sequence. As can be seen this
            // must be done by calling the query operator Distinct() on the "inner" query expression
            // (whose result sequence is of type IEnumerable<Person>). The type of the distinct
            // property must provide a suitable implementation of the methods Equals() and
            // GetHashCode() (i.e. overriding Equals() alone will not help), because the
            // implementation of Distinct() will use the EqualityComparer<Type>.Default to get the
            // distinct values. In this case the type Person does itself provide a suitable
            // implementation of Equals() and GetHashCode().
            var distinctPersons = (from person in persons
                                   select person).Distinct();
            foreach (var person in distinctPersons)
            {
                Debug.WriteLine(person);
            }

            // If Person would not implement Equals() and GetHashCode() you could specifiy a
            // property of Person to be distinct (as the Age), but then you only get a result
            // sequence of the distinct values of that property (i.e. a distinct sequence of ints,
            // not a sequence of Persons with distinct ages). 
            // This query selects all distinct Ages of the passed Person sequence.
            var distinctAges = (from person in persons
                                select person.Age).Distinct();
            foreach (var age in distinctAges)
            {
                Debug.WriteLine(age);
            }

            // Sidebar: EqualityComparer<Type>.Default tries to access the interface
            // IEqualityComparer<Type> of Type. If Type implements that interface, it will be used
            // to call GetHashCode() and Equals(Type) (of IEqualityComparer<Type>). If Type does
            // not implement IEqualityComparer<Type>, a default implementation of
            // IEqualityComparer<Type> will be used to call GetHashCode() and Equals(object)
            // (inherited from object).

            // If you need all Persons with a distinct age (yes, this is a contrived example) or
            // more generally speaking, the type doesn't provide the semantics of equality you
            // need for distinction, then you can pass a suitable implementation of
            // IEqualityComparer<T> as argument to Distinct().
            // This query selects all Persons having a distinct Age of the passed Person sequence.
            var distinctPersonsByAge = (from person in persons
                                        select person).Distinct(new PersonByAgeEqualityComparer());

            // Sidebar: The order of the items in the resulting sequence is undefined. And it is
            // also undefined, which item of a couple of equal items will be put into the result
            // sequence.

            // Sidebar: VB has an extra query keyword for Distinct().
        }


        private static void JoinExamples(IEnumerable<Person> persons,
            IEnumerable<State> statesOfUs, IEnumerable<Company> companies)
        {
            /*-----------------------------------------------------------------------------------*/
            // Inner Joins:

            // This query joins the passed sequence of persons with the passed sequence of 
            // statesOfUS on the properties State and USPS as "inner join". The inner join selects
            // only items of both sequences, for which matching keys are present in both sequences.
            // The matching state is selected with the matching person each, then the whole person
            // and the name of the state is put into the projection.
            // Notice that the order of the expressions written in the on clause is crucial,
            // because they have to map to the parameters of the called query operator Join()
            // (SQL's join has a more forgiving nature on the order of arguments). - To cut the
            // story short the "left" and "right" part of LINQ's from-in/join-in must match the
            // on/equals (see below).
            // It is remarkable that the keyword "equals" must be used to join the sequences (not
            // the operator == or the method Equals()). The type of the joined key properties has
            // to implement the methods Equals() and GetHashCode() in a suitable manner (i.e the 
            // EqualityComparer mechanism - the compared properties Person.State and State.USPS in
            // this example are of type string and string fulfills these requirements). LINQ's
            // join query expression keyword resembles the SQL-92 standard.
            // The query operator Join() will eagerly read the right sequence completely and
            // buffer it to avoid repeated iteration on that sequence. - But it still executes
            // deferred, as this buffering of the second sequence will happen when the result
            // sequence is started to be read. The query operators Except() and Intersect() follow
            // the same pattern. (In opposite to that the query operators Zip(), Concat() and
            // SelectMany() are completely streamed.)

            var personsToState =
                from person in persons     // left
                join state in statesOfUs   // right
                    on person.State        // left
                       equals state.USPS   // right
                select new { Person = person, State = state.Name };
            foreach (var item in personsToState)
            {
                Debug.WriteLine(item);
            }
            // Depending on the query expression after the join clause a transparent identifier
            // will be introduced by the compiler to satisfy the call to the underlying query
            // operator (see AggregateLetClauseAndTransparentIdentifierExamples()).


            // The same query can be expressed with the query operator SelectMany(), which is
            // expressed as two from clauses and a single where clause connecting the sequences.
            // The projection stays the same. This syntax of joins resembles the SQL-82 standard.
            // In opposite to the join query expression the order of the arguments of the
            // == operator needs not to match their order in the from clauses each. Also you are
            // not restricted to equi joins with SelectMany() and Where(), other operators than
            // == can be used.

            var personsToState2 =
                from person in persons              // left
                from state in statesOfUs            // right
                where person.State == state.USPS    // left/right order doesn't matter on where
                select new { Person = person, State = state.Name };
            foreach (var item in personsToState2)
            {
                Debug.WriteLine(item);
            }

            // Sidebar: When to use the join keyword and when to use a couple of from keywords with
            // the where keyword?
            // - In LINQ to Object queries, join is more efficient as it uses LINQ's Lookup type.
            // - In LINQ to SQL queries, multiple froms (i.e. SelectMany()) are more efficient.
            // - SelectMany() is more flexible as you can use it for outer- and non-equi-joins, and
            //   also the order of the being-compared expressions doesn't matter.
            // - But in the end: choose the syntax, which most readable to you!

            // In order to join by multiple data you have to use an anonymous type that plays the
            // role of a combined (i.e. multi-column) key. Mind that anonymous types provide an
            // implementation of Equals() and GetHashCode() (i.e. they implement IEquateable<T>),
            // which compares all of its property values like a value type. It looks like this
            // (contrived) example:

            var personsToState3 =
                from person in persons
                join state in statesOfUs
                    on new { State = person.State, Initial = person.Name[0] }
                    equals new { State = state.USPS, Initial = state.Name[0] }
                select new { Person = person, State = state.Name };


            /*-----------------------------------------------------------------------------------*/
            // Group Joins:

            // The next type of joins are so called "group joins". Group joins happen, when the
            // result of a join is collected into an intermediate result with the into keyword.
            // This technique is similar to groupings, but the join will also include states into
            // the result, which have no associated persons. I.e. it is a kind of _outer join_!
            // In other words: in opposite to inner joins and normal groupings in group joins the
            // left sequence has a one-to-one association to the result sequence even if some of
            // the elements in the left sequence don’t match any elements of the right sequence.
            // As known from the grouping examples above, it is possible to post-process the
            // results being collected into the intermediate result. But when the into keyword is
            // used with a join this is _no_ query continuation. Unlike a query continuation join
            // into will introduce a new range variable (personsInThatState in this example, 
            // containing all the matches in the join) and _all_ previously defined range variables
            // will still be _accessible_ (except the one in the join clause) (see
            // "GroupByQueryContinuationAndLookupExamples()").
            var personsInState = from state in statesOfUs
                                 join person in persons
	                                  on state.USPS equals person.State
	                                  into personsInThatState
                                 select new { State = state, PersonsInState = personsInThatState };
            foreach (var state in personsInState)
            {
                Debug.WriteLine("Persons in state: " + state.State.Name);
                foreach (var person in state.PersonsInState)
                {
                    Debug.WriteLine("\t" + person);
                }
            }

            // Compare the result above to the result of an ordinary grouping (with query
            // continuation, i.e. all the range variables being defined before personsInThatState
            // are not accessible). Such a grouping will result in an _inner join_ of the
            // contributed sequences; it means that only states having associated persons will be
            // present in the result.
            var personsInState2 = from person in persons
                                  group person by person.State
                                      into personsInThatState
                                      select new
                                      {
                                          State = personsInThatState.Key,
                                          PersonsInState = personsInThatState
                                      };
            foreach (var state in personsInState2)
            {
                Debug.WriteLine("Persons in state: " + state.State);
                foreach (var person in state.PersonsInState)
                {
                    Debug.WriteLine("\t" + person);
                }
            }

            // Sidebar: VB has an extra query keyword for group joins.


            /*-----------------------------------------------------------------------------------*/
            // Outer Joins:

            // This query expression performs an "outer join" between the passed sequence of
            // statesOfUS and the sequence of persons. The outer join selects _all_ states, it
            // doesn't matter whether there exists a person dwelling in a state or not (in opposite
            // to the inner join). The matching persons are put into the intermediate extra group
            // "personsInThatState", this a "group join" as known from above. Then the contents of
            // the group of persons is blown up to the size of the stateOfUS sequence via the query
            // operator DefaultIfEmpty(). - So all states will be selected along with their
            // persons, for states having no persons default(Person) will be selected, which has
            // the value null. The query operator DefaultIfEmpty() does either just return the
            // sequence on which it was called, or if that sequence is empty a new sequence only
            // containing one item of value default(Type).
            // Now both sequences can be outer joined by selecting all Persons from the blown up
            // group and putting the matching Persons and States into the projection. If you desire
            // to join from States to Persons, you have to exchange their placement in the query
            // expression (typically in SQL there exist "left" and "right" outer joins for this).
            var statesToPersons = from state in statesOfUs
                                  join person in persons
                                      on state.USPS equals person.State
                                      into personsInThatState
                                  from person in personsInThatState.DefaultIfEmpty()
                                  select new { Person = person, State = state.Name };
            foreach (var item in statesToPersons)
            {
                Debug.WriteLine(item);
            }


            /*-----------------------------------------------------------------------------------*/
            // Multiple Joins:

            // With multiple joins multiple sequences can be joined. Here the sequence persons is
            // joined for three times. In the result in each item each person will be put for three
            // times.
            var personPersonPerson = from person1 in persons
                                     join person2 in persons on person1 equals person2
                                     join person3 in persons on person2 equals person3
                                     select new { person1, person2, person3 };
            foreach (var item in personPersonPerson)
            {
                Debug.WriteLine(item);
            }

            // Normally you'll use multiple joins to join related data of multiple sequences like
            // this:
            var personToStateToCompany = from person in persons
                                         join state in statesOfUs
                                            on person.State equals state.USPS
                                         join company in companies
                                            on person.Company equals company.PublicNasdaq
                                         select new
                                         {
                                             PersonName = person.Name,
                                             StateName = state.Name,
                                             CompanyName = company.Name
                                         };
            foreach (var item in personToStateToCompany)
            {
                Debug.WriteLine(item);
            }


            /*-----------------------------------------------------------------------------------*/
            // Cross Joins and Flat-Outs:

            // With multiple from clauses (i.e. multiple applications of the SelectMany() query
            // operator) _without_ any kind of conditions relating or joining them, you'll get a
            // cartesian product or "cross join". Each from clause introduces a new range variable.
            // Cartesian products are the results when the right sequence is _independent_ of the
            // current value of the left sequence. In the result of this cross join each
            // combination of three persons will be selected - This makes a count of 
            // person1.Count() * person2.Count() * person3.Count() items in the result.
            // SelectMany() is a very mighty query operator as it can be used to express other
            // query operators (Where(), Select() etc.). In terms of functional programming this is
            // called the "bind"-operation. In opposite to Join() the query operator SelectMany()
            // is completely streamed.)
            var person1XPerson2XPerson3 =
                from person1 in persons
                from person2 in persons
                from person3 in persons
                select new { person1, person2, person3 };
            foreach (var personTriple in person1XPerson2XPerson3)
            {
                Debug.WriteLine(personTriple);
            }

            // Sidebar: Transparent identifiers will also be introduced to cope with the range
            // variables each from clause produces (see
            // AggregateLetClauseAndTransparentIdentifierExamples()) - so the range variables can
            // be used in subsequent clauses. Again the C# compiler will introduce anonymous types
            // to get things done; the example above will be translated to something like this:
            var person1XPerson2XPerson32 =
               persons.SelectMany(
                   person1 => persons,
                   (person1, person2) => new { person1, person2 })
                    .SelectMany(
                // Introduces the transparent identifier t:
                        t => persons,
                        (t, person3) => new { t.person1, t.person2, person3 });
            // You'll agree that it is fairly better to use a query expression for cross joins,
            // because in query expressions the application of transparent identifiers is really
            // transparent...


            // Multiple from clauses can be used to select all the sub-items of all items in a
            // sequence. E.g. we can select all the friends of all the persons contained in
            // sequence persons, i.e. we are about to "flat out" the set of friends from the
            // persons. Flat-outs are the results when the right sequence is _dependent_ on the
            // current value of the left sequence. To avoid duplicate friends it makes sense to add
            // a call to the Distinct() query operator.
            var allFriends = from person in persons
                             where null != person.Friends
                             from friend in person.Friends
                             select friend;
            foreach (var friend in allFriends.Distinct())
            {
                Debug.WriteLine(friend);
            }
        }


		private static void SetOperations()
		{
			/*-----------------------------------------------------------------------------------*/
			// Set operations with LINQ:

			// Subsets:
			{
				IList<int> A = new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				IList<int> B = new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				IList<int> C = new List<int>{ 3, 6, 8 };

				// Subset:
				bool BIsSubsetOfA = !B.Except(A).Any();
				bool CIsSubsetOfA = !C.Except(A).Any();

				// Proper Subset:
				bool BIsProperSubSetOfA = A.Except(B).Any();
				bool CIsProperSubSetOfA = A.Except(C).Any();
				// Sidebar: A proper subset means, that a set is a subset of another set, but both sets
				// are _not_ equal!
			}

			// Union:
			{			
				// LINQ's Union() extension method will create a  _new_ sequence:
				IList<int> A = new List<int>{ 1, 2, 3, 4, 5, 6 };
				IList<int> B = new List<int>{ 4, 5, 6, 7, 8, 9 };
				IEnumerable<int> AUnionB = A.Union(B);
				// >{1, 2, 3, 4, 5, 6, 7, 8, 9}
			}

			// Difference:
			{
				// LINQ's Except() method will create a _new_ sequence:
				ISet<int> A = new SortedSet<int>{ 1, 2, 3, 4, 5, 6 };
				ISet<int> B = new SortedSet<int>{ 4, 5, 6, 7, 8, 9 };
				IEnumerable<int> AExceptB = A.Except(B);
				// >{1, 2, 3}
			}

			// Symmetric Difference:
			{
				// Using LINQ we can create a _new_ sequence:
				ISet<int> A = new SortedSet<int>{ 1, 2, 3, 4, 5, 6 };
				ISet<int> B = new SortedSet<int>{ 4, 5, 6, 7, 8, 9 };
				IEnumerable<int> ASymmetricExceptB = A.Except(B).Union(B.Except(A));
				// >{1, 2, 3, 7, 8, 9}
			}
		}


        private static void SubqueriesAndNestedQueriesExample(IEnumerable<Person> persons)
        {
            /*-----------------------------------------------------------------------------------*/
            // Subqueries:

            // This variant of a subquery applies a subquery as an input sequence for another
            // query. Such queries can be used to express query continuation w/o the into keyword
            // as well. (This query can be expressed within only one direct query expression as
            // well.)
            var queriedPersons = from person in
                                     (from person in persons
                                      where person.Name.StartsWith("P")
                                      select person)
                                 where person.Age != 40
                                 select person;
            foreach (var person in queriedPersons)
            {
                Debug.WriteLine(person);
            }

            // Now, let's simulate an SQL "in"-expression with a subquery with a LINQ query
            // expression with a subquery.
            queriedPersons = from person in persons
                             where (from name in new[] { "Angela", "Tory" }
                                    select name).Contains(person.Name)
                             select person;
            foreach (var person in queriedPersons)
            {
                Debug.WriteLine(person);
            }


            /*-----------------------------------------------------------------------------------*/
            // Nested Queries:

            var csvContents = "Holly,42,UT" + Environment.NewLine +
                                "Celine,39,ID" + Environment.NewLine +
                                "Leela,28,MA";

            // Compared to the most query expressions in this code file, this query expression
            // resembles the structure of the result it will generate. You'll see this pattern very
            // often in LINQ to XML, where it is called "Functional Construction".
            var newPersons = from line in csvContents.ToLines()
                             let personData = line.Split(',')
                             select new Person
                             {
                                 Name = personData[0],
                                 Age = Convert.ToInt32(personData[1]),
                                 State = personData[2],
                                 // Construct the friends with a nested query expression. - All
                                 // persons dwelling in the same state are friends...
                                 Friends = from person in persons
                                           where string.Equals(person.State, personData[2])
                                           select person
                             };
            foreach (var person in newPersons)
            {
                Debug.WriteLine(person);
                Debug.WriteLine("Friends of " + person.Name);
                foreach (var friend in person.Friends)
                {
                    Debug.WriteLine("\t\t" + friend);
                }
            }
        }


        private static void SequentializationExample()
        {
            /*-----------------------------------------------------------------------------------*/
            // Sequentialization:

            // In LINQ parlance objects implementing IEnumerable<T> are called sequences,
            // objects only implementing the weakly typed interface IEnumerable are no
            // sequences. - So how can weakly typed Collections be used with LINQ?

            // ArrayList is a classic old style weakly typed (non-generic) Collection. This
            // collection implements the non-generic interface IEnumerable, which does only support
            // a small set of query operators as extension methods.
            ArrayList nonGeneric = new ArrayList { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // One of IEnumerable's extension methods is the method Cast(). When this method is
            // called with an appropriate type argument (int in this case) a new sequence
            // (IEnumerable<int>) is returned (in a deferred manner). On this sequence all the
            // valuable query operators can be called as extension methods.
            IEnumerable<int> generic1 = nonGeneric.Cast<int>();

            // It is also possible to use a simple selection query expression with an explicitly
            // typed range variable (int i in this case). Underneath the compiler will generate
            // code, which calls Cast() as well.
            IEnumerable<int> generic2 = from int i in nonGeneric
                                        select i;
        }


        private static void MaterializationAndConversionExamples(Person[] persons)
        {
            /*-----------------------------------------------------------------------------------*/
            // Materialization:

            // Materialization means that the resulting collection is independent from the input
            // sequence (a shallow copy). Even if the input sequence is backed by an array, List,
            // Dictionary or Lookup, always a new collection will be returned (mind the
            // "To"-prefix). This also means that modifications (adding/removing elements) to the
            // input sequence will not reflect in the result afterwards, the resulting collection
            // is a snapshot! 

            // Take the first two Persons, but the generation of the result is deferred.
            IEnumerable<Person> twoPersons = persons.Take(2);
            // Will iterate the first two persons:
            foreach (var person in twoPersons)
            {
                Debug.WriteLine(person);
            }
            // Now we'll modify the input sequence persons (replace the first person):
            Person firstPerson = persons[0];
            Person harry = new Person { Name = "Harry", Age = 42 };
            persons[0] = harry;
            // Will iterate _another_ two persons, because twoPersons depends from the input
            // sequence that we've just modified:
            foreach (var person in twoPersons)
            {
                Debug.WriteLine(person);
            }

            // Reset to the original sequence:
            persons[0] = firstPerson;
            // Generate the result of twoPersons immediately and put the result into a Person[].
            // In LINQ to Objects ToArray() is optimized for ICollection<T>:
            // ICollection<T>.CopyTo() is called internally.
            Person[] arrayOfPersons = twoPersons.ToArray();
            // Will iterate the first two persons:
            foreach (var person in arrayOfPersons)
            {
                Debug.WriteLine(person);
            }
            // Now we'll again modify the input sequence (replace the first person):
            persons[0] = harry;
            // Will still iterate the first two persons w/o harry, because arrayOfPersons is
            // independent from the original input sequence:
            foreach (var person in arrayOfPersons)
            {
                Debug.WriteLine(person);
            }
            // Reset to the original sequence:
            persons[0] = firstPerson;


            // Generate the result of twoPersons immediately and put the result into a
            // List<Person>.
            List<Person> listOfPersons = twoPersons.ToList();

            // Generate the result of twoPersons immediately and put the result into a
            // Dictionary<string, Person> with a key mapped to the property Name of the Persons.
            Dictionary<string, Person> nameToPerson = twoPersons.ToDictionary(p => p.Name);

            // Sidebar: If the input sequence of ToDictionary() yields dublicate keys an 
            // ArgumentException will be thrown, because "An item with the same key has already
            // been added."

            // ToLookup() generates a lookup over all ages of the persons immediately. ILookup is
            // similar to the type IDictionary, but:
            // - ILookup it is designed to store multiple values for one key (i.e. like groups, see
            //   GroupByQueryContinuationAndLookupExamples()) similar to a multimap in C++.
			// - ToLookup() is more stable than ToDictionary(), because it can handle multiple values
			//   having the same key.
            // - In opposite to IDictionary the interface ILookup is designed to be readonly.
            // - The result of getting an unknown key of ILookup is an empty sequence, but
            //   IDictionary will throw a KeyNotFoundException.

            // Let's use it in an example: Because there can be multiple persons with the same age
            // a lookup must be used instead of a dictionary:
            ILookup<int, Person> ageToPerson = persons.ToLookup(person => person.Age);
			// ToLookup() returns and interface of type ILookup<> whereas ToDictionary() returns an
			// instance of the concrete type Dictionary<>. The only .Net implementation of type 
			// ILookup is internal.


			/*-----------------------------------------------------------------------------------*/
			// Warning: Don't materialize only to call ForEach():

			// Here, I'm going to talk about a very commonly found piece of code, which is really bad
			// and against the ideas of LINQ.

			var moreNumbers = Enumerable.Range(1, 100);

			// Very bad code(!):
			moreNumbers.Select(item => item.ToString())
					   .ToList()
					   .ForEach(item => Console.WriteLine(item));
			// This piece of code is a terrible waste of memory and some waste of run time performance!
			// Materialization via ToList() and then the ForEach() call both do an inner loop. ToList()
			// will execute and materialize the sequence created with Range().Select() and ForEach() will
			// iterate the result! Because ForEach() is also feed with an anonymous function (a lambda
			// expression in this case), for this anonymous function argument another object must be
			// created. - With a foreach statement only one loop for executing the sequence 
			// and iterating the sequence is required and no anonymous function object is required. The
			// simple fact is, that materialization and anonymous functions are completely superfluous
			// in this case. The foreach statement _is designed for iterating sequences w/o materialization_!

			// This code is much better and much more clever:
			foreach (var item in moreNumbers.Select(item => item.ToString()))
			{
				Console.WriteLine(item);
			}

			// - Keep in mind, that iteration in a ForEach()-call can neither be broken nor continued!
			//   Flow control with break and continue is only possible with the foreach statement!
            // - However, ForEach() also provides fail-fast iteration.
			// - ForEach() was not designed with LINQ in mind! You can notice that from its definition
			//   in List<T> and not as extension method of IEnumerable<T> and from the fact that was
			//   introduced in .NET 2.0, predating LINQ.

			// Basically the only reasonable, still superfluous, use of ForEach() is calling it on an
			// already existing List (not materilized in place) together with a method group:
			List<Person> personList = new List<Person>{ persons[0], harry };
			personList.ForEach(Console.WriteLine); // Mind, that no materialization was required!

			// Guideline:
			// The way to go is keeping results of LINQ operations in sequence as long as possible and
			// then only iterate those results via foreach statements.
            
            // Sidebar: an ordinary for-loop, i.e. a counting loop is generally more efficient than foreach
            // as well as ForEach() on indexed collections, because it doesn't need to make any virtual
            // method calls on contributing IEnumerator<T> objects and it also performs no fail-fast
            // checks.


            /*-----------------------------------------------------------------------------------*/
            // Cast() and OfType():

            ArrayList nonGenericCollection = new ArrayList(persons);
            // If you have a non-generic collection you can convert it to a sequence (instance of
            // IEnumerable<Person> in this case).
            IEnumerable<Person> sequenceOfPersons = nonGenericCollection.Cast<Person>();

            // If the input sequence is already of type IEnumerable<Person> the query operator
            // Cast() (see: AsEnumerable() below) will just return the input sequence immediately
            // (i.e. non-deferred):
            IEnumerable<Person> sequenceOfPersons2 = persons.Cast<Person>();
            bool sequenceReferencesAreEqual = persons.Equals(sequenceOfPersons2);

            // Besides Cast() there also exists the query operator OfType(), which is a kind of
            // filter operator (like Where()). OfType will only return items of the input sequence
            // that are of the specified type and not null. This means that OfType() will always
            // return a new sequence (in a deferred manner), because of the filtering ability.
            // This query will only return the Person-references not being null:
            IEnumerable<Person> sequenceOfPersons3 =
                persons.Concat(new Person[] { null }).OfType<Person>();

            // Sidebar: Cast() and OfType() will only perform reference and unboxing conversions
            // (these are only conversions, for which the cast type can be answered with true, when
            // the operator "is" is used, i.e. _no_ user conversions).


            /*-----------------------------------------------------------------------------------*/
            // Wrapper Operators:

            // In opposite to the conversion operators there are wrapper operators like
            // AsEnumerable(), AsQueryable(), AsParallel() and AsSequential() (mind the
            // "As"-prefix) that wrap the input sequence.
            // - With AsEnumerable() and AsQueryable() you can switch between "in-process"
            //   execution (like LINQ to Objects) and "out-process" execution (like LINQ to SQL).
            //   Technically you could use the Cast() query operator instead of AsEnumerable() 
            //   (both just return the source as IEnumerable<T>), but if the sequences' type is
            //   anonymous, you'll not be able to pass a type to Cast<T>(), but AsEnumerable() can
            //   infer the type.
            // - With AsParallel() and AsSequential() (available since .Net 4 and key enabler for
            //   PLINQ) you can switch between parallel and sequential execution.
        }


        private static void QueryOperatorArgumentCheckingExamples()
        {
            /*-----------------------------------------------------------------------------------*/
            // Checking Arguments of Iterator Functions immediately:

            // All these calls won't throw an exception, because the code checking the arguments
            // will be executed in a deferred manner in MyWhere().
            var result = Enumerable.Range(1, 9).MyWhere(null);
            var result2 = ((IEnumerable<int>)null).MyWhere(i => i < 5);
            var result3 = ((IEnumerable<int>)null).MyWhere(null);

            // Would throw, because predicate is null.
            //foreach (var item in result)
            //{
            //    Debug.WriteLine(item);
            //}

            // Would throw, because input is null.
            //foreach (var item in result2)
            //{
            //    Debug.WriteLine(item);
            //}

            // Would throw, because input is null.
            //foreach (var item in result3)
            //{
            //    Debug.WriteLine(item);
            //}

            // The checking of the arguments of MyWhere2() is done immediately, _before_ any
            // deferred operation will be performed. - Therefore MyWhere2() was implemented in two
            // methods: the first one, the wrapper function, checks the arguments and calls the
            // second function if the arguments are ok. The second function, the implementation
            // function, applies the deferred execution (uses an iterator block). Whenever you use
            // an iterator block such a split implementation should be considered.
            // (Please see above.)
			try
			{
				var result4 = Enumerable.Range(1, 9).MyWhere2(null);
				var result5 = ((IEnumerable<int>)null).MyWhere2(i => i < 5);
				var result6 = ((IEnumerable<int>)null).MyWhere2(null);
			}
			catch(ArgumentNullException)
			{
				Console.WriteLine("ArgumentNullException was thrown.");
			}
        }


        private static XElement FunctionalConstructionWithLinqToXml(IEnumerable<Person> persons)
        {
            /*-----------------------------------------------------------------------------------*/
            // Functional Construction with LINQ to XML:

            // This example applies functional construction to build an XML from the passed
            // sequence persons. The types XElement and XAttribute (of namespace System.Xml.Linq)
            // along with LINQ's query operators make this task very easy! Notice, how the 
            // resulting XML resembles the code (notice the indentation) that creates it.

            // Result:
            //<Persons>
            //  <Person Name="Patricia" Age="38" State="ID" /> 
            //  <Person Name="Angela" Age="40" State="UT" /> 
            //  <Person Name="Caroline" Age="35" State="MA" /> 
            //  <Person Name="Dana" Age="40" State="NM" /> 
            //  <Person Name="Tory" Age="40" State="UT" /> 
            //  <Person Name="Bonnie" Age="37" State="UT" /> 
            //</Persons>

            // Code:
            return new XElement("Persons",
                        from person in persons
                        select new XElement("Person",
                                    new XAttribute("Name", person.Name),
                                    new XAttribute("Age", person.Age),
                                    new XAttribute("State", person.State)
                            )
                );
        }


        private static void DynamicQueriesExamples(IEnumerable<Person> persons)
        {
            /*-----------------------------------------------------------------------------------*/
            // Dynamic Queries:

            // This Method shows some examples how LINQ Expressions (Query Expressions or Chained
            // Extension Methods) can be influenced at run time, i.e. dynamically.

            // By now you've just seen rather static queries, whose results are only depending from
            // the input sequence. - The "recipe of LINQ steps" was rigid in all that examples.
            // Here let's analyze, how that recipe can be influenced dynamically.


            /*-----------------------------------------------------------------------------------*/
            // Captured Variables:

            // In fact the scenario that variables "from outside" contribute to the LINQ expression
            // (query expression or chained extension methods) is very common. As you may remember
            // such variables are captured by the query expression. Such captured variables may
            // play the role of user input and influence the query's result.
            int age = 30;
            var personsOlderThanX = from person in persons
                                    where person.Age > age
                                    select person;
            foreach (var personOlderThan30 in personsOlderThanX)
            {
                Debug.WriteLine(personOlderThan30);
            }

            // It is possible to rerun the query with another age being queried simply by modifying
            // the variable age, which is still captured by the query expression.
            age = 35;
            foreach (var personOlderThan35 in personsOlderThanX)
            {
                Debug.WriteLine(personOlderThan35);
            }

            // It is also possible to update the queried input sequence (persons in this case) and
            // to rerun the query to get a different result.
            // But it should be said that unwanted capture to modified variables and multiple
            // (apparently independent) query calls can be a nasty source of bugs!


            /*-----------------------------------------------------------------------------------*/
            // Enabling Custom Order in Queries:

            // Assume we want to order the persons by age. No problem!
            var personsByAge = from person in persons
                               orderby person.Age
                               select person;
            foreach (var person in personsByAge)
            {
                Debug.WriteLine(person);
            }
            // The assume that we want to order the person by age if there are more than three
            // persons, else we want to order them by name! - Oups! That's not that easy! Query 
            // expressions alone will not help here.

            // We can just select among query expressions depending on the count of persons with
            // an if statement:
            var orderedPersons = Enumerable.Empty<Person>();
            if (3 < persons.Count())
            {
                orderedPersons = from person in persons
                                 orderby person.Age
                                 select person;
            }
            else
            {
                orderedPersons = from person in persons
                                 orderby person.Name
                                 select person;
            }

            foreach (var person in orderedPersons)
            {
                Debug.WriteLine(person);
            }

            // Or we can get rid of the query expressions altogether and use extension methods
            // instead. - This time the extension method syntax is even more compact than the
            // query expression syntax, also the conditional operator was used.
            orderedPersons = (3 < persons.Count())
                                ? persons.OrderBy(person => person.Age)
                                : persons.OrderBy(person => person.Name);
            foreach (var person in orderedPersons)
            {
                Debug.WriteLine(person);
            }


            /*-----------------------------------------------------------------------------------*/
            // Building Queries Conditionally:

            // Now let's create a pretty strange query dynamically:
            // If the max age of all persons is 40, then select all persons with the age being 
            // unequal to 40. If the max age is not 40, then order the persons in descending manner
            // by the state.
            // If the resulting count of persons is equal to one, then order that resulting persons
            // by the name. If the resulting count of persons is equal to two, then order that
            // resulting persons by the name in descending manner. If the resulting count of
            // persons is neither equal to one nor two, then order the persons by the age.

            // To cut the story short: you can't express that query with a single query expression.
            // Instead you have to break the described conditions into separate conditional 
            // expressions or control flow statements.

            var queriedPersons = (40 == persons.Max(person => person.Age))
                                    ? from person in persons
                                      where 40 != person.Age
                                      select person
                                    : from person in persons
                                      orderby person.State descending
                                      select person;
            if (1 == queriedPersons.Count())
            {
                // Notice that the syntax "queriedPersons = from person..." does not
                // mean, that queriedPersons will be overwritten! It expresses that we are about
                // to chain the calls (as subqueries). E.g. queriedPersons could mean:
                // from person in (from person in persons where 40 != person.Age
                // select person)
                // orderby person.Name select person - or -
                // from person in (from person in persons orderby person.State descending
                // select person)
                // orderby person.Name select person
                // after this statement have been evaluated!
                queriedPersons = from person in queriedPersons orderby person.Name select person;
            }
            else if (2 == queriedPersons.Count())
            {
                queriedPersons = from person in queriedPersons
                                 orderby person.Name descending
                                 select person;
            }
            else
            {
                queriedPersons = from person in queriedPersons
                                 orderby person.Age
                                 select person;
            }

            foreach (var person in queriedPersons)
            {
                Debug.WriteLine(person);
            }

            // Here is the same query in principle, but expressed with extension methods (i.e.
            // the standard query operators). The extension methods' syntax is leaner here, because
            // degenerate selects can be used (you do not have to write a chained call to the
            // query operator Select() each time).
            queriedPersons = (40 == persons.Max(person => person.Age))
                                ? persons.Where(person => 40 != person.Age)
                                : persons.OrderByDescending(person => person.State);
            if (1 == queriedPersons.Count())
            {
                // Notice that the syntax "queriedPersons = queriedPersons.OrderBy(..." does not
                // mean, that queriedPersons will be overwritten! It expresses that we are about
                // to chain the calls. E.g. queriedPersons could mean:
                // persons.Where(person => 40 != person.Age)
                //        .OrderBy(person => person.Name) - or -
                // persons.OrderByDescending(person => person.State)
                //        .OrderBy(person => person.Name)
                // after this statement have been evaluated!
                queriedPersons = queriedPersons.OrderBy(person => person.Name);
            }
            else if (2 == queriedPersons.Count())
            {
                queriedPersons = queriedPersons.OrderByDescending(person => person.Name);
            }
            else
            {
                queriedPersons = queriedPersons.OrderBy(person => person.Age);
            }

            foreach (var person in queriedPersons)
            {
                Debug.WriteLine(person);
            }


            /*-----------------------------------------------------------------------------------*/
            // Building Queries Completely at Run Time - Using Code as Data:

            // Last but not least we can also build queries completely by Expression Trees. The 
            // topic of Expression Trees is somewhat complicated, but in principle one just needs
            // to combine sub expressions created from a set of factory methods provided by the
            // Expression Trees API (System.Linq.Expression).
            // This is the mightiest, most flexible but most complicated variant to build queries 
            // at run time.

            // So let's do it: If we have more than two persons in the passed sequence, query only
            // that persons of age 40. Else query all persons being older than 35. With an
            // Expression Tree this can be done like this:

            var aPerson = Expression.Parameter(typeof(Person), "person");
            var ageExpression = (2 < persons.Count())
                                    ? Expression.Equal(Expression.Property(aPerson, "Age"),
                                        Expression.Constant(40))
                                    : Expression.GreaterThan(Expression.Property(aPerson, "Age"),
                                        Expression.Constant(35));

            var predicate = Expression.Lambda(ageExpression, aPerson);
            queriedPersons = Enumerable.Where(persons, (Func<Person, bool>)predicate.Compile());
            foreach (var person in queriedPersons)
            {
                Debug.WriteLine(person);
            }

            // In a sense working with Expression Trees can be compared to working with DOM trees
            // in XML.
        }


        private static void DotNotationOrQueryExpression(IEnumerable<Person> persons)
        {
            /*-----------------------------------------------------------------------------------*/
            // Dot Notation or Query Expression:

            // Every query expression can be expressed as C#-code using calls to query operators as
            // extension methods. But the opposite is not true; only a small subset of the standard
            // query operators can be used as keywords in query expressions. In other words query
            // expressions have some limitations that the method-call mechanism does not have:

            // 1. Some query operators have simply no query expression equivalent, e.g. ToArray():
            Person[] arrayOfPersons = persons.ToArray();

            // 2. We can't use all kinds of overloads in query expressions. E.g. there is an
            // overload of Select() that awaits the index of the currently iterated object; you can
            // not call this overload within a query expression:
            IEnumerable<string> indexAndPerson =
                persons.Select((person, index) => string.Format("{0}: {1}", index, person));

            // 3. We can't use statement lambdas in query expressions. - This is the cause why
            // object and collection initializers have been introduced into the C# language.
            bool writeLog = true;
            var x = persons.Select(person =>
            {
                if (writeLog)
                {
                    Console.WriteLine(person);
                }
                return person;
            });
        }


        private static void DangerousOperatorsAndHowToCircumventThem(IEnumerable<Person> persons)
        {
            /*-----------------------------------------------------------------------------------*/
            // Handling of huge Sequences:

            // Operations being greedy on the passed sequence need to read the complete passed
            // sequence and can't interrupt their execution untimely. E.g. the query operator 
            // Where() in the query expression below must read the complete sequence being passed,
            // because it can not know, when there are no more items of a value less than ten are
            // being produced. (Yes, the reader knows that, because AllIntegers() will produce a
            // sequence of _increasing_ integers, i.e. after the nine was produced there will be no
            // more items less than ten.) Notice that we could've used Enumerable.Range(), but
            // AllIntegers() was deliberately implemented, so you can inspect its code.
            var smallNumbers = from i in Utilities.AllIntegers()
                               where i < 10
                               select i;
            //Don't iterate this! - It will run for a very long time period!
            //foreach (var smallNumber in smallNumbers)
            //{
            //    Debug.WriteLine(smallNumber);
            //}

            // To reduce the problem you need to apply paging on the huge sequence produced by
            // Utilities.AllIntegers(), e.g. via the application of the query operators Skip() and
            // Take():
			{
				const int pageSize = 10;

				int took = 0;
				bool getNextPage = true;
				var page = Utilities.AllIntegers();
				do 
				{
					Console.WriteLine("Page {0}:", (took / pageSize) + 1);

					// Fine: front load paging operations to get control over huge sequences:
					foreach (var smallNumber in page.Take(pageSize))
					{
						Console.WriteLine(smallNumber);
					}

					took += pageSize;
					Console.WriteLine("Next page (y/n)?");
					char answer = Console.ReadLine().FirstOrDefault();
					// Btw: the default(char) is the value '\0'.
					getNextPage = default(char) != answer && 'Y' == char.ToUpperInvariant(answer);
					if (getNextPage)
					{
						page = page.Skip (pageSize);
					}
				}
				while (getNextPage);
			}

			// However, if you are after performance, and in production code, we're all after
			// performance, you shouldn't use LINQ's paging as shown above, but rather the
			// underlying IEnumerator to implement paging directly. As a matter of fact, it is
			// even simpler than the LINQ-algorithm shown above, but more performant:
			{
				const int pageSize = 10;

				int took = 0;
				bool getNextPage = true;
				using (var page = Utilities.AllIntegers().GetEnumerator())
				{
					do 
					{
						Console.WriteLine("Page {0}:", (took / pageSize) + 1);

						int currentPageItemNo = 0;
						while (currentPageItemNo++ < pageSize && page.MoveNext())
						{
							int smallNumber = page.Current;
							Console.WriteLine(smallNumber);
						}

						took += pageSize;
						Console.WriteLine("Next page (y/n)?");
						char answer = Console.ReadLine().FirstOrDefault();
						getNextPage = default(char) != answer && 'Y' == char.ToUpperInvariant(answer);
					}
					while (getNextPage);
				}
			}
			// Explanation: The downside of using Skip() for multiple times in a "cascading manner"
			// is, that it will not really store the "pointer" of the iteration, where it was last
			// skipped. - Instead the original sequence will be front-loaded with skip calls, which
			// will lead to consuming the whole sequence over and over again. - You can prove that
			// yourselves, when you have a sequence that yields side effects. -> Even if you have
			// skipped the items 10-20 and 20-30 and want to process 40+, you'll see all side effects
			// of 10-30 be executed again, before you start iterating 40+. The variant using IEnumerable
			// directly, will instead remember the position of the end of the last logical page, so no
			// explicit skipping is needed and side effects won't be repeated.

            // Sidebar: In most real-world applications of paging, e.g. if we want to page data
            // on a UI, we need to ensure that the sources' data is ordered. This is esp. the case
            // if you query data from databases, where the order of the returned values is not
            // predictable per se.
            // In our scenario ordering would be wrong: we added paging to handle huge sequences,
            // not to spread the data over multiple UI-elements. Ordering would _buffer_ the 
            // _complete_ sequence, and all our work to page the sequence would work just
            // _too late_.

            // Sidebar: VB has extra query keywords for Take(), TakeWhile(), Skip() and
            // SkipWhile().


            /*-----------------------------------------------------------------------------------*/
            // The Query Operators Count() and LongCount():

            // When you'd like to check whether a sequence contains items you can check it like
            // this:
            bool hasPersons = 0 != persons.Count(); // Not good!
            // You shouldn't do that! For most sequences Count() will require to read the whole
            // sequence to count the items. Only for sequences that implement ICollection<T> and 
            // (in .Net 4) ICollection their property Count will be used w/o iterating the
            // sequence. (This optimization is not present for the query operator LongCount()) Mind
            // that this "optimization" assumes that ICollection/<T> is faster than an explicit
            // iteration, but there is no guarantee that ICollection/<T>.Count will execute with
            // O(1)! - In the programming language Lisp following fact is well known: if you need
            // to get the count of elements within a list in your algorithm, then you've already
            // lost the game. Also mind how the foreach loop is handy w/ sequences, as it doesn't
            // need to know the length of the to-be-iterated Collection/sequence.
            //
            // Sidebar: In this case the sequence persons is backed by an array and arrays are the
            // only collections w/ direct support in the CLR. A single dimensional array explicitly
            // implements the interface IList<T>, which inherits ICollection<T>, so the
            // Count-optimization would be activated. Rectangular arrays explicitly implement the
            // interface IList (the Count-optimization would also work). Arrays are only mutable on
            // their elements, i.e. you can not modify their fixed size, so all the modifying
            // methods of IList/<T> (Add(), Remove(), etc.) are implemented to throw
            // NotSupportedException.
            //
            // In this case the better alternative is the query operator Any():
            hasPersons = persons.Any(); // Better!
            // Any() will return as soon as it finds a (matching) item; it the worst case it has to
            // iterate the whole sequence, though. But it is almost always a better idea to use
            // Any() instead of Count() to check for the presence of any items.


            /*-----------------------------------------------------------------------------------*/
            // Getting the Item on a certain Index:

            // You can iterate persons like this:
            int nPersons = persons.Count(); // Yes, we've just learned that this a bad idea in the
            // section above...
            for (int i = 0; i < nPersons; ++i)
            {
                Person person = persons.ElementAt(i);
                Debug.WriteLine(person);
            }
            // This might be a bad idea, because it is possible that the items of the sequence
            // persons must be generated anew, when the values at the indexes are taken. I.e. for
            // the index "2" the first two items must be generated from the sequence, if the index
            // "3" is passed the first three items must be generated from the sequence and so
            // forth. - In short: a horrible assumption!
            // Well, in this case we know that persons is backed by an array. The type array
            // implements the interface IList<T> (see the sidebar above, persons implements
            // IList<Person> in this case), and for sequences implementing IList<T> the query
            // operators ElementAt() and ElementAtOrDefault() are optimized by calling the indexer
            // directly.
            // You'd better convert the sequence to an array explicitly like this:
            Person[] personArray = persons.ToArray(); // Better!
            for (int i = 0; i < personArray.Length; ++i)
            {
                Person person = personArray[i];
                Debug.WriteLine(person);
            }


            /*-----------------------------------------------------------------------------------*/
            // The Issue about Reverse():

            // The query operator Reverse() performs buffering! That means that you can not use it
            // on very large or even infinite sequences. As the last item of the sequence passed to
            // Reverse() will be the first one in the resulting sequence ask yourself; what is the
            // last item of an infinite sequence...
            //foreach (int item in Utilities.AllIntegers().Reverse()) // Don't iterate the result!
            //{                                                       // the first call to Next()
            //    Debug.WriteLine(item);                              // would lead to the 
            //}                                                       // immediate buffering of the
            // items returned from
            // Utilities.AllIntegers().


            /*-----------------------------------------------------------------------------------*/
            // Trouble with Contains():

            HashSet<string> set =
                new HashSet<string>(StringComparer.CurrentCultureIgnoreCase) { "A", "B", "C" };

            // This will call HashSet<string>.Contains(). This call will return true and that is
            // no surprise.
            bool isContained = set.Contains("a");

            IEnumerable<string> setAsSequence = set;
            // This will call Enumerable.Contains(). This call will return true but that _is_
            // a surprise! In fact because HashSet<string> implements ICollection<string> the
            // method ICollection<string>.Contains() will be called internally (it uses the
            // Comparer (StringComparer.CurrentCultureIgnoreCase in this case), which was used on
            // set's ctor call). No default EqualityComparer will be used. This behavior is very
            // misleading and will not be expected by most developers, but it can be circumvented.
            isContained = setAsSequence.Contains("a");

            // This way we can force to use EqualityComparer<string>.Default for the string
            // comparison:
            isContained = setAsSequence.Select(s => s).Contains("a");

            // Alternatively we can pass another Comparer to be used for Contains():
            isContained = setAsSequence.Contains("a", StringComparer.Ordinal);
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Our Test Data to play with:

            IEnumerable<State> statesOfUs = State.StatesOfUs;

            IEnumerable<Company> companies = Company.Companies;

            IEnumerable<Person> persons = Person.Persons;


            /*-----------------------------------------------------------------------------------*/
            // Calling the Test Methods:

            OrderByExamples(persons);


            GroupByQueryContinuationAndLookupExamples(persons);


            AggregateLetClauseAndTransparentIdentifierExamples(persons);


            DistinctExample(persons);


            JoinExamples(persons, statesOfUs, companies);


			SetOperations();


            SubqueriesAndNestedQueriesExample(persons);


            SequentializationExample();


            MaterializationAndConversionExamples(persons.ToArray());


            DangerousOperatorsAndHowToCircumventThem(persons);


            QueryOperatorArgumentCheckingExamples();


            XElement result = FunctionalConstructionWithLinqToXml(persons);


            DotNotationOrQueryExpression(persons);


            DynamicQueriesExamples(persons);
        }
    }
}