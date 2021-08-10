using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    public float m_BulletDamage;
    public GameObject m_HitDecal;

    public AnimationCurve m_VerticalRecoil;

    // ============= INTERNAL BOOKKEEPING ============= //
    [HideInInspector]
    public float m_VerticalProgress = 0.0f; // To track position on the recoil curve.

    [HideInInspector]
    public Vector3 m_OriginalLocalPosition; // Public so that HostController.cs can access it when lerping back to the weapons original pos.
    [HideInInspector]
    public Vector3 m_OriginalGlobalPosition;

	// ================================================ //


	private void Awake()
	{
        m_OriginalLocalPosition = transform.localPosition;
        m_OriginalGlobalPosition = transform.position;
	}

    // Update is called once per frame
    void Update()
    {

    }
}
