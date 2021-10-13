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

	

	[ReadOnly]
	public bool m_StandingInZone = false;

	[ReadOnly]
	public bool m_Held;

	// Cache
	PariahController m_currentPariah = null;
	Inventory m_currentInv = null; 

	private void Start()
	{
		m_Held = GameManager.s_Instance.IsHoldingHeal;
		
	}
	private void Update()
	{
		m_Held = GameManager.s_Instance.IsHoldingHeal;

	}

	// Note. It's cheaper to get the Inventory script when trying to get Agent's because the Inventory script already has a reference to the WhiteWillow.Agent script.
	private void OnTriggerEnter(Collider other)
	{
		UIManager uiManager = UIManager.s_Instance;

		
		if (m_Type == GENERATION_TYPE.HOST)
		{
			Inventory agentInv;
			if (other.TryGetComponent<Inventory>(out agentInv) && agentInv.Owner.Possessed) // Only apply to agents that the player is controlling.
			{
				uiManager.ToggleGenertationText(true);
				m_StandingInZone = true;
				m_currentInv = agentInv;
			}
		}
		else if (m_Type == GENERATION_TYPE.PARIAH)
		{
			// We only want to heal pariah.
			PariahController pariah;
			if (other.TryGetComponent<PariahController>(out pariah))
			{
				uiManager.ToggleGenertationText(true);
				m_StandingInZone = true;
				m_currentPariah = pariah;
			}
		}
		else if (m_Type == GENERATION_TYPE.BOTH)
		{
			// This zone is selected to health both pariah and the agent.
			PariahController pariah = GameManager.s_Instance.m_Pariah;
			Inventory agentInv;

			if (other.TryGetComponent<Inventory>(out agentInv) && agentInv.Owner.Possessed) // If the player walks into this zone as an agent, heal them up.
			{
				uiManager.ToggleGenertationText(true);
				m_StandingInZone = true;
				m_currentInv = agentInv;
			}
			else if (other.TryGetComponent<PariahController>(out pariah)) // Or, they may just walk into it as pariah, so in this case, we can't heal any agent but we'll still heal their Pariah life essence.
			{
				uiManager.ToggleGenertationText(true);
				m_StandingInZone = true;
				m_currentPariah = pariah;
			}

		}

	}

	private void OnTriggerStay(Collider other)
	{
		switch (m_Type)
		{
			case GENERATION_TYPE.HOST:
				if (m_currentInv)
				{
					TryRegenerate(m_HealingPerTick, m_TickRate, null, GameManager.s_Instance.m_Pariah);
				}
				break;
			case GENERATION_TYPE.PARIAH:
				if (m_currentPariah)
				{
					TryRegenerate(m_HealingPerTick, m_TickRate, null, m_currentPariah);
				}
				break;
			case GENERATION_TYPE.BOTH:
				if (m_currentInv && m_currentPariah)
				{
					TryRegenerate(m_HealingPerTick, m_TickRate, m_currentInv, m_currentPariah);
				}
				break;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		UIManager uiManager = UIManager.s_Instance;
		if (m_Type == GENERATION_TYPE.HOST)
		{
			WhiteWillow.Agent agent;
			if (other.TryGetComponent<WhiteWillow.Agent>(out agent) && agent.Possessed)
			{
				uiManager.ToggleGenertationText(false);
				// If the player leaves the trigger, stop healing them.
				m_StandingInZone = false; // The coroutine should see that this bool is false and gracefully exit.
				m_currentInv = null; // Setting to null so OnTriggerStay() doesn't attempt to call heal.
				GameManager.s_Instance.m_HealingRoutineActive = false;
			}

		}
		else if (m_Type == GENERATION_TYPE.PARIAH)
		{
			PariahController pariah;
			if (other.TryGetComponent<PariahController>(out pariah))
			{
				uiManager.ToggleGenertationText(false);
				m_StandingInZone = false;
				m_currentInv = null;
				GameManager.s_Instance.m_HealingRoutineActive = false;
			}
		}

		else if (m_Type == GENERATION_TYPE.BOTH)
		{
			PariahController pariah;
			if (other.TryGetComponent<PariahController>(out pariah)) // Even though they've selected both, I think we only have to check for the pariah controller.
			{														 // Because even if they are in a host, the pariah controller will also still be there and all we need
				m_StandingInZone = false;                            // to do is set m_StandingInZone to false if they leave.
				uiManager.ToggleGenertationText(false);
				m_currentInv = null;
				m_currentPariah = null;
				GameManager.s_Instance.m_HealingRoutineActive = false;
			}
		}
	}

	private void TryRegenerate(int healingPerTick, int tickRate, Inventory agentInv = null, PariahController pariah = null)
	{
		if (GameManager.s_Instance.IsHoldingHeal && !GameManager.s_Instance.m_HealingRoutineActive)
		{
			GameManager.s_Instance.m_HealingRoutine = GameManager.s_Instance.StartCoroutine(Regenerate(healingPerTick, tickRate, agentInv, pariah));
			GameManager.s_Instance.m_HealingRoutineActive = true;
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

		GameManager.s_Instance.m_HealingRoutineActive = false;
	}
}
