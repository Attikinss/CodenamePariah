using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhiteWillow;

public class BoxTriggerToggle : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public enum ToggleType
    {
        /// <summary>Turn on a gameobject.</summary>
        TurnOn,
        /// <summary>Turn off a gameobject.</summary>
        TurnOff,
        /// <summary>Toggle a gameobject.</summary>
        Toggle
    }

    [Tooltip("This determines whether or not the toggable object will be remembered between checkpoint loads.")]
    public bool m_ShouldSaveData = true;

    [Tooltip("This ID should be unique to this instance.")]
    public int m_ID = 0;

    [SerializeField]
    [Tooltip("The entities that can activate this script.")]
    private GameObject[] m_TargetEntities;

    [SerializeField]
    [Tooltip("The object to be toggled.")]
    private GameObject m_ToggleObject;

    [SerializeField]
    [Tooltip("What type of event will be activated here.")]
    private ToggleType m_ToggleType;

    [SerializeField]
    [Tooltip("Destroy this object upon activation?")]
    private bool m_DestroyOnActivate = false;

    [SerializeField]
    [Tooltip("Is of type OnTriggerEnter.")]
    private bool m_Enter = true;

    [SerializeField]
    [Tooltip("Is of type OnTriggerStay.")]
    private bool m_Stay = false;

    [SerializeField]
    [Tooltip("Is of type OnTriggerExit.")]
    private bool m_Exit = false;

    private delegate void TriggerState();
    private TriggerState OnEnter;
    private TriggerState OnStay;
    private TriggerState OnExit;

    void Awake()
    {
        if (m_Enter)
            OnEnter = Trigger;

        if (m_Stay)
            OnStay = Trigger;

        if (m_Exit)
            OnExit = Trigger;

        if (m_ShouldSaveData)
        { 
            // Telling the game manager about this toggle.
            // The ArenaManager script contains both an open and close game object since it is really a door manager script.
            // This script only has a reference to the object it will turn on or off, so we have to find out what this script will be doing (turning on or off) and
            // then pass that information to the GameManager.
            if (m_ToggleType == ToggleType.TurnOn)
            {
                GameManager.AddToggable(m_ID, m_ToggleObject, null, false); // This assumes closed by default.
            }
            else if (m_ToggleType == ToggleType.TurnOff)
            {
                GameManager.AddToggable(m_ID, null, m_ToggleObject, true); // This assumes open by default.
            }
        }
    }
    
    /// <summary>
    /// Changes the state of a gameobject depending on ToggleType.
    /// </summary>
    private void Trigger()
    {
        switch (m_ToggleType)
        {
            case ToggleType.TurnOn:
                m_ToggleObject?.SetActive(true);
                if(m_ShouldSaveData)
                    GameManager.s_Instance.SendDoorData(true, m_ID);
                break;
            case ToggleType.TurnOff:
                m_ToggleObject?.SetActive(false);
                if(m_ShouldSaveData)
                    GameManager.s_Instance.SendDoorData(false, m_ID);
                break;
            case ToggleType.Toggle:
                m_ToggleObject?.SetActive(!m_ToggleObject.activeSelf);
                break;
        }

        // May need to destroy the object as well
        if (m_DestroyOnActivate)
            Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Enter)
        {
            foreach (var target in m_TargetEntities)
            {
                if (other.gameObject == target)
                {
                    Trigger();
                    return;
                }
            }

            if (other.TryGetComponent(out Agent agent))
            {
                if (agent.Possessed)
                    Trigger();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_Stay)
        {
            foreach (var target in m_TargetEntities)
            {
                if (other.gameObject == target)
                {
                    Trigger();
                    return;
                }

            }

            if (other.TryGetComponent(out Agent agent))
            {
                if (agent.Possessed)
                    Trigger();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_Exit)
        {
            foreach (var target in m_TargetEntities)
            {
                if (other.gameObject == target)
                {
                    Trigger();
                    return;
                }
            }

            if (other.TryGetComponent(out Agent agent))
            {
                if (agent.Possessed)
                    Trigger();
            }
        }
    }
}
