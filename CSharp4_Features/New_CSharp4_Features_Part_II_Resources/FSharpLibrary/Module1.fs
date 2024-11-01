module Module1

/// Simply returns a tuple of the passed two parameters.
let AsTuple a b =
    (a, b)


/// Accepts a tuple of numbers and returns their product.
let AcceptTuple (a, b) =
    a * b


/// Simply returns a tuple of the passed eleven parameters.
let AsLargeTuple a b c d e f g h i j k =
    (a, b, c, d, e, f, g, h, i, j, k)