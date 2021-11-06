using UnityEngine;

public class EQSSceneNode : MonoBehaviour
{
    public EnvironmentQuerySystem.EQSNode EQSNode;

    public bool m_DrawAlways = false;


	private void Awake()
	{
        EQSNode.GenerateID();
        EQSNode.Position = transform.position;
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

    private void OnEnable() => EQSSceneNodeTracker.AddNode(this);
    private void OnDisable() => EQSSceneNodeTracker.RemoveNode(this);
}
