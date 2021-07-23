using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteWillow.Nodes
{
    public enum RepeatCondition { Indefinite, UntilCount, UntilFailure, UntilSuccess }

    [Category("Decorator")]
    public class Repeat : Decorator
    {
        public override string IconPath { get; } = "Icons/Repeat";

        public RepeatCondition RepeatCondition = RepeatCondition.Indefinite;
        public uint Repeats = 0;
        [SerializeField]
        private uint m_CurrentRepeats = 0;

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {

        }

        protected override NodeResult OnTick()
        {
            // Prevents traversal of this node while it's locked
            if (Locked) return NodeResult.Locked;

            // No point doing anything
            if (Child == null)
            {
                m_CurrentRepeats = 0;
                return NodeResult.Failure;
            }

            // Repeat according to condition
            switch (RepeatCondition)
            {
                case RepeatCondition.Indefinite:
                    {
                        State = Child.Tick();

                        break;
                    }
                case RepeatCondition.UntilCount:
                    {
                        State = Child.Tick();

                        if (State != NodeResult.Running)
                        {
                            if (++m_CurrentRepeats >= Repeats)
                            {
                                // Reset
                                m_CurrentRepeats = 0;

                                return State;
                            }

                            // TODO: Implement abort
                        }

                        break;
                    }
                case RepeatCondition.UntilFailure:
                    {
                        State = Child.Tick();

                        // Ensure node runs until it either succeeds or is aborted
                        if (State == NodeResult.Failure)
                        {

                            return State;
                        }

                        // TODO: Implement abort

                        break;
                    }
                case RepeatCondition.UntilSuccess:
                    {
                        State = Child.Tick();

                        // Ensure node runs until it either succeeds or is aborted
                        if (State == NodeResult.Success)
                        {

                            return State;
                        }

                        // TODO: Implement abort

                        break;
                    }

                default:
                    {
                        return NodeResult.Failure;
                    }
            }

            (Parent as Composite)?.SetRunningChild(this);
            return NodeResult.Running;
        }
    }
}