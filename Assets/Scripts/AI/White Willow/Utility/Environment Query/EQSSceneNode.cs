using UnityEngine;

public class EQSSceneNode : MonoBehaviour
{
    public EnvironmentQuerySystem.EQSNode EQSNode;

    public bool m_DrawAlways = false;

    private void Start()
    {
        EQSNode.GenerateID();
        EQSNode.Position = transform.position;

        EQSSceneNodeTracker.AddNode(this);
    }

    private void OnDrawGizmos()
    {
        if (m_DrawAlways)
            Draw();
    }

    private void OnDrawGizmosSelected()
    {
        if (!m_DrawAlways)
            Draw();
    }

    private void Draw()
    {
        Gizmos.color = EQSNode.Taken ? EQSNode.TakenColour : EQSNode.VacantColour;
        Gizmos.DrawSphere(transform.position, EQSNode.Size);
    }
}
