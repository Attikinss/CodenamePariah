using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteWillow.Nodes;
using System.IO;
using System.Linq;

namespace WhiteWillow.Editor
{
    public class BehaviourTreeGraphView : GraphView
    {
        public Action<NodeView> OnNodeSelected;
        public new class UxmlFactory : UxmlFactory<BehaviourTreeGraphView, UxmlTraits> { }

        /// <summary>The provider for the node search window.</summary>
        public SearchWindowProvider SearchWindowProvider { get; private set; }

        /// <summary>The listener for edge connections between ports.</summary>
        public EdgeConnectorListener EdgeConnectorListener { get; private set; }

        /// <summary>The tree editor window instance.</summary>
        private BehaviourTreeEditorWindow m_EditorWindow;

        /// <summary>The root node of the tree. This cannot be deleted from the graph.</summary>
        private RootNodeView m_RootNode;

        /// <summary>The current active/selected behaviour tree.</summary>
        public BehaviourTree CurrentTree;

        public static readonly string c_RootPath = "Assets/White Willow";
        public static readonly string c_EditorResourcePath = $"{c_RootPath}/Editor/Resources";
        public static readonly string c_IconPath = $"{c_EditorResourcePath}/Icons";

        public void Construct(BehaviourTreeEditorWindow editorWindow)
        {
            m_EditorWindow = editorWindow;

            // Set callback for key down events
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<MouseDownEvent>(OnMouseDown);

            // Setup the zoom values of the graph
            SetupZoom(0.4f, 2.0f, 0.2f, 1.25f);

            // Add content manipulators for graph interaction
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

            // Initialise the search window provider
            SearchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            SearchWindowProvider.Initialise(m_EditorWindow, this);

            // Initialse the edge connection listener
            EdgeConnectorListener = new EdgeConnectorListener(SearchWindowProvider);

            // Add callback method for handling graph changes
            graphViewChanged = OnGraphViewChange;
            OnNodeSelected = m_EditorWindow.OnNodeSelectionChanged;

            // Set up callback for node creation requests
            nodeCreationRequest = ctx =>
            {
                // Ensure the port isn't selected
                SearchWindowProvider.ConnectedPort = null;

                // Open node search window
                SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), SearchWindowProvider);
            };

            // Load the style for the graph grid
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/BehaviourTreeEditor"));

            // Create the background for the graph view
            GridBackground background = new GridBackground();

            // "Infinitely" stretches the background
            background.StretchToParentSize();

            // Ensure background is behind everything else in the graph view
            Insert(0, background);

