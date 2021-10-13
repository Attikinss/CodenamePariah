using UnityEngine;

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
	private void Update() => m_EventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
}
