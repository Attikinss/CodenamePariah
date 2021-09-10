﻿using UnityEngine;

namespace WhiteWillow.Nodes
{
    ///<summary>
    ///<br>A node that iterates through its child nodes until one returns a success or running result.</br>
    ///<br>If it never receives a success or running result it will return a failure result to its parent.</br>
    ///</summary>
    [Category("Composites")]
    public sealed class Selector : Composite
    {
        public override string IconPath { get; } = "Icons/Selector";

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {
            RunningChild = null;
        }

        protected override NodeResult OnTick()
        {
            // No point doing anything
            if (Children.Count == 0) return NodeResult.Failure;

            // Ignore all other children if there is a running child
            var lastRunningChild = RunningChild;
            if (lastRunningChild != null)
            {
                NodeResult runningResult = TickRunningChild();
                if (runningResult != NodeResult.Failure) return runningResult;
            }

            foreach (BaseNode child in Children)
            {
                // Skip to running child's following node
                if (lastRunningChild != null && child.ExecutionOrder <= lastRunningChild.ExecutionOrder)
                    continue;

                // Ignore locked nodes
                if (child.Locked) continue;

                // Get current child's result
                NodeResult result = child.Tick();

                // TODO: Implement abort

                if (result != NodeResult.Failure)
                {
                    if (result == NodeResult.Running)
                        (Parent as Composite)?.SetRunningChild(this);

                    return result;
                }
            }

            // No success or running results were returned from children
            return NodeResult.Failure;
        }
    }
}