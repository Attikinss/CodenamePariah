using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace WhiteWillow.Editor
{
    public class BehaviourTreeEditorWindow : EditorWindow
    {
        public static BehaviourTreeEditorWindow Instance { get; private set; }

        private BehaviourTreeGraphView m_GraphView;
        private InspectorView m_Inspector;
        public Vector2 MousePosition = Vector2.zero;

        private ToolbarMenu m_FileMenu;
        private ToolbarMenu m_TreeMenu;
        public BehaviourTree AssetOpenTree;

        /// <summary>Opens the editor window via menu item.</summary>
        [MenuItem("Tools/White Willow/Tree Editor")]
        public static void OpenWindow()
        {
            // Open a new tree editor window
            Instance = GetWindow<BehaviourTreeEditorWindow>("Behaviour Tree Editor");

            // Set window min size
            Instance.minSize = new Vector2(400, 300);
        }

        /// <summary>Opens the editor window via asset open.</summary>
        [OnOpenAsset()]
        public static bool OpenWindow(int id, int line)
        {
            // Get a unity object from the ID of the opened asset
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(id);

            if (item is BehaviourTree)
            {
                // Open a new tree editor window
                Instance = GetWindow<BehaviourTreeEditorWindow>("Behaviour Tree Editor");

                // Set window min size
                Instance.minSize = new Vector2(400, 300);

                // Set the current tree object field value as the opened asset
                Instance.AssetOpenTree = item as BehaviourTree;

                return true;
            }

            return false;
        }

        private void CreateToolbarMenu()
        {
            m_FileMenu = rootVisualElement.Q<ToolbarMenu>("FileMenu");

            m_FileMenu.menu.AppendAction("New", ctx => { m_GraphView.NewTree(); AddTreesToTreeMenu(); });
            m_FileMenu.menu.AppendAction("Save As New", ctx => { m_GraphView.NewTree(); AddTreesToTreeMenu(); });
        }

        private void CreateTreeMenu()
        {
            m_TreeMenu = rootVisualElement.Q<ToolbarMenu>("TreeMenu");
        }

        private void CreateGraphView()
        {
            m_GraphView = null;
            m_GraphView = rootVisualElement.Q<BehaviourTreeGraphView>();
            m_GraphView.Construct(this);
        }

        private void AddTreesToTreeMenu()
        {
            m_TreeMenu.menu.MenuItems().Clear();

            var trees = FindAllTrees();
            trees.ForEach(tree => m_TreeMenu.menu.AppendAction(tree.name, ctx => m_GraphView.LoadTree(tree)));
        }

        public void OnNodeSelectionChanged(NodeView node)
        {
            m_Inspector.UpdateSelection(node);
        }

        private void OnEnable()
        {
            // Import UXML
            var visualTree = Resources.Load<VisualTreeAsset>("Styles/BehaviourTreeEditor");
            visualTree.CloneTree(rootVisualElement);

            // Import style sheet
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("Styles/BehaviourTreeEditor"));

            // Query for graph view and inspector
            m_Inspector = rootVisualElement.Q<InspectorView>();

            CreateGraphView();
            CreateToolbarMenu();
            CreateTreeMenu();
            AddTreesToTreeMenu();

            EditorApplication.playModeStateChanged += ModeChanged;
        }

        private void ModeChanged(PlayModeStateChange change)
        {
            m_GraphView.CurrentTree = null;
            m_GraphView.ClearGraph();
            AddTreesToTreeMenu();
        }

        private void Update()
        {
            m_GraphView?.UpdateNodeStates();

            Repaint();
        }

        private void OnGUI()
        {
            if (AssetOpenTree != null)
                m_GraphView.LoadTree(AssetOpenTree);

            AssetOpenTree = null;
            MousePosition = Event.current.mousePosition;
        }

        private void OnSelectionChange()
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (tree != null)
                m_GraphView.LoadTree(tree);
        }

        private List<BehaviourTree> FindAllTrees()
        {
            List<BehaviourTree> assets = new List<BehaviourTree>();
            
            if (!EditorApplication.isPlaying)
            {
                string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(BehaviourTree)));
                for (int i = 0; i < guids.Length; i++)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                    BehaviourTree asset = AssetDatabase.LoadAssetAtPath<BehaviourTree>(assetPath);
                    if (asset != null)
                        assets.Add(asset);
                }
            }
            else
            {
                Assembly defaultAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp");
                List<Type> scriptsWithTrees = defaultAssembly?.GetTypes().Where(script => script.IsClass && script.IsSubclassOf(typeof(UnityEngine.Object))).ToList();

                scriptsWithTrees.ForEach(script =>
                {
                    List<UnityEngine.Object> objectsWithScript = FindObjectsOfType(script).ToList();
                    objectsWithScript.ForEach(item =>
                    {
                        var treeField = item.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList().FindAll(field =>
                        field.FieldType == typeof(BehaviourTree) && field.Name == "m_RuntimeTree").FirstOrDefault();
                        if (treeField != null)
                        {
                            var tree = treeField.GetValue(item);
                            assets.Add(tree as BehaviourTree);
                        }
                    });
                });
            }

            return assets;
        }
    }
}