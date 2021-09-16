using System;
using UnityEngine;

[Serializable]
public class FloatEntry : BlackboardEntry
{
    public override object Value { get => FloatValue; set => FloatValue = (float)value; }

    [SerializeField]
    private float FloatValue = 0.0f;
}