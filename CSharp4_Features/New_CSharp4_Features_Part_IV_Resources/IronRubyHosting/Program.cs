using System;
using System.Linq;

// IronRuby 1.1.3 can be installed as NuGet package for the project.
using IronRuby;
using IronRuby.Hosting;
using Microsoft.Scripting.Hosting;

using System.Dynamic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Security.Permissions;
using System.Security;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


// These examples show:
// - Simple hosting of IronRuby from C#.
// - Interop between IronRuby and C#.
// - IronRuby hosting within an isolated AppDomain.
// (Generally all these examples should work with IronPython as well.) 
namespace IronRubyHosting
{
    public class Program
    {
        private static void ExecuteIronRubyFromCSharp()
        {
            // Here we get the script engine of IronRuby, we'll use it in all the following
            // examples:
            ScriptEngine rubyEngine = Ruby.CreateEngine();


            /*-----------------------------------------------------------------------------------*/
            // Executing IronRuby from C# (IronRuby Objects and C# Objects as Peers):

            /// IronRuby Expressions:
            // Executing a single Ruby expression: the result is of type dynamic.
            dynamic result = rubyEngine.Execute("2 + 4");
            // This dynamic result can be converted into an .Net Int32 directly (it is a Ruby
            // Fixnum).
            int result2 = rubyEngine.Execute("2 + 4");
            // Via the generic version of Execute() we can directly retrieve an Int32 (w/o
            // conversion).
            int result3 = rubyEngine.Execute<int>("2 + 4");

            /// How IronRuby projects native Ruby containers to .Net Collections:
            // IronRuby.Builtins.RubyArray implements IList<object>:
            dynamic anArray = rubyEngine.Execute("[1, 2, 3]");
            IList<object> underlyingList = anArray;
            // IronRuby.Builtins.RubyArray implements IDictionary<object, object>:
            dynamic aHash = rubyEngine.Execute("{1 => '1', 2 => '2', 3 => '3'}");
            IDictionary<object, object> underlyingDictionary = aHash;

            /// IronRuby Code Fragments:
            // (In Ruby expressions and statements are both expressions, so it doesn't matter, if
            // we execute a single expression or a set of statements.)
            // Here we create a Ruby class, return it to the C# host, create an instance of that
            // class and call a method on that object on the host's side.
            const string rubyClassDefinition =
                @"class RubyClass
                        def do_it
                            'Hello world'
                        end
                  end";
            #region Alternative to convert Ruby string to CLR string in Ruby code:
            const string rubyClassDefinition2 =
                @"class RubyClass
                        def do_it
                            'Hello world'.to_clr_string()
                        end
                  end";
            #endregion
            // The execution of the code fragment contained in rubyClassDefinition will create a
            // new ruby class 'RubyClass'.
            rubyEngine.Execute(rubyClassDefinition);
            // Get a reference to RubyClass' definition (in Ruby a class is itself an object).
            dynamic rubyClass = rubyEngine.Runtime.Globals.GetVariable("RubyClass");
            // Create an instance of RubyClass.
            dynamic rubyObject = rubyEngine.Operations.CreateInstance(rubyClass);
            // Calling the method do_it() on the created instance. The Ruby run time binder _is_
            // case sensitive, so we need to call do_it() exactly! We have to cast the result to a
            // string, because do_it() will return a Ruby string, a type different from a CLR
            // string. Alternatively we could have typed resultOfMethod as dynamic or we could have
            // called the method to_clr_string() to convert the Ruby string to a CLR string (no
            // cast to string would be required then).
            string resultOfMethod = (string)rubyObject.do_it();
            Debug.Assert("Hello world".Equals(resultOfMethod));

            /// Call a single Method:
            const string aRubyMethod =
                @"def say_hello(name)
                    'Hello '+name
                  end";
            // In order to access the symbols defined in a IronRuby script, we need to create a 
            // ScriptScope. Then we pass this ScriptScope to the Engine's Execute() method along
            // with the Ruby code.
            ScriptScope scope1 = rubyEngine.CreateScope();
            rubyEngine.Execute(aRubyMethod, scope1);
            // So get the method from the ScriptScope as a variable. The returned object enables
            // us to call the "Invoke()" dynamic operation by the usage of dynamic dispatch. (Note
            // that no .Net Delegate instance will be returned or called here.)
            dynamic rubyMethod = scope1.GetVariable("say_hello");
            string answer = (string)rubyMethod("Trish");
            Debug.Assert("Hello Trish".Equals(answer));

            /// Call IronRuby Script:
            // Just load the IronRuby script into a ScriptSource and execute it.
            ScriptSource scriptSource = rubyEngine.CreateScriptSourceFromFile("ARubyScript.rb");
            dynamic resultFromRubyScript = scriptSource.Execute();
            string resultAsString = resultFromRubyScript.ToString();
            Debug.WriteLine(resultAsString);

            /// Call a compiled variant of a Script:
            // Compile() returns a compiled (Expression tree) form of the referenced script. The
            // idea is to precompile repeatedly called scriptcode:
            CompiledCode compiledCode = scriptSource.Compile();
            for (int i = 0; i < 10; ++i)
            {
                Debug.WriteLine(compiledCode.Execute<DateTime>());
            }
        }


