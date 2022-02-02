using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

[Category("Task", "User")]
public class FireAtTarget : Task
{
    public NodeMember<GameObject> Target;
    public NodeMember<float> EngagementDistance;

    [Min(0)]
    public int MaximumShots = 10;

    [Min(0)]
    public int MinimumShots = 3;

    private int m_ShotsToFire = 0;
    private int m_CurrentShots = 0;

    private float m_ShootDelay = 0.1f;
    private float m_CurrentShootTime = 0.0f;

    private CombatToken m_Token;

    // Called before node is executed
    protected override void OnEnter()
    {
        Target.Validate(Owner.Blackboard);

        m_Token = AIDirector.Instance.RequestToken();
        m_ShotsToFire = Random.Range(MinimumShots, MaximumShots + 1);
    }

    // Called after node has finished executing or is aborted
    protected override void OnExit()
    {
        m_ShotsToFire = 0;
        m_CurrentShots = 0;
        m_CurrentShootTime = 0.0f;

        m_Token?.Use();
        m_Token = null;
    }

    // This is where the behaviour is updated
    protected override NodeResult OnTick()
    {
        if (Target.Value != null)
        {
            // Turn to face target
            Owner.Agent.LookAt(Target.Value.transform.position);

            if (m_Token != null)
            {
                if (Owner.Agent.TargetWithinViewRange(Target.Value, EngagementDistance.Value))
                {
                    // Shoot volley
                    if (m_CurrentShootTime >= m_ShootDelay && Owner.Agent.ShootAt(Target.Value, m_CurrentShots == 0))
                    {
                        m_CurrentShots++;
                        m_CurrentShootTime = 0.0f;
                    }
                    else
                        m_CurrentShootTime += Time.deltaTime;

                    if (m_CurrentShots >= m_ShotsToFire)
                        return NodeResult.Success;
                }

                return NodeResult.Running;
            }
        }

	    return NodeResult.Failure;
    }
}