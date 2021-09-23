using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

[Category("Decorator", "User")]
public class DestinationInTargetView : Decorator
{
    public NodeMember<GameObject> Target;

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
        if (Child != null && Target.Value != null)
        {
            Vector3 destination = Owner.Agent.Destination;
            Vector3 direction = destination - Target.Value.transform.position;

            if (Physics.Raycast(destination, direction, out RaycastHit hitInfo))
            {
                if (hitInfo.transform.gameObject == Target.Value)
                    return Child.Tick();
            }
        }

        return NodeResult.Failure;
    }
}