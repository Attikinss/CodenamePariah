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

    public FMODAudioEvent m_AudioDestroySound;

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


        // =========== Weapon Skinned Mesh Renderer Initialisation =========== //
        // Initialising the skinned mesh renderers here too incase they haven't been initalised yet.
        for (int i = 0; i < m_Weapons.Count; i++)
        {
            if (!m_Weapons[i].m_InitialisedSkinnedMeshRenderers)
                m_Weapons[i].InitSkinnedMeshes();
        }
        // This is a safety because weapons usually initalise this on Start() but most weapons start deactivated.
        // =================================================================== //
    }


    /// <summary>Takes health away equal to the damage value.</summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage, bool fromAbility = false)
    {
        m_Health -= damage;
        if (m_Health <= 0)
        {
            m_Health = 0;//
            if (TryGetComponent(out WhiteWillow.Agent agent))
            {
                PariahController pariah = GameManager.s_Instance.m_Pariah;
                if (agent.Possessed)
                {
                    if (fromAbility)
                    { 
                        Telemetry.TracePosition("Agent-PlayerKill", transform.position);
                        pariah.m_Power++; // Incrementing this so the power bar charges up.
                        // Set power bar ui to match.
                        m_UIManager.SetDeathIncarnateBar((float)pariah.m_Power / GameManager.s_CurrentHost.m_DeathIncarnateAbility.requiredKills);
                        if (pariah.m_Power >= m_Controller.m_DeathIncarnateAbility.requiredKills)
                        {
                            m_UIManager.ToggleReadyPrompt(false);
                        }
                    }
                    else
                        Telemetry.TracePosition("Agent-Death", transform.position);

                    UIManager.s_Instance.HideCanvas();
                }
                else
                { 
                    Telemetry.TracePosition("Agent-PlayerKill", transform.position);

                    if (GameManager.s_CurrentHost)
                    {
                        
                        pariah.m_Power++; // Incrementing this so the power bar charges up.
                        // Set power bar ui to match.
                        m_UIManager.SetDeathIncarnateBar((float)pariah.m_Power / GameManager.s_CurrentHost.m_DeathIncarnateAbility.requiredKills);
                        if (pariah.m_Power >= m_Controller.m_DeathIncarnateAbility.requiredKills)
                        {
                            m_UIManager.ToggleReadyPrompt(false);
                        }
                    }
                }

               
                PlayDestroySound();
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
            weaponComponent.m_CharIcon = m_UIManager.m_DualWieldCharIcon;
            weaponComponent.m_CharName = m_UIManager.m_DualWieldCharName;
            weaponComponent.m_WeaponIcon = m_UIManager.m_DualWieldPlate;
            weaponComponent.m_WeaponAmmoText = m_UIManager.m_DualWieldRightWeaponAmmoText;

            weaponComponent.m_TransformInfo.m_OriginalLocalPosition = newWeapon.transform.localPosition;
            weaponComponent.m_TransformInfo.m_OriginalGlobalPosition = newWeapon.transform.position;

            m_Weapons.Add(weaponComponent);

            // New weapon has to be deactivated be default.
            newWeapon.SetActive(false);

            if (m_Weapons.Count == 1) // If we just added the only weapon we have, set current weapon to this weapon and update UI.
                SetWeapon(0);

            m_UIManager.UpdateWeaponUI(m_CurrentWeapon);

            // Because we are hiding the skinned mesh renderers of all weapons on Start(), we have to unhide them when the player picks up a new weapon.
            weaponComponent.ToggleWeapon(true);

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
        m_CurrentWeapon.m_WeaponAmmoText?.gameObject.SetActive(false);
        m_CurrentWeapon = null; // Resetting the current weapon reference.

        if (m_Weapons.Count > 0) // If we haven't removed every single weapon from the list.
        { 
            m_CurrentWeapon = m_Weapons[m_CurrentWeaponNum]; // Setting the current weapon to the updated list.
            UnhideWeapon(m_CurrentWeaponNum); // If we have moved weapons, we should unhide the newly equipped weapon. Sometimes this will be redundant.
        }

        m_UIManager.UpdateWeaponUI(m_CurrentWeapon);
    }
    public bool UpgradeWeapon(int weapon, GameObject newPrefab, WEAPONTYPE requiredPrequisiteWep)
    {
        if (!HasWeapon(weapon))
            return false;

        // We can only allow them to upgrade if they hold the prequiste weapon. Let's check if they are holding it.
        bool hasPrerequisite = false;
        for (int i = 0; i < m_Weapons.Count; i++)
        {
            if (m_Weapons[i].m_TypeTag == requiredPrequisiteWep)
            { 
                hasPrerequisite = true;
                break;
            }

        }
        if (!hasPrerequisite)
            return false; // Early out, they don't have the required prerequisite.

        // Remove old weapon since we are upgrading it.
        RemoveWeapon(weapon);
        
        // Add a new weapon according to the parameters.
        AddWeapon(newPrefab);

        // Since the new weapon is on the end of the list, we'll swap to the last element to make it seem like they are still holding on to the same gun.
        SetWeapon(m_Weapons.Count - 1);

        m_UIManager.UpdateWeaponUI(m_CurrentWeapon);

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

    public void PlayDestroySound()
    {
        if (m_AudioDestroySound)
            m_AudioDestroySound.Trigger();
    }
}
