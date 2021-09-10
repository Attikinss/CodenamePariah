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

        public BehaviourTreeGraphView GraphView { get; private set; }
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

            m_FileMenu.menu.AppendAction("New", ctx => { GraphView.NewTree(); AddTreesToTreeMenu(); });
            m_FileMenu.menu.AppendAction("Save As New", ctx => { GraphView.NewTree(); AddTreesToTreeMenu(); });
        }

        private void CreateTreeMenu()
        {
            m_TreeMenu = rootVisualElement.Q<ToolbarMenu>("TreeMenu");
        }

        private void CreateGraphView()
        {
            GraphView = null;
            GraphView = rootVisualElement.Q<BehaviourTreeGraphView>();
            GraphView.Construct(this);
        }

        private void AddTreesToTreeMenu()
        {
            m_TreeMenu.menu.MenuItems().Clear();

            var trees = FindAllTrees();
            trees.ForEach(tree => m_TreeMenu.menu.AppendAction(tree.name, ctx =>
            {
                GraphView.LoadTree(tree);
                m_Inspector.CreateBlackboardView();
            }));
        }

        private void CreateInspectorPanel()
        {
            ToolbarToggle properties = rootVisualElement.Q<ToolbarToggle>("PropertiesToggle");
            ToolbarToggle blackboard = rootVisualElement.Q<ToolbarToggle>("BlackboardToggle");
            ToolbarToggle settings = rootVisualElement.Q<ToolbarToggle>("SettingsToggle");

            properties?.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    blackboard.value = false;
                    settings.value = false;
                    m_Inspector.SelectPropertyEditor();
                }
                else
                {
                    if (!blackboard.value && !settings.value)
                        properties.SetValueWithoutNotify(true);
                }
            });

            blackboard?.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    properties.value = false;
                    settings.value = false;
                    m_Inspector.SelectBlackboardEditor();
                }
                else
                {
                    if (!properties.value && !settings.value)
                        blackboard.SetValueWithoutNotify(true);
                }
            });

            settings?.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    properties.value = false;
                    blackboard.value = false;
                    m_Inspector.SelectSettingsEditor();
                }
                else
                {
                    if (!properties.value && !blackboard.value)
                        settings.SetValueWithoutNotify(true);
                }
            });

            m_Inspector.SelectPropertyEditor();
        }

        public void OnNodeSelectionChanged(NodeView node)
        {
            m_Inspector.UpdateNodeSelection(node);
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

            // Ensures that if the editor window is opening after Unity recompiles
            // the instance isn't lost/reset and dependents don't freak out
            if (Instance == null)
                Instance = this;

            Rebuild();
        }

        private void Rebuild()
        {
            CreateGraphView();
            CreateToolbarMenu();
            CreateTreeMenu();
            AddTreesToTreeMenu();
            CreateInspectorPanel();

            EditorApplication.playModeStateChanged += ModeChanged;
        }

        private void ModeChanged(PlayModeStateChange change)
        {
            GraphView.CurrentTree = null;
            GraphView.ClearGraph();
            AddTreesToTreeMenu();
            CreateInspectorPanel();
        }

        private void Update()
        {
            GraphView?.UpdateNodeStates();
        }

        public void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (AssetOpenTree != null)
            {
                GraphView.LoadTree(AssetOpenTree);
                CreateInspectorPanel();
            }

            AssetOpenTree = null;
            MousePosition = Event.current.mousePosition;
        }

        private void OnSelectionChange()
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (tree != null)
            {
                GraphView.LoadTree(tree);
                CreateInspectorPanel();
            }
        }

        private List<BehaviourTree> FindAllTrees()
        {
            List<BehaviourTree> assets;
            
            if (!EditorApplication.isPlaying)
                assets = Utility.GetAllAssetsOfType<BehaviourTree>();
            else
            {
                assets = new List<BehaviourTree>();

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
                            Debug.Log(item);
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