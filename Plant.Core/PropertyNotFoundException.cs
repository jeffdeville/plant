using System;

namespace Plant.Core
{
    public class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException(string propertyName, object propertyValue) :
            base(string.Format("Property #{0} with value ${1}", propertyName, propertyValue))
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;

        }
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
    }
}