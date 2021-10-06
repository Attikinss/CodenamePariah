using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Tasks")]
    public class Wait : Task
    {
        public override string IconPath { get; } = "Icons/Wait";

        public enum WaitValueType { Fixed, Varying, Random }

        [Tooltip("Determines the final value of the wait time.\nFixed = Duration never changes\nVarying = Duration slightly flucuates around the original value\nRandom = Duration is randomised")]
        public WaitValueType WaitType = WaitValueType.Fixed;

        [Tooltip("The wait time.")]
        [Range(0.0f, 30.0f)]
        public float Duration = 1.0f;

        [SerializeField]
        //[ReadOnly]
        private float m_ElapsedTime = 0.0f;
        private float m_StartTime = 0.0f;
        private float m_Duration = 0.0f;

        protected override void OnEnter()
        {
            if (WaitType == WaitValueType.Varying)
                m_Duration += Mathf.Clamp(Random.Range(-Duration, Duration) / 2, 0.0f, 5.0f);
            else if (WaitType == WaitValueType.Random)
                m_Duration = Random.Range(0.0f, 5.0f);
            else
                m_Duration = Duration;

            m_StartTime = Time.time;
        }

        protected override void OnExit()
        {

        }

        protected override NodeResult OnTick()
        {
            m_ElapsedTime = Time.time - m_StartTime;
            if (m_ElapsedTime >= m_Duration)
                return NodeResult.Success;

            return NodeResult.Running;
        }
    }
}