using UnityEngine;

public class EQSSceneNode : MonoBehaviour
{
    [SerializeField]
    private EnvironmentQuerySystem.EQSNode m_EQSNode;

    public bool m_DrawAlways = false;

    private void Start()
    {
        m_EQSNode = new EnvironmentQuerySystem.EQSNode();
        m_EQSNode.Position = transform.position;
    }

    private void OnDrawGizmos()
    {
        if (!m_DrawAlways) return;

        Draw();
    }

    private void OnDrawGizmosSelected()
    {
        if (m_DrawAlways) return;

        Draw();
    }

    private void Draw()
    {
        Gizmos.color = m_EQSNode.Taken ? m_EQSNode.TakenColour : m_EQSNode.VacantColour;
        Gizmos.DrawSphere(transform.position, m_EQSNode.Size);
    }
}
