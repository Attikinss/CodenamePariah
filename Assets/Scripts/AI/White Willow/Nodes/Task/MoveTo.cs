using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Tasks")]
    public class MoveTo : Task
    {
        public override string IconPath { get; } = "Icons/MoveTo";

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {

        }

        protected override NodeResult OnTick()
        {
            if (!Owner.Agent.MovingToPosition())
                Owner.Agent.MoveToPosition();

            if (Owner.Agent.AtPosition())
                return NodeResult.Success;
            else
                return NodeResult.Running;
        }
    }
}