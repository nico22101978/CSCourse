// This example shows:
// - A _non-recursive_ Scala algorithm that generates a powerset (i.e. the set of all subsets) of a
// given set. In Scala this can be performed with so called _folding_.

def powerset[T](inputSet : List[T]) =
    inputSet.foldLeft(List(List.empty[T])){
      (set, element) =>
        set.union(set.map(element +: _))
    }

for (subset <- powerset(List(1, 2, 3)))
{
    println("Set: " + subset);
}