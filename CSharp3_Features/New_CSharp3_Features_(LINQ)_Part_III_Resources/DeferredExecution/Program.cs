using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


// These examples show:
// - An implemention of the logical-or-operator with deferred execution.
// - The evolution of an old style algorithm to an algorithm applying deferred execution.
// - How extension methods can choose optimized implementations on the run time type.
// - Reimplemented algorithm SubSequencesCore() applying deferred execution.
namespace DeferredExecution
{
    internal static class Utilities
    {
        #region Methods for EvolutionOfExtensionMethods():
        /// <summary>
        /// First variant of the method MakeUniqueAndPrintToConsole. Pass a List of int to this
        /// method and its unique values will be printed to the console.
        /// </summary>
        /// <param name="nums">Represents the List of int values.</param>
        internal static void MakeUniqueAndPrintToConsole(List<int> nums)
        {
            var uniqueValues = new Dictionary<int, int>();
            foreach (var num in nums)
            {
                if (!uniqueValues.ContainsKey(num))
                {
                    uniqueValues.Add(num, num);
                    Debug.WriteLine(num);
                }
            }
        }
        // The problem of MakeUniqueAndPrintToConsole() is that it's not reusable. It contains two
        // responsibilities: getting the unique values of the passed List and print them on the
        // console. This is a good example that the name of a method can indicate its reusability.
        // By the way this algorithm is cast in stone to work with ints only. It can be improved.


        /// <summary>
        /// Official documentation:
        /// Filters a sequence of int values based on uniqueness.
        /// 
        /// Inofficial details:
        /// Second variant of the method Unique. Pass an IEnumerable of int to this method and its
        /// unique values will be returned as IEnumerable of int. Note that the return value is a
        /// sequence, not a readily constructed collection.
        /// </summary>
        /// <param name="nums">Represents the IEnumerable of int values.</param>
        /// <returns>An IEnumerable of int that contains the unique elements from the input
        /// sequence.</returns>
        internal static IEnumerable<int> Unique2(IEnumerable<int> nums)
        {
            var uniqueValues = new Dictionary<int, int>();
            foreach (var num in nums)
            {
                if (!uniqueValues.ContainsKey(num))
                {
                    uniqueValues.Add(num, num);
                    yield return num;
                }
            }
        }
        // This method Unique2() is much better than the first variant. The responsibility to print
        // anything to the console has been factored away, so a better reusability is given. Also
        // do we use IEnumerables (sequences) as input and output parameters, so we are no longer
        // constrainted to use a List of int. And the execution of resulting items of the sequence
        // is deferred until the next item of the sequence was really requested, non-unique values
        // are being shortcut by the algorithm transparently. - Sometimes this is called
        // "Continuation Method". By the way this algorithm is still cast in stone to work with
        // ints only. So we can still do it better!


        /// <summary>
        /// Official documentation:
        /// Filters a sequence of T values based on uniqueness.
        /// 
        /// Inofficial details:
        /// Third variant of the method Unique. Pass an IEnumerable of T to this method and its
        /// unique values will be returned as IEnumerable of T. So we have completely removed the
        /// necessity to use only input sequences of type int! Additionally in the implementation
        /// the more efficient (more efficient as Dictionary in this case) type HashSet of T was
        /// used (new in .Net 3.5).
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">Represents the IEnumerable of T values.</param>
        /// <returns>An IEnumerable of T that contains the unique elements from the input sequence.
        /// </returns>
        internal static IEnumerable<T> Unique3<T>(IEnumerable<T> source)
        {
            var uniqueValues = new HashSet<T>();
            foreach (var item in source)
            {
                if (!uniqueValues.Contains(item))
                {
                    uniqueValues.Add(item);
                    yield return item;
                }
            }
        }
        // Hm, that looks fine. But we can still do it better to fit into C#3.


