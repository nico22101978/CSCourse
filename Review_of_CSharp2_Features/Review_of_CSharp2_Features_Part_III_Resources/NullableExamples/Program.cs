using System;
using System.Diagnostics;

// These examples show:
// - The basic usage of the type Nullable<T>.
namespace NullableExamples
{
    public class Program
    {
        public static void BasicOperations()
        {
            /*-----------------------------------------------------------------------------------*/
            // Basic Operations on Nullables:

            // Creating a Nullable<int>:
            Nullable<int> nullableInt = new Nullable<int>();
            // The operators ==/!= get lifted and null gets converted to int?.
            Debug.Assert(null == nullableInt); 
            Debug.Assert(!nullableInt.HasValue);

            // The default value of a Nullable<T> is null:
            nullableInt = default(Nullable<int>);
            Debug.Assert(null == nullableInt);

            // Equal meaning with a compact (often preferred) syntax:
            int? nullableInt2 = null; // Yes, on initialization null can be assigned now :)!
            // You can also use this syntax (instead of new Nullable<int>()):
            int? nullableInt2a = new int?();
            Debug.Assert(null == nullableInt2);
            Debug.Assert(!nullableInt2.HasValue);
            Debug.Assert(null == nullableInt2a);
            Debug.Assert(!nullableInt2a.HasValue);

            // OK, now let's set a value:
            // (Wrapping) The conversion from T to T? is done implicitly.
            nullableInt = 42; 
            Debug.Assert(null != nullableInt);
            Debug.Assert(nullableInt.HasValue);

            // (Unwrapping) The conversion from T? to T must be done explicitly.
            int anotherInt = (int)nullableInt;

            // Some examples of using operators:
            // Invalid! Cannot implicitly convert from int? to int:
            //int result1 = nullableInt + nullableInt2;
            // OK (uses the lifted operator+, temporary int? sum is explicitly unwrapped into an
            // int), but risky, if any operand is null, accessing Value while performing the
            // addition will throw an InvalidOperationException.            
            //int result2 = (int)(nullableInt + nullableInt2);
            // OK, but risky, if any operand is null, accessing Value will throw an
            // InvalidOperationException.
            //int result3 = nullableInt.Value + nullableInt2.Value;
            // OK (temporary int sum will be implicitly wrapped into an int?), but risky, if any
            // operand is null, accessing Value will throw an InvalidOperationException. 
            //int? result4 = nullableInt.Value + nullableInt2.Value;
            // OK, (uses the lifted operator+) but the programmer needs to check for null before
            // accessing the Value of result5.
            int? result5 = nullableInt + nullableInt2;              

            // How to get the value of a nullable safely?
            // If nullableInt is null use 0.
            int value1 = nullableInt.HasValue ? nullableInt.Value : 0;
            // If nullableInt is null use default(int) (this is also 0).  
            int value2 = nullableInt.GetValueOrDefault();
            // If nullableInt is null default to 42.  
            int value3 = nullableInt.GetValueOrDefault(42); 
            // Using the null coalescing operator to combine checking, getting and falling back on
            // a value.
            // If nullableInt is null use nullableInt2 (the result is int? alltogether).
            int? value4 = nullableInt ?? nullableInt2;
            // If nullableInt is null use 0 (same as the first example, the expression evaluates to
            // the underlying type int automatically).
            int value5 = nullableInt ?? 0;
            // The result is the first non-null value.                  
            int value6 = nullableInt ?? nullableInt2 ?? 0;


            /*-----------------------------------------------------------------------------------*/
            // Basic Object Operations on Nullables wrapping null:

            int? value7 = null;
            // stringOfWrappedNull is "" (an empty string):
            string stringOfWrappedNull = value7.ToString();

            // wrappedNullIsEqualToNull is true:
            bool wrappedNullIsEqualToNull = value7.Equals(null);
            // wrappedNullIsEqualToNull is false:
            bool wrappedNullIsEqualToNonNull = value7.Equals(42);

            // hashCodeOfWrappedNull is 0:
            int hashCodeOfWrappedNull = value7.GetHashCode();


            /*-----------------------------------------------------------------------------------*/
            // Boxing and Unboxing of Nullables:

            int? unboxedNullableInt = 23;
            // The boxing of nullables is somewhat special, the wrapped type will be boxed, not
            // the nullable type.
            // boxedInt will contain a boxed int, not a boxed int?.
            object boxedInt = unboxedNullableInt;
            // Unboxing to int? is no problem, the value is 23. 
            int? againUnboxed = (int?)boxedInt;

            int? unboxedNullableInt2 = null;
            // boxedInt will contain null, not a boxed int? being null.
            object boxedInt2 = unboxedNullableInt2;
            // Unboxing to int? is no problem, the value is null. 
            int? againUnboxed2 = (int?)boxedInt2;
            // This will result in a NullReferenceException! 
            //int wrappedInt = (int)boxedInt2;


            /*-----------------------------------------------------------------------------------*/
            // Nullables and the as-Operator:

            // The as-operator can be used for reference types and for boxed value types as 
            // Nullables like this:
            object boxedFloat = 67.0f;
            // boxedFloat boxes no int, the result is null.
            int? asInt = boxedFloat as int?;
            // boxedFloat boxes a float, the result is the boxed float value. 
            float? asFloat = boxedFloat as float?; 
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            BasicOperations();
        }
    }
}
