
using System;

namespace Constellation.Package
{
    //
    // Summary:
    //     Provides data for various Constellation.StateObject changed events.
    public class StateObjectChangedEventArgs : EventArgs
    {
        public StateObjectChangedEventArgs()
        {

        }

        //
        // Summary:
        //     Gets or sets the Constellation.StateObject before the change.
        public StateObject OldState { get; set; }
        //
        // Summary:
        //     Gets or sets the Constellation.StateObject after the change.
        public StateObject NewState { get; set; }
        //
        // Summary:
        //     Gets a value indicating if it's a new Constellation.StateObject (= the OldValue
        //     is null)
        public bool IsNew { get { return (OldState == null);  } }
    }
}