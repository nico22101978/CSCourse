//
//  MissingMethods.h
//  MissingMethods
//
//  Created by Nicolay Ludwig on 11.02.12.
//

#import <Cocoa/Cocoa.h>

@interface PropertyBag : NSObject
{
	NSMutableDictionary* properties_;
}

- (NSString*) getProperty:(NSString*)propertyName;
- (void) setProperty:(NSString*)propertyName value:(id)propertyValue;
- (void) forwardInvocation:(NSInvocation *)anInvocation;
- (NSMethodSignature *)methodSignatureForSelector:(SEL)aSelector;
@end
