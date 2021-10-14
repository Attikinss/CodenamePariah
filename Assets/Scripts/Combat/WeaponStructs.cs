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

	public ParticleSystem m_BulletParticle;

	public ParticleSystem m_AdditionalBulletParticle;

	public void PlayBulletEffect(bool dual, bool hasHit, Vector3 direction)
	{
		if (!dual)
		{
			if (m_BulletParticle)
			{
				if (hasHit)
				{
					// Rotate particle to thing.
					Vector3 tempFix;
					m_BulletParticle.transform.forward = (direction - m_BulletParticle.transform.position).normalized;
					tempFix = m_BulletParticle.transform.eulerAngles;
					tempFix.x -= 90;
					m_BulletParticle.transform.eulerAngles = tempFix;
					
				}
				m_BulletParticle.Play();
			}
		}
		else // If dual is true, we play the additional bullet particle instead.
		{
			if (m_AdditionalBulletParticle)
			{
				if (hasHit)
				{
					m_AdditionalBulletParticle.transform.forward = (direction - m_AdditionalBulletParticle.transform.position).normalized;
					Vector3 tempFix = m_AdditionalBulletParticle.transform.eulerAngles;
					tempFix.x = -90;
					m_AdditionalBulletParticle.transform.eulerAngles = tempFix;
				}
				m_AdditionalBulletParticle.Play();
			}
		}
	}
}

[System.Serializable]
public class Animators
{
	public List<Animator> m_GunAnimators;
	public List<Animator> m_ArmsAnimators;

	public bool m_WeaponInspectAnimation;

	public Coroutine m_WeaponInspectRoutine;

	//public bool CheckCinematic()
	//{ 
	//	m_GunAnimators[0].GetCurrentAnimatorStateInfo(0).isName("Equip")
	//}
	public bool CheckWeaponInspect() { return m_WeaponInspectAnimation; }
	public IEnumerator RunWeaponInspect(float length)
	{
		m_WeaponInspectAnimation = true;
		float counter = 0;
		float testLength = length;

		while (counter < testLength)
		{ 
			counter += Time.deltaTime;
			yield return null;
		}

		m_WeaponInspectAnimation = false;
	}

}
