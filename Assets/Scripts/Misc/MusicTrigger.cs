using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent<WhiteWillow.Agent>(out WhiteWillow.Agent agent))
		{
			if (agent.Possessed)
			{
				// Do the thing.
				GameManager.s_Instance.RestartMusic();
			}
		}
		else if (other.gameObject == GameManager.s_Instance.m_Pariah)
		{
			// Do the thing.
			GameManager.s_Instance.RestartMusic();
		}
	}
}
