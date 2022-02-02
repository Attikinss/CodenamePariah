using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WhiteWillow.Editor
{
    public class BlackboardView : UnityEditor.Editor
    {
        private Blackboard m_Target;

        public override void OnInspectorGUI()
        {
            m_Target = BehaviourTreeEditorWindow.Instance.GraphView.CurrentTree.Blackboard;
            m_Target = EditorGUILayout.ObjectField("Blackboard", m_Target, typeof(Blackboard), false) as Blackboard;

            if (m_Target != null)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUIStyle fontStyle = new GUIStyle()
                { alignment = TextAnchor.MiddleCenter, fontSize = 16 };
                fontStyle.normal.textColor = Color.white;
                EditorGUILayout.LabelField("Entries", fontStyle);

                // Draw a list element per entry
                if (m_Target.Entries != null && m_Target.Entries.Count > 0)
                {
                    for (int i = 0; i < m_Target.Entries.Count; i++)
                        DrawEntry(m_Target.Entries[i]);
                }

                if (GUILayout.Button("Add Entry"))
                {
                    m_Target.AddEntry<object>("New Entry", null);
                    EditorGUILayout.Space();
                }

                GUILayout.EndVertical();
            }

            BehaviourTreeEditorWindow.Instance.GraphView.CurrentTree.Blackboard = m_Target;
        }

        private void DrawEntry(BlackboardEntry entry)
        {
            List<System.Reflection.FieldInfo> fields = entry.GetType().GetFields()?.ToList();
            SerializedObject so = new SerializedObject(entry);
            bool delete = false;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUIStyle headerStyle = "DropDownButton";
            entry.Expand = /*true;*/ EditorGUILayout.BeginFoldoutHeaderGroup(entry.Expand, entry.Name, headerStyle, null, headerStyle);
            
            if (entry.Expand)
            {
                WWEditorUtility.DrawLineSeparator();

                foreach (var field in fields)
                {
                    SerializedProperty sp = so.FindProperty(field.Name);

                    if (sp != null)
                    {
                        if (sp.name != "Expand")
                            EditorGUILayout.PropertyField(sp, new GUIContent(field.Name.TrimStart('m', '_')), false);
                    }
                }

                switch (entry.ValueType)
                {
                    case ValueTypes.Bool:
                        if (entry.GetType() != typeof(BoolEntry))
                        {
                            var newEntry = ScriptableObject.CreateInstance<BoolEntry>();
                            newEntry.Expand = entry.Expand;
                            newEntry.Name = entry.Name;
                            newEntry.name = "Bool Entry";
                            newEntry.ReadOnly = entry.ReadOnly;
                            newEntry.ValueType = ValueTypes.Bool;

                            entry = m_Target.ReplaceEntry(entry, newEntry);
                        }
                        else
                            entry.Value = EditorGUILayout.Toggle("Value", (bool)entry.Value);

                        break;

                    case ValueTypes.Float:
                        if (entry.GetType() != typeof(FloatEntry))
                        {
                            var newEntry = ScriptableObject.CreateInstance<FloatEntry>();
                            newEntry.Expand = entry.Expand;
                            newEntry.Name = entry.Name;
                            newEntry.name = "Float Entry";
                            newEntry.ReadOnly = entry.ReadOnly;
                            newEntry.ValueType = ValueTypes.Float;

                            entry = m_Target.ReplaceEntry(entry, newEntry);
                        }
                        else
                            entry.Value = EditorGUILayout.FloatField("Value", (float)entry.Value);

                        break;

                    case ValueTypes.GameObject:
                        if (entry.GetType() != typeof(GameObjectEntry))
                        {
                            var newEntry = ScriptableObject.CreateInstance<GameObjectEntry>();
                            newEntry.Expand = entry.Expand;
                            newEntry.Name = entry.Name;
                            newEntry.name = "GameObject Entry";
                            newEntry.ReadOnly = entry.ReadOnly;
                            newEntry.ValueType = ValueTypes.GameObject;

                            entry = m_Target.ReplaceEntry(entry, newEntry);
                        }
                        else
                            entry.Value = EditorGUILayout.ObjectField("Value", (GameObject)entry.Value, typeof(GameObject), true);

                        break;

                    case ValueTypes.Int:
                        if (entry.GetType() != typeof(IntEntry))
                        {
                            var newEntry = ScriptableObject.CreateInstance<IntEntry>();
                            newEntry.Expand = entry.Expand;
                            newEntry.Name = entry.Name;
                            newEntry.name = "Int Entry";
                            newEntry.ReadOnly = entry.ReadOnly;
                            newEntry.ValueType = ValueTypes.Int;

                            entry = m_Target.ReplaceEntry(entry, newEntry);
                        }
                        else
                            entry.Value = EditorGUILayout.IntField("Value", (int)entry.Value);

                        break;

                    case ValueTypes.String:
                        if (entry.GetType() != typeof(StringEntry))
                        {
                            var newEntry = ScriptableObject.CreateInstance<StringEntry>();
                            newEntry.Expand = entry.Expand;
                            newEntry.Name = entry.Name;
                            newEntry.name = "String Entry";
                            newEntry.ReadOnly = entry.ReadOnly;
                            newEntry.ValueType = ValueTypes.String;

                            entry = m_Target.ReplaceEntry(entry, newEntry);
                        }
                        else
                            entry.Value = EditorGUILayout.TextField("Value", (string)entry.Value);

                        break;

                    case ValueTypes.Vector:
                        if (entry.GetType() != typeof(VectorEntry))
                        {
                            var newEntry = ScriptableObject.CreateInstance<VectorEntry>();
                            newEntry.Expand = entry.Expand;
                            newEntry.Name = entry.Name;
                            newEntry.name = "Vector Entry";
                            newEntry.ReadOnly = entry.ReadOnly;
                            newEntry.ValueType = ValueTypes.Vector;

                            entry = m_Target.ReplaceEntry(entry, newEntry);
                        }
                        else
                            entry.Value = EditorGUILayout.Vector3Field("Value", (Vector3)entry.Value);

                        break;

                    default:
                        if (entry.GetType() != typeof(EmptyEntry))
                        {
                            var empty = ScriptableObject.CreateInstance<EmptyEntry>();
                            empty.Expand = entry.Expand;
                            empty.Name = entry.Name;
                            empty.name = "Empty Entry";
                            empty.ReadOnly = entry.ReadOnly;
                            empty.ValueType = ValueTypes.None;

                            entry = m_Target.ReplaceEntry(entry, empty);
                        }
                        break;
                }

                delete = GUILayout.Button("Delete");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(entry);

            if (delete)
                m_Target.RemoveEntry(entry.Name);
        }
    }
}