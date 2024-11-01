//
//  MissingMethods.m
//  MissingMethods
//
//  Created by Nicolay Ludwig on 23.07.11.
//

#import "PropertyBag.h"


@implementation PropertyBag
// Initializes the property bag.
-(id) init
{
	if(self = [super init])
	{
		properties_ = [NSMutableDictionary new];
	}
	return self;
}


// This method handles the "getting" of property values. The method forwardInvocation: will forward
// the unknown method call to this method, if it has no parameters.
-(id) getProperty:(NSString*)propertyName
{
	NSLog(@"I can handle getProperty '%@'.", propertyName);
	
	return properties_[propertyName];
}


// This method handles the "setting" of property values. The method forwardInvocation: will forward
// the unknown method call to this method, if it has exactly one parameter.
-(void) setProperty:(NSString*)propertyName value:(id)propertyValue
{
	NSLog(@"I can handle setProperty '%@' value: '%@'.", propertyName, propertyValue);
	
	properties_[propertyName] = propertyValue;
}


// 1. methodSignatureForSelector:(SEL) will be called. It decides, whether for the passed selector
//    a compatible method is present (then a signature object to call that method on self will be
//    created and returned), or if the method can not be handled (then nil will be returned). But
//	  here we overwrote this behavior; if self can not handle the passed selector with a method, we
//    tell the runtime that we would forward an invocation of this selector to another method.
//    Messages to unknown methods w/o parameter will be handled by getProperty:, such to methods
//    having exactly one parameter will be handles by setProperty:value:.
- (NSMethodSignature*) methodSignatureForSelector:(SEL)aSelector
{	
	if([self respondsToSelector:aSelector])
	{// If self can handle the passed message: return the matching signature.
		 return [[self class] instanceMethodSignatureForSelector:aSelector];
	}
	else
	{
		NSString* selectorToAnalyze = NSStringFromSelector(aSelector);
		NSArray* selectorComponents = [selectorToAnalyze componentsSeparatedByString:@":"];
		if (1 == [selectorComponents count]) 
		{// No parameter: interpret as getter.
			return [[self class]
				instanceMethodSignatureForSelector:NSSelectorFromString(@"getProperty:")];
		}
		else if(2 == [selectorComponents count]) 
		{// One parameter: interpret as setter.
			return [[self class]
				instanceMethodSignatureForSelector:NSSelectorFromString(@"setProperty:value:")];
		}
		
		// Other messages can not be handled: ask the super class.
		return [[super class] methodSignatureForSelector:aSelector];
	}
}


// 2. forwardInvocation:(NSInvocation*) will be called. Try to forward 
- (void) forwardInvocation:(NSInvocation*)invocation
{
	const int nArguments = [[invocation methodSignature] numberOfArguments];
	
	// We can only handle invocations with three or four arguments. The arguments zero and one
	// represent self (id) and _cmd (SEL), with argument three the real arguments start.
	if(3 == nArguments || 4 == nArguments)
	{
		// The argument two is the value to be set for the property, it will be safed here.
		id propertyValue;
		[invocation getArgument:&propertyValue atIndex:2];
		
		// The name of the property; it is the first part of the invocations selector.
		NSString* propertyName =
			[[NSStringFromSelector(invocation.selector)
				componentsSeparatedByString:@":"]
					objectAtIndex:0];
		
		// In order to reuse the invocation object, we set the argument two to the name of the
		// property.
		[invocation setArgument:&propertyName atIndex:2];
		// Forward to getter or setter depending on the count of arguments.
		if(3 == nArguments)
		{
			[invocation setSelector:NSSelectorFromString(@"getProperty:")];
		}
		else if(4 == nArguments)
		{
			// For the setter we have to set argument three to the property value.
			[invocation setArgument:&propertyValue atIndex:3];
			[invocation setSelector:NSSelectorFromString(@"setProperty:value:")];
		}
		[invocation invokeWithTarget:self];
	}
}
@end
