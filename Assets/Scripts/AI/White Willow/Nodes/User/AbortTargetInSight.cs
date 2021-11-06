using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

[Category("Decorator")]
public sealed class AbortTargetInSight : Decorator
{
    public NodeMember<GameObject> Target;

    protected override void OnEnter()
    {
        Target.Validate(Owner.Blackboard);
    }

    protected override void OnExit() { }

    protected override NodeResult OnTick()
    {
        if (Child == null)
            return NodeResult.Failure;

        if (Target.Value != null && Owner.Agent.TargetVisible(Target.Value, out float distance))
        {
            if (distance < Owner.Agent.m_FiringRange)
            {
                Debug.Log($"{Owner.Agent.gameObject.name}: Aborted");
                return Child.Abort();
            }
        }

        return Child.Tick();
    }
}
