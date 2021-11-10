using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggableObject
{
    public string ID;

    public GameObject m_openObj = null;
    public GameObject m_closedObj = null;

    public bool m_IsOpen = false;

    public ToggableObject(string arenaID, GameObject openDoorObj, GameObject closeDoorObj, bool open)
    {
        ID = arenaID;
        openDoorObj = m_openObj;
        closeDoorObj = m_closedObj;
        m_IsOpen = open;
    }

    public void Toggle(bool toggle)
    {
        if (toggle)
        {
            // Currently theres an issue that some doors are not "Awoken" when the game starts, so they wont send the GameManager their object straight away.
            // The GameManager then tries to refresh all doors but doesn't have references to the doors. Safety check for now so it fails gracefully.
            //if (m_openObj /*|| m_closedObj*/) // BoxTriggerToggle created doors might not have an m_openObj or a m_closedObj so we can't rely on this.
            //{
                m_IsOpen = true;

            if (m_openObj) // Even though this toggable object has been set to open, it may not have an open object.
            {
                // The door might not have both a open and closed obj. This is because the ArenaManager nicely packs doors with open and closed references, but
                // other doors in the game are opened with the BoxTriggerToggle script which only has a reference for one, either the open or closed.
                if (m_openObj)
                    m_openObj.SetActive(true);
                if (m_closedObj)
                    m_closedObj.SetActive(false);
                //}
            }
            else
            {
                if (m_closedObj)
                    m_closedObj.SetActive(true);
            }
        }
        else
        {
            //if (m_openObj/* || m_closedObj*/) // BoxTriggerToggle created doors might not have an m_openObj or a m_closedObj so we can't rely on this.
            //{ 
                m_IsOpen = false;

            if (m_closedObj)        // Even though this object has been set to closed, it may not have a m_closedObj. 
            {
                if (m_openObj)
                    m_openObj.SetActive(false);
                if (m_closedObj)
                    m_closedObj.SetActive(true);
                //}
            }
            else
            {
                if (m_openObj)
                    m_openObj.SetActive(true);
            }
        }

    }
}
