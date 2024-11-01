using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

// These examples show:
// - Intrinsic covariance of arrays.
// - Generic co- and contravariance.
namespace VarianceExample
{
    #region Hierarchy with illegal Covariance:
    public class Egg
    { }


    public class DuckEgg : Egg
    { }


    public class Bird
    {
        public virtual Egg Lay()
        {
            return new Egg();
        }
    }


    public class Duck : Bird
    {
        // Invalid! Duck.Lay()'s return type must be Egg to match overridden member Bird.Lay().
        // It is ok to express return type covariance like this in C++ and Java, but not in C#.
        //public override DuckEgg Lay() 
        //{
        //    return new DuckEgg();
        //}
    }
    #endregion


    #region Hierarchy with illegal Contravariance:
    //class Person { }


    //class Pianist : Person { }


    //class Piano
    //{
    //    public virtual void Play(Pianist player) { }
    //}


    //class GrandPiano : Piano
    //{
    //    // Invalid! Piano.Play(Person): no suitable method found to override	
    //    // So this Play(Person)-method can not override Play(Pianist), because it needs to have the
    //    // _exact_ signature. - This does also not work in C++ and Java, as you need the exact
    //    // signature for a valid override of a method.
    //    //public override void Play(Person player)
    //    //{
    //    //}
    //} 
    #endregion


    public class Program
    {
        #region Methods for IntrinsicCSharpCovarianceWArrays():
        private static void AcceptsControlArray(Control[] controls)
        {
            // This is ok! It is always _safe to read_ from an array as its item types are
            // covariant to the array's type.
            Control firstControl = controls[0];

            //DANGEROUS! If controls has a more specific dynamic item type (e.g. Form), it _won't
            // be safe to write_ a value to the array. An ArrayTypeMismatchException will be
            // thrown, in this case.
            //controls[0] = new Control(); // Will throw ArrayTypeMismatchException!

            // Sidebar: Because of this danger, methods with out and ref parameters are also not
            // variant on that parameters.
        }


        // This works also with param array as parameters.
        private static void AcceptsControlParamsArray(params Control[] controls)
        {
            // This is ok! It is always _safe to read_ from an array as its item types are
            // covariant to the array's type.
            Control firstControl = controls[0];

            // This is _also_ ok! - Because the params keyword will create a new array of type
            // Control[], even if the caller passed arguments with a more specific type (e.g. of
            // type Form). It is ok to write to a params array.
            controls[0] = new Control();
        }
        #endregion


