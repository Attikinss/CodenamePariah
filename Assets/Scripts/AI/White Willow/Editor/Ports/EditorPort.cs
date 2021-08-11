using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEngine;

namespace WhiteWillow.Editor
{
    public class EditorPort : Port
    {
        /// <summary>The node that the port belongs to.</summary>
        public NodeView Owner { get; private set; }

        /// <summary>The port details.</summary>
        public PortDescription Description { get; private set; }

        protected EditorPort(PortDescription portDescription) : base(Orientation.Vertical, portDescription.Direction, portDescription.Capacity, typeof(float))
        {
            Owner = portDescription.Owner;
            Description = portDescription;

            m_EdgeConnector = new EdgeConnector<Edge>(Owner.GraphView.EdgeConnectorListener);

            this.AddManipulator(m_EdgeConnector);
        }

        /// <summary>Factory function for creating ports.</summary>
        /// <param name="portDescription">The details of the port.</param>
        /// <returns>A new port to attach to a node.</returns>
        public static EditorPort Create(PortDescription portDescription)
        {
            return new EditorPort(portDescription);
        }

        /// <summary>Clears all connections from the port and removes them from the graph view.</summary>
        public void ClearConnections()
        {
            foreach (var edge in connections.ToArray())
            {
                // Disconnect edge's connected output port if one exists
                edge.output?.Disconnect(edge);

                // Disconnect edge's connected input port if one exists
                edge.input?.Disconnect(edge);

                // Remove the edge from the graph through its parent
                edge.parent?.Remove(edge);
            }
        }

        public void SetColour(Color colour)
        {
            //portColor = colour;
            //m_ConnectorBox.style.borderLeftColor = colour;
            //m_ConnectorBox.style.borderRightColor = colour;
            //m_ConnectorBox.style.borderTopColor = colour;
            //m_ConnectorBox.style.borderBottomColor = colour;
            //m_ConnectorBox.MarkDirtyRepaint();
            //
            //m_ConnectorBoxCap.style.backgroundColor = colour;
            //m_ConnectorBoxCap.MarkDirtyRepaint();
        }

        public override void OnStartEdgeDragging()
        {
            base.OnStartEdgeDragging();
        }

        public override void OnStopEdgeDragging()
        {
            base.OnStopEdgeDragging();
        }
    }
}