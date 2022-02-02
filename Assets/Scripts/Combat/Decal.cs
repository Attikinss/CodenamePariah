using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// ========================= NOTE ============================= //
// This Decal class is going under a refactor. It will no longer have to contain a decal game object.
// Instead it will just be used to store and update the location for decal projectors.
public class Decal
{
    //public GameObject m_Decal;
    public Vector3 m_hitPoint;
    private Transform m_objAttached;
    private Vector3 m_Dir;
    private Matrix4x4 m_LocalMat;
    private Vector3 m_hitPointWorld;

    public GameObject m_ProjectorObj;

    public Decal() { }
    public Decal(GameObject projectorPrefab)
    {
        //m_Decal = hitDecal;

        m_ProjectorObj = GameObject.Instantiate(projectorPrefab);
        m_ProjectorObj.transform.position = new Vector3(0, -100, 0); // Hiding it below the map.
    }
    //public Decal(Transform obj, Vector3 hitPoint, GameObject hitDecal, Vector3 dir)
    //{
    //    m_Decal = GameObject.Instantiate<GameObject>(hitDecal);
    //    m_objAttached = obj;
    //    m_hitPoint = m_objAttached.InverseTransformPoint(hitPoint);
    //    m_Dir = m_objAttached.InverseTransformDirection(dir);
    //    m_LocalMat = m_objAttached.worldToLocalMatrix;
    //    m_hitPointWorld = hitPoint;
    //}
    /// <summary>
    /// This constructor can be used in case you don't have a texture to display. The visual of the decal will only be represented by a gizmo sphere.
    /// </summary>
    /// <param name="obj">The object that was hit.</param>
    /// <param name="hitPoint">The point where you hit relative to the object.</param>
    /// <param name="dir">The direction you want this decal to face. Usually the normal of the surface.</param>
    //public Decal(Transform obj, Vector3 hitPoint, Vector3 dir)
    //{
    //    m_objAttached = obj;
    //    m_hitPoint = m_objAttached.InverseTransformPoint(hitPoint);
    //    m_Dir = m_objAttached.InverseTransformDirection(dir);
    //    m_LocalMat = m_objAttached.worldToLocalMatrix;
    //    m_hitPointWorld = hitPoint;
    //}

    //public Decal()
    //{ 

    //}

    // =============================================== NOTE =============================================== //
    // rotate bullet decal randomly, with up vector.
    public void SetDecal(Transform obj, Vector3 hitPoint, Vector3 dir)
    {
        m_objAttached = obj;


        m_hitPoint = m_objAttached.InverseTransformPoint(hitPoint);
        m_Dir = m_objAttached.InverseTransformDirection(dir);
        m_LocalMat = m_objAttached.worldToLocalMatrix;
        m_hitPointWorld = hitPoint;

        // Setting transforms.

        Vector3 localPos = m_objAttached.TransformPoint(m_hitPoint);
        localPos += m_objAttached.TransformVector(m_Dir) * 0.009f;
        //m_ProjectorObj.transform.LookAt(localPos + m_objAttached.TransformVector(m_Dir));


        //m_ProjectorObj.transform.eulerAngles = dir;  Doesn't make decals face towards direction.
        m_ProjectorObj.transform.forward = dir;        //This does though.

        m_ProjectorObj.transform.position = localPos;
    }

    //public void SetDecal(Transform obj, Vector3 hitPoint, Vector3 dir, GameObject decal)
    //{
    //    m_objAttached = obj;
    //    m_hitPoint = m_objAttached.InverseTransformPoint(hitPoint);
    //    m_Dir = m_objAttached.InverseTransformDirection(dir);
    //    m_LocalMat = m_objAttached.worldToLocalMatrix;
    //    m_hitPointWorld = hitPoint;

    //    m_Decal = decal;
    //}
    

}

