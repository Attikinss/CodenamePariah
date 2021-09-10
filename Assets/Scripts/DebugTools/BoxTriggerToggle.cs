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
                break;
            case ToggleType.TurnOff:
                m_ToggleObject?.SetActive(false);
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
