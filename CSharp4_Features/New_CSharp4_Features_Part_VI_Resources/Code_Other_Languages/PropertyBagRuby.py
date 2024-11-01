class PropertyBag:
    def __init__():
        properties = {}
    def __getattr__(self, name):
        properties[name]
    def __setattr__(self, name, *args):
        properties[name] = args
        
        
#---------------------------------------------------------------------------------------------------
# Python's default Behavior on Calling unknown Methods/Properties:
    
# Let's check Python's default behavior, if we try to invoke a method/property that does not
# exist on an object's type.
anObject = object
print 'anObject created: %s' % anObject

# If we execute following line Python will issue a TypeError with the message "can't set attributes
# of built-in/extension type 'object'".
#anObject.aProperty = 'text'

	 
#----------------------------------------------------------------------------------------------
# Now the User defined PropertyBag Type with dynamic Dispatching:

propertyBag = PropertyBag

# Set a number as new property and then get this number and write it to the console:
propertyBag.numberProperty = 42;
print 'Here the stored number: %s' % propertyBag.numberProperty

# As you can see the type of the formerly added property "numberProperty" is not fixed to int. It
# could be set to an object of type str ('text' in this case):
propertyBag.numberProperty = 'text'
print 'Here the stored text: %s' % propertyBag.numberProperty
    
# But in such a case we are better off introducing a new property like so:
propertyBag.textProperty = 'another text'
print 'Here the stored text: %s' % propertyBag.textProperty