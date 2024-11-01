//
//  main.m
//  PropertyBag
//
//  Created by Nicolay Ludwig on 19.06.11.
//

#import <Cocoa/Cocoa.h>

#import "PropertyBag.h"

int main(int argc, char *argv[])
{		
    //----------------------------------------------------------------------------------------------
    // Obj-C's default Behavior on Calling unknown Methods/Properties:
    
    // Let's check Obj-C's default behavior, if we try to invoke a method/property that does not
    // exist on an object's type. 
	id anObject = [NSObject new];
    NSLog(@"anObject created: %@", anObject);
	
	// If we execute following line Obj-C's message forwarding will call doesNotRecognizeSelector:,
    // which throws an NSInvalidArgumentException.
	//[anObject aProperty:@"text"];
    [anObject release];
	
    
    //----------------------------------------------------------------------------------------------
    // Now the User defined PropertyBag Type with dynamic Dispatching:

    id propertyBag = [PropertyBag new];
    
    id pool = [NSAutoreleasePool new];
    // Set a number as new property and then get this number and write it to the console:
    [propertyBag numberProperty:@42];
    NSLog(@"Here the stored number: %@", [propertyBag numberProperty]);
    
    // As you can see the type of the formerly added property "numberProperty" is not fixed to
    // NSNumber. It could be set to an object of type NSString (@"text" in this case):
    [propertyBag numberProperty:@"text"];
    NSLog(@"Here the stored text: %@", [propertyBag numberProperty]);
    
    // But in such a case we are better off introducing a new property like so:
    [propertyBag textProperty:@"another text"];
    NSLog(@"Here the stored text: %@", [propertyBag textProperty]);

    // But such a method having multiple parameters will be not understood by our object, because it
    // is neither recognized as property setter nor as property getter. If we execute this line
    // Obj-C's message forwarding will call doesNotRecognizeSelector:, which throws an
    // NSInvalidArgumentException.
    //[propertyBag anotherMethod: Nil with: @"that"];
    [pool drain];
    
    [propertyBag release];
    
    return NSApplicationMain(argc,  (const char **) argv);
}
