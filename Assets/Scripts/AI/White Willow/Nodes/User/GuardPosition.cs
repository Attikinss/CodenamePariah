using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

[Category("Task", "User")]
public class GuardPosition : Task
{
    [ReadOnly]
    public Vector3 Origin;
    [ReadOnly]
    public Vector3 TargetPosition;

    public NodeMember<float> Distance;

    // Called before node is executed
    protected override void OnEnter()
    {
        Distance.Validate(Owner.Blackboard);

        Origin = Owner.Agent.transform.position;
        TargetPosition = Origin + Random.insideUnitSphere * Distance.Value;
        Owner.Agent.SetDestination(TargetPosition);
    }

    // Called after node has finished executing or is aborted
    protected override void OnExit()
    {
	    
    }

    // This is where the behaviour is updated
    protected override NodeResult OnTick()
    {
        if (Owner.Agent.DestinationAttainable(TargetPosition))
        {
            if (!Owner.Agent.MovingToPosition())
                Owner.Agent.MoveToPosition();

            if (Owner.Agent.AtPosition())
                return NodeResult.Success;
            else
            {
                if (Owner.Agent.Stuck())
                    return NodeResult.Failure;

                return NodeResult.Running;
            }
        }

        return NodeResult.Failure;
    }
}