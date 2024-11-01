using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Threading;


// These examples show:
// - How to use C# dynamic coding to put simple COM automation into effect (the Internet Explorer (IE)
//   will be automated).
// - The automation of a user-defined COM server.
namespace DynamicComInteropCSharp
{
    public class Program
    {
        private static void DynamicActivationAndAutomation()
        {
            /*-----------------------------------------------------------------------------------*/
            // This Example shows the Application of C# dynamic Coding to allow simple COM
            // Automation:

            // Mind, that no creation of PIAs (for the "Microsoft Internet Controls" in case of the
            // Internet Explorer) is required: you do not need to deal with any static or generated
            // types. - This reduces and simplifies the required code dramatically!

            dynamic ieApplication = null;
            try
            {
                // The calls of Type.GetTypeFromProgID() and Activator.CreateInstance() are roughly
                // equivalent to VB's special method Interaction.CreateObject() (see below). Esp.
                // Activator.CreateInstance() is a kind of late bound object creation.
                Type ieApplicationType = Type.GetTypeFromProgID("internetexplorer.application");
                // Stick IE's automation object into a dynamic variable to enable dynamic dispatch.
                ieApplication = Activator.CreateInstance(ieApplicationType);
                ieApplication.Visible = true;
                ieApplication.Navigate("www.avid.com");

                // Sidebar: You can also use named arguments on calling methods via COM Dispatch.

                while (ieApplication.Busy)
                {
                    Thread.Sleep(500);
                }

                // Mind, that we do not need any casts! The whole call chain is dispatched
                // dynamically, and the right methods are getting called.
                // Actually the properties Document and images do implement IHTMLDocument2 and
                // IHTMLElementCollection each (good for statically typed compile time binding
                // (e.g. w/ C++ and C#3 etc.)), and both implement IDispatch (good for dynamically
                // typed run time dispatching (e.g. w/ VB script and for the ComRuntimeBinder and
                // the DLR we use here)).
                // In effect all the IDispatchs returned from and passed to methods and properties
                // are silently handled as dynamics to let the call chain fly...
                // Indeed the interop idea was taken very far for DLR COM run time binders, e.g.
                // all dynamically dispatched calls are case-insensitive.
                dynamic length = ieApplication.Document.images.length;

                // Sidebar: Effectively we are using Internet Explorer's DOM here. If we use the
                // DOM in JavaScript being executed within the Internet Explorer, we use the very
                // same COM objects from JavaScript!

                if (0 < length)
                {
                    // More DLR work happens here, as image's property item awaits an index to be
                    // passed being of type VARIANT, we can just pass an int argument directly. The
                    // DLR handles all the VARIANT arguments and returned values as dynamics
                    // silently. The returned item is of type IDispatch again...
                    dynamic item = ieApplication.Document.images.item(1);

                    // Pitfall!
                    // In order to call Debug.WriteLine() we have to get rid of dynamic dispatch. -  
                    // Debug.WriteLine has a compile time custom attribute of type Conditional, it
                    // can not be called with a dynamic variable, because the expression tree could
                    // be broken due to the condition... Alas this problem will only show up as a
                    // warning on compilation (warning CS1974) and during run time a 
                    // RuntimeBinderException will be thrown: Cannot dynamically invoke method
                    // 'WriteLine' because it has a Conditional attribute.

                    // The href is of type BSTR - no problem for the CLR type string, the DLR will
                    // manage the conversion.
                    string href = item.href;
                    Debug.WriteLine(href);
                }
            }
            finally
            {
                if (null != ieApplication)
                {
                    // Case-insensitive call to the method Quit() - no problem for DLR's COM
                    // binder.
                    ieApplication.quit();
                }
            }

            // Release the COM objects by setting the reference to their runtime callable wrappers
            // (RCWs) to null, then the references are eligible for gc'ing. - When the finalizers
            // are called, the RCW's internal marshaling counter is decremented (please see the
            // next sidebar).
            ieApplication = null;

            // Sidebar: You could also call Marshal.ReleaseComObject(), this call will
            // deterministically decrement the RCW's internal marshaling counter (Not COM's
            // reference counter!). Normally the RCW holds a COM object with a COM reference
            // counter of 1, and when the RCW's internal marshaling count reaches 0 the COM
            // reference count will be decremented to 0, which finally leads to the COM object
            // deleting itself.

            #region With VB's Interaction class:
            // If you reference the assembly Microsoft.VisualBasic.dll and use the namespace
            // Microsoft.VisualBasic, you can call Interaction.CreateObject() directly. Then you
            // are forced to use dynamic typing to perform the COM automation like so:
            dynamic ieApplication2 = Interaction.CreateObject("internetexplorer.application");
            ieApplication2.Visible = true;
            ieApplication2.Navigate("www.avid.com");
            ieApplication2.Quit();
            ieApplication2 = null;
            #endregion


            #region Getting an object from the ROT:
            // It is also possible to get an existing object from the ROT and then to enable
            // dynamic dispatching on it:
            // (This isn't possible w/ IE9 at the time of this writing as it does not register
            // objects in the ROT.)
            // dynamic rotObj = System.Runtime.InteropServices.Marshal.GetActiveObject("<progid>");
            // rotObj.Method();
            #endregion


            /*-----------------------------------------------------------------------------------*/
            // Dynamic Dispatch and COM Automation with a User Defined COM Server:

            Type myComType = Type.GetTypeFromProgID("NicosComClass");
            if (null != myComType) 
            {
                dynamic myComObject = Activator.CreateInstance(myComType);
                dynamic data = myComObject.GetData();
                string resultText = data;
                Debug.Assert("Hello World!".Equals(resultText));
                System.Runtime.InteropServices.Marshal.FreeBSTR((IntPtr)data);
                myComObject = null;
            }
            else
            {
                Debug.WriteLine("Possibly you forgot to register AComServer.dll (available in this"
                    +" solution).");
            }
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:

            DynamicActivationAndAutomation();
        }
    }
}