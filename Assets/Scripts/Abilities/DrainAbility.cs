using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DrainAbility
{
    // Temporary host drain ability stuff.
    // These are integers because the Inventory.cs script has health stored as an integer.
    public int damage;         // By setting them to the same value, its a 1:1 ratio of drain/restoration.
    public int restore;
    [Range(0, 2)]
    public float drainInterval;

    public float drainCounter;
    public bool isDraining;



    // drain damage 2
    // drain restore 2
    // drain interval 0.15

}
