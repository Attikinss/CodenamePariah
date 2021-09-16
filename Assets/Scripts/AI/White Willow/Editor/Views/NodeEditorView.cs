using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WhiteWillow.Nodes;
using System.Reflection;

namespace WhiteWillow.Editor
{
    [CustomEditor(typeof(BaseNode))]
    public class NodeEditorView : UnityEditor.Editor
    {
        private BaseNode m_SelectedNode;
        private List<FieldInfo> m_FieldElements;

        public void SetTarget(BaseNode targetNode)
        {
            m_SelectedNode = targetNode;

            var fields = m_SelectedNode?.GetType().GetFields().ToList();
            m_FieldElements = new List<FieldInfo>();
            foreach (var field in fields)
            {
                if ((field.IsPublic && !field.CustomAttributes.Any(attrib => attrib.AttributeType == typeof(HideInInspector))) ||
                    (!field.IsPublic && field.CustomAttributes.Any(attrib => attrib.AttributeType == typeof(SerializeField))))
                {
                    m_FieldElements.Add(field);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            // Iterate backwards because of the way elements are
            // added to the list during the "discovery" pahse 
            for (int i = m_FieldElements.Count - 1; i >= 0; i--)
            {
                // Obtain an editable property version of the current field
                SerializedObject serialisedObj = new SerializedObject(m_SelectedNode);
                SerializedProperty property = serialisedObj.FindProperty(m_FieldElements[i].Name);

                // Remove hungarian notation for presentation
                string formattedName = m_FieldElements[i].Name.TrimStart('m', '_');

                if (IsNodeMember(m_FieldElements[i]))
                {
                    // Get the current value of the field
                    object untypedMember = m_FieldElements[i].GetValue(m_SelectedNode);
                    FieldInfo blackboardValueField = m_FieldElements[i].FieldType?.GetField("BlackboardValue");
                    FieldInfo expandField = m_FieldElements[i].FieldType?.GetField("Expand");

                    bool foldOut = (bool)expandField.GetValue(untypedMember);
                    expandField.SetValue(untypedMember, EditorGUILayout.Foldout(foldOut, formattedName, true));

                    if (foldOut)
                    {
                        bool isBlackboardValue = (bool)blackboardValueField.GetValue(untypedMember);
                        blackboardValueField.SetValue(untypedMember, EditorGUILayout.Toggle(blackboardValueField.Name, isBlackboardValue));

                        if (isBlackboardValue)
                        {
                            // Get the expected value type of the entry
                            System.Type entryGeneric = untypedMember.GetType().GetGenericArguments()[0];                            

                            // Retrieve all values available in the blackboard of the same type
                            var matchingBlackboardEntries = m_SelectedNode.Owner.Blackboard?.GetEntriesOfType(entryGeneric);

                            CreateBlackboardField(untypedMember, matchingBlackboardEntries);
                        }
                        else
                        {
                            // Yikes...

                            FieldInfo defaultValueField = m_FieldElements[i].FieldType?.GetField("Value");
                            if (defaultValueField.FieldType == typeof(bool))
                            {
                                bool value = (bool)defaultValueField.GetValue(untypedMember);
                                defaultValueField.SetValue(untypedMember, EditorGUILayout.Toggle(defaultValueField.Name, value));
                            }
                            else if (defaultValueField.FieldType == typeof(float))
                            {
                                float value = (float)defaultValueField.GetValue(untypedMember);
                                defaultValueField.SetValue(untypedMember, EditorGUILayout.FloatField(defaultValueField.Name, value));
                            }
                            else if (defaultValueField.FieldType == typeof(GameObject))
                            {
                                GameObject value = (GameObject)defaultValueField.GetValue(untypedMember);
                                defaultValueField.SetValue(untypedMember,
                                    EditorGUILayout.ObjectField(defaultValueField.Name, value, typeof(GameObject), true));
                            }
                            else if (defaultValueField.FieldType == typeof(int))
                            {
                                int value = (int)defaultValueField.GetValue(untypedMember);
                                defaultValueField.SetValue(untypedMember, EditorGUILayout.IntField(defaultValueField.Name, value));
                            }
                            else if (defaultValueField.FieldType == typeof(string))
                            {
                                string value = (string)defaultValueField.GetValue(untypedMember);
                                defaultValueField.SetValue(untypedMember, EditorGUILayout.TextField(defaultValueField.Name, value));
                            }
                            else if (defaultValueField.FieldType == typeof(Vector3))
                            {
                                Vector3 value = (Vector3)defaultValueField.GetValue(untypedMember);
                                defaultValueField.SetValue(untypedMember, EditorGUILayout.Vector3Field(defaultValueField.Name, value));
                            }
                        }
                    }
                }
                else
                {
                    // Draw the field and update value
                    EditorGUILayout.PropertyField(property, new GUIContent(formattedName), true);
                    serialisedObj.ApplyModifiedProperties();
                }
            }
        }

        public static void CreateBlackboardField(object member, List<BlackboardEntry> entries = null)
        {
            if (entries == null || entries.Count == 0)
            {
              string[] dummy = { "-" };
              EditorGUILayout.Popup("Value", 0, dummy);
            }
            else
            {
                List<string> menuChoices = new List<string>();
                foreach (var item in entries)
                    menuChoices.Add(item.Name);
                
                FieldInfo selectionInfo = member?.GetType()?.GetField("Selection");
                if (selectionInfo != null)
                {
                    int selection = (int)selectionInfo.GetValue(member);
                    selectionInfo.SetValue(member, EditorGUILayout.Popup("Value", selection, menuChoices.ToArray()));

                    FieldInfo valueInfo = member.GetType().GetField("Value");

                    valueInfo.SetValue(member, entries[selection].Value);
                }
            }
        }

        private bool IsNodeMember(FieldInfo field)
        {
            return field.FieldType.Name.Contains("NodeMember");
        }
    }
}