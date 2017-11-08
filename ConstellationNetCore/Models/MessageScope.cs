
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Constellation
{
    //
    // Summary:
    //     Represent a Scope where send a Constellation's message
    public class MessageScope
    {
        //
        // Summary:
        //     Initializes a new instance of the Constellation.MessageScope class.
        //
        // Parameters:
        //   scope:
        //     The type of the scope.
        //
        //   args:
        //     The arguments of the scope.
        public MessageScope(ScopeType scope, params string[] args)
        {
            Scope = scope;
            Args = new List<string>(args);
        }

        //
        // Summary:
        //     Gets or sets the saga identifier for this scope.
        public string SagaId { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether this scope is a saga.
        [JsonIgnore]
        public bool IsSaga { get {
                if (SagaId == null)
                    return false;
                return true;
            } }
        //
        // Summary:
        //     Gets or sets the type of the scope.
        public ScopeType Scope { get; set; }
        //
        // Summary:
        //     Gets or sets the arguments of the scope.
        public List<string> Args { get; set; }

        //
        // Summary:
        //     Creates a scope.
        //
        // Parameters:
        //   scope:
        //     The type of the scope.
        //
        //   args:
        //     The arguments of the scope.
        //
        // Returns:
        //     MessageScope
        public static MessageScope Create(ScopeType scope, params string[] args)
        {
            return new MessageScope(scope, args);

        }
        //
        // Summary:
        //     Returns a System.String that represents this instance.
        //
        // Returns:
        //     A System.String that represents this instance.
        public override string ToString()
        {
            return Scope.ToString();
        }

        //
        // Summary:
        //     Specifies the different types of scope
        public enum ScopeType
        {
            //
            // Summary:
            //     Empty scope
            None = 0,
            //
            // Summary:
            //     Send message to specific group(s) define in the arguments
            Group = 1,
            //
            // Summary:
            //     Send message to specific package(s) define in the arguments
            Package = 2,
            //
            // Summary:
            //     Send message to specific sentinel(s) define in the arguments
            Sentinel = 3,
            //
            // Summary:
            //     Send message to other connected packages
            Others = 4,
            //
            // Summary:
            //     Send message to other connected packages and control applications
            All = 5
        }
    }
}