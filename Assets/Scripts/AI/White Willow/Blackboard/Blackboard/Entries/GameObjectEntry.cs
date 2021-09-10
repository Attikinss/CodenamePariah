using System;
using UnityEngine;

[Serializable]
public class GameObjectEntry : BlackboardEntry
{
    public override object Value { get => GameObjectValue; set => GameObjectValue = (GameObject)value; }

    [SerializeField]
    private GameObject GameObjectValue = null;
}