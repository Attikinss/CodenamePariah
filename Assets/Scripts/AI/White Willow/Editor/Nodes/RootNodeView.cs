using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using WhiteWillow.Nodes;

namespace WhiteWillow.Editor
{
    public class RootNodeView : NodeView
    {
        public NodeView Child { get; protected set; }

        protected override void Construct()
        {
            capabilities = capabilities & ~Capabilities.Deletable;

            // Add an output port to the node
            AddOutputPort();

            // Add constructed node to the graph view
            AddToGraphView();
        }

        /// <summary>Get the ports on the node.</summary>
        public override List<EditorPort> GetPorts() => new List<EditorPort>() { OutputPort };

        public override void ConnectNodes(Port port, Direction connectionDirection)
        {
            if (connectionDirection == Direction.Output)
                SetChild((port as EditorPort).Owner);
        }

        /// <summary>Handles the deletion/removal of the node from the graph view.</summary>
        public override IEnumerable<Edge> OnDelete()
        {
            Child?.SetParent(null);
            Child = null;

            return null;
        }

        /// <summary>Root node cannot have parents. This function does nothing in this context.</summary>
        public override void SetParent(NodeView node) { }

        public void SetChild(NodeView node)
        {
            // No point continuing
            if (Child == node)
                return;

            // Clear child's parent if child exists
            Child?.SetParent(null);

            if (node != null) (Node as Root).SetChild(node.Node.GUID);
            else
            {
                BaseNode temp = null;
                (Node as Root).SetChild(temp);
            }

            Child = node;

            // Set child's parent to this if child exists
            Child?.SetParent(this);
        }

        public override void OnMove()
        {
            Node.GraphDimensions = Position;
        }
    }
}