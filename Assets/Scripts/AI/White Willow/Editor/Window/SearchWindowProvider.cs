using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteWillow.Nodes;

namespace WhiteWillow.Editor
{
    /// <summary>A container of information to display entries in a search window.</summary>
    public struct NodeEntry
    {
        /// <summary>The display name of the node entry in the search window.</summary>
        public string[] Category;

        /// <summary>The node that will be created on the graph.</summary>
        public NodeView Node;
        
        /// <summary>The name/ID of which port(s) of the displayed nodes can be connected to.</summary>
        public string CompatiblePortID;
    }

    /// <summary></summary>
    public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        /// <summary>The port that will connect to the port of any nodes created via the search window.</summary>
        public EditorPort ConnectedPort { get; set; }

        /// <summary>The graph view the node is active in.</summary>
        private BehaviourTreeGraphView m_GraphView;

        /// <summary>The editor window the node is active in.</summary>
        private BehaviourTreeEditorWindow m_EditorWindow;

        /// <summary>Used to trick the search window into indenting items.</summary>
        private Texture2D m_Icon;

        /// <summary>Temporary list containing ports to connect to of a given node.</summary>
        private List<EditorPort> m_Ports = new List<EditorPort>();

        /// <summary>Sets up the connections between the provider, the editor window and graph view.</summary>
        /// <param name="editorWindow">The active editor window.</param>
        /// <param name="graphView">The active graph view.</param>
        public void Initialise(BehaviourTreeEditorWindow editorWindow, BehaviourTreeGraphView graphView)
        {
            m_EditorWindow = editorWindow;
            m_GraphView = graphView;

            // Transparent icon to trick search window into indenting items
            m_Icon = new Texture2D(1, 1);
            m_Icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            m_Icon.Apply();
        }

        private void OnDestroy()
        {
            if (m_Icon != null)
            {
                DestroyImmediate(m_Icon);
                m_Icon = null;
            }
        }

