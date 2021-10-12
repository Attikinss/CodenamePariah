using UnityEngine;

namespace WhiteWillow.Nodes
{
    public enum RepeatCondition { Indefinite, UntilCount, UntilFailure, UntilSuccess }

    [Category("Decorator")]
    public sealed class Repeat : Decorator
    {
        public override string IconPath { get; } = "Icons/Repeat";

        public RepeatCondition RepeatCondition = RepeatCondition.Indefinite;

        public bool RepeatIfFailure = true;
        public uint Repeats = 0;
        [SerializeField]
        private uint m_CurrentRepeats = 0;

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {
            m_CurrentRepeats = 0;
        }

        protected override NodeResult OnTick()
        {
            // No point doing anything
            if (Child == null)
                return NodeResult.Failure;

            // Repeat according to condition
            switch (RepeatCondition)
            {
                case RepeatCondition.Indefinite:
                    {
                        State = Child.Tick();

                        // TODO: Implement abort

                        break;
                    }
                case RepeatCondition.UntilCount:
                    {
                        State = Child.Tick();

                        if (State != NodeResult.Running)
                        {
                            if ((State == NodeResult.Failure && !RepeatIfFailure) || ++m_CurrentRepeats >= Repeats)
                                return State;

                            // TODO: Implement abort
                        }

                        break;
                    }
                case RepeatCondition.UntilFailure:
                    {
                        State = Child.Tick();

                        // Ensure node runs until it either succeeds or is aborted
                        if (State == NodeResult.Failure)
                            return State;

                        // TODO: Implement abort

                        break;
                    }
                case RepeatCondition.UntilSuccess:
                    {
                        State = Child.Tick();

                        // Ensure node runs until it either succeeds or is aborted
                        if (State == NodeResult.Success)
                            return State;

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