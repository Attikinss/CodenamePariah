using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door
{
    public int ID = 0;

    public GameObject m_openDoorObj;
    public GameObject m_closedDoorObj;

    public bool m_IsOpen = false;

    public Door(int arenaID, GameObject openDoorObj, GameObject closeDoorObj, bool open)
    {
        ID = arenaID;
        openDoorObj = m_openDoorObj;
        closeDoorObj = m_closedDoorObj;
        m_IsOpen = open;
    }

    public void Toggle(bool toggle)
    {
        if (toggle)
        {
            // Currently theres an issue that some doors are not "Awoken" when the game starts, so they wont send the GameManager their object straight away.
            // The GameManager then tries to refresh all doors but doesn't have references to the doors. Safety check for now so it fails gracefully.
            if (m_openDoorObj && m_closedDoorObj)
            { 
                m_IsOpen = true;
                m_openDoorObj.SetActive(true);
                m_closedDoorObj.SetActive(false);
            }
        }
        else
        {
            if (m_openDoorObj && m_closedDoorObj)
            { 
                m_IsOpen = false;
                m_openDoorObj.SetActive(false);
                m_closedDoorObj.SetActive(true);
            }
        }

    }
}