        /// <summary>
        /// Official documentation:
        /// Filters a sequence of T values based on uniqueness.
        /// 
        /// Inofficial details:
        /// Fourth variant of the method Unique. Its implementation is identical to the third
        /// variant, but Unique4() is an extension method.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">Represents the IEnumerable of T values.</param>
        /// <returns>An IEnumerable of T that contains the unique elements from the input
        /// sequence.</returns>
        internal static IEnumerable<T> Unique4<T>(this IEnumerable<T> source)
        {
            var uniqueValues = new HashSet<T>();
            foreach (var item in source)
            {
                if (!uniqueValues.Contains(item))
                {
                    uniqueValues.Add(item);
                    yield return item;
                }
            }
        }
        // This last variant suits very well to the other extension methods of type IEnumerable of
        // T! 
        #endregion


        #region Methods for ChooseRuntimeType():
        /// <summary>
        /// This version of MyReverse() analyzes the run time type of the passed sequence. If the
        /// run time type supports random access (i.e. implements IList(Of T)), then an optimized
        /// implementation w/o buffering is chosen.
        /// In fact Reverse() was not optimized this way in LINQ to Objects. The problem is that in
        /// this case Reverse() would take a snapshot of input sequence under certain circumstances
        /// (which are transparent to the caller). And if the input sequence will be modified
        /// during iteration wrong assumptions are taken as of the snapshot. On the other hand it
        /// is a hard job to guard an iterator against modification.
        /// There seem to be only following run time type optimizations present in LINQ to objects
        /// (state in October 2011)
        /// - ElementAt() and ElementAtOrDefault() are optimized if the passed sequence
        ///   implements IList(Of T).
        /// - Count() (but not LongCount()) is optimized for sequences implementing
        ///   ICollection/(Of T).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static IEnumerable<T> MyReverse<T>(this IEnumerable<T> sequence)
        {
            var list = sequence as IList<T>;
            if (null == list) // If the sequence is not an IList<T>, we have to buffer (copy) its
                              // content in order to yield the iterator upside down.
            {
                list = new List<T>(sequence);
                for (int i = list.Count - 1; i >= 0; --i)
                {
                    yield return list[i];
                }
            }
            else // If the sequence is an IList<T>, we can access its items randomly via the
                 // indexer and yield the iterator upside down!
            {
                for (int i = list.Count - 1; i >= 0; --i)
                {
                    yield return list[i];
                }
            }
        }
        #endregion


        #region A more sophisticated Example (taken from Part III of the C#2 Review):
        // This is the core of the implementation to get the subsets of a given set. Mind, that this
        // example will _not_ retrieve the whole powerset, but an iterator to get each subset
        // respectively. The implementation is recursive, so iterators can also be used
        // recursively!
        private static IEnumerable<IList<T>> SubsetsCore<T>(IList<T> remainderList,
            IList<T> inputList)
        {
            if (0 == remainderList.Count)
            {
                yield return inputList;
            }
            else
            {
                var newInputList = new List<T>(inputList) { remainderList[0] };
                var newRemainderList = new List<T>(remainderList);
                newRemainderList.RemoveAt(0);

                // Recursion:
                foreach (var subSet in SubsetsCore(newRemainderList, newInputList))
                {
                    yield return subSet;
                }

                // Recursion:
                foreach (var subSet in SubsetsCore(newRemainderList, inputList))
                {
                    yield return subSet;
                }
            }
        }


        // Get an IEnumerable with the subsets of the passed set.
        public static IEnumerable<IList<T>> SubsetsOf<T>(IList<T> set)
        {
            // Just feeds the core implementation function and forwards the returned iterator to
            // the return value of _this_ function
            return SubsetsCore(set, new List<T>());
        }
        #endregion


        #region A more sophisticated Example (pure functional and deferred Implementation):
        // A Lisp implementation of the subset's algorithm (this implementation gets the full
        // (defun powerset (l)
        //  (if (null l)
        //   '(nil)
        //   (let ((ps (powerset (cdr l))))
        //    (append ps (mapcar #'(lambda (x) (cons (car l) x)) ps)))))
        //
        // This is the core of the implementation to get the subsequences of a given sequence.
        // The effect of this implementation is equivalent to SubsetsCore, but it accepts
        // IEnumerables of T as sequence parameters. The most differences can be found in its
        // implementation itself: 
        // - It is pure functional, w/o side effects, w/o having the need to materialize any
        //   intermediate results into lists.
        // - The whole algorithm was expressed within a single expression!
        private static IEnumerable<IEnumerable<T>> SubSequencesCore<T>(
            IEnumerable<T> remainderSequence, IEnumerable<T> inputSequence)
        {
            return remainderSequence.Any()
                ? SubSequencesCore(  // Recurse
                    remainderSequence
                        .Skip(1), // Skip(1) is the equivalent to lisp's cdr/rest.
                    inputSequence
                        .Concat( // Concat() is the equivalent to lisp's append.
                            remainderSequence
                                .Take(1))) // Take(1) is the equivalent to lisp's car/first.
                      .Concat(
                          SubSequencesCore( // Recurse
                              remainderSequence
                                  .Skip(1),
                              inputSequence))
                // End of recursion: make a sequence of one item.
                : Enumerable.Repeat(inputSequence, 1); 
        }