        public static void IntrinsicCSharpCovarianceWArrays()
        {
            /*-----------------------------------------------------------------------------------*/
            // Covariance of Arrays:

            Form[] forms = new[]
            {
                new Form(),
                new Form() 
            };

            // This covariant conversion is valid. The conversion is allowed if there is a
            // reference conversion from the items' source type to the target type available. So
            // boxing, value type and user defined conversions are not supported covariantly, but
            // inheritance and interface (of implicitly or explicitly implemented interfaces)
            // conversions are.
            Control[] controls = forms;

            // This is ok! It is always _safe to read_ from an array as its item types are
            // covariant to the array's type.
            Control firstControl = controls[0];

            // Dangerous! If controls has a more specific dynamic item type (e.g. Form), it _won't
            // be safe to write_ a value to the array. An ArrayTypeMismatchException will be
            // thrown, in this case.
            //controls[0] = new Control(); // Will throw ArrayTypeMismatchException!

            // Notice, that this case does normally happen when arrays are being passed to a
            // method. Within a method you can not assume an other array-type than the static type
            // of the parameter like so:
            AcceptsControlArray(forms);
            // But with param arrays as parameters there is no problem with covariance (see the 
            // implementation of AcceptsControlParamsArray):
            AcceptsControlParamsArray(new Form(), new Form());

            // Sidebar: Lesson learned - try to limit the usage of array parameters to param-
            // arrays, as they are much safer.


            /*-----------------------------------------------------------------------------------*/
            // Conversion not being supported by Array's Variance:

            // To cut the story short: only reference conversions can be performed on variant
            // types. - Only these conversions are considered to be type safe and these conversions
            // do also not create new instances. Boxing, value type and user defined conversions
            // do create new instances of the target type, so they can not be used on variant
            // types!


            //Invalid: boxing conversion!
            //int[] arrayOfInts = new[] { 6, 3, 8 };
            //object[] arrayOfObjects = arrayOfInts;

            //Invalid: value type conversion!
            //int[] arrayOfInts = new[] { 6, 3, 8 };
            //long[] arrayOfLongs = arrayOfInts;
            //
            //Invalid: another value type conversion!
            //int[] arrayOfInts = new[] { 6, 3, 8 };
            //int?[] arrayOfnullableInts = arrayOfInts;

            //Invalid: user defined conversion!
            //TextAndLength[] sequenceOfTextAndLengths = new[] 
            //{
            //    new TextAndLength("emerald"),
            //    new TextAndLength("ruby"),
            //    new TextAndLength("sapphire")
            //};
            //int[] sequenceOfInts = sequenceOfTextAndLengths;


            /*-----------------------------------------------------------------------------------*/
            // Invariance of Generic Collections:

            List<Form> formsList = new List<Form>
            {
                new Form(),
                new Form() 
            };

            // Invalid! Cannot implicitly convert type 'List<Form>' to 'List<Control>'. As
            // covariance is not supported on Lists, this line will not compile.
            //List<Control> controlsList = formsList;

            // Invalid! So you are also not able to let the compiler compile this line legally:
            //controlsList.Add(new Control());

            // In effect C#/.Net prohibits the variant usage of generics in order to avoid run time
            // type problems seen at covariant arrays. And C#4/.Net 4 did not change this
            // restriction (generic collections are invariant), but it introduced a way to express
            // variances in a type safe way.            
        }


        #region Types for unsupported Type Conversion on Variance:
        public class TextAndLength
        {
            private readonly string _text;


            public TextAndLength(string text)
            {
                _text = text;
            }


            public static implicit operator int(TextAndLength that)
            {
                return null != that._text
                        ? that._text.Length
                        : 0;
            }
        }
        #endregion


        #region Types for ArrayLikeButTypeSafe():
        /*-----------------------------------------------------------------*/
        // How should this Ability to define Variance help me? 

        // You can have a type safe array-like type!
        // Well, it helps as you are _not_ able to define a generic interface using a variant type
        // on parameter _and_ return types. E.g. you can not have an indexer dealing with a co- and
        // or contravariant type, its type must be invariant! So you can enforce an invariant, thus
        // type safe array-like type.
        internal interface ISafeArray<T>
        {
            T this[int index]
            {
                get;
                set;
            }
        }

        // An implementation of that interface.
        internal class MyList<T> : ISafeArray<T>, IEnumerable<T>
        {
            private IList<T> _t = new List<T>();
            public T this[int index]
            {
                get
                {
                    return _t[index];
                }
                set
                {
                    _t[index] = value;
                }
            }


            public IEnumerator<T> GetEnumerator()
            {
                return _t.GetEnumerator();
            }


            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public void Add(T item)
            {
                _t.Add(item);
            }
        } 
        #endregion


        public static void ArrayLikeButTypeSafe()
        {
            /*-----------------------------------------------------------------------------------*/
            // Invariance of Array-like Types - Now we can express this:

            ISafeArray<Form> forms = new MyList<Form> 
            {
                new Form(),
                new Form() 
            };


            // Invalid! There exists no implicit conversion from ISafeArray<Form> to
            // ISafeArray<Control>, because ISafeArray<T> is invariant! So you can not even call
            // the indexer the wrong way.
            //ISafeArray<Control> controls = (ISafeArray<Control>)forms;

            // Sidebar: You _can_ explicitly convert forms to ISafeArray<Control>! This is
            // possible, because the object forms can reference an instance that indeed implements
            // ISafeArray<Control>. So the conversion that is tried in this case has nothing to do
            // with the relation of ISafeArray<Form> to ISafeArray<Control>, because there is no
            // relation, i.e. no variance; it is a bare interface conversion ("cast contact lens").
            // This conversion will of course fail during run time with an InvalidCastException, as
            // MyList<Form> does not implement ISafeArray<Control>. 
        }


