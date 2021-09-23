using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCupboard : MonoBehaviour
{
    public HostController[] m_InitialAgents;

    public GameObject[] m_Cupboards;

    [SerializeField]
    private float m_MinimumTotalHealthPercentage;

    [SerializeField]
    [ReadOnly]
    private float m_InitialHealth;

    [SerializeField]
    [ReadOnly]
    float m_CurrentHealth;

    private void Start()
    {
        InitialiseArenaHealthPool();
    }

    private void Update()
    {
        if (HealthPoolCheck())
        {
            for (int i = 0; i < m_Cupboards.Length; i++)
            {
                // do all the things.
                m_Cupboards[i].GetComponent<Animator>()?.SetTrigger("Start");
            }
            this.GetComponent<MonsterCupboard>().enabled = false;
        }
    }

    private void InitialiseArenaHealthPool()
    {
        for (int i = 0; i < m_InitialAgents.Length; i++)
        {
            m_InitialHealth += m_InitialAgents[i].m_Inventory.GetHealth();
        }
    }

    private bool HealthPoolCheck()
    {
        m_CurrentHealth = 0;
        for (int i = 0; i < m_InitialAgents.Length; i++)
        {
            if (m_InitialAgents[i] != null)
                m_CurrentHealth += m_InitialAgents[i].m_Inventory.GetHealth();
        }
        if (m_CurrentHealth <= (m_MinimumTotalHealthPercentage / 100) * m_InitialHealth)
            return true;
        else
            return false;
    }
}
