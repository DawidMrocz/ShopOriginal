using System;

namespace ServiceBroker.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageTypeAttribute : Attribute
    {
        public readonly string Type;

        public MessageTypeAttribute(string type)
        {
            Type = type;
        }
    }
}