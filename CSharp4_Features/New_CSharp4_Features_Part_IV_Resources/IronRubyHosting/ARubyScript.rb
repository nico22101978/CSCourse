## Using .Net Types from IronRuby:

require 'System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' 
require 'System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
require 'System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

include System
include System::Drawing
include System::Windows::Forms

# We can also inherit from .Net classes and you can implement .Net interfaces.
class MyForm < Form
    def initialize
        label = Label.new
        label.text = 'A text...'
        label.location = Point.new(10, 10)
        self.controls.add(label)
    end
end

#my_form = MyForm.new
#Application.Run(my_form)


###################################################################################################
## Using .Net Types from IronRuby:

# A function:
def say_hello(name)
    'Hello '+name
end

# A global variable (as this is the last expression of this script, it is also the result of the
# script):
$now = Time.now