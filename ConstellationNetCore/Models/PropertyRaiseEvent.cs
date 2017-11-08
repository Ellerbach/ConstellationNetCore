using Constellation.Package;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Constellation.Package
{
    class PropertyRaiseEvent
    {
        public PropertyRaiseEvent(StateObjectLinkAttribute StateObjectsToCheck, StateObjectNotifier StateNotifyer, PropertyInfo Property, Action<StateObject> ActionState)
        {
            this.StateObjectsToCheck = StateObjectsToCheck;
            this.StateNotifyer = StateNotifyer;
            this.Property = Property;
            this.ActionState = ActionState;
        }
        public StateObjectLinkAttribute StateObjectsToCheck { get; set; }
        public StateObjectNotifier StateNotifyer { get; set; }
        public PropertyInfo Property { get; set; }
        public Action<StateObject> ActionState;
    }
}
