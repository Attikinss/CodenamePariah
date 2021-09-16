using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

[Category("Tasks", "User")]
public class FireAtTarget : Task
{
    public float m_RaycastSpacing = 0.02f;
    public int m_RaycastsPerStep = 7;
    [Header("Cone Check")]
    public int m_RaycastSteps = 3;

    [Min(0)]
    public int MinimumShots = 1;

    [Min(0)]
    public int MaximumShots = 7;

    private int m_ShotsToFire = 0;
    private int m_CurrentShots = 0;
    private bool m_Shooting = false;

    public NodeMember<float> EngagementDistance;
    public NodeMember<GameObject> Target;

    // Called before node is executed
    protected override void OnEnter()
    {
        m_ShotsToFire = Random.Range(MinimumShots, MaximumShots + 1);
    }

    // Called after node has finished executing or is aborted
    protected override void OnExit()
    {
        m_ShotsToFire = 0;
    }

    // This is where the behaviour is updated
    protected override NodeResult OnTick()
    {
        if (Target.Value != null)
        {
            // Check if target is within general range and in agent's view
            if (TargetWithinRange() && TargetWithinViewRange())
            {
                // Check if target is actually visible (not obscured by the environment/other agents)
                if (TargetVisible())
                {
                    Debug.Log("Miss first shot");
                    m_Shooting = ++m_CurrentShots >= m_ShotsToFire;
                }
            }
        }

	    return NodeResult.Failure;
    }

    private bool TargetWithinRange()
    {
        return (Owner.Agent.transform.position - Target.Value.transform.position).sqrMagnitude
            < EngagementDistance.Value * EngagementDistance.Value;
    }

    private bool TargetWithinViewRange()
    {
        Vector3 direction = Target.Value.transform.position - Owner.Agent.transform.position;
        float angle = Vector3.Angle(direction, Owner.Agent.transform.forward);

        // TODO: Add agent variable/data structure for handling sensory values
        return angle >= -45.0f && angle <= 45.0f;
    }

    private bool TargetVisible()
    {
        Vector3 direction = Owner.Agent.transform.forward;
        RaycastHit hitInfo = new RaycastHit();

        // Do a simple raycast to target
        if (Physics.Raycast(Owner.Agent.transform.position, direction, out hitInfo, Owner.Agent.m_ViewRange))
        {
            if (hitInfo.transform.gameObject == Target.Value)
                return true;
        }
        // If the player is obscured, try to "shotgun raycast" to find
        // them just in case the matchstick issue may be occuring
        else
        {
            // TODO: This section will need a rework if performance becomes an issue

            for (int j = 0; j < m_RaycastSteps; j++)
            {
                float angle = 0.0f;
                for (int i = 0; i < m_RaycastsPerStep; i++)
                {
                    float x = Mathf.Sin(angle);
                    float y = Mathf.Cos(angle);
                    angle += (2 * Mathf.PI) / m_RaycastsPerStep;

                    Vector3 offset = new Vector3(x * m_RaycastSpacing * j, y * m_RaycastSpacing * j, 1.0f);
                    direction = Owner.Agent.transform.TransformDirection(offset);

                    if (Physics.Raycast(Owner.Agent.transform.position, direction.normalized, out hitInfo, Owner.Agent.m_ViewRange))
                    {
                        if (hitInfo.transform.gameObject == Target.Value)
                            return true;
                    }
                }
            }
        }

        return false;
    }
}