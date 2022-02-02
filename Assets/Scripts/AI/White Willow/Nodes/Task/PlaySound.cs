using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Task")]
    public sealed class PlaySound : Task
    {
        [SerializeField]
        private AudioClip m_Sound;

        [SerializeField]
        [Tooltip("How long in seconds the sound is delayed before playing.")]
        [Min(0)]
        public float m_Delay = 0.0f;

        [SerializeField]
        [Tooltip("If set to true the agent will wait until the sound has finished before continuing.")]
        private bool m_WaitUntilFinished = false;

        private AudioSource m_Source;

        protected override void OnEnter()
        {
            // TODO: Add audio source selection

            if (m_Source != null && m_Sound != null)
            {
                m_Source.clip = m_Sound;
                m_Source.PlayDelayed(m_Delay);
            }
        }

        protected override void OnExit()
        {

        }

        protected override NodeResult OnTick()
        {
            if (m_Source.isPlaying && m_WaitUntilFinished)
                return NodeResult.Running;

            return NodeResult.Success;
        }
    }
}