        #region Types for IronRubyAndCSharpInterop():
        public class NamedCSharpType
        {
            public string Text { get; set; }


            public int Count { get; set; }


            public string CountAndText()
            {
                return Text + Count;
            }
        }
        #endregion


        private static void IronRubyAndCSharpInterop()
        {
            // Here we get the script engine of IronRuby, we'll use it in all the following
            // examples:
            ScriptEngine rubyEngine = Ruby.CreateEngine();


            /*-----------------------------------------------------------------------------------*/
            // Expose CLR Objects to IronRuby:

            // We also need a ScriptScope to pass objects to the IronRuby code as well as get
            // objects from the IronRuby code:
            ScriptScope scope = rubyEngine.CreateScope();
            // Here we expose a C# class to IronRuby as ExpandoObjects.
            // Interoperability when hosting a DLR scripting language: Expose your C# classes to
            // IronRuby as ExpandoObjects!

            /// Pass a named C# type's instance to a IronRuby Script:
            NamedCSharpType namedCSharpTypeInstance =
                new NamedCSharpType { Text = "Text1", Count = 1 };
            // Set the created instance as variable named_csharp_type_instance into the
            // ScriptScope.
            scope.SetVariable("named_csharp_type_instance", namedCSharpTypeInstance);
            // Execute the IronRuby script: named_csharp_type_instance's properties can be easily
            // evaluated as well as _modified_ from IronRuby.  
            rubyEngine.Execute(@"# Reading the CLR instance:
                                named_csharp_type_instance.count.times do
                                    puts 'Text: '+named_csharp_type_instance.Text
                                    puts named_csharp_type_instance.count_and_text
                                end
                                # Modifying the CLR instance:
                                named_csharp_type_instance.Text = 'Text changed'",
                               scope);
            // Sidebar: As we can see the type NamedCSharpType also contains the method
            // CountAndText(). In IronRuby we can call a CLR type's methods like this, i.e. with
            // pascal case (mind, that Ruby is a case-sensitive language), or we can call that CLR
            // methods with the lower-case-underscore-notation, count_and_text in this case. The
            // idea behind this ability is that the underscore-notation is Ruby's conventional
            // notation and conventions are important to be obeyed to in Ruby.

            // Sidebar: CLR exceptions, which emerge in the IronRuby code, are directly serialized
            // to the caller of Execute(). IronRuby exceptions, i.e. such, which are risen with the
            // "raise" expression, are thrown as IronRuby.Builtins.RuntimeError to the caller of
            // Execute().

            string newText = scope.GetVariable<NamedCSharpType>("named_csharp_type_instance").Text;
            Debug.Assert("Text changed".Equals(newText));

            /// Pass an anonymous C# type's instance to a IronRuby Script:
            var anonymousCSharpTypeInstance = new { Text = "Text2", Count = 2 };
            // Add the created instance as variable anonymousCSharpTypeInstance into the
            // ScriptScope.
            scope.SetVariable("anonymous_csharp_type_instance", anonymousCSharpTypeInstance);
            // Execute the IronRuby script: anonymous_csharp_type_instance's properties can be
            // easily evaluated from IronRuby. The modification of properties of anonymous types
            // is not allowed (there is simply no "setter").
            rubyEngine.Execute(@"# Reading the CLR instance:
                                anonymous_csharp_type_instance.Count.times do
                                    puts 'Text: '+anonymous_csharp_type_instance.Text
                                end
                                # Modifying the properties of an anonymous type is invalid:
                                #anonymous_csharp_type_instance.Text = 'Text changed'",
                               scope);

            // Since ScriptScope is itself a dynamic type, we can access and modify the scope's
            // variables via dynamic dispatch. Just assign the ScriptScope to a dynamic variable,
            // then we can write:
            dynamic scope2 = scope;
            dynamic lastObject = scope2.anonymous_csharp_type_instance;
            string lastText = lastObject.Text;
            Debug.Assert("Text2".Equals(lastText));

            /// Pass an ExpandoObject to an IronRuby Script:
            dynamic dynamicInstance = new ExpandoObject();
            dynamicInstance.Text = "Text3";
            dynamicInstance.Count = 3;
            // Set the created instance as variable dynamic_instance into the ScriptScope.
            scope.SetVariable("dynamic_instance", dynamicInstance);
            // Execute the IronRuby script: dynamic_instance's properties can be easily evaluated
            // as well as _modified_ from IronRuby.  
            rubyEngine.Execute(@"dynamic_instance.Count.times do
                                    puts 'Text: '+dynamic_instance.Text
                                end
                                # Modifying the ExpandoObject instance:
                                dynamic_instance.Text = 'Text changed'
                                # Add a new property to the passed ExpandoObject instance:
                                dynamic_instance.Number = 42",
                               scope);
            // Here it is ok to retrieve the variable as dynamic and call Text directly, rather
            // than static typing to ExpandoObject:
            string modifiedText = (string)scope.GetVariable("dynamic_instance").Text;
            int addedInt = (int)scope.GetVariable("dynamic_instance").Number;

            Debug.Assert("Text changed".Equals(modifiedText));
            Debug.Assert(42 == addedInt);
        }



