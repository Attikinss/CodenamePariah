using System;
using UnityEngine;

[Serializable]
public class VectorEntry : BlackboardEntry
{
    public override object Value { get => VectorValue; set => VectorValue = (Vector3)value; }

    [SerializeField]
    private Vector3 VectorValue = Vector3.zero;
}