using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [Tooltip("Agents in the room.")]
    public GameObject[] m_ArenaAgents;

    [Tooltip("Door gameobject to open.")]
    public GameObject m_OpenDoor;

    [Tooltip("Door gameobject to close.")]
    public GameObject m_ClosedDoor;

    [SerializeField]
    [Tooltip("Minimum amount of kills required before opening a door.")]
    private int m_MinimumKills = 5;

    [Tooltip("Counter that increments when an agent in the room is dead.")]
    int counter = 0;

    private void Update()
    {
        if (EnemyCount())
        {
            //Temporary code - will need animations.
            if (m_OpenDoor != null)
                m_OpenDoor.SetActive(true);
            if (m_ClosedDoor != null)
                m_ClosedDoor.SetActive(false);
            this.GetComponent<ArenaManager>().enabled = false;
        }
    }

    /// <summary>Checks whether enough enemies are dead and if there are, opens door then deactivates this script.</summary>
    /// <returns></returns>
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