        #region Hierarchy with legal Covariance:
        public class Egg
        {
        }


        public class DuckEgg : Egg
        {
        }


        public interface IBird<out T>
        {
            T Lay();
        }


        public class Bird : IBird<Egg>
        {
            public Egg Lay()
            {
                return new Egg();
            }
        }


        public class Duck : IBird<DuckEgg>
        {
            public DuckEgg Lay()
            {
                return new DuckEgg();
            }
        }
        #endregion


        #region Covariant Method with IBirds:
        public static void CovariantBirdLaysEgg(IBird<Egg> aBird)
        {
            Egg anEgg = aBird.Lay();
        } 
        #endregion


        public static void CSharpCovariance()
        {
            /*-----------------------------------------------------------------------------------*/
            // The prove that we can express the Bird/Lay/Egg Consistency now:

            IBird<Egg> aBird = new Bird();
            Egg anEgg = null;
            // This code will work with substituted IBird-types.
            anEgg = aBird.Lay();

            // We can substitute Bird (as IBird<Egg>) with Duck, because the generic parameter is
            // covariant. - An implicit conversion takes place.
            aBird = new Duck();
            // Voilá it works with Ducks, the return type is covariant!
            anEgg = aBird.Lay();

            // Mind, that before we introduced the interface IBird we have not been even able to
            // express the covariance on the return type! Now we can express the relation of IBird
            // and Egg to be covariant.


            /*-----------------------------------------------------------------------------------*/
            // Covariant IBirds applied on a Method:

            // You can also show this covariance in action, as you can pass different IBirds to a 
            // method that can handle all IBirds consistently (laying Eggs and subtypes of those).
            CovariantBirdLaysEgg(new Bird());
            CovariantBirdLaysEgg(new Duck());


            /*-----------------------------------------------------------------------------------*/
            // A real Life Example with IEnumerable<out T>:

            IEnumerable<Form> forms = new[]
            {
                new Form(),
                new Form()
            };

            IEnumerable<Control> controls = forms;
            #region Remember C#3:
            // IEnumerable<Control> controls = forms;
            //This line was Invalid in C#3! The compiler could not implicitly convert type 
            // 'IEnumerable<Form>' to 'IEnumerable<Control>', because these types were not
            // recognized to be related. You had to iterate through forms and convariantly convert
            // each item to Control explicitly. This iteration can also be expressed with the query
            // operator Cast():
            IEnumerable<Control> controls2 = forms.Cast<Control>();
            // This costly iteration and explicit conversion is gone with the now covariant
            // IEnumerable<T>. The example shows the mere benefit of covariance in C#: it reduces
            // compile time errors on apperently legal lines of code.
            #endregion


            /*-----------------------------------------------------------------------------------*/
            // Conversion not beeing supported by Variance:

            // To cut the story short: only reference conversions can be performed on variant
            // types. - Only these conversions are considered to be type safe and these conversions
            // do also not create new instances. Boxing, value type and user defined conversions
            // do create new instances of the target type, so they can not be used on variant
            // types!

            // Boxing conversion:
            IEnumerable<int> sequenceOfInts = new[] { 6, 3, 8 };
            //Invalid: implicit covariant boxing conversion!
            //IEnumerable<object> sequenceOfObjects = sequenceOfInts;
            //
            //OK: Conversion with LINQ's Cast()-query operator.
            IEnumerable<object> sequenceOfObjects = sequenceOfInts.Cast<object>();

            // Value type conversion:
            //Invalid: implicit covariant value type conversion!
            //IEnumerable<long> sequenceOfLongs = sequenceOfInts;
            //
            //OK: Conversion with LINQ's Cast()-query operator. But will fail on run time, when the
            //sequenceOfLongs is iterated.
            IEnumerable<long> sequenceOfLongs = sequenceOfInts.Cast<long>();
            
            // Another value type conversion:
            //Invalid: implicit covariant value type conversion!
            //IEnumerable<int?> sequenceOfNullableInts = sequenceOfInts;
            //
            //OK: Conversion with LINQ's Cast()-query operator.
            IEnumerable<int?> sequenceOfNullableInts = sequenceOfInts.Cast<int?>();

            // User defined conversion:
            IEnumerable<TextAndLength> sequenceOfTextAndLengths = new[] 
            {
                new TextAndLength("emerald"),
                new TextAndLength("ruby"),
                new TextAndLength("sapphire")
            };
            //Invalid: implict covariant user defined conversion!
            //IEnumerable<int> sequenceOfInts = sequenceOfTextAndLengths;
            //
            //OK: Conversion with LINQ's Cast()-query operator.
            IEnumerable<int> sequenceOfInts2 = sequenceOfTextAndLengths.Cast<int>();
        }


