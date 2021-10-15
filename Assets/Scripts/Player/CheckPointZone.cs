using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointZone : MonoBehaviour
{
    [Tooltip("Checkpoints must have a unique level number!")]
    public int m_CheckPointLevel = 0;

	private void OnTriggerEnter(Collider other)
	{
		PariahController pariah;
		WhiteWillow.Agent agent;
		if (other.TryGetComponent<PariahController>(out pariah))
		{
			// Player has walked into this check point.
			GameManager.SetCheckPoint(m_CheckPointLevel, transform.position);
		}
		else if (other.TryGetComponent<WhiteWillow.Agent>(out agent))
		{
			if (agent.Possessed)
				GameManager.SetCheckPoint(m_CheckPointLevel, transform.position);

		}
	}
}
