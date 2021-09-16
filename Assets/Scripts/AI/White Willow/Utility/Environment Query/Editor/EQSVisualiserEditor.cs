using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EQSVisualiser))]
public class EQSVisualiserEditor : Editor
{
    public int m_DrawRadius = 7;
    public int m_CheckRingDensity = 40;
    public float m_CheckDistanceDensity = 1.5f;

    public EQSVisualiser Visualiser;
    public WhiteWillow.Agent Agent;
    public bool m_GetPositionsClosestToTarget = true;

    public override void OnInspectorGUI()
    {
        if (Visualiser == null)
            Visualiser = target as EQSVisualiser;
        else
            base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        if (Visualiser == null)
            return;

        Agent = Visualiser.GetComponent<WhiteWillow.Agent>();

        Query agentQuery = Agent?.CurrentQuery;

        if (agentQuery != null)
        {
            foreach (var node in agentQuery.Values)
                DrawSphere(node.Position);
        }
    }

    private void DrawSphere(Vector3 offset)
    {
        Vector3 drawPosition = offset;
        Color sphereColour = Visualiser.m_OptimalColour;

        if (m_GetPositionsClosestToTarget)
        {
            float distance = (drawPosition - Agent.Target.transform.position).sqrMagnitude;
            float avoidanceRange = 7.5f;
            float enagementDepth = 5.0f;

            float avoidT = (distance / (avoidanceRange * avoidanceRange) * 10.0f) / 10.0f;
            float engageT = distance / (enagementDepth * enagementDepth) - (avoidanceRange * avoidanceRange);
            sphereColour = Color.Lerp(Color.Lerp(Visualiser.m_SuboptimalColour, Visualiser.m_OptimalColour, avoidT / 2.5f),
                Visualiser.m_SuboptimalColour, engageT / (avoidanceRange * avoidanceRange));
        }

        //sphereColour = IsColliding(drawPosition, out drawSphere) ? m_NotIdealColour : sphereColour;
        sphereColour.a = 0.5f;
        Handles.color = sphereColour;
        Handles.SphereHandleCap(0, drawPosition, Quaternion.identity, Visualiser.m_NodeSize, EventType.Repaint);
    }

    //private bool IsColliding(Vector3 position, out bool drawSphere)
    //{
    //    var colliders = Physics.OverlapSphere(position, m_SphereSize).ToList();
    //    drawSphere = false;
    //
    //    var ground = colliders.Find(obj => obj.CompareTag("Ground"));
    //    if (ground != null)
    //        drawSphere = true;
    //
    //    foreach (var col in colliders)
    //    {
    //        if (col.gameObject != ground?.gameObject && Physics.OverlapSphere(position, 0.0f).ToList().Contains(col))
    //            drawSphere = false;
    //        else if (col.gameObject == ground?.gameObject)
    //            drawSphere = true;
    //    }
    //
    //    foreach (var col in colliders)
    //    {
    //        // Ignore ground
    //        if (!col.CompareTag("Ground"))
    //            return true;
    //    }
    //
    //    return false;
    //}
}
