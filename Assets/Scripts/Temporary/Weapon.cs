using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    public float m_BulletDamage;
    public GameObject m_HitDecal;
    public Vector3 m_MidPoint { get; private set; }

    public AnimationCurve m_VerticalRecoil;

    // ============= INTERNAL BOOKKEEPING ============= //
    [HideInInspector]
    public float m_VerticalProgress = 0.0f; // To track position on the recoil curve.

    [HideInInspector]
    public Vector3 m_OriginalPosition; // Public so that HostController.cs can access it when lerping back to the weapons original pos.

	// ================================================ //


	private void Awake()
	{
        m_OriginalPosition = transform.localPosition;
		
	}

	// Start is called before the first frame update
	void Start()
    {
        m_MidPoint = transform.localPosition;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
