using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace WhiteWillow.Editor
{
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        /// <summary>Collection of changes in the graph view.</summary>
        private GraphViewChange m_GraphViewChange;

        /// <summary>Collection of edges to create in the graph view.</summary>
        private List<EdgeView> m_EdgesToCreate;

        /// <summary>Collection of edges to delete from the graph view.</summary>
        private List<GraphElement> m_EdgesToDelete;

        /// <summary>Provider for the pop up node search window.</summary>
        private SearchWindowProvider m_SearchWindowProvider;

        /// <summary>Edge connector listener constructor.</summary>
        /// <param name="searchWindowProvider">The search window provider for the pop up search window.</param>
        public EdgeConnectorListener(SearchWindowProvider searchWindowProvider)
        {
            m_SearchWindowProvider = searchWindowProvider;

            m_EdgesToCreate = new List<EdgeView>();
            m_EdgesToDelete = new List<GraphElement>();

            List<Edge> temp = new List<Edge>();
            foreach (var element in m_EdgesToCreate)
            {
                element.input.portColor = Color.white;
                element.output.portColor = Color.white;
                temp.Add(element);
            }

            m_GraphViewChange.edgesToCreate = temp;
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            // Reset the create list
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add((edge as EdgeView));

            // Clear the delete list
            m_EdgesToDelete.Clear();

            // If the edge's input port is single capacity, delete all other
            // edges connected to the node before the new edge can connect
            if (edge.input.capacity == Port.Capacity.Single)
            {
                // Iterate through all node connections of the edge
                foreach (var edgeToDelete in edge.input.connections)
                {
                    // Add any disconnected edges to the delete list
                    if (edgeToDelete != edge)
                        m_EdgesToDelete.Add(edgeToDelete);
                }
            }

            // If the edge's output port is single capacity, delete all other
            // edges connected to the node before the new edge can connect
            if (edge.output.capacity == Port.Capacity.Single)
            {
                // Iterate through all node connections of the edge
                foreach (var edgeToDelete in edge.output.connections)
                {
                    // Add any disconnected edges to the delete list
                    if (edgeToDelete != edge)
                        m_EdgesToDelete.Add(edgeToDelete);
                }
            }

            // Delete any newly disconnected edges
            if (m_EdgesToDelete.Count > 0)
                graphView.DeleteElements(m_EdgesToDelete);

            // Create new list so we're not modifying the original list
            var edgesToCreate = m_EdgesToCreate;

            // Ensure a graph view change callback has been 
            // assigned and retrieve any edge creations
            if (graphView.graphViewChanged != null)
            {
                List<EdgeView> temp = new List<EdgeView>();
                foreach (var element in graphView.graphViewChanged(m_GraphViewChange).edgesToCreate)
                    temp.Add(element as EdgeView);
                edgesToCreate = temp;
            }

            foreach (var newEdge in edgesToCreate)
            {
                // Add new edge to the graph
                graphView.AddElement(newEdge);

                // Connect the edges input port to the new edge
                edge.input.Connect(newEdge);
                
                // Connect the edges outut port to the new edge
                edge.output.Connect(newEdge);

                // Connect nodes to eachother now the ports are connected
                (edge.input as EditorPort).Owner.ConnectNodes(edge.output, Direction.Input);
                (edge.output as EditorPort).Owner.ConnectNodes(edge.input, Direction.Output);
            }
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            // Get the port from with the edge was dragged from
            var draggedPort = ((edge as EdgeView).output != null ? (edge as EdgeView).output.edgeConnector.edgeDragHelper.draggedPort : null) ??
                              ((edge as EdgeView).input != null ?  (edge as EdgeView).input.edgeConnector.edgeDragHelper.draggedPort : null);

            // Set the connected port of the search window provider so that
            // it can auto connect the from node to any newly created nodes
            m_SearchWindowProvider.ConnectedPort = (EditorPort)draggedPort;

            // Open the node creation/search window
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), m_SearchWindowProvider);
        }
    }
}
