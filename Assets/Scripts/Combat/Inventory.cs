using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Current health.")]
    private int m_Health = 100;

    //[SerializeField]
    //[Tooltip("Health Text UI.")]//
    //private TextMeshProUGUI m_HealthText;

    //[SerializeField]
    //[Tooltip("Health Sprite UI")]
    //private GameObject m_HealthSprite;

    [SerializeField]
    [Tooltip("Weapons this character has.")]
    public List<Weapon> m_Weapons;  // Has been made public so I can access it within the HostController.cs script. Only temporary.
    
    [HideInInspector]
    public Weapon m_CurrentWeapon; // I know this wasn't public by default, but I'm going to make it publically accessible so that I can swap weapons around form the HostController.cs script.
    private int m_CurrentWeaponNum = 0;

    public WhiteWillow.Agent Owner { get; private set; }

    UIManager m_UIManager;

    public Camera m_Camera; // This transform will be used when we add new weapons to the inventory.
    private HostController m_Controller;

    /// <summary>
    /// I've added a Awake() function here because m_CurrentWeapon was always unintialised. I'm going to initialise it here.
    /// </summary>
	private void Awake()
	{
        Owner = GetComponent<WhiteWillow.Agent>();

        if (m_Weapons.Count > 0)
            m_CurrentWeapon = m_Weapons[0]; // For now, m_CurrentWeapon will always start off as the first element in the m_Weapons list.

        if (!m_Camera)
            Debug.LogError("This inventory script is missing a reference to a camera transform!");
	}

	private void Start()
	{
        m_UIManager = UIManager.s_Instance;
        m_Controller = GetComponent<HostController>(); // This assumes the inventory is on the same object as the HostController.cs script.
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

        m_UIManager.UpdateHealthUI();
    }

    public void AddHealth(int amount)
    {
        // TODO: Replace hard coded max health
        m_Health = (int)Mathf.Clamp(m_Health + amount, 0, 100);

        m_UIManager.UpdateHealthUI();
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
            if (m_CurrentWeapon) // If we have a current weapon, hide it and get ready to swap to the new one.
            { 
                m_CurrentWeapon.ResetFire();
                m_CurrentWeapon.ResetAim();

                // Hiding old weapon.
                HideWeapon(m_CurrentWeaponNum);
            }

            m_CurrentWeaponNum = wep;
            m_CurrentWeapon = m_Weapons[wep]; // Reassigning m_CurrentWeapon to match new weapon.

            // Unhiding new weapon.
            UnhideWeapon(m_CurrentWeaponNum);
        }
    }
    public void UnhideWeapon(int wep) { m_Weapons[wep].gameObject.SetActive(true); }
    public void HideWeapon(int wep) { m_Weapons[wep].gameObject.SetActive(false); }
    /// <summary>
    /// AddWeapon() pushes a new weapon to the back of the list.
    /// </summary>
    public bool AddWeapon(GameObject weaponPrefab)
    {
        Weapon prefabWeaponComponent;
        if (weaponPrefab.TryGetComponent<Weapon>(out prefabWeaponComponent))
        {
            GameObject newWeapon = Instantiate(weaponPrefab, m_Camera.transform);                               // The problem with adding a new prefab is that the position might not be in the bottom right hand corner (typical FPS gun
            //newWeapon.transform.SetParent(m_Camera); // --> Thing we attach the weapons to. // position). Hopefully if the prefab's position is set properly it's position will be correct.

            Weapon weaponComponent = newWeapon.GetComponent<Weapon>();
            weaponComponent.m_Inventory = this;
            weaponComponent.m_Controller = m_Controller;
            weaponComponent.SetCamera(m_Camera);

            weaponComponent.m_TransformInfo.m_OriginalLocalPosition = newWeapon.transform.localPosition;
            weaponComponent.m_TransformInfo.m_OriginalGlobalPosition = newWeapon.transform.position;

            m_Weapons.Add(weaponComponent);

            // New weapon has to be deactivated be default.
            newWeapon.SetActive(false);

            if (m_Weapons.Count == 1) // If we just added the only weapon we have, set current weapon to this weapon and update UI.
                SetWeapon(0);

            return true;

        }
        else
        { 
            Debug.LogError("Attempted to AddWeapon() but the passed in prefab is not a weapon!");
            return false;
        }

    }
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
        m_CurrentWeapon = null; // Resetting the current weapon reference.

        if (m_Weapons.Count > 0) // If we haven't removed every single weapon from the list.
        { 
            m_CurrentWeapon = m_Weapons[m_CurrentWeaponNum]; // Setting the current weapon to the updated list.
            UnhideWeapon(m_CurrentWeaponNum); // If we have moved weapons, we should unhide the newly equipped weapon. Sometimes this will be redundant.
        }

        m_UIManager.UpdateWeaponUI(m_CurrentWeapon);
    }
    public bool UpgradeWeapon(int weapon, GameObject newPrefab)
    {
        if (!HasWeapon(weapon))
            return false;

        // Remove old weapon since we are upgrading it.
        RemoveWeapon(weapon);

        // Add a new weapon according to the parameters.
        AddWeapon(newPrefab);

        // Since the new weapon is on the end of the list, we'll swap to the last element to make it seem like they are still holding on to the same gun.
        SetWeapon(m_Weapons.Count - 1);

        return true;
    }

    public WeaponConfiguration GetCurrentConfig() 
    {
        if (m_CurrentWeapon)
            return m_CurrentWeapon.m_Config;
        else
            return null;
    }

    public int GetWeaponNum() { return m_CurrentWeaponNum; }
}
