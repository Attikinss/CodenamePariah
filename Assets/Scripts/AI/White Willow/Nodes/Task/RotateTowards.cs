using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Tasks")]
    public class RotateTowards : Task
    {
        public Transform Target;

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {

        }

        protected override NodeResult OnTick()
        {
            if (Target == null)
                return NodeResult.Failure;

            Owner.Agent.RotateToFaceTarget(Target);
            if (Owner.Agent.FacingTarget(Target))
                return NodeResult.Success;
            else
                return NodeResult.Running;
        }
    }
}