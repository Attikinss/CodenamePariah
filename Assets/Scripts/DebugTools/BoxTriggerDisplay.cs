using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTriggerDisplay : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The colour of the box collider.")]
    private Color m_BoxColour;

    [SerializeField]
    [Tooltip("The colour of the box collider's edges.")]
    private Color m_WireColour;

    /// <summary>Draws a cube at the position of any box colliders on the gameobject this script is attached too.</summary>
    private void OnDrawGizmos()
    {
        BoxCollider boxTrigger = GetComponent<BoxCollider>();
        Vector3 drawVector = this.transform.lossyScale;
        drawVector.x *= boxTrigger.size.x;
        drawVector.y *= boxTrigger.size.y;
        drawVector.z *= boxTrigger.size.z;

        Vector3 drawPos = this.transform.position + boxTrigger.center;

        Gizmos.matrix = Matrix4x4.TRS(drawPos, this.transform.rotation, drawVector);
        Gizmos.color = m_BoxColour;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = m_WireColour;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
