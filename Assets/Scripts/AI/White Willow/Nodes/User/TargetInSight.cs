using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

[Category("Decorator", "User")]
public class TargetInSight : Decorator
{
    public NodeMember<GameObject> Target;
    public NodeMember<float> EngagementDistance;

    [Header("Cone Check")]
    public int m_RaycastSteps = 3;
    public int m_RaycastsPerStep = 7;
    public float m_RaycastSpacing = 0.02f;

    // Called before node is executed
    protected override void OnEnter()
    {
        Target.Validate(Owner.Blackboard);
        Owner.Agent.Target = Target.Value;
    }

    // Called after node has finished executing or is aborted
    protected override void OnExit()
    {
	    
    }

    // This is where the behaviour is updated
    protected override NodeResult OnTick()
    {
        // Check if target is within general range
        if (Target.Value != null)
        {
            if (Owner.Agent.TargetWithinRange(Target.Value, EngagementDistance.Value))
            {
                // Check if target would be visible assuming the agent were facing it
                if (Owner.Agent.TargetVisible(Target.Value) || Owner.Agent.ShotgunRaycast(Target.Value, Target.Value.transform.position -
                    Owner.Agent.transform.position, m_RaycastSteps, m_RaycastsPerStep, m_RaycastSpacing))
                {
                    return Child.Tick();
                }
            }
        }

        return NodeResult.Failure;
    }
}