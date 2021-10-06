using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using WhiteWillow.Nodes;
using System;
using System.Linq;

namespace WhiteWillow.Editor
{
    public abstract class NodeView : Node
    {
        protected readonly Color ActiveColour = new Color(0.2f, 0.8f, 0.1f);
        protected readonly Color InactiveColour = new Color(0.8f, 0.8f, 0.8f);
        protected readonly Color InactiveSelectionColour = new Color(0.8f, 0.8f, 0.8f);
        protected readonly Color InactivePortColour = new Color(0.8f, 0.8f, 0.8f);

        public Action<NodeView> OnNodeSelected;

        /// <summary>The base node that was used to construct this node.</summary>
        public BaseNode Node { get; protected set; }

        /// <summary>The parent of the node view.</summary>
        public NodeView Parent { get; protected set; }

        /// <summary>The dimensions and position of the node.</summary>
        public Rect Position { get => GetPosition(); set { SetPosition(value); Node.GraphDimensions = value; } }

        /// <summary>The input/parent port of the node.</summary>
        public EditorPort InputPort { get; protected set; }

        /// <summary>The output/child(ren) port of the node.</summary>
        public EditorPort OutputPort { get; protected set; }

        /// <summary>The graph view the node is active in.</summary>
        public BehaviourTreeGraphView GraphView { get => m_GraphView; }

        /// <summary>The editor window the node is active in.</summary>
        public BehaviourTreeEditorWindow EditorWindow { get => m_EditorWindow; }

        /// <summary>The graph view the node is active in.</summary>
        public BehaviourTreeGraphView m_GraphView;

        /// <summary>The editor window the node is active in.</summary>
        public BehaviourTreeEditorWindow m_EditorWindow;

        public bool m_Highlight = false;

        /// <summary>The default size of a new node.</summary>
        protected static readonly Vector2 DefaultNodeSize = new Vector2(150, 200);

        //////////////////////////////////////////////
        ////    FOR NODE HIGHLIGHTING PURPOSES    ////
        //////////////////////////////////////////////
        protected VisualElement m_NodeBorder;
        protected VisualElement m_NodeSelectionBorder;
        protected VisualElement m_InputConnector;
        protected VisualElement m_OutputConnector;
        protected VisualElement m_InputConnectorCap;
        protected VisualElement m_OutputConnectorCap;

        protected NodeView() : base("Assets/Scripts/AI/White Willow/Editor/Resources/Styles/NodeView.uxml") { }

        /// <summary>Constructor that builds an graph node using a base node.</summary>
        /// <param name="editorWindow">The active editor window.</param>
        /// <param name="graphView">The active graph view.</param>
        /// <param name="node">The node that will be used to construct the new graph node.</param>
        public static NodeView ConstructNode(BehaviourTreeEditorWindow editorWindow, BehaviourTreeGraphView graphView, BaseNode node)
        {
            var nodeType = node.GetType();
            NodeView nodeView = null;

            // Remove connection between this node and parent node
            if (nodeType.IsSubclassOf(typeof(Composite)))
            {
                nodeView = new CompositeNodeView();
                nodeView.AddToClassList("composite");

                if (nodeType == typeof(Selector))
                    nodeView.AddToClassList("selector");
                else if (nodeType == typeof(Sequence))
                    nodeView.AddToClassList("sequence");
            }
            else if (nodeType.IsSubclassOf(typeof(Decorator)))
            {
                nodeView = new DecoratorNodeView();
                nodeView.AddToClassList("decorator");
            }
            else if (nodeType.IsSubclassOf(typeof(Task)))
            {
                nodeView = new TaskNodeView();
                nodeView.AddToClassList("task");
            }
            else if (nodeType == typeof(Root))
            {
                nodeView = new RootNodeView();
                nodeView.AddToClassList("root");
            }

            nodeView.Node = node;
            nodeView.title = !string.IsNullOrWhiteSpace(node.Title) ? node.Title : node.GetType().Name;
            nodeView.m_EditorWindow = editorWindow;
            nodeView.m_GraphView = graphView;

            // Get the styling for the node
            nodeView.styleSheets.Add(Resources.Load<StyleSheet>("Styles/NodeView"));

            // Build the node with specific node elements
            nodeView.Construct();

            // Set a default size
            nodeView.Position = node.GraphDimensions;

            // Add the icon to the node
            nodeView.SetNodeIcon(Resources.Load<Sprite>(nodeView.Node.IconPath));

            nodeView.Node.OnNodeActive = nodeView.OnActive;
            nodeView.Node.OnNodeInactive = nodeView.OnInactive;

            nodeView.OnNodeSelected = graphView.OnNodeSelected;

            // Cache a bunch of elements that will be highlighted during runtime
            nodeView.m_NodeBorder = nodeView.Children().FirstOrDefault(element => element.name == "node-border");
            nodeView.m_NodeSelectionBorder = nodeView.Children().FirstOrDefault(element => element.name == "selection-border");

            return nodeView;
        }

