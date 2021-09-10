using System.Linq;
using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

public enum DistanceCheck { ClosestSelf, ClosestTarget, FurthestSelf, FurthestTarget, None }
public enum AngleCheck { Back, Front, ToTarget, FromTarget, None }

[Category("Tasks", "User")]
public class EQSQuery : Task
{
    public NodeMember<float> Angle;
    public AngleCheck AngleCheck = AngleCheck.None;

    public float Threshold = 5.0f;
    public NodeMember<float> Distance;
    public DistanceCheck DistanceCheck = DistanceCheck.None;

    public bool MaintainTargetsSight = false;

    public NodeMember<float> Range;
    public NodeMember<GameObject> Target;

    private bool m_Cull = false;

    // Called before node is executed
    protected override void OnEnter()
    {
        if (Target.Value != null)
        {
            float distanceToPlayer = (Owner.Agent.transform.position - Target.Value.transform.position).sqrMagnitude;
            if (distanceToPlayer > 50 * 50)
                m_Cull = true;
        }
    }

    // Called after node has finished executing or is aborted
    protected override void OnExit()
    {
        m_Cull = false;
    }

    // This is where the behaviour is updated
    protected override NodeResult OnTick()
    {
        if (m_Cull)
            return NodeResult.Failure;

        Query query = /*new Query();*/ EnvironmentQuerySystem.NewQuery(Owner.Agent.transform.position, Range.Value);
        Owner.Agent.CurrentQuery = query;

        if (Target.Value != null)
        {
            QueryAngleCheck(ref query);
            QueryDistanceCheck(ref query);

            if (!query.Empty())
            {
                // Offset to simulate eye height
                Vector3 verticalOffset = Vector3.up * 0.7f;

                query.ShuffleValues();
                foreach (var node in query.Values)
                {
                    Vector3 direction = (Target.Value.transform.position + verticalOffset) - (node.Position + verticalOffset);
                    if (Physics.Raycast(node.Position + verticalOffset, direction, out RaycastHit hitInfo, Range.Value))
                    {
                        if (hitInfo.transform.gameObject == Target.Value)
                        {
                            if (MaintainTargetsSight)
                            {
                                float targetFacingAngle = Vector3.Angle(node.Position + verticalOffset
                                    - Target.Value.transform.position, Target.Value.transform.forward);

                                // Not in players peripheral/immediate view
                                if (targetFacingAngle < -75.0f || targetFacingAngle > 75.0f)
                                    continue;
                            }

                            Owner.Agent.SetDestination(node.Position);
                            return NodeResult.Success;
                        }
                    }
                }
                // Get position that is:
                // • In line of sight to player
                // • Distance to target is appropriate
                // • Distance to other agents is appropriate
            }
        }        

        return NodeResult.Failure;
    }

    private void QueryAngleCheck(ref Query query)
    {
        switch (AngleCheck)
        {
            case AngleCheck.Back:
                query.FilterByAngle(Owner.Agent.transform.position, Owner.Agent.transform.forward, Angle.Value);
                break;

            case AngleCheck.Front:
                query.FilterByAngle(Owner.Agent.transform.position, -Owner.Agent.transform.forward, Angle.Value);
                break;

            case AngleCheck.ToTarget:
                query.FilterByAngle(Owner.Agent.transform.position, Target.Value.transform.position
                    - Owner.Agent.transform.position, Angle.Value);
                break;

            case AngleCheck.FromTarget:
                query.FilterByAngle(Owner.Agent.transform.position, Owner.Agent.transform.position
                    - Target.Value.transform.position, Angle.Value);
                break;

            default: break;
        }
    }

    private void QueryDistanceCheck(ref Query query)
    {
        switch (DistanceCheck)
        {
            case DistanceCheck.ClosestSelf:
                query.FilterByClosest(Owner.Agent.transform.position, Threshold);
                break;

            case DistanceCheck.ClosestTarget:
                query.FilterByClosest(Target.Value.transform.position, Threshold);
                break;

            case DistanceCheck.FurthestSelf:
                query.FilterByFurthest(Owner.Agent.transform.position, Threshold);
                break;

            case DistanceCheck.FurthestTarget:
                query.FilterByFurthest(Target.Value.transform.position, Threshold);
                break;

            default: break;
        }
    }
}