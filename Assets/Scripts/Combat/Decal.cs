using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Decal
{
    public GameObject m_Decal;
    public Vector3 m_hitPoint;
    private Transform m_objAttached;
    private Vector3 m_Dir;
    private Matrix4x4 m_LocalMat;
    private Vector3 m_hitPointWorld;


    public Decal(GameObject hitDecal)
    {
        m_Decal = hitDecal;
    }
    public Decal(Transform obj, Vector3 hitPoint, GameObject hitDecal, Vector3 dir)
    {
        m_Decal = GameObject.Instantiate<GameObject>(hitDecal);
        m_objAttached = obj;
        m_hitPoint = m_objAttached.InverseTransformPoint(hitPoint);
        m_Dir = m_objAttached.InverseTransformDirection(dir);
        m_LocalMat = m_objAttached.worldToLocalMatrix;
        m_hitPointWorld = hitPoint;
    }
    /// <summary>
    /// This constructor can be used in case you don't have a texture to display. The visual of the decal will only be represented by a gizmo sphere.
    /// </summary>
    /// <param name="obj">The object that was hit.</param>
    /// <param name="hitPoint">The point where you hit relative to the object.</param>
    /// <param name="dir">The direction you want this decal to face. Usually the normal of the surface.</param>
    public Decal(Transform obj, Vector3 hitPoint, Vector3 dir)
    {
        m_objAttached = obj;
        m_hitPoint = m_objAttached.InverseTransformPoint(hitPoint);
        m_Dir = m_objAttached.InverseTransformDirection(dir);
        m_LocalMat = m_objAttached.worldToLocalMatrix;
        m_hitPointWorld = hitPoint;
    }

    public Decal()
    { 
        
    }

    public void SetDecal(Transform obj, Vector3 hitPoint, Vector3 dir)
    {
        m_objAttached = obj;
        m_hitPoint = m_objAttached.InverseTransformPoint(hitPoint);
        m_Dir = m_objAttached.InverseTransformDirection(dir);
        m_LocalMat = m_objAttached.worldToLocalMatrix;
        m_hitPointWorld = hitPoint;
    }

    public void SetDecal(Transform obj, Vector3 hitPoint, Vector3 dir, GameObject decal)
    {
        m_objAttached = obj;
        m_hitPoint = m_objAttached.InverseTransformPoint(hitPoint);
        m_Dir = m_objAttached.InverseTransformDirection(dir);
        m_LocalMat = m_objAttached.worldToLocalMatrix;
        m_hitPointWorld = hitPoint;

        m_Decal = decal;
    }
    public void Update()
    {
        if (m_Decal) // We are checking if the decal's game object exists because there is a possibility it does not have one if it was created using the constructor that doesn't need a texture.
        {
            if (m_objAttached)
            { 
                Vector3 localPos = m_objAttached.TransformPoint(m_hitPoint);
                m_Decal.transform.LookAt(localPos + m_objAttached.TransformVector(m_Dir));
                localPos += m_objAttached.TransformVector(m_Dir) * 0.009f;
                m_Decal.transform.position = localPos;
            }
        }
    }

    public void DrawGizmo()
    {
        if (m_Decal)
        {
            Gizmos.DrawSphere(m_Decal.transform.position, 0.5f);
        }
        else if (m_objAttached != null) // This decal doesn't have a game object since it's not using a texture.
        {
            Vector3 position = m_hitPointWorld;
            Gizmos.DrawSphere(position, 0.1f);
        }
        else
        { 
            // The decal is completely unintialised so do nothing.
            // This will be the case for the ones that are in the unused object pool.

            // doing nothing.
        }
    }

}

