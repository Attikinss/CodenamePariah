using UnityEditor.Experimental.GraphView;

namespace WhiteWillow.Editor
{
    public class PortDescription
    {
        /// <summary>The node this description belongs to.</summary>
        public NodeView Owner { get; private set; }

        /// <summary>The internal name of the port.</summary>
        public string MemberName { get; }
        
        /// <summary>The display name of the port.</summary>
        public string DisplayName { get; } = "";
        
        /// <summary>The ports connection direction.</summary>
        public Direction Direction { get; }

        /// <summary>How many connections the port can have.</summary>
        public Port.Capacity Capacity { get; }

        /// <summary>The value type of the connection.</summary>
        public PortValueType ValueType { get; } = PortValueType.Boolean;

        /// <summary>Defines whether or not the port is an input port.</summary>
        public bool IsInputPort { get { return Direction == Direction.Input; } }
        
        /// <summary>Defines whether or not the port is an output port.</summary>
        public bool IsOutputPort { get { return Direction == Direction.Output; } }

        public PortDescription(NodeView owner, string memberName, string displayName, Direction portDirection, Port.Capacity capacity)
        {
            Owner = owner;
            MemberName = memberName;
            DisplayName = displayName;
            Direction = portDirection;
            Capacity = capacity;
        }

        public bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Boolean;
        }

        public bool IsCompatibleWith(PortDescription otherPortDescription)
        {
            return otherPortDescription != null
                && otherPortDescription.Owner != Owner
                && otherPortDescription.IsInputPort != IsInputPort
                && ((IsInputPort ? otherPortDescription.IsCompatibleWithInputSlotType(ValueType) : IsCompatibleWithInputSlotType(otherPortDescription.ValueType)));
        }

        public enum PortValueType
        {
            Boolean
        }
    }
}