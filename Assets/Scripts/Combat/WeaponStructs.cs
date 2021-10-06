using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


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

	// Weapon bob sway stuff.
	[HideInInspector]
	public float m_SwayTimer;
	[HideInInspector]
	public float m_WaveSlice;
	[HideInInspector]
	public float m_WaveSliceX;

	// m_BobSpeed = 5;
	// m_BobDistance = 0.05f;
}

[System.Serializable]
public struct WeaponAction
{
	public bool m_IsFiring;
	public bool m_IsAiming; // m_IsAiming can also be used to track if the left gun is firing.
	public bool m_IsReloading;

	public bool m_IsReloadingLeft;

	// both false on default i guess.
}

[System.Serializable]
public struct RecoilTest
{
	public float m_RecoilTestIntervals;
	public float m_RecoilTestRestTime;

	[HideInInspector]
	public bool m_IsRecoilTesting;
	[HideInInspector]
	public bool m_IsTestResting;
	[HideInInspector]
	public float m_RecoilTestCounter;

	// m_RecoilTestIntervals = 3;
	// m_RecoilTestRestTime = 2;

}

public struct RecoilInfo
{
	[HideInInspector]
	public float m_VerticalProgress;

	[HideInInspector]
	public Vector3 m_WeaponRecoilRot;

	//[HideInInspector]
	//public Vector3 m_OriginalLocalPosition;
	//[HideInInspector]
	//public Vector3 m_OriginalGlobalPosition;
}

public struct WeaponTransformInfo
{
	[HideInInspector]
	public Vector3 m_OriginalLocalPosition;
	[HideInInspector]
	public Vector3 m_OriginalGlobalPosition;
}

[System.Serializable]
public struct ParticleEffects
{
	public List<VisualEffect> m_MuzzleFlashes;
	public List<ParticleSystem> m_BulletCasings;
}

[System.Serializable]
public struct Animators
{
	public List<Animator> m_GunAnimators;
	public List<Animator> m_ArmsAnimators;
}