        protected abstract void Construct();

        /// <summary>Get the ports on the node.</summary>
        public abstract List<EditorPort> GetPorts();

        public virtual void SetNode(BaseNode node)
        {
            if (node == null)
                return;

            Node = node;
            Node.OnNodeActive = OnActive;
            Node.OnNodeInactive = OnInactive;
            Position = node.GraphDimensions;
            viewDataKey = node.GUID;
            name = node.GetType().Name.ToString();
            title = node.GetType().Name.ToString();
        }

        /// <summary>Sets the node's icon.</summary>
        /// <param name="sprite">The new icon.</param>
        protected void SetNodeIcon(Sprite sprite)
        {
            // Check if the icon element already exists
            //if (titleContainer.ElementAt(0)?.name == "status-icon-container")
            //    return;

            // Create an container for the icon
            VisualElement iconElement = new VisualElement() { name = "status-icon-container" };

            // Add an image to the container
            iconElement.Add(new Image()
            {
                name = "status-icon",
                tintColor = Color.white,
                image = sprite.texture,
            });

            // Insert the icon element to the front of the title container
            // list so that the icon is the furthest left UI element
            titleContainer.Insert(0, iconElement);
        }

        /// <summary>Sets the parent of the node.</summary>
        /// <param name="node">The node that will be set as the parent.</param>
        public abstract void SetParent(NodeView node);

        /// <summary>Adds an input port to the node.</summary>
        protected void AddInputPort()
        {
            // Early out if the input port already exists
            if (inputContainer.Contains(InputPort))
                return;

            InputPort = EditorPort.Create(new PortDescription(this, "Input", "Input", Direction.Input, Port.Capacity.Single));
            InputPort.portName = "";
            InputPort.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(InputPort);
        }

        public void UpdateInfo()
        {
            if (title != Node.Title)
            {
                if(!string.IsNullOrWhiteSpace(Node.Title))
                {
                    title = Node.Title;
                    Node.name = Node.Title;
                }
                else
                {
                    title = Node.GetType().Name;
                    Node.name = title;
                }
            }
        }

        /// <summary>Adds an output port to the node.</summary>
        protected void AddOutputPort(Port.Capacity capacity = Port.Capacity.Single)
        {
            // Early out if the output port already exists
            if (outputContainer.Contains(OutputPort))
                return;

            OutputPort = EditorPort.Create(new PortDescription(this, "Output", "Output", Direction.Output, capacity));
            OutputPort.portName = "";
            OutputPort.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(OutputPort);
        }

        public void RecaluclateExecutionOrder()
        {
            CompositeNodeView parent = Parent as CompositeNodeView;
            if (parent != null)
            {
                List<KeyValuePair<int, float>> nodeLocations = new List<KeyValuePair<int, float>>();
                for (int i = 0; i < parent.Children.Count; i++)
                {
                    // For some fkn reason the native position rect doesn't update??
                    // Using Node.GraphDimensions for position data does though so we all g here
                    nodeLocations.Add(new KeyValuePair<int, float>(i, parent.Children.ElementAt(i).Node.GraphDimensions.position.x));
                }

                nodeLocations.Sort(delegate (KeyValuePair<int, float> a, KeyValuePair<int, float> b) { return a.Value.CompareTo(b.Value); });

                for (int i = 0; i < parent.Children.Count; i++)
                    parent.Children.ElementAt(i).Node.ExecutionOrder = Convert.ToUInt16(nodeLocations.FindIndex(itr => itr.Key == i) + 1);

                parent.Children = new HashSet<NodeView>(parent.Children.OrderBy(node => node.Node.ExecutionOrder), null);
            }
        }