        #region Hierarchy with legal Contravariance:
        public class Person { }


        public class Pianist : Person { }


        public interface IPiano<in T>
        {
            void Play(T player);
        }


        public class Piano : IPiano<Pianist>
        {
            public void Play(Pianist player)
            {
            }
        }


        public class GrandPiano : IPiano<Person>
        {
            public void Play(Person player)
            {
            }
        }
        #endregion


        #region Contravariant Method with IPianos:
        public static void ContravariantIPianoPlayedByPianists(IPiano<Pianist> aPiano,
            Pianist aPianist)
        {
            aPiano.Play(aPianist);
        }
        #endregion


        #region Types for CSharpContravariance():
        internal sealed class ControlMinimumSizeWidthComparer : IComparer<Control>
        {
            public int Compare(Control lhc, Control rhc)
            {
                return lhc.MinimumSize.Width.CompareTo(rhc.MinimumSize.Width);
            }
        }
        #endregion


        private static void CSharpContravariance()
        {
            /*-----------------------------------------------------------------------------------*/
            // The prove that we can express the Piano/GrandPiano/Pianist/Person Consistency now:

            IPiano<Pianist> aPiano = new Piano();
            Pianist pianist = new Pianist();
            // This code will work with substituted IPiano-types.
            aPiano.Play(pianist);

            // (Look closely! This is the really strange part of the code.) We can substitute
            // Piano (as IPiano<Pianist>) with GrandPiano, because the generic parameter is
            // contravariant. - An implicit conversion takes place.
            aPiano = new GrandPiano();
            // Voilá it works with GrandPiano, the parameter type is contravariant!
            aPiano.Play(pianist);


            /*-----------------------------------------------------------------------------------*/
            // Contravariant IPianos applied on a Method:

            // You can also show this contravariance in action, as you can pass different IPianos
            // and Pianists to a method that can handle all IPianos consistently (being playable by
            // Pianists and its basetypes).
            ContravariantIPianoPlayedByPianists(new Piano(), new Pianist());
            ContravariantIPianoPlayedByPianists(new GrandPiano(), new Pianist());


            /*-----------------------------------------------------------------------------------*/
            // A real Life Example with IComparable<in T>:

            List<Form> forms = new List<Form>
            {
                new Form { MinimumSize = new Size(width: 22, height: 15) },
                new Form { MinimumSize = new Size(width: 20, height: 46) }
            };

            // Why shouldn't we use the ControlMinimumSizeWidthComparer to sort a List of Forms?
            IComparer<Form> controlComparer = new ControlMinimumSizeWidthComparer();
            forms.Sort(controlComparer); // OK!
            #region Remember C#3:
            // IComparer<Form> controlComparer = new ControlMinimumSizeWidthComparer();
            // This line was Invalid in C#3! The compiler could not implicitly convert type
            // 'ControlMinimumSizeWidthComparer' to 'IComparer<Form>', because these types were not
            // recognized to be related.
            // The example shows the mere benefit of contravariance in C#: it reduces
            // compile time errors on apperently legal lines of code.
            #endregion
        }


