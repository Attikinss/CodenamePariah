using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using WhiteWillow.Nodes;

namespace WhiteWillow.Editor
{
    public class CompositeNodeView : NodeView
    {
        /// <summary>Children of the composite node.</summary>
        public HashSet<NodeView> Children { get; protected set; } = new HashSet<NodeView>();

        protected override void Construct()
        {
            // Add an input port to the node
            AddInputPort();

            // Add an output port to the node
            AddOutputPort(Port.Capacity.Multi);
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
            // Remove each child node's relation to this node
            foreach (var child in Children)
                child.SetParent(null);

            // Clear child node list
            Children.Clear();

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

        public void AddChildren(params NodeView[] nodes)
        {
            foreach (var node in nodes)
            {
                (Node as Composite).AddChildren(node.Node.GUID);
                Children.Add(node);
                node.SetParent(this);
            }
        }

        public void RemoveChildren(params NodeView[] nodes)
        {
            foreach (var node in nodes)
            {
                (Node as Composite).RemoveChildren(node.Node.GUID);
                Children.Remove(node);
                node.SetParent(null);
            }
        }
    }
}