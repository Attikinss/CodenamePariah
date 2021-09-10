using System;
using UnityEngine;

[Serializable]
public class StringEntry : BlackboardEntry
{
    public override object Value { get => StringValue; set => StringValue = (string)value; }

    [SerializeField]
    private string StringValue = "";
}