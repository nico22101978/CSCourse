class PropertyBag
  def method_missing(method_id, *attr)
    property_name = method_id.to_s.end_with?('=')? method_id.to_s.chop : method_id.to_s
    self.class.send(:define_method, property_name + '=') { |val| instance_variable_set('@' + property_name, val) }
    self.class.send(:define_method, property_name) { instance_variable_get('@' + property_name) }
    self.__send__(method_id, attr)
  end
end


#---------------------------------------------------------------------------------------------------
# Ruby's default Behavior on Calling unknown Methods/Properties:
    
# Let's check Ruby's default behavior, if we try to invoke a method/property that does not
# exist on an object's type.
$anObject = Object.new
puts 'anObject created: %s' % $anObject
	
# If we execute following line Ruby will issue a NoMethodError with the message "undefined method
# `aProperty=' for #<Object:0x...>.
#$anObject.aProperty = 'text'


#----------------------------------------------------------------------------------------------
# Now the User defined PropertyBag Type with dynamic Dispatching:
$propertyBag = PropertyBag.new

# Set a number as new property and then get this number and write it to the console:
$propertyBag.numberProperty = 42;
puts('Here the stored number: %s' % $propertyBag.numberProperty)
    
# As you can see the type of the formerly added property "numberProperty" is not fixed to Fixnum. It
# could be set to an object of type String ('text' in this case):
$propertyBag.numberProperty = 'text'
puts('Here the stored text: %s' % $propertyBag.numberProperty)
    
# But in such a case we are better off introducing a new property like so:
$propertyBag.textProperty = 'another text'
puts('Here the stored text: %s' % $propertyBag.textProperty)
