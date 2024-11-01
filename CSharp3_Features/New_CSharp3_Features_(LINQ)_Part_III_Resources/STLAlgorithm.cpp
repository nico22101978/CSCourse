#include "stdafx.h"

#include <iostream>
#include <vector>
#include <algorithm>
#include <random>
#include <functional>
#include <iterator>

// Test

// This example shows how an algorithm operating on data can be expressed in C++0x (TR1) with STL.
// (see <http://msdn.microsoft.com/en-us/library/dd465215(VS.100).aspx>).
// - type inference (the auto keyword)
// - lambda expressions
// - the new <random> standard functions
int _tmain(int argc, _TCHAR* argv[])
{
	// A vector of size ten as container.
	std::vector<int> list(10);
	
	// Create a generator for random (fixed seed mersenne twister) ints between one and nine.
	const std::uniform_int_distribution<int> distribution(1, 9);
	const std::mt19937 engine;
	auto generator(std::bind(distribution, engine));

	// Fill the container with ten random ints.
	std::generate(list.begin(), list.end(), [&generator](){return generator();});

	// Sort the container, this operation is needed to prepare the container for the call of
	// std::unique().
	std::sort(list.begin(), list.end());

	// Call std::unique(); this will modify the arrangement of the items in the container, but not
	// the size of the container. The range of unique values in the result reaches from
	// list.begin() to the iterator returned from std::unique(), so we've to remember to store the
	// result of std::unique() into a variable!
	const auto newEnd(std::unique(list.begin(), list.end()));
	
	// To filter the even values use the std::partition() function. No! - We have to use
	// std::stable_partition() in order to maintain the relative order! Additionally we have to
	// remember to pass the new end iterator of the unique range, not the whole containers's range!
	// As a result we'll get the new end iterator of the range with filtered values. - As
	// std::unique() does, std::stable_partition() does only rearrange the values of the passed
	// sequence!
	const auto newEnd2(std::stable_partition(list.begin(), newEnd,
		[](int item){return 0 == item % 2;}));

	// Finally we can output the unique and filtered values to the console. We have to remember to
	// pass the new end iterator in order to only output the range with the filtered values.
	std::for_each(list.begin(), newEnd2, [](int item){std::cout<<item<<std::endl;});

	// Phew!
	// That was a lot of work, because although the C++ syntax has been streamlined, it's
	// needed to perform many independent and error prone operations to get the result. But, on
	// the other hand, this code doesn't use any loop or explicit type or function declaration.
	// C#3 provides another solution: extension methods and method chaining!
	return EXIT_SUCCESS;
}