        private static void MoreOnExecutionAndHosting()
        {
            /*-----------------------------------------------------------------------------------*/
            // Compiled and interpreted Mode:

            // The IronRuby ScriptEngine does by default run in compiler mode. In that mode source
            // code gets loaded and compiled to IL when respective methods are executed. In the
            // interpreted mode IronRuby expressions get translated into DLR trees and then
            // directly executed. So in the interpreted mode the startup time is better, but the
            // run time is worse.
            // The interpreted mode can be set like this:
            ScriptEngine rubyEngineForInterpretedMode =
                Ruby.CreateEngine(setup => { setup.Options["InterpretedMode"] = true; });


            /*-----------------------------------------------------------------------------------*/
            // Host IronRuby within an own AppDomain with specially configured CAS:

            // Create a PermissionSet to declare "read-only" permission for all drives with Code
            // Access Security (CAS); the to-be-created AppDomain can be understood as a sandbox:
            PermissionSet permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            foreach (string drive in Directory.GetLogicalDrives())
            {
                permissionSet.AddPermission(
                    new FileIOPermission(FileIOPermissionAccess.Read, drive));
            }

            // Create a new AppDomain to host the ScriptRuntime, initialized with the restricting
            // permission (see above):
            AppDomain appDomain =
                AppDomain.CreateDomain("ScriptDomain",
                    null,
                    AppDomain.CurrentDomain.SetupInformation,
                    permissionSet);
            ScriptRuntimeSetup scriptRuntimeSetup = new ScriptRuntimeSetup();
            scriptRuntimeSetup.LanguageSetups.Add(
                new LanguageSetup(
                    "IronRuby.Runtime.RubyContext, IronRuby",
                    "IronRuby",
                    new[] { "IronRuby", "Ruby", "rb" },
                    new[] { ".rb" }));

            // Sidebar: The relevant DLR scripting types inherit MarshalByRefObject. This enables
            // .Net Remoting, so we can access instances of those types from different AppDomains.
            // .Net Remoting also enables hosting a ScriptRuntime in separate processes or even in
            // separate physical machines (these hosting variants are not shown in this example).

            // Create a new ScriptRuntime in the realm of the recently created AppDomain
            // "ScriptDomain". The returned object is this AppDomain's proxy of the ScriptRuntime
            // object in the AppDomain "ScriptDomain":
            ScriptRuntime scriptRuntime =
                ScriptRuntime.CreateRemote(appDomain, scriptRuntimeSetup);
            // Get the Ruby engine's proxy:
            ScriptEngine rubyEngine = scriptRuntime.GetEngine("Ruby");
            // Execute a Ruby script and get a proxy to the ScriptScope. The execution of this
            // script would throw a SecurityException if its code tried to write a file on
            // drive C:
            ScriptScope scope = rubyEngine.ExecuteFile("ARubyScript.rb");

            // A remoting object handle:
            ObjectHandle helloFunction = scope.GetVariableHandle("say_hello");
            ObjectHandle resultHandle =
                rubyEngine.Operations.Invoke(helloFunction, new[] { "Leslie" });
            // Attention! Here we have to use the Unwrap<T>() method of the Ruby ScriptEngine,
            // because the wrapped _mutable Ruby string_ needs to be converted into a CLR string.
            string result = rubyEngine.Operations.Unwrap<string>(resultHandle);
            Debug.Assert("Hello Leslie".Equals(result));
        }


        public static void Main(string[] args)
        {
            /*-----------------------------------------------------------------------------------*/
            // Calling the Example Methods:


            ExecuteIronRubyFromCSharp();


            IronRubyAndCSharpInterop();


            MoreOnExecutionAndHosting();
        }
    }
}