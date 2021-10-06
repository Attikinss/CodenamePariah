using UnityEngine;
using UnityEngine.UIElements;

namespace WhiteWillow.Editor
{
    public class AgentPropertyView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AgentPropertyView, UxmlTraits> { }

        private UnityEditor.Editor m_PropertyEditor;

        public void UpdateSelection(NodeView nodeView)
        {
            Clear();

            if (nodeView == null)
                return;

            Object.DestroyImmediate(m_PropertyEditor);
            m_PropertyEditor = UnityEditor.Editor.CreateEditor(nodeView.Node, typeof(BehaviourTree));

            IMGUIContainer container = new IMGUIContainer(() => { m_PropertyEditor.OnInspectorGUI(); });
            container.style.marginTop = 5;
            container.style.marginLeft = 5;
            container.style.minWidth = 250;
            Add(container);
        }

        public void Update()
        {
            //m_PropertyEditor?.Repaint();
        }
    }
}