using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// tlbimp shdocvw.dll /out:shdocvwpia.dll
using ShDocVwPia = shdocvwpia;
using System.Runtime.InteropServices;
using System.Threading;

// This example shows:
// - The usage COM automation with the new abilities of the C#4 compiler.
namespace ComInterop
{
    public class Program
    {
        private static void EvolutionOfComInteropImprovements()
        {
            /*-----------------------------------------------------------------------------------*/
            // This Example shows how C#4 helped to simplify the Usage of COM (esp. on Methods
            // having many Parameters):

            // Sidebar: A typical problem of COM components are methods with many parameters, where
            // often only some of the parameters are really used and the other parameters are
            // passed as "Missing" or "NULL". Why is that so? In short: COM does not support method
            // overloading!
            // Also do almost all COM methods use ref parameters for performance reasons (typically
            // the passed arguments will not be modified within COM methods).

            // We are going to automate the Internet Explorer (from the "Microsoft Internet
            // Controls").

            // VS 2008 with the csc compiler for C#3:
            ShDocVwPia.IWebBrowser2 ie = new ShDocVwPia.InternetExplorer { Visible = true };
            
            // Because we have to call the method Navigate() with ref parameters, we require to
            // introduce variable to pass them as ref parameters legally.
            object targetFrameName = "_self";
            // Also do we have to fill all the unused parameters with the value Type.Missing. We
            // require to introduce another variable to pass Type.Missing as ref parameter. The
            // call must be poluted with the "filling" arguments, which may lead to confusing the
            // programmer the positions of the different parameters.
            object missing = Type.Missing;
            ie.Navigate("www.avid.com", ref missing, ref targetFrameName, ref missing, ref missing);
            while (ie.Busy)
            {
                Thread.Sleep(500);
            }
            ie.Quit();


            // VS 2010 with the csc compiler for C#4:
            ShDocVwPia.IWebBrowser2 ie2 = new ShDocVwPia.InternetExplorer { Visible = true };
            // - The need to create a variable to pass it as ref parameter is no longer needed. You
            //   can pass the _value_ (e.g. the string literal) directly as parameter, the ref
            //   qualifier is longer needed as well. The compiler will synthesize a variable that
            //   carries the value automatically. (COM methods do typically not modify the passed
            //   arguments. If they do modify values, these modifications will be ignored.)
            // - The need to fill the missing parameters by explicitly passing Type.Missing was
            //   reduced, because optional parameters with the default value Type.Missing have been
            //   used in the generated code. Notice, that these parameters are ref parameters, but
            //   have default values: this only allowed for the generated code of interop
            //   assemblies, not in user code. - The compiler will synthesize a variable that
            //   carries the value automatically as well.
            // - The application of named arguments reduces the confusion of parameters for
            //   programmers and readers.
            ie2.Navigate(URL: "www.avid.com", TargetFrameName: "_self");
            while (ie2.Busy)
            {
                Thread.Sleep(500);
            }
            ie2.Quit();

            // Release the COM objects by setting the reference to their runtime callable wrappers
            // (RCWs) to null, then the references are eligible for gc'ing. - When the finalizers
            // are called, the RCW's internal marshaling counter is decremented (please see the
            // next sidebar).
            ie = null;
            ie2 = null;
            
            // Sidebar: You could also call Marshal.ReleaseComObject(), this call will
            // deterministically decrement the RCW's internal marshaling counter (Not COM's
            // reference counter!). Normally the RCW holds a COM object with a COM reference
            // counter of 1, and when the RCW's internal Marshalling count reaches 0 the COM
            // reference count will be decremented to 0, which finally leads to the COM object
            // deleting itself.
            
            // Sidebar: more simplifications on working with COM:
            // - Named indexers can be called easily. Named indexers can only be called in C#, but
            //   you can not define new ones. (These so called indexed properties are known in the
            //   CLR and applicable in VB since .Net 1. The unnamed indexed property (i.e. the
            //   "indexer") present in C# is called default property/member.) Named indexers in
            //   generated interop code that only have optional arguments, can also be called from
            //   C#.
            // - Interop assemblies can now be linked in instead of only referenced.
            //      - Only the used types will be linked/embedded into your resulting assembly.
            //      - Self contained assemblies have a smaller "footprint" and reduce version
            //        problems.
            // - No-PIA deployment bases on .Net 4's new feature "type equivalence". It allows the
            //   run time to consider certain types as being interchangeable even if they are
            //   defined in different assemblies, so it enhances interoperability. The idea of PIA
            //   was that only one assembly contains all the types, this was relaxed with type
            //   equivalence. Every linked in copy of the type gets a TypeIdentifier attribute that
            //   declares equivalent types with a common identifier (not for types w/ behavior,
            //   only interfaces, delegates, enums and PODs).
            // - If no-PIA deployed types are used the original VARIANT and IDispatch parameter and
            //   return types will be provided as of type dynamic instead of object. - This enables
            //   dynamic dispatch with C#4 and the Dynamic Language Runtime (DLR).
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            EvolutionOfComInteropImprovements();
        }
    }
}