using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GENERATION_TYPE
{ 
    HOST,
    PARIAH,
    BOTH
}
public class RegenerationZone : MonoBehaviour
{
    public GENERATION_TYPE m_Type;
	[Range(0, 100)]
	public int m_HealingPerTick = 5;

	[Range(0, 10)]
	public int m_TickRate = 1;

	Coroutine m_HealingRoutine;

	[ReadOnly]
	public bool m_StandingInZone = false;


	// Note. It's cheaper to get the Inventory script when trying to get Agent's because the Inventory script already has a reference to the WhiteWillow.Agent script.
	private void OnTriggerEnter(Collider other)
	{
		if (m_Type == GENERATION_TYPE.HOST)
		{
			Inventory agentInv;
			if (other.TryGetComponent<Inventory>(out agentInv) && agentInv.Owner.Possessed) // Only apply to agents that the player is controlling.
			{
				m_StandingInZone = true;
				StartCoroutine(Regenerate(m_HealingPerTick, m_TickRate, agentInv));
			}
		}
		else if (m_Type == GENERATION_TYPE.PARIAH)
		{
			// We only want to heal pariah.
			PariahController pariah;
			if (other.TryGetComponent<PariahController>(out pariah))
			{
				m_StandingInZone = true;
				StartCoroutine(Regenerate(m_HealingPerTick, m_TickRate, null, pariah));
			}
		}
		else if (m_Type == GENERATION_TYPE.BOTH)
		{
			// This zone is selected to health both pariah and the agent.
			PariahController pariah = GameManager.s_Instance.m_Pariah;
			Inventory agentInv;

			if (other.TryGetComponent<Inventory>(out agentInv) && agentInv.Owner.Possessed) // If the player walks into this zone as an agent, heal them up.
			{
				m_StandingInZone = true;
				StartCoroutine(Regenerate(m_HealingPerTick, m_TickRate, agentInv, pariah));
			}
			else if (other.TryGetComponent<PariahController>(out pariah)) // Or, they may just walk into it as pariah, so in this case, we can't heal any agent but we'll still heal their Pariah life essence.
			{
				m_StandingInZone = true;
				StartCoroutine(Regenerate(m_HealingPerTick, m_TickRate, null, pariah));
			}

		}

	}

	private void OnTriggerExit(Collider other)
	{
		if (m_Type == GENERATION_TYPE.HOST)
		{
			WhiteWillow.Agent agent;
			if (other.TryGetComponent<WhiteWillow.Agent>(out agent) && agent.Possessed)
			{
				// If the player leaves the trigger, stop healing them.
				m_StandingInZone = false; // The coroutine should see that this bool is false and gracefully exit.
			}

		}
		else if (m_Type == GENERATION_TYPE.PARIAH)
		{
			PariahController pariah;
			if (other.TryGetComponent<PariahController>(out pariah))
			{
				m_StandingInZone = false;
			}
		}

		else if (m_Type == GENERATION_TYPE.BOTH)
		{
			PariahController pariah;
			if (other.TryGetComponent<PariahController>(out pariah)) // Even though they've selected both, I think we only have to check for the pariah controller.
			{														 // Because even if they are in a host, the pariah controller will also still be there and all we need
				m_StandingInZone = false;							 // to do is set m_StandingInZone to false if they leave.
			}
		}
	}

	IEnumerator Regenerate(int healingPerTick, int tickRate, Inventory agentInv = null, PariahController pariah = null)
	{
		float time = 0;
		while (m_StandingInZone)
		{
			time += Time.deltaTime;
			if (time >= tickRate)
			{
				time = 0; // Reset time so it can count the next tick.

				if (agentInv)
				{
					// If they've passed in an agent inventory, lets heal them up.
					agentInv.AddHealth(healingPerTick);
				}
				if (pariah)
				{
					// If they've passed in pariah's controller, lets heal up pariah.
					pariah.AddHealth(healingPerTick);
				}
			}

			yield return null;

		}
	}
}
