using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

[Category("Tasks", "User")]
public class GuardPosition : Task
{
    public NodeMember<Vector3> Position;
    public NodeMember<float> Distance;

    // Called before node is executed
    protected override void OnEnter()
    {
	    
    }

    // Called after node has finished executing or is aborted
    protected override void OnExit()
    {
	    
    }

    // This is where the behaviour is updated
    protected override NodeResult OnTick()
    {
	    return NodeResult.Failure;
    }
}