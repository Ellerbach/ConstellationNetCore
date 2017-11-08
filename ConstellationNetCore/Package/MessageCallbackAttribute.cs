
using System;

namespace Constellation.Package
{
    //
    // Summary:
    //     Declares the method as a MessageCallback on Constellation
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageCallbackAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the Constellation.Package.MessageCallbackAttribute
        //     class.
        public MessageCallbackAttribute()
        {
            //TODO 
            // Get the method name
            // Key = 
        }
        //
        // Summary:
        //     Initializes a new instance of the Constellation.Package.MessageCallbackAttribute
        //     class.
        //
        // Parameters:
        //   key:
        //     The key.
        public MessageCallbackAttribute(string key)
        {
            Key = key;
        }
        //
        // Summary:
        //     Initializes a new instance of the Constellation.Package.MessageCallbackAttribute
        //     class.
        //
        // Parameters:
        //   responseType:
        //     Type of the response.
        public MessageCallbackAttribute(Type responseType)
        {
            ResponseType = responseType;
        }
        //
        // Summary:
        //     Initializes a new instance of the Constellation.Package.MessageCallbackAttribute
        //     class.
        //
        // Parameters:
        //   key:
        //     The key.
        //
        //   responseType:
        //     Type of the response.
        public MessageCallbackAttribute(string key, Type responseType)
        {
            Key = key;
            ResponseType = responseType;
        }

        //
        // Summary:
        //     Gets or sets the message key for this MessageCallback. By default, the key is
        //     the method's name
        public string Key { get; set; }
        //
        // Summary:
        //     Gets or sets the description.
        public string Description { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether this MessageCallback is hidden on Constellation.
        public bool IsHidden { get; set; }
        //
        // Summary:
        //     Gets or sets the type of the response.
        public Type ResponseType { get; set; }
    }
}