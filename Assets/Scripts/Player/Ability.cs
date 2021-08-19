using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AbilityScriptableObject")]
public class Ability : ScriptableObject
{
    public string m_AbilityName;
    public Image m_Icon { get; set; }

    public float m_Cooldown;
    private float m_Counter = 0.0f;
    private bool m_Charging = false;


    private bool m_Active = false;


    public void SetIcon(Image icon) { m_Icon = icon; }
    public bool IsActive() { return m_Active; }
    public float GetCounter() { return m_Counter; }
    private void Awake()
    {
        Debug.Log("Ability Awake() function called.");
        
    }

    /// <summary>
    /// This is not a Unity Update() function so I'm calling it somewhere manually.
    /// </summary>
    public void Update()
    {
        if (m_Active)
        {
            // Activate cooldown.
            m_Counter += Time.deltaTime;
            if (m_Counter >= m_Cooldown)
            {
                m_Active = false;
                m_Counter = 0;
            }
        }

        IconCooldown();
    }

    public void Invoke(bool inputActive)
    {
        // Do all actions.
        if(inputActive && !m_Active)
            m_Active = true;
    }

    private void IconCooldown()
    {
        if (m_Active)
            m_Icon.fillAmount = m_Counter / m_Cooldown;
        else
            m_Icon.fillAmount = 1;
    }
}
