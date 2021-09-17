using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Current health.")]
    private int m_Health = 100;

    [SerializeField]
    [Tooltip("Health Text UI.")]//
    private TextMeshProUGUI m_HealthText;

    [SerializeField]
    [Tooltip("Health Sprite UI")]
    private GameObject m_HealthSprite;

    [SerializeField]
    [Tooltip("Weapons this character has.")]
    public List<Weapon> m_Weapons;  // Has been made public so I can access it within the HostController.cs script. Only temporary.
    
    [HideInInspector]
    public Weapon m_CurrentWeapon; // I know this wasn't public by default, but I'm going to make it publically accessible so that I can swap weapons around form the HostController.cs script.
    private int m_CurrentWeaponNum = 0;

    public WhiteWillow.Agent Owner { get; private set; }

    /// <summary>
    /// I've added a Awake() function here because m_CurrentWeapon was always unintialised. I'm going to initialise it here.
    /// </summary>
	private void Awake()
	{
        Owner = GetComponent<WhiteWillow.Agent>();

        if (m_Weapons.Count > 0)
            m_CurrentWeapon = m_Weapons[0]; // For now, m_CurrentWeapon will always start off as the first element in the m_Weapons list.
	}

    /// <summary>Displays the current health.</summary>
    void UpdateHealthUI() //move to ui Manager
    {
        // Doesn't matter that much given agents are
        // destroyed along with everything attached
        m_HealthSprite.SetActive(m_Health > 0);

        m_HealthText?.SetText(m_Health.ToString());
    }

    /// <summary>Takes health away equal to the damage value.</summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage, bool fromAbility = false)
    {
        m_Health -= damage;
        if (m_Health <= 0)
        {
            if (TryGetComponent(out WhiteWillow.Agent agent))
            {
                if (agent.Possessed)
                {
                    if (fromAbility)
                        Telemetry.TracePosition("Agent-PlayerKill", transform.position);
                    else
                        Telemetry.TracePosition("Agent-Death", transform.position);
                }
                else
                    Telemetry.TracePosition("Agent-PlayerKill", transform.position);

                agent.Kill();
            }
        }

        UpdateHealthUI();
    }

    public void AddHealth(int amount)
    {
        // TODO: Replace hard coded max health
        m_Health = (int)Mathf.Clamp(m_Health + amount, 0, 100);

        UpdateHealthUI();
    }

    public int GetHealth() { return m_Health; }

    public bool HasWeapon(int wep) 
    {
        if (m_Weapons[wep] != null)
            return true;
        else
            return false;
    }
    public void SetWeapon(int wep)
    {
        if (wep < m_Weapons.Count && wep >= 0) // It's a valid wep number.
        {
            m_CurrentWeapon.ResetFire();
            m_CurrentWeapon.ResetAim();

            // Hiding old weapon.
            HideWeapon(m_CurrentWeaponNum);

            m_CurrentWeaponNum = wep;
            m_CurrentWeapon = m_Weapons[wep]; // Reassigning m_CurrentWeapon to match new weapon.

            // Unhiding new weapon.
            UnhideWeapon(m_CurrentWeaponNum);
        }
    }
    public void UnhideWeapon(int wep) { m_Weapons[wep].gameObject.SetActive(true); }
    public void HideWeapon(int wep) { m_Weapons[wep].gameObject.SetActive(false); }
}
