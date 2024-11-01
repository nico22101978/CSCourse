open System

// This example shows:
// - A recursive F# algorithm that generates a powerset (i.e. the set of all subsets) of a given
//   set.

let rec powerset = function
    | [] -> [[]]
    | first::rest -> [for subset in powerset rest do
                        yield! [subset; first::subset]]

// The main function:
let main = 
    let mutable nSubset = 0
    for subset in powerset [1 .. 3] do
        printf "["
        nSubset <- nSubset + 1
        for item in subset do
            printf "%d;" item
        printfn (if subset.IsEmpty then "]" else "\b]")
    nSubset

// Call main:
printf "%d" main
printf "end"