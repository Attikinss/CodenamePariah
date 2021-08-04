using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace WhiteWillow.Editor
{
    public class TaskNodeView : NodeView
    {
        protected override void Construct()
        {
            // Add an input port to the node
            AddInputPort();
        }

        /// <summary>Get the ports on the node.</summary>
        public override List<EditorPort> GetPorts() => new List<EditorPort>() { InputPort };

        public override void ConnectNodes(Port port, Direction connectionDirection)
        {
            if (connectionDirection == Direction.Input)
                SetParent((port as EditorPort).Owner);
        }

        public override void SetParent(NodeView node)
        {
            if (Parent == node)
                return;

            Parent = node;

            if (Parent != null)
            {
                // Get parent type
                var parentType = Parent.GetType();

                // Create connection between this node and parent node
                if (parentType == typeof(CompositeNodeView))
                    (Parent as CompositeNodeView).AddChildren(this);
                else if (parentType == typeof(DecoratorNodeView))
                    (Parent as DecoratorNodeView).SetChild(this);
                else if (parentType.IsSubclassOf(typeof(RootNodeView)))
                    (Parent as RootNodeView).SetChild(this);

                Node.SetParent(Parent.Node);
            }
        }

        /// <summary>Handles the movement and execution order assignment of the node in the graph view.</summary>
        public override void OnMove()
        {
            CaluclateExecutionOrder();

            Node.GraphDimensions = Position;
        }

        public override IEnumerable<Edge> OnDelete()
        {
            // Remove connection between this node and child node
            if (Parent != null)
            {
                // Get parent type
                var parentType = Parent.GetType();

                // Remove connection between this node and parent node
                if (parentType == typeof(CompositeNodeView))
                    (Parent as CompositeNodeView).RemoveChildren(this);
                else if (parentType == typeof(DecoratorNodeView))
                    (Parent as DecoratorNodeView).SetChild(null);
                else if (parentType.IsSubclassOf(typeof(RootNodeView)))
                    (Parent as RootNodeView).SetChild(null);

                Parent = null;
            }

            Node.Owner?.DeleteNode(Node);

            return null;
        }
    }
}