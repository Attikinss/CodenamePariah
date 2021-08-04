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

    public Decal(Transform obj, Vector3 hitPoint, GameObject hitDecal, Vector3 dir)
    {
        m_Decal = GameObject.Instantiate<GameObject>(hitDecal);
        m_objAttached = obj;
        m_hitPoint = m_objAttached.InverseTransformPoint(hitPoint);
        m_Dir = m_objAttached.InverseTransformDirection(dir);
        m_LocalMat = m_objAttached.worldToLocalMatrix;
    }

    public void Update()
    {
        Vector3 localPos = m_objAttached.TransformPoint(m_hitPoint);
        m_Decal.transform.LookAt(localPos + m_objAttached.TransformVector(m_Dir));
        localPos += m_objAttached.TransformVector(m_Dir) * 0.009f;
        m_Decal.transform.position = localPos;
    }

    public void DrawGizmo()
    {
        Gizmos.DrawSphere(m_Decal.transform.position, 0.5f);
    }

}

