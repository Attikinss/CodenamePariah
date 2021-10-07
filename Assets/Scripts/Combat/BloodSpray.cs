using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class BloodSpray
{
    public Transform m_AttachedObj;
    public Vector3 m_Pos;
    public Vector3 m_FacingDirection;

    VisualEffect m_Particle;
    GameObject m_ParticleObj;

    public BloodSpray(GameObject prefab)
    {
        m_ParticleObj = GameObject.Instantiate(prefab);
        m_Particle = m_ParticleObj.GetComponent<VisualEffect>();
        if (!m_Particle)
        {
            // If we hit this that means the passed in prefab doesn't have a visual effect on it.
            Debug.LogError("GameManager blood spray prefab does not have a visual effect component!");
        }


        m_ParticleObj.transform.position = Vector3.zero; // Putting it at 0, 0, 0.
    }
    BloodSpray(Transform attach, Vector3 hitPosition)
    {
        m_AttachedObj = attach;
        m_Pos = hitPosition;
    }

    public void Update()
    {
        if (m_AttachedObj) // It wont try update for blood sprays that are just created and have no attached object yet.
        {
            Vector3 localPos = m_AttachedObj.TransformPoint(m_Pos);
            
            m_ParticleObj.transform.LookAt(localPos + m_AttachedObj.TransformVector(m_FacingDirection));
            localPos += m_AttachedObj.TransformVector(m_FacingDirection) * 0.009f;

            m_ParticleObj.transform.position = localPos;
        }
    }

    public void SetSpray(Transform attach, Vector3 hitPoint, Vector3 facing)
    {
        m_AttachedObj = attach;
        m_Pos = m_AttachedObj.InverseTransformPoint(hitPoint);
        m_FacingDirection = facing;
        //m_LocalMat = m_objAttached.worldToLocalMatrix;
        //m_hitPointWorld = hitPoint;

        //m_Pos = hitPoint;
        //m_FacingDirection = facing;

        // Play visual effect after being set.
        m_Particle.Play();
    }

}
