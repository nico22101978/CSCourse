#include "stdafx.h"

#include <cstdlib>
#include <iterator>
#include <vector>
#include <iostream>
#include <type_traits>

// This example shows:
// - A recursive C++/STL algorithm that generates a powerset (i.e. the set of all subsets) of a
//   given set.

//  A Lisp implementation of the subset's algorithm (this implementation gets the full powerset):
// (defun powerset (l)
//  (if (null l)
//   '(nil)
//   (let ((ps (powerset (cdr l))))
//    (append ps (mapcar #'(lambda (x) (cons (car l) x)) ps)))))
//
// The core implementation of the subset algorithm.
template<typename ResultContainerType,
			typename IteratorType,/*Different Iter types, cause ResultContainerType can be const*/
			typename ResultIteratorType>
void subsetsCore(std::insert_iterator<ResultContainerType>& destination,
					IteratorType remainderSequenceBegin, IteratorType remainderSequenceEnd,				
					ResultIteratorType inputSequenceBegin, ResultIteratorType inputSequenceEnd)
{
	typedef typename ResultContainerType::value_type InputContainerType;
	typedef typename std::iterator_traits<IteratorType>::difference_type Difftype;
	
	static_assert(std::is_same<typename std::iterator_traits<IteratorType>::iterator_category,
		std::random_access_iterator_tag>::value,
		"IteratorType's iterator_category must be random!"); 
	static_assert(std::is_integral<Difftype>::value,
		"IteratorType's difference_type must be integral!");

	if(static_cast<Difftype>(0) == std::distance(remainderSequenceBegin, remainderSequenceEnd))
	{
		destination = InputContainerType(inputSequenceBegin, inputSequenceEnd);
		++destination;
	}
	else
	{
		InputContainerType input(inputSequenceBegin, inputSequenceEnd);
		input.insert(input.end(), remainderSequenceBegin, remainderSequenceBegin + 1);
			
		subsetsCore(destination, remainderSequenceBegin + 1, remainderSequenceEnd, input.begin(),
			input.end());
		subsetsCore(destination, remainderSequenceBegin + 1, remainderSequenceEnd,
			inputSequenceBegin, inputSequenceEnd);
	}
}
	
	
// This C++ algorithm calculates all the subsequences of the passed sequence from
// inputSequenceBegin to inputSequenceEnd. The resulting subsequences will be stored into the
// destination via an insert_iterator.
template<typename ResultContainerType, typename IteratorType>
void subsets(std::insert_iterator<ResultContainerType> destination, 
	IteratorType inputSequenceBegin, IteratorType inputSequenceEnd)
{
	typename ResultContainerType::value_type emptySequence;
	subsetsCore(destination, inputSequenceBegin, inputSequenceEnd, emptySequence.cbegin(),
		emptySequence.cend());
}


int _tmain(int argc, _TCHAR* argv[])
{
	//// Subsets:	
	// With std::vector<int>:
	typedef std::vector<int> InputContainerType;
	typedef std::vector<InputContainerType> ResultContainerType;
	ResultContainerType result;
	InputContainerType inputToGetSubsets;
	inputToGetSubsets.push_back(0);
	inputToGetSubsets.push_back(1);
	inputToGetSubsets.push_back(2);
	
	subsets(std::inserter(result, result.begin()), inputToGetSubsets.cbegin(),
		inputToGetSubsets.cend());
	const auto end(result.cend());
	for(auto iter(result.cbegin()); iter < end; ++iter)
	{
		const auto innerend(iter->cend());
		for(auto inneriter(iter->cbegin()); inneriter < innerend; ++inneriter)
		{
			std::cout<<*inneriter<<' ';
		}
		std::cout<<std::endl;
	}

	return EXIT_SUCCESS;
}