using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DeathIncarnateAbility
{
    //DrainSettings()
    //{
    //    deathIncarnateDamage = 100;
    //    deathIncarnateRadius = 10;
    //    deathIncarnateCooldown = 5;
    //    deathIncarnateDelay = 0.75f;
    //    deathIncarnateRequiredHold = 0.75f;
    //
    //    drawingDeathIncarnate = false;
    //    deathIncarnatePos = Vector3.zero;
    //    deathIncarnateUsed = false;
    //}

    // temporary death incarnate ability stuff.
    public int requiredKills;
    public int deathIncarnateDamage;
    public float deathIncarnateRadius;
    public float deathIncarnateCooldown;
    [Tooltip("The time it takes for the ability to begin after activating.")]
    [Range(0, 5)]
    public float deathIncarnateDelay;
    [Tooltip("The required time needed to hold the button before activating the ability.")]
    [Range(0, 2)]
    public float deathIncarnateRequiredHold;

    
    public bool drawingDeathIncarnate; // Will be used to draw a sphere for death incarnate for a few seconds after being used.
    public Vector3 deathIncarnatePos; // Cached pos of last Death Incarnate.

    // public for now so I can display it on my UI HUD thing.
    public bool deathIncarnateUsed;


    // testing couroutines.
    [HideInInspector]
    public Coroutine chargeRoutine;
    public bool hasRoutineStarted;


    public bool IsActive; // Tracks whether the ability is being performed. This is so we can prevent other abilities from
                                  // happening while this one is happening.
}

// damage - 100;
// radius - 10;
// cooldown - 5;
// delay - 0.75f;
// requiredHold - 0.75f;
