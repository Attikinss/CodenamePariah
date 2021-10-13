using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [Tooltip("This ID should be unique to this instance.")]
    public int m_ID = 0;

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

	private void Awake()
	{
        // On awake, we're gonna upload the doors to the GameManager so it has a reference to their state.
        GameManager.AddDoor(m_ID, m_OpenDoor, m_ClosedDoor, false); // Telling the GameManager about this door.
		
	}
	private void Start()
	{
	}
	private void Update()
    {
        if (EnemyCount())
        {
            //Temporary code - will need animations.
            m_OpenDoor.SetActive(true);
            m_ClosedDoor.SetActive(false);
            this.GetComponent<ArenaManager>().enabled = false;

            SendDoorData(true); // Telling the GameManager that this door has been opened.
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

    /// <summary>
    /// Sends updated door information to the GameManager so it has memory of the state of the door when it reloads at a checkpoint.
    /// </summary>
    private void SendDoorData(bool isOpen)
    {
        Door ourDoor = GameManager.GetDoor(m_ID);
        ourDoor.m_IsOpen = isOpen;
    }
}