        /// <summary></summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // First build up temporary data structure containing group & title as an array of strings (the last one is the actual title) and associated node type.
            var nodeEntries = new List<NodeEntry>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BaseNode)))
                    {
                        var attrs = type.GetCustomAttributes(typeof(CategoryAttribute), false) as CategoryAttribute[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            var node = (BaseNode)ScriptableObject.CreateInstance(type);
                            var nodeView = NodeView.ConstructNode(m_EditorWindow, m_GraphView, node);
                            AddEntries(nodeView, attrs[0].Category, nodeEntries);
                        }
                    }
                }
            }

            // Sort the entries lexicographically by group then title with the requirement that items always comes before sub-groups in the same group.
            // Example result:
            // - Art/BlendMode
            // - Art/Adjustments/ColorBalance
            // - Art/Adjustments/Contrast
            nodeEntries.Sort((entry1, entry2) =>
            {
                for (var i = 0; i < entry1.Category.Length; i++)
                {
                    if (i >= entry2.Category.Length)
                        return 1;
                    var value = entry1.Category[i].CompareTo(entry2.Category[i]);
                    if (value != 0)
                    {
                        // Make sure that leaves go before nodes
                        if (entry1.Category.Length != entry2.Category.Length && (i == entry1.Category.Length - 1 || i == entry2.Category.Length - 1))
                            return entry1.Category.Length < entry2.Category.Length ? -1 : 1;
                        return value;
                    }
                }
                return 0;
            });

            //* Build up the data structure needed by SearchWindow.

            // `groups` contains the current group path we're in.
            var groups = new List<string>();

            // First item in the tree is the title of the window.
            var tree = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };

            foreach (var nodeEntry in nodeEntries)
            {
                // `createIndex` represents from where we should add new group entries from the current entry's group path.
                var createIndex = int.MaxValue;

                // Compare the group path of the current entry to the current group path.
                for (var i = 0; i < nodeEntry.Category.Length; i++)
                {
                    var group = nodeEntry.Category[i];
                    if (i >= groups.Count)
                    {
                        // The current group path matches a prefix of the current entry's group path, so we add the
                        // rest of the group path from the currrent entry.
                        createIndex = i;
                        break;
                    }
                    if (groups[i] != group)
                    {
                        // A prefix of the current group path matches a prefix of the current entry's group path,
                        // so we remove everyfrom from the point where it doesn't match anymore, and then add the rest
                        // of the group path from the current entry.
                        groups.RemoveRange(i, groups.Count - i);
                        createIndex = i;
                        break;
                    }
                }

                // Create new group entries as needed.
                // If we don't need to modify the group path, `createIndex` will be `int.MaxValue` and thus the loop won't run.
                for (var i = createIndex; i < nodeEntry.Category.Length; i++)
                {
                    var group = nodeEntry.Category[i];
                    groups.Add(group);
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(group)) { level = i + 1 });
                }

                // Finally, add the actual entry.
                tree.Add(new SearchTreeEntry(new GUIContent(nodeEntry.Node.Node.GetType().Name, m_Icon)) { level = nodeEntry.Category.Length + 1, userData = nodeEntry });
            }

            return tree;
        }

        private void AddEntries(NodeView node, string[] category, List<NodeEntry> nodeEntries)
        {
            if (ConnectedPort == null)
            {
                nodeEntries.Add(new NodeEntry
                {
                    Node = node,
                    Category = category,
                    CompatiblePortID = ""
                });
                return;
            }

            m_Ports.Clear();
            m_Ports = node.GetPorts();

            bool hasSingleSlot = m_Ports.Count(port => PortTypeMatch(port)) == 1;
            m_Ports.RemoveAll(port =>
            {
                if (port == null)
                    return false;
                
                var portConfig = port.Description;
                return !portConfig.IsCompatibleWith(ConnectedPort.Description);
            });

            if (hasSingleSlot && m_Ports.Count == 1)
            {
                nodeEntries.Add(new NodeEntry
                {
                    Node = node,
                    Category = category,
                    CompatiblePortID = m_Ports.First().Description.DisplayName
                });
                return;
            }

            foreach (var port in m_Ports)
            {
                if (port == null)
                    continue;

                var entryTitle = new string[category.Length];
                category.CopyTo(entryTitle, 0);
                //entryTitle[entryTitle.Length - 1] += ": " + port.Description.m_DisplayName;
                nodeEntries.Add(new NodeEntry
                {
                    Category = entryTitle,
                    Node = node,
                    CompatiblePortID = port.Description.MemberName
                });
            }
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            var nodeEntry = (NodeEntry)entry.userData;
            var node = nodeEntry.Node;

            var windowMousePosition = m_EditorWindow.rootVisualElement.ChangeCoordinatesTo(m_EditorWindow.rootVisualElement.parent, context.screenMousePosition - m_EditorWindow.position.position);
            var graphMousePosition = m_GraphView.contentViewContainer.WorldToLocal(windowMousePosition);

            node.SetNode(m_GraphView.CurrentTree?.CreateNode(node.Node));
            node.Position = new Rect(graphMousePosition, Vector2.zero);

            if (ConnectedPort != null)
            {
                if (ConnectedPort.capacity == Port.Capacity.Single)
                    ConnectedPort.ClearConnections();

                if (ConnectedPort.Description.IsInputPort)
                {
                    m_GraphView.AddElement(ConnectedPort.ConnectTo(node.OutputPort));

                    ConnectedPort.Owner.ConnectNodes(node.OutputPort, Direction.Input);
                    node.OutputPort.Owner.ConnectNodes(ConnectedPort, Direction.Output);

                    (ConnectedPort.node as NodeView).SetParent(node);
                }
                else
                {
                    m_GraphView.AddElement(ConnectedPort.ConnectTo(node.InputPort));

                    node.InputPort.Owner.ConnectNodes(ConnectedPort, Direction.Input);
                    ConnectedPort.Owner.ConnectNodes(node.InputPort, Direction.Output);

                    node.SetParent((NodeView)ConnectedPort.node);
                }
            }

            node.AddToGraphView();

            return true;
        }

        private bool PortTypeMatch(EditorPort port)
        {
            if (port == null)
                return false;

            return port.Description.IsOutputPort != ConnectedPort.Description.IsOutputPort;
        }
    }
}