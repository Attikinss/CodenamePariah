using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Tasks")]
    public sealed class MoveTo : Task
    {
        public override string IconPath { get; } = "Icons/MoveTo";
        public NodeMember<Vector3> Position;

        protected override void OnEnter()
        {
            Position.Validate(Owner.Blackboard);
        }

        protected override void OnExit()
        {
            Owner.Agent.Stop();
        }

        protected override NodeResult OnTick()
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
    }
}