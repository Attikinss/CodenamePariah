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

	public ParticleSystem m_ClonedBulletParticle;

	public ParticleSystem m_AdditionalBulletParticle;

	Vector3 CachedGunSpaceBulletPos;
	Vector3 CachedAdditionalGunSpaceBulletPos;
	Transform BulletParent;
	Transform AdditionalBulletParent;

	public void CacheBulletParticle()
	{
		if (m_BulletParticle)
		{ 
			CachedGunSpaceBulletPos = m_BulletParticle.transform.localPosition;
			BulletParent = m_BulletParticle.transform.parent;
			m_BulletParticle.transform.parent = null;
		}
		if (m_AdditionalBulletParticle)
		{
			CachedAdditionalGunSpaceBulletPos = m_AdditionalBulletParticle.transform.localPosition;
			AdditionalBulletParent = m_AdditionalBulletParticle.transform.parent;
			m_AdditionalBulletParticle.transform.parent = null;
		}
	}

	public void PlayBulletEffect(bool dual, bool hasHit, Vector3 direction)
	{
		if (!dual)
		{
			if (m_BulletParticle)
			{
				if (hasHit)
				{
					m_BulletParticle.transform.position = BulletParent.TransformPoint(CachedGunSpaceBulletPos);

					m_BulletParticle.transform.forward = (direction - m_BulletParticle.transform.position).normalized;	
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
					m_AdditionalBulletParticle.transform.position = AdditionalBulletParent.TransformPoint(CachedAdditionalGunSpaceBulletPos);
					m_AdditionalBulletParticle.transform.forward = (direction - m_AdditionalBulletParticle.transform.position).normalized;
					//Vector3 tempFix = m_AdditionalBulletParticle.transform.eulerAngles;
					//tempFix.x = -90;
					//m_AdditionalBulletParticle.transform.eulerAngles = tempFix;
				}
				m_AdditionalBulletParticle.Play();
			}
		}
	}

	public void DrawBulletPFXGizmo()
	{
		Gizmos.color = Color.cyan;
		if (m_BulletParticle)
		{
			// If we have the bullet particle, draw the gizmo.
			Gizmos.DrawRay(m_BulletParticle.transform.position, m_BulletParticle.transform.forward * 1000);
			Ray bulletRay = new Ray(m_BulletParticle.transform.position, m_BulletParticle.transform.forward);

			RaycastHit hit;
			if (Physics.Raycast(bulletRay, out hit, 1000))
			{

				Gizmos.DrawSphere(hit.point, 0.25f);
			}
		}
		if (m_AdditionalBulletParticle)
		{
			// This would be used for the dual wield since it has two of those bullet particles.
			Gizmos.DrawLine(m_AdditionalBulletParticle.transform.position, m_BulletParticle.transform.position + m_AdditionalBulletParticle.transform.forward);
		}
	}
}

[System.Serializable]
public class Animators
{
	
	public List<SkinnedMeshRenderer> m_SkinnedMeshes;

	public List<Animator> m_GunAnimators;
	public List<Animator> m_ArmsAnimators;

	public bool m_WeaponInspectAnimation;

	public Coroutine m_WeaponInspectRoutine;

	

	// Prevents CancelWeaponInspect() from being called repeadetly.
	public bool IsCancellingEquip = false;

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

	/// <summary>
	/// This is used to allow the player to fire even if they are inspecting their weapon.
	/// </summary>
	/// <param name="t">Time until it is cancelled.</param>
	/// <param name="equipEvent">Equip audio event to cancel.</param>
	public IEnumerator CancelWeaponInspect(float t, FMODAudioEvent equipEvent)
	{
		if (!IsCancellingEquip) // Will only do this if we aren't already cancelling the animation.
		{
			IsCancellingEquip = true;
			float elapsedTime = 0;
			m_GunAnimators[0].SetTrigger("CancelEquip");
			m_ArmsAnimators[0].SetTrigger("CancelEquip");

			// Stop equip sound when it gets cancelled.
			equipEvent.StopSound(FMOD.Studio.STOP_MODE.IMMEDIATE);
			

			while (elapsedTime <= t)
			{
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			m_WeaponInspectAnimation = false;
			IsCancellingEquip = false;
		}
	}

}
