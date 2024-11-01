using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

// These examples show:
// - Generator functions to produce sequences with iterator blocks.
namespace IteratorExamples
{
    public static class Utils
    {
        #region A more sophisticated Iterator Block Example w/ Recursion:
        // A Lisp implementation of the subset's algorithm (this implementation gets the full
        // powerset):
        // (defun powerset (l)
        //  (if (null l)
        //   '(nil)
        //   (let ((ps (powerset (cdr l))))
        //    (append ps (mapcar #'(lambda (x) (cons (car l) x)) ps)))))
        //
        // This is the core of the implementation to get the subsets of a given set. Mind that this
        // example will _not_ retrive the whole powerset, but a sequence to get each subset
        // respectively. The implementation is recursive, so iterator blocks can also be used
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
                List<T> newInputList = new List<T>(inputList);
                newInputList.Add(remainderList[0]);

                List<T> newRemainderList = new List<T>(remainderList);
                newRemainderList.RemoveAt(0);
                // Recursion:
                foreach (IList<T> list in SubsetsCore(newRemainderList, newInputList))
                {
                    yield return list;
                }
                // Recursion:
                foreach (IList<T> list in SubsetsCore(newRemainderList, inputList))
                {
                    yield return list;
                }
            }
        }


        // Get sequence with the subsets of the passed set.
        public static IEnumerable<IList<T>> SubsetsOf<T>(IList<T> set)
        {
            if (0 != set.Count)
            {
                // Just feeds the core implementation iterator block and forwards the returned
                // sequence's item to the return value of _this_ iterator block.
                foreach (IList<T> list in SubsetsCore(set, new List<T>()))
                {
                    yield return list;
                }
            }
            // else: the sequence remains empty.
        }
        #endregion


        #region Generated Code for the more sophisticated Iterator Block Example w/ Recursion (Implementations of IEnumerable<T> and IEnumerator<T>):
        //private sealed class d__0<T> : IEnumerable<IList<T>>, IEnumerator<IList<T>>
        //{
        //    private int _state;
        //    private IList<T> _current;
        //    public IList<T> _inputList;
        //    public IList<T> _remainderList;
        //    public IEnumerator<IList<T>> _wrap5;
        //    public IEnumerator<IList<T>> _wrap7;
        //    private int _initialThreadId;
        //    public IList<T> _3;
        //    public IList<T> _4;
        //    public List<T> _1;
        //    public List<T> _2;
        //    public IList<T> inputList;
        //    public IList<T> remainderList;

        //    public d__0(int state)
        //    {
        //        this._state = _state;
        //        this._initialThreadId = Thread.CurrentThread.ManagedThreadId;
        //    }

        //    private void _Finally6()
        //    {
        //        this._state = -1;
        //        if (this._wrap5 != null)
        //        {
        //            this._wrap5.Dispose();
        //        }
        //    }

        //    private void _Finally8()
        //    {
        //        this._state = -1;
        //        if (this._wrap7 != null)
        //        {
        //            this._wrap7.Dispose();
        //        }
        //    }

        //    public bool MoveNext()
        //    {
        //        bool CS_1_0000;
        //        try
        //        {
        //            switch (this._state)
        //            {
        //                case 0:
        //                    this._state = -1;
        //                    if (0 != this.remainderList.Count)
        //                    {
        //                        break;
        //                    }
        //                    this._current = this.inputList;
        //                    this._state = 1;
        //                    return true;
        //                case 1:
        //                    this._state = -1;
        //                    goto Label_01A6;
        //                case 3:
        //                    long goto Label_0119;
        //                case 5:
        //                    long goto Label_0186;
        //                default:
        //                    goto Label_01A6;
        //            }
        //            this._1 = new List<T>(this.inputList);
        //            this._1.Add(this.remainderList[0]);
        //            this._2 = new List<T>(this.remainderList);
        //            this._2.RemoveAt(0);
        //            this._wrap5 = Program.SubsetsCore<T>(this._2, this._1).GetEnumerator();
        //            this._state = 2;
        //            while (this._wrap5.MoveNext())
        //            {
        //                this._3 = this._wrap5.Current;
        //                this._current = this._3;
        //                this._state = 3;
        //                return true;
        //            Label_0119:
        //                this._state = 2;
        //            }
        //            this._state = 2;
        //            this._Finally6();
        //            this._wrap7 = Program.SubsetsCore<T>(this._2, this.inputList).GetEnumerator();
        //            this._state = 4;
        //            while (this._wrap7.MoveNext())
        //            {
        //                this._4 = this._wrap7.Current;
        //                this._current = this._4;
        //                this._state = 5;
        //                return true;
        //            Label_0186:
        //                this._state = 4;
        //            }
        //            this._Finally8();
        //        Label_01A6:
        //            CS_1_0000 = false;
        //        }
        //        fault
        //        {
        //            ((IDisposable)this).Dispose();
        //        }
        //        return CS_1_0000;
        //    }

        //    IEnumerator<IList<T>> IEnumerable<IList<T>>.GetEnumerator()
        //    {
        //        Program.d__0<T> d__;
        //        if ((Thread.CurrentThread.ManagedThreadId == this._initialThreadId) && (this._state == -2))
        //        {
        //            this._state = 0;
        //            d__ = this;
        //        }
        //        else
        //        {
        //            d__ = new Program.d__0<T>(0);
        //        }
        //        d__.remainderList = this._remainderList;
        //        d__.inputList = this._inputList;
        //        return d__;
        //    }

        //    IEnumerator IEnumerable.GetEnumerator()
        //    {
        //        return ((System.Collections.Generic.IEnumerable<IList<T>>)this).GetEnumerator();
        //    }

        //    void IEnumerator.Reset()
        //    {
        //        throw new NotSupportedException();
        //    }

        //    void IDisposable.Dispose()
        //    {
        //        switch (this._state)
        //        {
        //            case 2:
        //            case 3:
        //                try
        //                {
        //                }
        //                finally
        //                {
        //                    this._Finally6();
        //                }
        //                break;
        //            case 4:
        //            case 5:
        //                try
        //                {
        //                }
        //                finally
        //                {
        //                    this._Finally8();
        //                }
        //                break;
        //        }
        //    }

        //    IList<T> IEnumerator<IList<T>>.Current
        //    {
        //        get
        //        {
        //            return this._current;
        //        }
        //    }

        //    object IEnumerator.Current
        //    {
        //        get
        //        {
        //            return this._current;
        //        }
        //    }
        //} 
        #endregion


        #region Non-generic GetRange():
        // This method returns a sequence that provides all ints from fromInclusive to "infinity"
        // with a step of step.
        public static IEnumerable GetRange(int fromInclusive, int step)
        {
            for (int i = fromInclusive; i < int.MaxValue; i += step)
            {
                yield return i;
            }
        }
        #endregion


        #region Non-generic GetRangeWithSideEffects() with Side Effects (often no good Idea):
        // Well, it's not very common or a good idea to program side effects into iterator blocks.
        // (Very dangerous: lock-statements!) It is possible to program side effects, but they are
        // not being performed, when the iterator block (the method using yield) is called...they
        // are performed, when the iteration of the (by the iterator block) returned iterator has
        // begun! - Therefor the yield keyword is said to enable _deferred execution_.
        // Besides side effects also Exceptions may be thrown, when this is the case the iteration
        // will be terminated, the sequence will be disposed of and the Exception will be thrown to
        // the iterator block's caller.
        public static IEnumerable GetRangeWithSideEffects(int fromInclusive, int step)
        {
            for (int i = fromInclusive; i < int.MaxValue; i += step)
            {
                // Yes, you can also place side effect code into the iterator block's code, but it
                // won't execute, before the iteration starts(!):
                Debug.WriteLine("Hey! I'm in GetRange() now.");

                yield return i;
                // But at least after yield you have no guarantee that this code will be ever
                // executed! - The caller can decide to end the iteration early or an exception may
                // be thrown...!
                Debug.WriteLine("You'll possibly never see this message.");
            }
        }
        #endregion


        #region Generic GetRange():
        // Same as GetRange() but as generic method.
        public static IEnumerable<int> GenericGetRange(int fromInclusive, int step)
        {
            for (int i = fromInclusive; i < int.MaxValue; i += step)
            {
                yield return i;
            }
        }
        #endregion


        #region Iterator Block w/o Loop:
        public static IEnumerable<int> GetItemsSimple()
        {
            yield return 78;
            yield return 23;
            yield return 96;
        }
        #endregion


        #region Generated Code for Iterator Block w/o Loop (Implementations of IEnumerable<T> and IEnumerator<T>):
        public sealed class d__0 : IEnumerable<int>, IEnumerator<int>
        {
            private int _state;
            private int _current;
            private readonly int _initialThreadId;

            public d__0(int state)
            {
                _state = state;
                _initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            public bool MoveNext()
            {
                switch (_state)
                {
                    case 0:
                        _state = -1;
                        _current = 78;
                        _state = 1;
                        return true;

                    case 1:
                        _state = -1;
                        _current = 23;
                        _state = 2;
                        return true;

                    case 2:
                        _state = -1;
                        _current = 96;
                        _state = 3;
                        return true;

                    case 3:
                        _state = -1;
                        break;
                }
                return false;
            }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                if ((Thread.CurrentThread.ManagedThreadId == _initialThreadId) && (_state == -2))
                {
                    _state = 0;
                    return this;
                }
                return new d__0(0);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<int>)this).GetEnumerator();

            }

            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            int IEnumerator<int>.Current
            {
                get
                {
                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }
        }
        #endregion


        #region Iterator Block with finally:
        public static IEnumerable<int> GetItems()
        {
            try
            {
                yield return 78;
                yield return 23;
                //yield break;
                yield return 96;
            }
            finally
            {
                // The code of the finally block will be executed:
                // - if the iterator block leaves execution, or
                // - if a yield break statement is executed, or
                // - if the iterator will be disposed of (which is done by foreach automatically)
                Debug.WriteLine("in finally block");
            }
        }
        #endregion


        #region Iterator Block as Property:
        // Iterator blocks can be provided as properties as well:
        public static IEnumerable<int> Range
        {
            get
            {
                for (int i = 1; i < 11; ++i)
                {
                    yield return i;
                }
            }
        }
        #endregion
    }


    public class Program
    {
        #region Types for WorkingWithIterators():
        public delegate IEnumerable<int> IntegerSequenceProvider();

        // MyEnumerable doesn't even implement IEnumerable! - It is sufficient providing the
        // method "IEnumerator/<T> GetEnumerator()" to fulfill foreach's contract.
        public class MyEnumerable
        {
            private readonly int[] _toBeIterated;
            public MyEnumerable(int[] toBeIterated)
            {
                _toBeIterated = toBeIterated;
            }

            public IEnumerator<int> GetEnumerator()
            {
                foreach (var item in _toBeIterated)
                {
                    yield return item;
                }
            }
        }
        #endregion


        private static void WorkingWithIterators()
        {
            /*-----------------------------------------------------------------------------------*/
            // Non-generic GetRange():

            foreach (object item in Utils.GetRange(0, 2))
            {
                Debug.WriteLine(string.Format("item: {0}", item));
                Thread.Sleep(500);
                if (6 == (int)item)
                {
                    break;
                }
            }


            /*-----------------------------------------------------------------------------------*/
            // Non-generic GetRangeWithSideEffects():

            foreach (object item in Utils.GetRangeWithSideEffects(0, 2))
            {
                Debug.WriteLine(string.Format("item: {0}", item));
                Thread.Sleep(500);

                if (0 == (int)item)     // You won't see the message after yield.  
                //if (2 == (int)item)   // You will see the message after yield.
                {
                    break;
                }
            }


            /*-----------------------------------------------------------------------------------*/
            // Generic GetRange():

            foreach (int item in Utils.GenericGetRange(0, 2))
            {
                Debug.WriteLine(string.Format("item: {0}", item));
                Thread.Sleep(500);
                if (6 == item)
                {
                    break;
                }
            }


            /*-----------------------------------------------------------------------------------*/
            // Using an Iterator Block that uses no Loop:

            foreach (int item in Utils.GetItemsSimple())
            {
                Debug.WriteLine(string.Format("item: {0}", item));
                Thread.Sleep(500);
            }

            #region Compiler generated stuff for calling that Iterator Block:
            foreach (int item in new Utils.d__0(-2))
            {
                Debug.WriteLine(string.Format("item: {0}", item));
                Thread.Sleep(500);
            }
            #endregion


            /*-----------------------------------------------------------------------------------*/
            // Sophisticated - Getting subsets:

            IList<string> inputSet = new List<string>(new string[] { "a", "b", "c" });

            int nSubset = 0;
            foreach (IList<string> subset in Utils.SubsetsOf(inputSet))
            {
                Debug.WriteLine(string.Format("List {0}", ++nSubset));
                foreach (string nextString in subset)
                {
                    Debug.WriteLine(nextString);
                }
            }


            /*-----------------------------------------------------------------------------------*/
            // It is so simple to be an Iterator:

            // MyEnumerable doesn't even implement IEnumerable! - It is sufficient providing the
            // method "IEnumerator/<T> GetEnumerator()" to fulfill foreach's contract.
            MyEnumerable enumerable = new MyEnumerable(new int[] { 1, 2, 3, 4, 5 });
            foreach (int item in enumerable)
            {
                Debug.WriteLine(item);
            }


            /*-----------------------------------------------------------------------------------*/
            // Not supported: Anonymous Iterator Blocks and Iterator Blocks with out or ref
            // Parameters:

            // The compiler does not support this. There is no technical reason, but the compiler
            // procedure to create a class for the Delegate as well as the compiler procedure to
            // rewrite the code for the iterator block was considered to be too much effort at the
            // C# team compared to the gain. - So iterator blocks must be top level methods in C#!
            //IntegerSequenceProvider firstTen = delegate NOT SUPPORTED!
            //{
            //    for (int i = 1; i < 11; ++i)
            //    {
            //        yield return i;
            //    }
            //};

            // However, it is planned that VB11 will have support for anonymous iterator blocks!

            // For similar reasons methods with out or ref parameters can not be iterator blocks.
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            WorkingWithIterators();
        }
    }
}