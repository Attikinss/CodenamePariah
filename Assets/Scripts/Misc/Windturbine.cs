using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windturbine : MonoBehaviour
{
    public FMODAudioEvent m_AudioTurbineEvent;

	/// <summary>
	/// Important that playing sounds happens in start, because in the GameManager's Awake() function
	/// we are muting all audio busses.
	/// </summary>
	public void Start()
	{
		if (m_AudioTurbineEvent)
			m_AudioTurbineEvent.Trigger();
		
	}
}
