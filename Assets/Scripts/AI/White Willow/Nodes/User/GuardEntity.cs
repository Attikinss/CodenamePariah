using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

[Category("Tasks", "User")]
public class GuardEntity : Task
{
    public NodeMember<GameObject> Entity;
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