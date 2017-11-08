
using System;

namespace Constellation.Package
{
    //
    // Summary:
    //     Specifies that the property is attach to StateObject.
    [AttributeUsage(AttributeTargets.Property)]
    public class StateObjectLinkAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the Constellation.Package.StateObjectLinkAttribute
        //     class.
        public StateObjectLinkAttribute()
        {
            Sentinel = "*";
            Package = "*";
            Name = "*";
            Type = "*";
            RequestValueOnInit = false;
        }
        //
        // Summary:
        //     Initializes a new instance of the Constellation.Package.StateObjectLinkAttribute
        //     class.
        //
        // Parameters:
        //   package:
        //     Name of the package.
        //
        //   name:
        //     The StateObject key.
        public StateObjectLinkAttribute(string package, string name)
        {
            Sentinel = "*";
            Package = package;
            Name = name;
            Type = "*";
            RequestValueOnInit = false;
        }
        //
        // Summary:
        //     Initializes a new instance of the Constellation.Package.StateObjectLinkAttribute
        //     class.
        //
        // Parameters:
        //   sentinelName:
        //     Name of the sentinel.
        //
        //   packageName:
        //     Name of the package.
        //
        //   name:
        //     The StateObject name.
        public StateObjectLinkAttribute(string sentinel, string package, string name)
        {
            Sentinel = sentinel;
            Package = package;
            Name = name;
            Type = "*";
            RequestValueOnInit = false;
        }
        //
        // Summary:
        //     Initializes a new instance of the Constellation.Package.StateObjectLinkAttribute
        //     class.
        //
        // Parameters:
        //   sentinelName:
        //     Name of the sentinel.
        //
        //   packageName:
        //     Name of the package.
        //
        //   name:
        //     The StateObject name.
        //
        //   type:
        //     The type.
        public StateObjectLinkAttribute(string sentinel, string package, string name, string type)
        {
            Sentinel = sentinel;
            Package = package;
            Name = name;
            Type = type;
            RequestValueOnInit = false;
        }

        //
        // Summary:
        //     Gets or sets the name of the sentinel.
        public string Sentinel { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the package.
        public string Package { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the StateObject.
        public string Name { get; set; }
        //
        // Summary:
        //     Gets or sets the type.
        public string Type { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether request the StateObject on initialize
        //     the property.
        public bool RequestValueOnInit { get; set; }
    }
}