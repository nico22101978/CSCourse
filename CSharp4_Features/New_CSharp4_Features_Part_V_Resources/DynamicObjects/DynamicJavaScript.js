// In JavaScript you _can not_ define own types! Instead you have to create an instance of
// JavaScript's complex reference type Object, which acts as an unordered list of key-value pairs,
// to express your kind of types as dynamic objects. You can add new key-value pairs by adding new
// dynamic properties, which extends the instance's interface depending on how you use it. - The
// dynamic or loosely typing of JavaScript makes the simple access to the values of such dynamic
// objects possible.


// Type augmenting: Adding of dynamic properties as we go:
// Create a new Object at the toplevel.
>var obj = new Object();
>
> obj.Test = "Text";
> obj.Test;
: "Text"
// Sidebar: The DOM storage (sessionStorage and localStorage) can be accessed and manipulated with
// properties (or with the bracket notation, see below). 
// Sidebar: This way of adding new properties to objects can be used in Internet Explorer's DOM
// manipulation to add new attributes to an element.
// Adding arrays:
>obj.names = ["Sandy", "Mandy"];
>obj.names;
: ["Sandy", "Mandy"]

// Adding of methods:
// (... as function literal assigned to a new property. - Think: a function is a named lambda.)
> obj.Fn = function () { return this.Test + " from function"; };
> obj.Fn();
: "Text from function"

// Accessing and inserting dynamic properties via the bracket notation:
> obj["Test"];
: "Text"
> obj["Fn"];
: function () { return this.Test + " from function"; };
// We can also add new properties with the bracket notation:
> obj["other"] = "--";
: "--"
// In fact the bracket notation is more general than the period notation. - This what we call
// key-value coding. The application of the bracket notation does also enable us to define
// properties having a name that is otherwise illegal in JavaScript (e.g. having an
// inner whitespace):
> obj["yet another"] = "--";
: "--"
// Hint: Sometimes the bracket notation can be used to access and modify members that have an
// automatically generated name.

// Checking the existance of a member. This feature is required, because in a dynamically typed
// language you need a way to "re-discover" possibly present properties:
> "Test" in obj
: true

// Enumerating all members:
> for(var name in obj){alert(name);}
: ("Test")
: ("Fn")

// Deleting a member:
> delete obj.Fn;

// Again enumerating all members:
> for(var name in obj){alert(name);}
: ("Test")

// new Object expressed with an object literal:
> obj = {};

// Object literals allow creating objects with properties on the fly:
> obj = { Name: "Nico", Age: 33};

// Also nested object literals are allowed:
> obj = { Name: "Nico", Age: 33, Friend: {Name: "Jane", Age: 32}};

// A JavaScript object can also be stored (or serialized) to a string expressed with the JavaScript
// Object Notation (JSON): 
> var jsonString = '{ "Name": "Nico", "Age": 33, "Friend": {"Name": "Jane", "Age": 32}}';
// Sidebar: In opposite to object literals, you need to write property names in quotes in JSON. This
// is required, because in JSON we can also define properties, which are invalid JavaScript symbols,
// e.g. such containing a space like so:
> var jsonString2 = '{ "My Name": "Nico", "My Age": 33}';
// The properties 'My Name' and 'My Age' can only be used via the bracket notation (the dot notation
// is not allowed).

// Sidebar: JSON is a lighweight alternative to XML in AJAX patterns, because JSON can be processed
// much faster than XML. - E.g. no XML parser needs to be created, JSON is directly supported by
// JavaScript.

// The JavaScript function eval() can "rehydrate" the JSON string into an object:
> var obj2 = eval("("+jsonString+")");

// The usage of eval() introduces possible security problems, because eval() allows to execute
// _any_ executable JavaScript code. So you need to either check the argument before passing it to
// eval(), or you should use the special object "JSON" and its method "parse()", which checks the
// argument before evaluating it. The object "JSON" is officially part of ECMAScript 3.1.

// The special object JSON can be used to convert an object into JSON (and vice versa):
> JSON.stringify(obj2);

: '{"Name":"Nico","Age":33,"Friend":{"Name":"Jane","Age":32}}'