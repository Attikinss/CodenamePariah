using UnityEngine;

namespace WhiteWillow.Nodes
{
    [Category("Tasks")]
    public class PlaySound : Task
    {
        public AudioClip Sound;

        [Tooltip("Mark the node as running until the sound has finished playing.")]
        public bool RunUntilFinished = false;

        [Tooltip("Fails the node if a sound hasn't been assigned.")]
        public bool FailIfNoSound = false;

        private AudioSource m_Source;

        protected override void OnEnter()
        {
            m_Source = Owner.Agent.gameObject.AddComponent<AudioSource>();
            if (m_Source.clip == null) m_Source.clip = Sound;
        }

        protected override void OnExit()
        {

        }

        protected override NodeResult OnTick()
        {
            if (Sound == null)
            {
                if (FailIfNoSound)
                    return NodeResult.Failure;

                return NodeResult.Success;
            }
            else
            {
                if (RunUntilFinished)
                {
                    if (!m_Source.isPlaying)
                        m_Source.Play();
                    else if (m_Source.time >= Sound.length - 0.1f)
                        return NodeResult.Success;
                
                    return NodeResult.Running;
                }

                m_Source.Play();
                return NodeResult.Success;
            }
        }
    }
}