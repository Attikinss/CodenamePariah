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
        if (wep > m_Weapons.Count - 1 || wep < 0)
            return false;
        else if (m_Weapons[wep] != null)
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
    public void RemoveWeapon(int wep)
    {
        if (!HasWeapon(wep))
            return;

        if (m_CurrentWeaponNum == wep) // We are removing the weapon we are holding.
        {
            // If we are the last gun in the list, we have to tell the player to hold the previous weapon in the list, if there is one.
            if (m_CurrentWeaponNum == m_Weapons.Count - 1)
            {
                if (m_Weapons.Count > 1) // If there is atleast more than 1 gun we can move to the previous gun.
                {
                    m_CurrentWeaponNum--;
                }
                else
                {
                    // Do nothing. m_CurrentWeaponNum will be left at 0.
                }
            }
            // We are holding the gun we are removing, but it's not the last in the list. We don't have to move to the previous gun in this case.
            else if (m_CurrentWeaponNum < m_Weapons.Count - 1)
            { 
                // Do nothing.
            }
            
        }

        else if (m_CurrentWeaponNum > wep) // We are holding a weapon further up in the list than the one we are removing.
        {
            // We have to subtract - to current weapon num since we are being the list is getting smaller.
            m_CurrentWeaponNum--;
        }
        HideWeapon(wep); // Hiding the weapon we will remove. We wont destroy it, we'll just hide it.
        m_Weapons.RemoveAt(wep);

        if (m_Weapons.Count > 0) // If we haven't removed every single weapon from the list.
        { 
            m_CurrentWeapon = m_Weapons[m_CurrentWeaponNum]; // Setting the current weapon to the updated list.
            UnhideWeapon(m_CurrentWeaponNum); // If we have moved weapons, we should unhide the newly equipped weapon. Sometimes this will be redundant.
        }
    }
    public void UpgradeWeapon(int weapon, GameObject newPrefab, Weapon newWeapon, WeaponConfiguration newConfig)
    {
        if (!HasWeapon(weapon))
            return;

        GameObject holder = m_Weapons[weapon].gameObject; // --> GunHolder object;
        GameObject camera = holder.gameObject; // --> Thing we will attach new weapon prefab to.

        GameObject newWeaponPrefab = Instantiate(newPrefab);
        newWeaponPrefab.transform.SetParent(camera.transform); // The problem with adding a new prefab is that the position might not be in the bottom right hand corner (typical FPS gun
                                                               // position). Hopefully if the prefab's position is set properly it's position will be correct.
    }
}
