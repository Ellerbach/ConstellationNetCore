
using System.Collections.Generic;

namespace Constellation
{
    //
    // Summary:
    //     Represent a description of the MessageCallbacks and StateObjects for a package
    public class PackageDescriptor
    {
        //
        // Summary:
        //     Initializes a new instance of the Constellation.PackageDescriptor class.
        public PackageDescriptor()
        {
            MessageCallbacks = new List<MessageCallbackDescriptor>();
            MessageCallbackTypes = new List<TypeDescriptor>();
            StateObjectTypes = new List<TypeDescriptor>();
        }

        //
        // Summary:
        //     Gets or sets the name of the package.
        public string PackageName { get; set; }
        //
        // Summary:
        //     Gets or sets the message callbacks desciptions.
        public List<MessageCallbackDescriptor> MessageCallbacks { get; set; }
        //
        // Summary:
        //     Gets or sets the message callback types.
        public List<TypeDescriptor> MessageCallbackTypes { get; set; }
        //
        // Summary:
        //     Gets or sets the state object types.
        public List<TypeDescriptor> StateObjectTypes { get; set; }

        //
        // Summary:
        //     Represent a MessageCallback description
        public class MessageCallbackDescriptor
        {
            public MessageCallbackDescriptor()
            {
                Parameters = new List<MemberDescriptor>();
            }

            //
            // Summary:
            //     Gets or sets the message key.
            public string MessageKey { get; set; }
            //
            // Summary:
            //     Gets or sets the description.
            public string Description { get; set; }
            //
            // Summary:
            //     Gets or sets the type of the response.
            public string ResponseType { get; set; }
            //
            // Summary:
            //     Gets or sets the parameters.
            public List<MemberDescriptor> Parameters { get; set; }

            //
            // Summary:
            //     Returns a System.String that represents this instance.
            //
            // Returns:
            //     A System.String that represents this instance.
            public override string ToString()
            {
                return Description;
            }
        }
        //
        // Summary:
        //     Represent a type description
        public class TypeDescriptor
        {
            public TypeDescriptor()
            {
                GenericParameters = new List<string>();
                EnumValues = new List<MemberDescriptor>();
            }

            //
            // Summary:
            //     Gets or sets the description.
            public string Description { get; set; }
            //
            // Summary:
            //     Gets or sets a value indicating whether this type is a generic.
            public bool IsGeneric { get; set; }
            //
            // Summary:
            //     Gets or sets a value indicating whether this type is an array.
            public bool IsArray { get; set; }
            //
            // Summary:
            //     Gets or sets the generic parameters.
            public List<string> GenericParameters { get; set; }
            //
            // Summary:
            //     Gets or sets a value indicating whether this type is an enum.
            public bool IsEnum { get; set; }
            //
            // Summary:
            //     Gets or sets the enum values.
            public List<MemberDescriptor> EnumValues { get; set; }
            //
            // Summary:
            //     Gets or sets the name of the type.
            public string TypeName { get; set; }
            //
            // Summary:
            //     Gets or sets the fullname of the type.
            public string TypeFullname { get; set; }
            //
            // Summary:
            //     Gets or sets the properties of the type.
            public List<MemberDescriptor> Properties { get; set; }

            //
            // Summary:
            //     Returns a System.String that represents this instance.
            //
            // Returns:
            //     A System.String that represents this instance.
            public override string ToString()
            {
                return Description;
            }
        }
        //
        // Summary:
        //     Represent a member description
        public class MemberDescriptor
        {
            public MemberDescriptor()
            {

            }

            //
            // Summary:
            //     Gets or sets the name.
            public string Name { get; set; }
            //
            // Summary:
            //     Gets or sets the name of the type.
            public string TypeName { get; set; }
            //
            // Summary:
            //     Gets or sets the description.
            public string Description { get; set; }
            //
            // Summary:
            //     Gets or sets the member's type.
            public MemberType Type { get; set; }

            //
            // Summary:
            //     Specifies the different type of a MemberDescriptor
            public enum MemberType
            {
                //
                // Summary:
                //     An enum value
                EnumValue = 0,
                //
                // Summary:
                //     A property
                Property = 1,
                //
                // Summary:
                //     A method parameter
                MethodParameter = 2
            }
        }
    }
}