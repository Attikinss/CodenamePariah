using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Achievements is not what is sounds. It's a struct to store special events like the first time the player controls a soldier, scientist, ect...
/// </summary>
public struct Achievements
{
    public bool hasEnteredScientist { get; private set; }
    public bool hasEnteredSoldier { get; private set; }

    public void EnteredScientist() { hasEnteredScientist = true; }
    public void EnteredSoldier() { hasEnteredSoldier = true; }
}
