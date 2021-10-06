using System.Linq;
using UnityEngine;
using WhiteWillow;
using WhiteWillow.Nodes;

public enum DistanceCheck { ClosestTarget, FurthestSelf, FurthestTarget, None }
public enum AngleCheck { Ahead, Behind, ToTarget, FromTarget, None }

[Category("Tasks", "User")]
public class EQSQuery : Task
{
    public NodeMember<GameObject> Target;
    public float Range;
    public bool MaintainTargetsSight = false;

    [Header("Query Checks")]
    public DistanceCheck DistanceCheck = DistanceCheck.None;
    public float Distance = 25.0f;
    public float Threshold = 5.0f;

    public AngleCheck AngleCheck = AngleCheck.None;
    public float Angle = 75.0f;

    private EnvironmentQuerySystem.QueryRequest m_EQSRequest = null;
    private EnvironmentQuerySystem.EQSNode m_CurrentNode = null;
    private bool m_EQSRequestSent = false;

    // Called before node is executed
    protected override void OnEnter()
    {
        if (Target.Value == null && Target.BlackboardValue)
            Target.Validate(Owner.Blackboard);
    }

    // Called after node has finished executing or is aborted
    protected override void OnExit()
    {
        m_EQSRequest = null;
        m_EQSRequestSent = false;
        Owner.Agent.CurrentQuery = null;
    }

    // This is where the behaviour is updated
    protected override NodeResult OnTick()
    {
        if (Target.Value != null)
        {
            // Ensure a request is available for submission
            if (m_EQSRequest == null)
                m_EQSRequest = new EnvironmentQuerySystem.QueryRequest(Owner.Agent.transform.position, Range);

            // Ensure a request was submitted successfully to the EQS
            if (!m_EQSRequestSent)
                m_EQSRequestSent = EnvironmentQuerySystem.NewQuery(m_EQSRequest);

            // If something went wrong, bail until the next tick
            if (!m_EQSRequestSent || !m_EQSRequest.QueryResolved || !EnvironmentQuerySystem.RetrieveQuery(m_EQSRequest, out Owner.Agent.CurrentQuery))
            {
                return NodeResult.Running;
            }

            QueryAngleCheck(ref Owner.Agent.CurrentQuery);
            QueryDistanceCheck(ref Owner.Agent.CurrentQuery);

            if (!Owner.Agent.CurrentQuery.Empty())
            {
                // Offset to simulate eye height
                Vector3 verticalOffset = Vector3.up * 1.5f;

                Owner.Agent.CurrentQuery.ShuffleValues();
                float prevPathLength = float.MaxValue;
                foreach (var node in Owner.Agent.CurrentQuery.Values)
                {
                    Vector3 direction = Target.Value.transform.position - (node.Position + verticalOffset);

                    // Line of sight
                    // Distance from next position to player
                    // Travel time/distance to new position

                    if (Physics.Raycast(node.Position + verticalOffset, direction, out RaycastHit hitInfo))
                    {
                        //Debug.Log($"Success: {Owner.Agent.gameObject}");
                        if (hitInfo.transform.gameObject == Target.Value &&
                            Vector3.Distance(node.Position, Target.Value.transform.position) <= Owner.Agent.m_ViewRange)
                        {
                            //float pathLength = Owner.Agent.CalculateTravelDistance(node.Position);
                            //
                            //if (pathLength < prevPathLength)
                            //    prevPathLength = pathLength;

                            if (MaintainTargetsSight)
                            {
                                float targetFacingAngle = Vector3.Angle((node.Position + verticalOffset)
                                    - Target.Value.transform.position, Target.Value.transform.forward);

                                // Not in players peripheral/immediate view
                                if (targetFacingAngle < -Angle || targetFacingAngle > Angle)
                                    continue;
                            }

                            m_CurrentNode?.Release();
                            m_CurrentNode = node;
                            m_CurrentNode.Reserve(Owner.Agent);
                            Owner.Agent.SetDestination(m_CurrentNode.Position);

                            Debug.Log($"Success: {Owner.Agent.gameObject}");

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
            case AngleCheck.Ahead:
                query.FilterByAngle(Owner.Agent.transform.position, -Owner.Agent.transform.forward, Angle);
                break;

            case AngleCheck.Behind:
                query.FilterByAngle(Owner.Agent.transform.position, Owner.Agent.transform.forward, Angle);
                break;

            case AngleCheck.ToTarget:
                query.FilterByAngle(Owner.Agent.transform.position, Target.Value.transform.position
                    - Owner.Agent.transform.position, Angle);
                break;

            case AngleCheck.FromTarget:
                query.FilterByAngle(Owner.Agent.transform.position, Owner.Agent.transform.position
                    - Target.Value.transform.position, Angle);
                break;

            default: break;
        }
    }

    private void QueryDistanceCheck(ref Query query)
    {
        switch (DistanceCheck)
        {
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