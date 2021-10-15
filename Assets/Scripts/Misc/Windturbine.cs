using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windturbine : MonoBehaviour
{
    public FMODAudioEvent m_AudioTurbineEvent;

	public void Start()
	{
		if (m_AudioTurbineEvent)
			m_AudioTurbineEvent.Trigger();
		
	}
}
