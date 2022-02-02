using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Task")]
    public sealed class MoveTo : Task
    {
        public override string IconPath { get; } = "Icons/MoveTo";
        public NodeMember<Vector3> Position;

        private bool m_StopAgentOnExit = true;

        protected override void OnEnter()
        {
            Position.Validate(Owner.Blackboard);
        }

        protected override void OnExit()
        {
            if (m_StopAgentOnExit)
                Owner.Agent.Stop();

            m_StopAgentOnExit = true;
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

        public override NodeResult Abort()
        {
            Owner.Agent.Stop(false);
            m_StopAgentOnExit = false;

            return base.Abort();
        }
    }
}