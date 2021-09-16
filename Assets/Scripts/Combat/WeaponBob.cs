using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Since the Weapon.cs script has a bunch of variables that could be abstracted out, I'll try put all the structs in this one file.
// This will prevent the making of a bunch of different scripts which will just take up space.


/// <summary>
/// WeaponBob contains information related to weapon bobbing. It's just to abstract some variables out of the Weapon.cs script.
/// </summary>
[System.Serializable]
public struct WeaponBob
{
	public float m_BobSpeed;
	[Range(0, 1)]
	public float m_BobDistance;
}

public struct WeaponAction
{
	public bool m_IsFiring;
	public bool m_IsAiming;
}
