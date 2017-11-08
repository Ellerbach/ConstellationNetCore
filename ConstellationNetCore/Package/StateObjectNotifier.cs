
using System;
using System.ComponentModel;
using Constellation;
using System.Runtime.CompilerServices;

namespace Constellation.Package
{
    //
    // Summary:
    //     StateObject container with events when the Constellation.StateObject changes.
    public class StateObjectNotifier : INotifyPropertyChanged
    {
        public StateObjectNotifier()
        {

        }

        //
        // Summary:
        //     Gets the dynamic value of the Constellation.StateObject (shortcut to 'this.Value.DynamicValue').
        public dynamic DynamicValue { get {
                return Value.DynamicValue;
            } }
        //
        // Summary:
        //     Gets or sets the Constellation.StateObject.
        public StateObject Value { get; set; }

        //
        // Summary:
        //     Occurs when a property value changes.
        public event PropertyChangedEventHandler PropertyChanged;
        //
        // Summary:
        //     Occurs when the Constellation.StateObject changes.
        public event EventHandler<StateObjectChangedEventArgs> ValueChanged;
    }
}