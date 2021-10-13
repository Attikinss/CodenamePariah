using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Decorator")]
    public class Timeout : Decorator
    {
        public NodeMember<float> Duration;
        private float m_StartTime = 0.0f;

        protected override void OnEnter()
        {
            Duration.Validate(Owner.Blackboard);

            m_StartTime = Time.time;
        }

        protected override void OnExit()
        {

        }

        protected override NodeResult OnTick()
        {
            if (Time.time - m_StartTime >= Duration.Value)
                return NodeResult.Failure;

            return Child.Tick();
        }
    }
}