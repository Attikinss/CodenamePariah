using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using WhiteWillow.Nodes;

namespace WhiteWillow.Editor
{
    [CustomEditor(typeof(BaseNode))]
    public class NodeEditorView : UnityEditor.Editor
    {
        private BaseNode m_SelectedNode;
        private VisualElement m_RootVisualElement;

        private List<VisualElement> m_FieldElements;

        public NodeEditorView(Object target)
        {
            m_FieldElements = new List<VisualElement>();
            m_SelectedNode = target as BaseNode;
            m_RootVisualElement = new VisualElement();

            Debug.Log("Noice");
        }

        public override VisualElement CreateInspectorGUI()
        {
            var fields = m_SelectedNode?.GetType().GetFields(System.Reflection.BindingFlags.Public).ToList();
            var hiddenFields = fields?.Where(field => field.GetCustomAttributes(typeof(HideInInspector), true) != null).ToList();
            hiddenFields?.ForEach(field => fields.Remove(field));

            foreach (var field in fields)
            {
                Debug.Log(field.FieldType.Name);
            }

            return m_RootVisualElement;
        }
    }
}