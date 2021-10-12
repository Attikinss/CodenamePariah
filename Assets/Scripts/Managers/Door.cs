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
            m_IsOpen = true;
            m_openDoorObj.SetActive(true);
            m_closedDoorObj.SetActive(false);
        }
        else
        {
            m_IsOpen = false;
            m_openDoorObj.SetActive(false);
            m_closedDoorObj.SetActive(true);
        }

    }
}
