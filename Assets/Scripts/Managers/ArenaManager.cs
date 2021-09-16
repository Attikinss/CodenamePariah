using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public GameObject[] m_ArenaAgents;
    public GameObject m_OpenDoor;
    public GameObject m_ClosedDoor;
    public int m_MinimumKills = 5;
    int counter = 0;

    private void Update()
    {
        if (EnemyCount())
        {
            //Temporary code - will need animations.
            m_OpenDoor.SetActive(true);
            m_ClosedDoor.SetActive(false);
            this.GetComponent<ArenaManager>().enabled = false;
        }
    }

    bool EnemyCount()
    {
        counter = 0;
        for (int i = 0; i < m_ArenaAgents.Length; i++)
        {
            if (m_ArenaAgents[i] == null)
                counter++;
        }

        if (counter >= m_MinimumKills)
            return true;
        else
            return false;
    }
}
