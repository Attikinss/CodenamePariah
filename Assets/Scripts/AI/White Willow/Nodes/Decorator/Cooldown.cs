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

        protected override void OnExit() { }

        protected override NodeResult OnTick()
        {
            if (m_CachedTime < Time.time)
            {
                if (Child != null)
                {
                    NodeResult result = Child.Tick();

                    if (result != NodeResult.Running)
                        m_CachedTime = Time.time + m_CooldownTime;

                    return result;
                }
            }

            return NodeResult.Failure;
        }
    }
}