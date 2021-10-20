using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Decorator")]
    public sealed class Cooldown : Decorator
    {
        public float m_CooldownTime = 2.5f;

        [ReadOnly]
        public float m_CachedTime = 0.0f;

        protected override void OnEnter() { }

        protected override void OnExit()
        {
            m_CachedTime = Time.time;
        }

        protected override NodeResult OnTick()
        {
            if (Time.time - m_CachedTime >= m_CooldownTime)
            {
                if (Child == null)
                    return Child.Tick();

                return NodeResult.Failure;
            }

            return NodeResult.Failure;
        }
    }
}