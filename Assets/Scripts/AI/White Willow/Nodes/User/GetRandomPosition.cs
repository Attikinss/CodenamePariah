using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

[Category("Tasks", "User")]
public class GetRandomPosition : Task
{
    [Tooltip("The distance of the random position from the agent.")]
    public NodeMember<float> Radius = new NodeMember<float>();

    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override NodeResult OnTick()
    {
        var offset = Random.insideUnitSphere * Radius.Value;
        offset.y = 0.0f;

        if (Owner.Agent.SetDestination(offset + Owner.Agent.transform.position))
            return NodeResult.Success;
        else
        {
            if (Interruptable)
                return NodeResult.Failure;
            else
                return NodeResult.Running;
        }
    }
}