        // Get an IEnumerable with the subsequences of the passed sequence.
        public static IEnumerable<IEnumerable<T>> SubSequencesOf<T>(IEnumerable<T> inputSequence)
        {
            // Just feeds the core implementation function and forwards the returned iterator to
            // the return value of _this_ function.
            return SubSequencesCore(inputSequence, Enumerable.Empty<T>());
        }
        #endregion
    }


    public class Program
    {
        private static void EvolutionOfExtensionMethods(List<int> values)
        {
            /*-----------------------------------------------------------------------------------*/
            // Presenting the Evolution from highly specialized Utilitiy Functions to highly
            // reusable Extension Methods:


            /*-----------------------------------------------------------------------------------*/
            // Calling the first Variant: 

            // The call of the method Unique() does, well, everything. It gets the unique values
            // and prints them on console on its sole behalf.
            Utilities.MakeUniqueAndPrintToConsole(values);


            /*-----------------------------------------------------------------------------------*/
            // Calling the second Variant:

            // The call of the iterator function Unique2() does just create an IEnumerable
            // "containing" the unique values. - In fact the IEnumerable won't contain the
            // resulting unique values, but rather will that values be created in a deferred
            // manner. It is now on the behalf of the caller to do anything with the generated
            // values. So the responsibilities have been better separated. Ok, we decide to print
            // them to the console, though.
            Debug.WriteLine("Result of Utilities.Unique2():");
            // Here we get the result sequence by calling Utilities.Unique2(), but no code in
            // Utilities.Unique2() was executed yet! - The execution is deferred until the
            // iteration of the result sequence result2 begins.
            IEnumerable<int> result2 = Utilities.Unique2(values);
            foreach (var item in result2)
            {
                Debug.WriteLine(item);
            }


            /*-----------------------------------------------------------------------------------*/
            // Calling the third Variant:

            // The call of the iterator function Unique3() looks equal to the call of Unique2():
            Debug.WriteLine("Result of Utilities.Unique3():");
            var result3 = Utilities.Unique3<int>(values);
            foreach (var item in result3)
            {
                Debug.WriteLine(item);
            }
            // New in variant three is that we can call it with input sequences of other types than
            // int:
            char[] moreValues = {
                                    'z', 't', 'e', 'r', 't', 'e',
                                    'e', 'z', 't', 'r', 'z', 't',
                                    'e', 'r', 'e', 'w', 'r', 'e'
                                };
            Debug.WriteLine("Result of Utilities.Unique3() with chars:");
            var result3a = Utilities.Unique3<char>(moreValues);
            foreach (var item in result3a)
            {
                Debug.WriteLine(item);
            }
            // Pardon? We don't have to name the type of that sequence explicitly! Unique3() is a
            // generic method and the type can be inferred from its argument:
            Debug.WriteLine("Result of Utilities.Unique3() with chars (inferred):");
            var result3b = Utilities.Unique3(moreValues);
            foreach (var item in result3b)
            {
                Debug.WriteLine(item);
            }


            /*-----------------------------------------------------------------------------------*/
            // Calling the fourth Variant:

            // Unique4() was finally implemented as extension method of IEnumerable of T. So we can
            // call it _on_ values, because List of T implements IEnumerable of T and values is
            // extended as well.
            Debug.WriteLine("Result of IEnumerable<T>.Unique4():");
            var result4 = values.Unique4();
            foreach (var item in result4)
            {
                Debug.WriteLine(item);
            }


            /*-----------------------------------------------------------------------------------*/
            // Chaining:

            // Because Unique4() returns another IEnumerable of T we can easily call the next
            // extension method on that result. This is what we call "chaining". Note that this
            // construct will take only the first two unique values, because of deferred execution
            // Unique4() will generate and return only a sequence of two unique values, instead of
            // fetching all unique values eagerly. And this code does much clearer express what it 
            // does; imagine a couple of cascaded loops instead of this code - brrr...!
            // The chained notation does also document the order, in which the operations are being
            // executed.
            // This style is very declarative. We just explain that we need the first two items of
            // the unique subsets, which we'd like to print. All responsibilities are broken down
            // into chunks. - We don't know _how_ Unique4() or Take() work, but their names make
            // very clear what a result we'll get!
            Debug.WriteLine("Result of IEnumerable<T>.Unique4().Take(2):");
            var chainResult = values.Unique4().Take(2);
            foreach (var item in chainResult)
            {
                Debug.WriteLine(item);
            }

            // We could have written equivalent code w/o chaining, but by using an intermediate
            // result:
            var intermediateResult = Utilities.Unique4(values);
            var finalResult = Enumerable.Take(intermediateResult, 2);
            foreach (var item in finalResult)
            {
                Debug.WriteLine(item);
            }

            // We could have written equivalent w/o chaining, but by using a cascaded call:
            foreach (var item in Enumerable.Take(Utilities.Unique4(values), 2))
            {
                Debug.WriteLine(item);
            }

            // To be frank the functionality of method Unique4() is already present as extension
            // method of IEnumerable of T in the .Net framework, it is called Distinct().
            Debug.WriteLine("Result of IEnumerable<T>.Distinct().Take(2):");
            var chainResulta = values.Distinct().Take(2);
            foreach (var item in chainResulta)
            {
                Debug.WriteLine(item);
            }
        }