            if (m_EditorWindow.AssetOpenTree != null) LoadTree(m_EditorWindow.AssetOpenTree);
        }

        /// <summary>Selects all elements in the graph.</summary>
        private void SelectAll()
        {
            // Add all elements in the graph to the selection
            foreach (var element in graphElements.ToList())
                AddToSelection(element);
        }

        private void CreateTree()
        {
            // Create a new serialised tree object
            BehaviourTree newTree = ScriptableObject.CreateInstance<BehaviourTree>();
            newTree.name = "New Behaviour Tree";

            // Create the save directory if it doesn't exist
            if (!Directory.Exists(c_RootPath + "/User/"))
                Directory.CreateDirectory(c_RootPath + "/User/");

            // Check if a file with the new object's name already exists
            if (File.Exists($"{c_RootPath}/User/{newTree.name}.asset"))
            {
                int newFileIndex = 1;

                // Check if incremented name variants exist
                while (File.Exists($"{c_RootPath}/User/{newTree.name} {newFileIndex}.asset"))
                    newFileIndex++;

                // Update name when an increment is free
                newTree.name += $" {newFileIndex}";
            }

            // Create the asset file at in the user folder
            AssetDatabase.CreateAsset(newTree, c_RootPath + "/User/" + newTree.name + ".asset");

            // Save the new asset
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();

            // Select the new tree in the project panel
            Selection.activeObject = newTree;

            // Select the new tree in the editor object field
            CurrentTree = newTree;
            ClearGraph();
            m_RootNode = NodeView.ConstructNode(m_EditorWindow, this, newTree.CreateRoot()) as RootNodeView;
        }

        public void UpdateNodeStates()
        {
            var nodeViews = nodes.ToList();
            nodeViews.ForEach(node =>
            {
                // Update the nodes name when it changes
                var nodeView = node as NodeView;
                nodeView?.UpdateInfo();

                // Unhighlight all nodes
                if (!nodeView.m_Highlight && EditorApplication.isPlaying) (nodeView as TaskNodeView)?.Unhighlight();
            });

            nodeViews.ForEach(node =>
            {
                var nodeView = node as TaskNodeView;
                if (nodeView != null && nodeView.m_Highlight && EditorApplication.isPlaying) (node as TaskNodeView)?.Highlight();
            });
        }

        public void NewTree()
        {
            CreateTree();
        }

        public void SaveTree()
        {
            if (CurrentTree == null)
                CreateTree();
        }

        public void LoadTree(BehaviourTree tree)
        {
            if (CurrentTree == tree)
                return;

            CurrentTree = tree;

            // Clear graph view
            ClearGraph();

            // Create root node
            if (m_RootNode == null)
                m_RootNode = NodeView.ConstructNode(m_EditorWindow, this, CurrentTree.CreateRoot()) as RootNodeView;
            else
                m_RootNode.SetNode(tree.Nodes.Find(itr => itr.GetType() == typeof(Root)));

            // Create all other nodes
            var nodeList = new List<NodeView>();
            CurrentTree.Nodes.ForEach(node =>
            {
                if (node != m_RootNode.Node)
                {
                    var nodeView = NodeView.ConstructNode(m_EditorWindow, this, node);
                    nodeView.AddToGraphView();
                    nodeList.Add(nodeView);
                }
            });

            // Connect all node views to eachother
            m_RootNode.SetChild(nodeList.Find(itr => itr.Node.Parent?.GUID == m_RootNode.Node.GUID));
            if (m_RootNode.Child != null)
            {
                var newEdge = m_RootNode.OutputPort.ConnectTo(m_RootNode.Child.InputPort);
                newEdge.input.portColor = Color.white;
                newEdge.output.portColor = Color.white;
                AddElement(newEdge);

                m_RootNode.ConnectNodes(m_RootNode.Child.InputPort, Direction.Output);
                m_RootNode.Child.ConnectNodes(m_RootNode.OutputPort, Direction.Input);
            }

            nodeList.ForEach(node =>
            {
                var nodeType = node.GetType();
                if (nodeType == typeof(CompositeNodeView))
                {
                    var compNode = node as CompositeNodeView;
                    compNode.AddChildren(nodeList.FindAll(itr => itr.Node.Parent?.GUID == compNode.Node.GUID).ToArray());
                    foreach (var n in compNode.Children)
                    {
                        var edge = node.OutputPort.ConnectTo(n.InputPort);
                        edge.input.portColor = Color.white;
                        edge.output.portColor = Color.white;
                        AddElement(edge);

                        node.ConnectNodes(n.InputPort, Direction.Output);
                        n.ConnectNodes(node.OutputPort, Direction.Input);
                    }

                    compNode.Children.FirstOrDefault()?.RecaluclateExecutionOrder();
                }
                else if (nodeType == typeof(DecoratorNodeView))
                {
                    var decoNode = node as DecoratorNodeView;
                    decoNode.SetChild(nodeList.Find(itr => itr.Node.Parent?.GUID == decoNode.Node.GUID));
                    if (decoNode.Child != null)
                    {
                        var edge = decoNode.OutputPort.ConnectTo(decoNode.Child.InputPort);
                        edge.input.portColor = Color.white;
                        edge.output.portColor = Color.white;
                        AddElement(edge);

                        decoNode.ConnectNodes(decoNode.Child.InputPort, Direction.Output);
                        decoNode.Child.ConnectNodes(decoNode.OutputPort, Direction.Input);
                    }
                }
            });
        }

        /// <summary>Clear all elements from the graph.</summary>
        public void ClearGraph()
        {
            // Select and delete all elements
            if (m_RootNode != null) m_RootNode.capabilities = m_RootNode.capabilities | Capabilities.Deletable;
            DeleteElements(graphElements);
            m_RootNode = null;
        }

        /// <summary>A callback for whenever a key down event is triggered.</summary>
        /// <param name="evt">The key down event.</param>
        public void OnKeyDown(KeyDownEvent evt)
        {
            // Select all with 'Ctrl + A'
            if (evt.ctrlKey && evt.keyCode == KeyCode.A)
                SelectAll();
            // Clear selection with 'Esc'
            else if (!evt.shiftKey && !evt.altKey && !evt.ctrlKey && !evt.commandKey && evt.keyCode == KeyCode.Escape)
                ClearSelection();
            // Focus on elements in the graoh using 'F'
            else if (evt.keyCode == KeyCode.F && !evt.shiftKey && !evt.altKey && !evt.ctrlKey && !evt.commandKey)
            {
                // Focus on selected elements
                if (selection.Count > 0)
                    FrameSelection();
                // Focus on all elements
                else
                    FrameAll();
            }
        }

        public void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 0)
            {
                Vector3 screenMousePosition = evt.localMousePosition;
                Vector2 worldMousePosition = screenMousePosition - contentViewContainer.transform.position;
                worldMousePosition *= 1 / contentViewContainer.transform.scale.x;

                bool mouseOverNode = nodes.Any(node => (node as NodeView).IsMouseOver(worldMousePosition));
                if (!mouseOverNode)
                    OnNodeSelected(null);
            }
        }

        /// <summary>Callback for when the graph is changed.</summary>
        /// <param name="change">The container with all changes made.</param>
        /// <returns>An updated set of changes.</returns>
        private GraphViewChange OnGraphViewChange(GraphViewChange change)
        {
            // Ensure there are elements to remove
            if (change.elementsToRemove != null)
            {
                change.elementsToRemove.ForEach(element =>
                {
                    // Delete any nodes awaiting removal
                    var nodeView = element as NodeView;
                    if (nodeView != null)
                    {
                        // Only delete and disconnect node if it was a user deletion
                        if (CurrentTree == nodeView.Node.Owner)
                            nodeView.OnDelete();

                        // Clear node inspector view
                        OnNodeSelected(null);
                    }

                    // Delete any edges and disconnect its nodes
                    var edge = element as Edge;
                    if (edge != null)
                    {
                        var inputNodeType = (edge?.output as EditorPort)?.Owner.GetType();
                        
                        // Ensure the deletion wasn't caused by a new tree selection
                        if (edge.output != null && (edge?.output as EditorPort)?.Owner.Node.Owner == CurrentTree)
                        {
                            // Disconnect child from parent
                            (edge?.output as EditorPort)?.Disconnect(edge);

                            // Remove connection between this node and parent node
                            if (inputNodeType == typeof(CompositeNodeView))
                                ((edge?.output as EditorPort)?.Owner as CompositeNodeView).RemoveChildren((edge?.input as EditorPort)?.Owner);
                            else if (inputNodeType == typeof(DecoratorNodeView))
                                ((edge?.output as EditorPort)?.Owner as DecoratorNodeView).SetChild(null);
                            else if (inputNodeType.IsSubclassOf(typeof(RootNodeView)))
                                ((edge?.output as EditorPort)?.Owner as RootNodeView).SetChild(null);

                            // Disconnect child from parent
                            (edge?.input as EditorPort)?.Disconnect(edge);
                        }
                    }
                });
            }

            if (change.movedElements != null)
            {
                foreach (var element in change.movedElements)
                {
                    // Delete any nodes awaiting removal
                    NodeView nodeView = element as NodeView;
                    nodeView?.OnMove();
                }
            }

            if (!EditorApplication.isPlaying && CurrentTree != null)
            {
                EditorUtility.SetDirty(CurrentTree);
                AssetDatabase.SaveAssets();
            }
            return change;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            // Create list for all found ports
            var compatiblePorts = new List<Port>();

            // Linq based for loop
            ports.ForEach((port) =>
            {
                // Add ports to list if they have nothing in common with the start port
                if (startPort != port && startPort.node != port.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
    }
}