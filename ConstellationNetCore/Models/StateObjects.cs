using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Constellation
{

    //used only to post objects as POST
    class PostStateObject
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
        public int Lifetime { get; set; }
        public object Metadatas { get; set; }
    }

    //
    // Summary:
    //     Represent a Constellation's StateObject
    [System.Runtime.Serialization.DataContractAttribute]
    public class StateObject
    {
        public StateObject()
        { }
        //
        // Summary:
        //     Gets or sets the name of the Sentinel.
        [System.Runtime.Serialization.DataMemberAttribute]
        public string SentinelName { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the package.
        [System.Runtime.Serialization.DataMemberAttribute]
        public string PackageName { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the StateObject.
        [System.Runtime.Serialization.DataMemberAttribute]
        public string Name { get; set; }
        //
        // Summary:
        //     Gets the unique identifier of the StateObject (eg. {SentinelName}/{PackageName}/{Name}).
        [JsonIgnore]
        public string UniqueId { get; }
        //
        // Summary:
        //     Gets or sets the type of the StateObject.
        [System.Runtime.Serialization.DataMemberAttribute]
        public string Type { get; set; }
        //
        // Summary:
        //     Gets or sets the metadatas of the StateObject.
        [System.Runtime.Serialization.DataMemberAttribute]
        public Dictionary<string, object> Metadatas { get; set; }
        //
        // Summary:
        //     Gets or sets the last update of the StateObject.
        [System.Runtime.Serialization.DataMemberAttribute]
        public DateTime LastUpdate { get; set; }
        //
        // Summary:
        //     Gets or sets the number of seconds to expire the StateObject.
        [System.Runtime.Serialization.DataMemberAttribute]
        public int Lifetime { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether this StateObject is expired.
        [System.Runtime.Serialization.DataMemberAttribute]
        public bool IsExpired { get; }
        //
        // Summary:
        //     Gets or sets the value of the StateObject.
        [System.Runtime.Serialization.DataMemberAttribute]
        public object Value { get; set; }
        //
        // Summary:
        //     Gets the value of the StateObject as dynamic.
        //
        // Returns:
        //     The value of the StateObject as dynamic.
        //[Dynamic]
        [JsonIgnore]
        public dynamic DynamicValue { get { return Value; } }

        //
        // Summary:
        //     Gets the StateObject's value as T.
        //
        // Type parameters:
        //   T:
        //     Type of the value
        //
        // Returns:
        //     The value as T
        public T GetValue<T>()
        {
            return (T)Value;
        }
        //
        // Summary:
        //     Converts the StateObject's value as T. A return value indicates whether the conversion
        //     succeeded.
        //
        // Type parameters:
        //   T:
        //     Type of the value
        //
        // Returns:
        //     true if the value was converted successfully; otherwise, false.
        public bool TryGetValue<T>(out T value)
        {
            try
            {
                value = (T)Value;
                return true;
            }
            catch (Exception)
            {

                value = default(T);
                return false;
            }

        }
    }

    //
    // Summary:
    //     Provides data for the StateObjectUpdated event.
    public class StateObjectUpdatedEventArgs : EventArgs
    {
        public StateObjectUpdatedEventArgs()
        { }

        //
        // Summary:
        //     Gets or sets the Constellation.StateObjectUpdatedEventArgs.StateObject.
        public StateObject StateObject { get; set; }
    }

    //
    // Summary:
    //     Provides data for Constellation.StateObject list events.
    public class StateObjectsListEventArgs : EventArgs
    {
        public StateObjectsListEventArgs() { }

        //
        // Summary:
        //     Gets or sets the Constellation.StateObject list.
        public List<StateObject> StateObjects { get; set; }
    }


    public class StateObjectFromGet
    {
        public StateObject StateObject { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class StateObjectAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class StateObjectKnownTypesAttribute : Attribute
    {
        public StateObjectKnownTypesAttribute(params Type[] knownType)
        {
            this.StateObjectKnownTypes = knownType;
        }

        public Type[] StateObjectKnownTypes { get; set; }
    }

}
