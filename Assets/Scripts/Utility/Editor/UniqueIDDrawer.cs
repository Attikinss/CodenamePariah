using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
[CustomPropertyDrawer (typeof(UniqueIdentifierAttribute))]
public class UniqueIDDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.stringValue == "")
		{
			Guid guid = Guid.NewGuid();
			property.stringValue = guid.ToString();
		}

		Rect textFieldPosition = position;
		textFieldPosition.height = 16;
		DrawLabelField(textFieldPosition, property, label);
	}

	void DrawLabelField(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.LabelField(position, label, new GUIContent(property.stringValue));
	}
}
