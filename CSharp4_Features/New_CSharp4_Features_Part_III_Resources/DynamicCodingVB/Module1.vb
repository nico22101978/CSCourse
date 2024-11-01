Option Strict Off
Option Infer Off

Imports System.Windows.Forms
Imports System.Threading


Module Module1

    Sub DynamicTypingVB()
        '------------------------------------------------------------------------------------------
        ' Dynamic Programming in VB:

        ' Here we create an object of type Form and assign it to the reference obj of type object:
        Dim obj = New Form()
        ' Interestingly we can _directly_ call any of Form's member directly through a reference of
        ' type object - downcasting to Form is not required:
        obj.AutoScroll = True

        ' Later we assign to the reference obj an object of type string:
        obj = New String("x"c, 10)
        ' And we can still _directly_ call all the members of type string through _the same_
        ' reference of type object:
        Dim result = obj.Split("c"c)

        ' 1. Aspect of dynamic programming: dynamic typing
        ' We can assign different objects of completely unrelated type to the same variable. In
        ' past versions of VB, e.g. in VB 6 this ability was based on the VB- and COM-type VARIANT.
        ' But VARIANT is best explained to be an example of weak typing rather than of dynamic
        ' typing.

        ' Sidbar: The feature, which we discuss here as dynamic typing is also known as loosely
        ' typing. - The main idea is that a dynamically typed variable is just a named placeholder
        ' for a value.

        ' 2. Aspect of dynamic programming: dynamic dispatching/late binding
        ' This interesting behavior, you can directly call the methods of the dynamic type through
        ' a reference of static type, is often called _late binding_.
        ' There is more! Late binding in VB is a key enabler to working with COM in a very simple
        ' manner. - It works so simple with VB that most VB programmers didn't even know, that they
        ' work with COM. Some people even state that VB 6 was only invented to act as a wrapper to
        ' work with COM in a more convenient way. The COM interface IDispatch is the deep key
        ' enabler of dynamic dispatch in VB.

        ' Sidebar: In VB 10 late binding needs to be explicitly enabled with the VB statements 
        ' "Option Strict Off" and "Option Infer Off". Up to VB 8 late binding was the default
        ' option.

        ' Sidebar: VB (esp. VB 6) is often considered to be more productive than other languages on
        ' the windows platform. This opinion was backed by VB 6's excellent RAD IDE, incl. forms-
        ' designer and the fact that VB does a lot for the programmer. And "doing a lot for a
        ' programmer" means, that not the most efficient code is generated in all cases. - This is
        ' esp. true when having Option Strict Off, as more checks need to be done during run time,
        ' which results in slower execution time. But in the end there is no notable difference on
        ' the execution time between e.g. VB and C#.
    End Sub


#Region "Types for DuckTypingVB():"
    Class Duck
        Public Property Name As String


        Public Sub New(ByVal theName As String)
            Name = theName
        End Sub


        Sub Quack()
            System.Diagnostics.Debug.WriteLine(
                String.Format("I'm a Duck, {0} - quack quack quack", Name))
        End Sub
    End Class
#End Region


    Sub DuckTypingVB()
        '------------------------------------------------------------------------------------------
        ' Duck Typing in VB:

        ' Creating an array with objects of completely unrelated types, some of the objects are
        ' Ducks, others not:
        Dim someObjects = {New Duck("Alfred J. Kwak"), 42, New Duck("Akka"), "Hello"}

        ' Now we try calling the method Quack() on each of these objects. We consider the method
        ' Quack() being a part of the public interface, i.e. we trust this convention and we are
        ' not interested and not dependent on how the type is virtually configured. - And if the
        ' convention is not meet, we can handle this as a run time error.
        For Each item As Object In someObjects
            Try
                item.Quack()
            Catch ex As MissingMemberException
                System.Diagnostics.Debug.WriteLine(String.Format("{0} is no Duck", item))
            End Try
        Next

        ' 3. Aspect of dynamic programming: duck typing or "convention over configuration"
        ' The idea of duck typing is simple: a type is only defined by the presence of a certain
        ' interface, i.e. certain methods.
        ' The term "duck typing" was coined by James Whitcomb Riley, he said:
        ' "When I see a bird that walks like a duck and swims like a duck and quacks like a duck,
        ' I call that bird a duck.“ 
    End Sub


    Sub ComInteropVB()
        ' -----------------------------------------------------------------------------------------
        ' This VB Code just opens the Internet Explorer on a Website and analyses its Contents:

        ' Mind, that no creation of PIAs (for the "Microsoft Internet Controls" in case of the
        ' Internet Explorer) is required: you do not need to deal with any static or generated
        ' types. - This reduces and simplifies the required code dramatically!


        Dim ieApplication As Object = Nothing

        Try
            ' Create the COM instance and stick ie's automation object into a variable to enable
            ' VB's magic (i.e. dynamic dispatch). - Then the variable can be used from VB to
            ' automate the ie.
            ieApplication = CreateObject("internetexplorer.application")
            ieApplication.Visible = True
            ieApplication.Navigate("www.avid.com")

            While (ieApplication.Busy)
                Thread.Sleep(500)
            End While

            ' In effect all returned and passed to objects implement COM's IDispatch interface
            ' and these objects are silently handled as dynamics to let the call chain fly...
            Dim length = ieApplication.Document.images.length

            ' Sidebar: Effectively we are using Internet Explorer's DOM here. If we use the DOM in
            ' JavaScript being executed within the Internet Explorer, we use the very same COM
            ' objects from JavaScript!
            If (0 < length) Then
                ' More VB's magic happens here, as image's property item awaits an index to be
                ' passed of type VARIANT, we can just pass an int argument directly. The DLR
                ' handles all the VARIANT arguments and returned values as dynamics silently.
                ' The returned item is of type IDispatch again...
                Dim item = ieApplication.Document.images.item(1)
                Dim href As String = item.href

                Debug.WriteLine(href)
            End If
        Finally
            If Not IsNothing(ieApplication) Then
                ieApplication.Quit()
            End If
        End Try
        ' Release the COM objects by setting the reference to their runtime callable wrappers
        ' (RCWs) to Nothing, then the references are eligible for gc'ing. - When the finalizers are
        ' called, the RCW's internal marshalling counter is decremented (please see the next
        ' sidebar).
        ieApplication = Nothing

        ' Sidebar: You could also call Marshal.ReleaseComObject(), this call will deterministically
        ' decrement the RCW's internal marshalling counter (Not COM's reference counter!). Normally
        ' the RCW holds a COM object with a COM reference counter of 1, and when the RCW's internal
        ' marshalling count reaches 0 the COM reference count will be decremented to 0, which
        ' finally leads to the COM object deleting itself.
    End Sub


    Sub Main()
        '------------------------------------------------------------------------------------------
        ' Calling the Example Methods:

        DynamicTypingVB()


        DuckTypingVB()


        ComInteropVB()
    End Sub
End Module
