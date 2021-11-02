using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class is a place to store all the general sounds in the game that don't really belong
/// on any other monobehaviour. It will be able to be referenced by the GameManager and will act as a
/// sort of sound manager.
/// </summary>
public class GeneralSounds : MonoBehaviour
{
	// ======== Singleton Stuff ======== //
	public static GeneralSounds s_Instance = null;
	// ================================= //


	public FMODAudioEvent m_DashAudioEvent;

	// Because these sound effects aren't built into the same event, we have separate events which we have to call through code.
	public FMODAudioEvent m_HostEnterAudioEvent1;
	public FMODAudioEvent m_HostEnterAudioEvent2;


	public void Awake()
	{
		if (s_Instance == null)
		{
			s_Instance = this;
		}
		else
		{
			// s_Instance has already been set, so that means there is more than one GeneralSounds monobehaviour in the world.
			Debug.LogWarning("There are multiple GeneralSounds scripts in the scene!");
			//Destroy(gameObject);
		}
	}

	/// <summary>
	/// Will trigger the attached m_DashAudioEvent object. We also have to move the game object that the FMODAudioEvent is attached to to the player
	/// so that we can hear the sounds.
	/// </summary>
	public void PlayDashSound(Transform playerTrans)
	{
		m_DashAudioEvent.ToggleManualPosition(true, playerTrans);
		FMOD.Studio.EventInstance instance = m_DashAudioEvent.GetEventInstance();
		instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(playerTrans));
		m_DashAudioEvent.Trigger();
	}

	/// <summary>
	/// Plays the entering host sound event. Because the sound doesn't need to play every time, you can pass in a number that represents that
	/// probability of the sound occuring out of 100.
	/// </summary>
	/// <param name="playerTrans"></param>
	public void PlayHostEnterSound(Transform playerTrans)
	{
		// Pick what enter sound to play.
		int randomNum = Random.Range(0, 2); // 0-1
		FMODAudioEvent enterEvent;
		if (randomNum == 0)
			enterEvent = m_HostEnterAudioEvent1;
		else
			enterEvent = m_HostEnterAudioEvent2;


		// Stop previous sounds if they are still playing.
		m_HostEnterAudioEvent1.StopSound(FMOD.Studio.STOP_MODE.IMMEDIATE);
		m_HostEnterAudioEvent2.StopSound(FMOD.Studio.STOP_MODE.IMMEDIATE);

		enterEvent.ToggleManualPosition(true, playerTrans);
		FMOD.Studio.EventInstance instance = enterEvent.GetEventInstance();
		instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(playerTrans));
		enterEvent.Trigger();
	}

	

}



