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

        public void SetTarget(BaseNode targetNode)
        {
            m_SelectedNode = targetNode;

            var fields = m_SelectedNode?.GetType().GetFields().ToList();
            Debug.Log($"Node: {fields.Count}");

            var hiddenFields = fields?.Where(f => f.GetCustomAttributes(typeof(HideInInspector), true) != null).ToList();
            hiddenFields?.ForEach(f => fields.Remove(f));

            Debug.Log($"Node: {fields.Count}");

            foreach (var field in fields)
                Debug.Log(field.Name);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}