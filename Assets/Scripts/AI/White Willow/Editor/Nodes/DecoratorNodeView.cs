using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using WhiteWillow.Nodes;

namespace WhiteWillow.Editor
{
    public class DecoratorNodeView : NodeView
    {
        /// <summary>The child of the node.</summary>
        public NodeView Child { get; protected set; }

        protected override void Construct()
        {
            // Add an input port to the node
            AddInputPort();

            // Add an output port to the node
            AddOutputPort();
        }

        public override void ConnectNodes(Port port, Direction connectionDirection)
        {
            if (connectionDirection == Direction.Input)
                SetParent((port as EditorPort).Owner);
        }

        public override List<EditorPort> GetPorts()
        {
            return new List<EditorPort>() { InputPort, OutputPort };
        }

        public override void OnMove()
        {
            CaluclateExecutionOrder();

            Node.GraphDimensions = Position;
        }

        public override IEnumerable<EdgeView> OnDelete()
        {
            // Remove connection between this node and child node
            Child?.SetParent(null);
            Child = null;

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

            // Return all edges needing removal from the graph view.
            InputPort.connections.Concat(OutputPort.connections);
            IEnumerable<EdgeView> connections = null;
            
            foreach (var edge in InputPort.connections)
                connections.Append(edge as EdgeView);

            return connections;
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

        public void SetChild(NodeView node)
        {
            // No point continuing
            if (Child == node)
                return;

            // Clear child's parent if child exists
            Child?.SetParent(null);

            if (node != null) (Node as Decorator).SetChild(node.Node.GUID);
            else
            {
                BaseNode temp = null;
                (Node as Decorator).SetChild(temp);
            }

            Child = node;

            // Set child's parent to this if child exists
            Child?.SetParent(this);
        }
    }
}