using System;
using UnityEngine;

[Serializable]
public class IntEntry : BlackboardEntry
{
    public override object Value { get => IntValue; set => IntValue = (int)value; }

    [SerializeField]
    private int IntValue = 0;
}