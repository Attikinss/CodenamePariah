using UnityEngine;
using FMODUnity;
public class FMODAudioEvent : MonoBehaviour
{
    public enum EventType
    {
        None,
        Fire,
        Reload,
        EmptyClip,

        Open,
        Close,
    }

    public EventType Type = EventType.None;

    [SerializeField]
    [FMODUnity.EventRef]
    private string EventID = "";

    private FMOD.Studio.EventInstance m_EventInstance;

    private bool m_HasManualPosition = false;
    private Transform m_ManualTransform = null; // Manually attached transform. (To be used if you don't want to rely on what the event's gameobject's position is.)

    private void Awake()
    {
        m_EventInstance = FMODUnity.RuntimeManager.CreateInstance(EventID);
        m_EventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    public void Trigger() => m_EventInstance.start();
	public void StopSound(FMOD.Studio.STOP_MODE mode)
	{
        m_EventInstance.stop(mode);
	}
    private void Update()
    {
        if (!m_HasManualPosition)
            m_EventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        else // If we do have m_HasManualPosition toggled, that means there is a reference to a transform we should be updating to.
        {
            if(m_ManualTransform) // Only if it still exists can we use it's transform.
                m_EventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(m_ManualTransform));
        }
    }

    /// <summary>
    /// To be used when other classes need a reference to the instance.
    /// </summary>
    /// <returns>The attached FMOD event instance.</returns>
    public ref FMOD.Studio.EventInstance GetEventInstance()
    {
        return ref m_EventInstance;
    }

    /// <summary>
    /// Toggling manual position is so that we can have some sounds be attached to places other than where their
    /// gameobject's transform is.
    /// </summary>
    /// <param name="toggle"></param>
    public void ToggleManualPosition(bool toggle, Transform obj)
    {
        m_HasManualPosition = toggle;
        m_ManualTransform = obj;
    }

    /// <summary>
    /// Returns true if the sound instance is playing and false if it has stopped.
    /// </summary>
    /// <returns>A bool specifying whether or not the sound is playing.</returns>
    public bool IsPlayingSound()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        m_EventInstance.getPlaybackState(out state);
        if (state != FMOD.Studio.PLAYBACK_STATE.STOPPED)
            return true; // We are playing the sound.


        return false;
    }

    /// <summary>
    /// Use to stop all the sounds in the game.
    /// </summary>
    public static void StopAllSounds()
    {
        FMOD.Studio.Bus allBussess = RuntimeManager.GetBus("bus:/");
        allBussess.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
