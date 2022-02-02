using UnityEngine;
using UnityEngine.UIElements;

namespace WhiteWillow.Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        private NodeEditorView m_PropertyEditor;
        private BlackboardView m_BlackboardEditor;
        private NodeEditorView m_SettingsEditor;

        public bool DrawPropertyView { get; private set; }
        public bool DrawBlackboardView { get; private set; }
        public bool DrawSettingsView { get; private set; }

        public void UpdateNodeSelection(NodeView nodeView)
        {
            CreateBlackboardView();

            if (nodeView == null)
                return;

            Object.DestroyImmediate(m_PropertyEditor);
            m_PropertyEditor = UnityEditor.Editor.CreateEditor(nodeView.Node, typeof(NodeEditorView)) as NodeEditorView;
            m_PropertyEditor.SetTarget(nodeView.Node);
            CreatePropertyView();
        }

        private void CreatePropertyView()
        {
            AddToInspector(() =>
            {
                if (DrawPropertyView)
                    m_PropertyEditor.OnInspectorGUI();
            });
        }

        public void CreateBlackboardView()
        {
            Clear();
            Object.DestroyImmediate(m_BlackboardEditor);

            Blackboard blackboard = null;
            if (BehaviourTreeEditorWindow.Instance != null && BehaviourTreeEditorWindow.Instance.GraphView.CurrentTree != null)
                blackboard = BehaviourTreeEditorWindow.Instance.GraphView.CurrentTree.Blackboard;

            m_BlackboardEditor = UnityEditor.Editor.CreateEditor(blackboard, typeof(BlackboardView)) as BlackboardView;

            AddToInspector(() =>
            {
                if (DrawBlackboardView)
                {
                    if (blackboard)
                        m_BlackboardEditor.OnInspectorGUI();
                    else
                    {
                        if (BehaviourTreeEditorWindow.Instance.GraphView.CurrentTree)
                        {
                            Blackboard newValue = UnityEditor.EditorGUILayout.ObjectField("Blackboard",
                            BehaviourTreeEditorWindow.Instance.GraphView.CurrentTree.Blackboard, typeof(Blackboard), false) as Blackboard;

                            if (newValue != blackboard)
                                BehaviourTreeEditorWindow.Instance.GraphView.CurrentTree.Blackboard = newValue;
                        }
                    }
                }
            });
        }

        private void AddToInspector(System.Action func)
        {
            IMGUIContainer container = new IMGUIContainer(func);

            container.style.marginTop = 5;
            container.style.marginLeft = 5;
            container.style.marginRight = 5;
            container.style.minWidth = 250;
            Add(container);
        }

        public void Update()
        {
            m_PropertyEditor?.Repaint();
            m_BlackboardEditor?.Repaint();
        }

        public void SelectPropertyEditor()
        {
            DrawPropertyView = true;
            DrawBlackboardView = false;
            DrawSettingsView = false;
        }

        public void SelectBlackboardEditor()
        {
            DrawPropertyView = false;
            DrawBlackboardView = true;
            DrawSettingsView = false;
        }

        public void SelectSettingsEditor()
        {
            DrawPropertyView = false;
            DrawBlackboardView = false;
            DrawSettingsView = true;
        }
    }
}