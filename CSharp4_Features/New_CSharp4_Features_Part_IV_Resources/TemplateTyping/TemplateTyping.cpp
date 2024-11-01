#include "stdafx.h"

#include <string>
#include <vector>
#include <set>

// These examples show:
// - Template Typing and Consistency.

// In C++ we don't (and for the time being we can't) specify a special interface that T must obey
// to. Instead we just directly call the method size() on t, which is of type T. As you see we do
// at least _implicitly_ require T to provide following things:
// 1. T has to provide a method of name size(), returning a value (please read on).
// 2. T has to provide an embedded type with the name size_type.
// 3. Instances of size_type must be able to be created from the result of the method size().
// In the end the code is checked at compile time.
template <typename T> void TemplateTyping(const T& t)
{
    const typename T::size_type size(t.size());
    std::printf("%d\n", size);
}


// Here we still don't express direct constraints on T or F, but we could reduce the _implicit_
// constraints on T:
// 1. T has to provide an embedded type with the name size_type.
// Instead we moved some _implicit_ constraints to F:
// 1. F has to provide a function call operator accepting an instance of type T, and
// 2. that function call operator needs to return a type convertible to T::size_type.
// The trick is that we removed the requirement on T to provide a method size(). Instead we suppose
// the user to encapsulate that detail into the functor lengthProvide. - This yields the same
// freedom in C++ that we have in C# with delegates to enable high-order function calling and good
// and easy reusability. In C++0x lengthProvider can be passed as lambda, please see below.
// In the end the code is also checked at compile time.
template <typename T, typename F> void TemplateTypingWLambda(const T& t, F lengthProvider)
{
    const typename T::size_type size(lengthProvider(t));
    std::printf("%d\n", size);
}


// Sidebar: Putting the C++ template thing together, the constraints the compiler puts on the 
// template parameters can be very loosly. Then the compiler just tries to replace the template
// parameters by the actual types you've used in your call, and if the code within the template-
// body doesn't match the replaced arguments you get a compiler error. But the compiler error will
// indicate a error in the _replaced_ code, but you'll just see the template code. Maybe you don't
// know the actual "replacement" type, because it was inferred by the compiler. So the messages of
// the compiler, if anything is not correct with the usage of the templates are very mysterious and
// you'll need some experience to understand what's going on. Esp. if you use template code from
// other developers things get worse, because you have to inspect foreign template code (yes, you
// need to have the template code available, but in most C++ implementations the template code
// needs to be inline anyway).
// Well, the C++ standardization committee recognized this problem and presented following
// solutions:
// 1. Compile time static_assert and special meta-programming type traits: These assertions can be
//    used to check, whether the type you've passed to the template meets specific criteria.
//	  Such like: is_pod or is_numeric. The trick is that these assertions can be combined with a
//	  userdefined message, which will be shown by the compiler if the assertion was not met! So a
//    template programmer can explicitly express, what he wants the passed types to be able to
//    provide. And if the assertion was not met the user will get a clear assertion message from
//	  the compiler, having a clear text provided by the original template programmer.
//	  => This feature was originally founded in the boost library. And you can also find more
//	  information on the problems static_assert tries to address in the (recommendable) book
//	  "Modern C++ Design: Generic Programming and Design Patterns Applied" (by Andrei Alexandrescu,
//    PhD). Finally static_assert made it's way into the C++0x standard, and static_assert can be
//	  used in VS2010 as well (but the VS2010's C++ compiler doesn't cover the complete C++0x
//	  standard at the time of this writing (June, 2011)).
// 2. Concepts: The idea of concepts is to write down a protocol of features a type can provide.
//	  E.g. a type needs to provide certain operators, ctors, methods or embedded types or to even
//	  obey to certain rather complex axioms. So called concept_maps will even provide a way to 
//	  adapt different templated libraries (read: concepts of that libraries) together. Then within
//	  your template you only have to declare that you require the type parameter to meet a special
//	  concept (e.g. to provide the embedded type size_type and a method size() returning
//    size_type). But concepts didn't make it into the C++0x standard, because the committee
//	  couldn't agree upon details as well as the basic motivation behind concepts.


int _tmain(int argc, _TCHAR* argv[])
{    
    /*-------------------------------------------------------------------------------------------*/
    // Consistency with Template Typing:

    // In C++, types to be used with a template don't need to derive from a common base class (in a
    // sense of a .Net interface). - Rather it is sufficient that the types have "enough in common"
    // (e.g. provide the method size()) to be considered as consistent with the template. Keep in
    // mind, that the check of the consistency is still based on static typing, i.e. the compiler
    // will check the presence of method size(). The C++ STL is only based on this kind of static
    // typing.

    // Consistency with the direct type: it works with std::vector<std::string> and std::set<int>
    // directly:
    std::vector<std::string> strings;
    strings.push_back("Hello");
    strings.push_back("World");
    // Works for std::vector<std::string> (e.g. there barely exists
    // std::vector<std::string>::size(), so it's consistent).
    TemplateTyping(strings); 

    std::set<int> numbers;
    numbers.insert(21);
    numbers.insert(42);
    // Works for std::set<int> as well (e.g. there barely exists std::set<int>::size(), so it's
    // consistent)
    TemplateTyping(numbers); 

    // Consistency with the lightweight variant with lambdas as functors. Then the constraint of the
    // template type argument to have a size() method is encapsulated away (into the lambda), and
    // only visible on the caller's side (just neat how easy it is to express consistency here):
    TemplateTypingWLambda(strings, [](std::vector<std::string> v){ return v.size(); });
    TemplateTypingWLambda(numbers, [](std::set<int> s){ return s.size(); });

    return EXIT_SUCCESS;
}