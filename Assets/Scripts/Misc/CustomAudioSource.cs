using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviour to help play generic gameplay sounds like alarms and one time voice lines. The idea
/// is to activate the gameobject this script is attached to so that the sound plays.
/// </summary>
public class CustomAudioSource : MonoBehaviour
{
    public FMODAudioEvent m_AudioEvent;
    public bool m_PlayOnStart = true;

    // Start is called before the first frame update
    /// <summary>
	/// Important that playing sounds happens in start, because in the GameManager's Awake() function
	/// we are muting all audio busses.
	/// </summary>
    public void Start()
    {
        if (m_AudioEvent)
            m_AudioEvent.Trigger();
        else
            Debug.LogWarning("A CustomAudioSource component is missing a reference to it's FMODAudioEvent.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
