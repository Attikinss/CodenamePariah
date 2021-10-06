using System;
using UnityEngine;

[Serializable]
public class BoolEntry : BlackboardEntry
{
    public override object Value { get => BoolValue; set => BoolValue = (bool)value; }

    [SerializeField]
    private bool BoolValue = false;
}