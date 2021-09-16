using System;
using UnityEngine;

public enum ValueTypes { None, Bool, Float, GameObject, Int, String, Vector }

[Serializable]
public abstract class BlackboardEntry : ScriptableObject
{
    public string Name = "New Entry";
    public bool ReadOnly = true;
    public ValueTypes ValueType = ValueTypes.None;
    public abstract object Value { get; set; }

    [HideInInspector]
    public bool Expand = false;

    public T Get<T>()
    {
        T result = (T)Value;
        return result;
    }
}