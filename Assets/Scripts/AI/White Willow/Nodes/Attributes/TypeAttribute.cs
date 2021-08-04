using System;

namespace WhiteWillow
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeType : Attribute
    {
        public readonly Type Type;
        public NodeType(Type type) => Type = type;
    }
}