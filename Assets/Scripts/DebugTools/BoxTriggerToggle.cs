using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTriggerToggle : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public enum ToggleType
    {
        /// <summary>Turn on a gameobject.</summary>
        turnOn,
        /// <summary>Turn off a gameobject.</summary>
        turnOff,
        /// <summary>Toggle a gameobject.</summary>
        toggle
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

    /// <summary>Has the target entity entered this collider?</summary>
    bool m_HasEntered = false;

    /// <summary>Has this trigger been activated?</summary>
    bool m_HasTriggered = false;

    [SerializeField]
    [Tooltip("Is of type OnTriggerEnter.")]
    private bool m_Enter = true;

    [SerializeField]
    [Tooltip("Is of type OnTriggerStay.")]
    private bool m_Stay = false;

    [SerializeField]
    [Tooltip("Is of type OnTriggerExit.")]
    private bool m_Exit = false;

    delegate void TriggerState();

    TriggerState OnEnter;

    TriggerState OnStay;

    TriggerState OnExit;

    /// <summary>Sphere colliders of all target entities.</summary>
    SphereCollider[] m_EntitiesColliders;//may need to change from sphere collider.

    /// <summary>The box collider of this gameobject.</summary>
    BoxCollider m_BoxCollider;

    void Awake()
    {
        m_BoxCollider = GetComponent<BoxCollider>();

        if (m_Enter)
        {
            OnEnter = Trigger;
        }
        else
        {
            OnEnter = delegate { };
        }

        if (m_Stay)
        {
            OnStay = Trigger;
        }
        else
        {
            OnStay = delegate { };
        }

        if (m_Exit)
        {
            OnExit = Trigger;
        }
        else
        {
            OnExit = delegate { };
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_EntitiesColliders = new SphereCollider[m_TargetEntities.Length];
        for (int i = 0; i < m_TargetEntities.Length; i++)
        {
            m_EntitiesColliders[i] = m_TargetEntities[i].GetComponent<SphereCollider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_HasTriggered = false;
        for (int i = 0; i < m_TargetEntities.Length; i++)
        {
            if (m_HasEntered)
            {
                if (!IsEntityInside(i))
                {
                    m_HasEntered = false;
                    OnExit();
                }
                else
                {
                    OnStay();
                }
            }
            else
            {
                if (IsEntityInside(i))
                {
                    m_HasEntered = true;
                    OnEnter();
                }
            }
        }
    }

    void LateUpdate()
    {
        if (m_DestroyOnActivate && m_HasTriggered)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>Checks if entity is inside this trigger.</summary>
    /// <param name="index"></param>
    /// <returns>True if entity is inside this trigger.</returns>
    bool IsEntityInside(int index)
    {
        Vector3 difference = m_TargetEntities[index].transform.position - m_BoxCollider.ClosestPoint(m_TargetEntities[index].transform.position);
        return (difference.magnitude < m_EntitiesColliders[index].radius);
    }

    /// <summary>
    /// Changes the state of a gameobject depending on ToggleType.
    /// </summary>
    void Trigger()
    {
        switch (m_ToggleType)
        {
            case ToggleType.turnOn:
                m_ToggleObject.SetActive(true);
                break;
            case ToggleType.turnOff:
                m_ToggleObject.SetActive(false);
                break;
            case ToggleType.toggle:
                m_ToggleObject.SetActive(!m_ToggleObject.activeSelf);
                break;
        }

        m_HasTriggered = true;
    }
}
