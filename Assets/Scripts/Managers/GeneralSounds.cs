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
}



