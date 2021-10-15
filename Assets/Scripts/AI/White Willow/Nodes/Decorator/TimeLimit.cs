using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Decorator")]
    public sealed class TimeLimit : Decorator
    {
        [SerializeField]
        private float m_TimeLimit = 2.5f;

        protected override void OnEnter() { }

        protected override void OnExit() { }

        protected override NodeResult OnTick()
        {
            if (Child == null || Child.Tick() != NodeResult.Running)
                return NodeResult.Failure;

            return NodeResult.Running;
        }
    }
}