        /// <summary>Connects two nodes.</summary>
        /// <param name="port">The port of the connecting node.</param>
        /// <param name="connectionDirection">The direction of the connection. e.g. Output port to input port = input</param>
        public abstract void ConnectNodes(Port port, Direction connectionDirection);

        /// <summary>Handles the movement and execution order assignment of the node in the graph view.</summary>
        public abstract void OnMove();

        /// <summary>Handles the deletion/removal of the node from the graph view.</summary>
        /// <returns>Any edges that need to be deleted/remove.</returns>
        public abstract IEnumerable<Edge> OnDelete();

        /// <summary>Adds the node to the graph view.</summary>
        public void AddToGraphView()
        {
            m_GraphView.AddElement(this);
            RecaluclateExecutionOrder();
        }

        /// <summary>Removes the node from the graph view.</summary>
        public void RemoveFromGraphView() => m_GraphView.RemoveElement(this);

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public bool IsMouseOver(Vector2 mousePos)
        {
            return (mousePos.x > Position.xMin && mousePos.y > Position.yMin &&
                mousePos.x < Position.xMax && mousePos.y < Position.yMax);
        }

        public void OnActive()
        {
            m_Highlight = true;
        }

        public void OnInactive()
        {
            m_Highlight = false;
        }

        public void Highlight()
        {
            //// Highlight borders
            //_NodeSelectionBorder.style.borderLeftColor = ActiveColour;
            //_NodeSelectionBorder.style.borderRightColor = ActiveColour;
            //_NodeSelectionBorder.style.borderTopColor = ActiveColour;
            //_NodeSelectionBorder.style.borderBottomColor = ActiveColour;
            //
            //_NodeBorder.style.borderLeftColor = ActiveColour;
            //_NodeBorder.style.borderRightColor = ActiveColour;
            //_NodeBorder.style.borderTopColor = ActiveColour;
            //_NodeBorder.style.borderBottomColor = ActiveColour;
            //_NodeBorder.style.borderTopWidth = 2f;
            //_NodeBorder.style.borderRightWidth = 2f;
            //_NodeBorder.style.borderLeftWidth = 2f;
            //_NodeBorder.style.borderBottomWidth = 2f;
            
            if (InputPort != null)
            {
                // Highlight the input port
                InputPort.SetColour(ActiveColour);
            }
            
            if (OutputPort != null)
            {
                // Highlight the output port
                OutputPort.SetColour(ActiveColour);
            }
            
            Edge edge = InputPort?.connections.FirstOrDefault();
            if (edge != null)
            {
                edge.edgeControl.inputColor = ActiveColour;
                edge.edgeControl.outputColor = ActiveColour;
            }
            
            m_Highlight = true;
            
            Parent?.Highlight();
        }

        public void Unhighlight()
        {
            //// Unhighlight borders
            //m_NodeSelectionBorder.style.borderLeftColor = InactiveSelectionColour;
            //m_NodeSelectionBorder.style.borderRightColor = InactiveSelectionColour;
            //m_NodeSelectionBorder.style.borderTopColor = InactiveSelectionColour;
            //m_NodeSelectionBorder.style.borderBottomColor = InactiveSelectionColour;
            //
            //m_NodeBorder.style.borderLeftColor = InactiveColour;
            //m_NodeBorder.style.borderRightColor = InactiveColour;
            //m_NodeBorder.style.borderTopColor = InactiveColour;
            //m_NodeBorder.style.borderBottomColor = InactiveColour;
            //m_NodeBorder.style.borderTopWidth = 1f;
            //m_NodeBorder.style.borderRightWidth = 1f;
            //m_NodeBorder.style.borderLeftWidth = 1f;
            //m_NodeBorder.style.borderBottomWidth = 1f;
            //
            //if (InputPort != null)
            //{
            //    // Unhighlight the input port
            //    InputPort.SetColour(InactivePortColour);
            //}
            //if (OutputPort != null)
            //{
            //    // Unhighlight the output port
            //    OutputPort.SetColour(InactivePortColour);
            //}
            //
            //Edge edge = InputPort?.connections.FirstOrDefault();
            //if (edge != null)
            //{
            //    edge.edgeControl.inputColor = InactivePortColour;
            //    edge.edgeControl.outputColor = InactivePortColour;
            //}
            //
            //m_Highlight = false;
            //
            //Parent?.Unhighlight();
        }
    }
}