        private static void ChooseRuntimeType()
        {
            /*-----------------------------------------------------------------------------------*/
            // Here it is presented how Extension Methods can choose a different Implementation
            // based on the Run Time Type:

            // Here the input sequence for MyReverse() is not a random access Collection as run
            // time type (IEnumerable<int>). So its content must be buffered with extension method
            // MyReverse() to revert the sequence.
            var result = Enumerable.Range(1, 9).MyReverse();
            foreach (var item in result)
            {
                Debug.WriteLine(item);
            }

            // Here the input sequence for MyReverse is a random access Collection as run time type
            // (List<int>). So the random access can be used to access the items upside down within
            // MyReverse(), no buffering is needed and streaming can be applied.
            var result2 = (new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }).MyReverse();
            foreach (var item in result2)
            {
                Debug.WriteLine(item);
            }
        }


        private static void SubsetsExampleRevisited()
        {
            /*-----------------------------------------------------------------------------------*/
            // Here the recursive SubsetsOf Example, taken from Part III of the C#2 Review was
            // reimplemented with the means of Extension Methods and complete deferred Execution
            // (It was renamed to SubSequencesOf().):

            var inputSet = new List<char>(new[] { 'a', 'b', 'c' });

            int nSubset = 0;
            foreach (var subset in Utilities.SubSequencesOf(inputSet))
            {
                Debug.WriteLine(string.Format("Subset {0}", ++nSubset));
                Debug.Write("{");
                foreach (var element in subset)
                {
                    Debug.Write(string.Format("{0},", element));
                }
                Debug.WriteLine(subset.Any() ? "\b}" : "}");
            }
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Our Example Data to play with:

            var values = new List<int>
                             {
                                 4,
                                 5,
                                 6,
                                 2,
                                 5,
                                 7,
                                 6,
                                 8,
                                 3,
                                 8,
                                 3,
                                 4,
                                 6,
                                 4,
                                 6,
                                 5,
                                 3,
                                 2,
                                 1,
                                 4,
                                 3,
                                 8,
                                 7,
                                 6,
                                 5,
                                 3,
                                 2,
                                 6,
                                 5,
                                 4,
                                 3,
                                 2,
                                 1
                             };


            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            EvolutionOfExtensionMethods(values);


            ChooseRuntimeType();


            SubsetsExampleRevisited();
        }
    }
}