        public static void VarianceInGenericDelegates()
        {
            /*-----------------------------------------------------------------------------------*/
            // These Examples present the generic Delegate Types Func<out T> and Action<in T>:

            // Covariance
            Control resultingControl = null;

            Func<Control> controlFactory = () => new Control();
            resultingControl = controlFactory();

            Func<Form> formFactory = () => new Form();
            // Covariant conversion (Func<Form> to Func<Control>):
            controlFactory = formFactory;
            resultingControl = controlFactory();


            // Contravariance
            Action<Control> printControlInformation =
                (Control ctrl) => Debug.WriteLine(ctrl.Text);
            printControlInformation(new Control { Text = "aControl" });

            // Contravariant conversion (Func<Control> to Action<Form>):
            Action<Form> printFormInformation = printControlInformation;
            printFormInformation(new Form { Text = "aForm" });

            // Sidebar: In C#2 another kind of delegate variance has been introduced. This delegate
            // variance made it possible to assign methods with variant signatures to delegate
            // variables having a certain type; then an implicit conversion takes place. But the
            // generic delegate variance introduced in C#4 enables us to assign, and implicitly
            // convert delegate instances of different types. Before C#4 following lines would have
            // been Invalid:
            // The covariant conversion (Func<Form> to Func<Control>):
            // controlFactory = formFactory;
            // The contravariant conversion (Func<Control> to Action<Form>):
            // Action<Form> printFormInformation = printControlInformation;
        }


        #region Methods for VariancePitfalls():
        private static void Accept(Action<Form> action)
        {
            Debug.WriteLine("in Accept(Action<Form>)");
        }


        private static void Accept(object somethingDifferent)
        {
            Debug.WriteLine("in Accept(object)");
        }
        #endregion


        private static void VariancePitfalls()
        {
            /*-----------------------------------------------------------------------------------*/
            // Breaking Run Time and Compile Time Changes, when Variances are introduced on present
            // Types:

            // 1. Explicit run time type checking will behave differently, when variance is enabled
            // on a present type:
            IEnumerable<Form> formsSequence = new[]
            {
                new Form(),
                new Form()
            };

            // On a CLR 3 isKindOfControlSequence will be false. On a CLR 4, having the covariance
            // on IEnumerable<T> turned on, isKindOfControlSequence will be true.
            bool isKindOfControlSequence = formsSequence is IEnumerable<Control>;


            // 2. The compile time method overload resolution will behave differently, when
            // variance is enabled on a present type:
            Action<Control> actionOnControl = (Control control) => { };
            // A C#3 compiler will resolve a call to Accept(object). On a C#4 compiler, having the
            // contravariance on Action<T> turned on, Accept(Action<Form>) will be resolved and
            // called.
            Accept(actionOnControl);


            /*-----------------------------------------------------------------------------------*/
            // Generic Variance on Delegates doesn't work with Multicasting:

            Action<string> stringAction = s => Debug.WriteLine("Action<string>");
            Action<object> objectAction = o => Debug.WriteLine("Action<object>");

            // From a variance point this line is ok, Action<object> is contravariant to
            // Action<string>, and no compiler error will be produced:
            //Action<string> action = objectAction + stringAction; // Will throw ArgumentException!

            // The problem is that the method Delegate.Combine() can only handle delegate instances
            // having the same type. The problem can be circumvented by wrapping the 
            // incompatible delegate instances with a compatible delegate instance:
            Action<string> action = new Action<string>(objectAction) + stringAction; // Ok!
            action("pass");
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:
            
            
            IntrinsicCSharpCovarianceWArrays();


            ArrayLikeButTypeSafe();


            CSharpCovariance();


            CSharpContravariance();


            VarianceInGenericDelegates();


            VariancePitfalls();
        }
    }
}