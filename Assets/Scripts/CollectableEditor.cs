using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// hi
[CustomEditor(typeof(Collectable))]
public class CollectableEditor : Editor
{
	SerializedProperty m_ItemProp;
	SerializedProperty m_RotationSpeedProp;
	SerializedProperty m_BobHeightProp;
	SerializedProperty m_BobSpeedProp;
	SerializedProperty m_StartPosProp;
	SerializedProperty m_ActionProp;
	SerializedProperty m_RequiredTargetProp;

	private void OnEnable()
	{
		m_ItemProp = serializedObject.FindProperty("m_ItemPrefab");
		m_RotationSpeedProp = serializedObject.FindProperty("m_RotationSpeed");
		m_BobHeightProp = serializedObject.FindProperty("m_BobHeight");
		m_BobSpeedProp = serializedObject.FindProperty("m_BobSpeed");
		m_StartPosProp = serializedObject.FindProperty("m_StartPosition");
		m_ActionProp = serializedObject.FindProperty("m_Action");
		m_RequiredTargetProp = serializedObject.FindProperty("m_RequiredTarget");
	}

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();

		Collectable script = target as Collectable;
		EditorGUILayout.PropertyField(m_ActionProp, new GUIContent("Type of Action"));
		EditorGUILayout.PropertyField(m_ItemProp, new GUIContent("Obtainable Item"));

		//EditorGUILayout.BeginToggleGroup("Target To Upgrade", script.m_Action == CollectableAction.UPGRADE_WEAPON);
		if (script.m_Action == CollectableAction.UPGRADE_WEAPON)
		EditorGUILayout.PropertyField(m_RequiredTargetProp, new GUIContent("Prerquisite"));
		//EditorGUILayout.EndToggleGroup();

		EditorGUILayout.Space(20);
		EditorGUILayout.Separator();

		EditorGUILayout.PropertyField(m_RotationSpeedProp, new GUIContent("Rotation Speed"));
		EditorGUILayout.PropertyField(m_BobHeightProp, new GUIContent("Bob Height"));
		EditorGUILayout.PropertyField(m_BobSpeedProp, new GUIContent("Bob Speed"));
		//EditorGUILayout.PropertyField(m_StartPosProp, new GUIContent("Start Position"));




		serializedObject.ApplyModifiedProperties();

		
	}
}
