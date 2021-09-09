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

    /// <summary>
    /// 
    /// </summary>
    
    [HideInInspector]
    public Weapon m_CurrentWeapon; // I know this wasn't public by default, but I'm going to make it publically accessible so that I can swap weapons around form the HostController.cs script.

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
    public void TakeDamage(int damage)
    {
        m_Health -= damage;
        if (m_Health <= 0)
        {
            if (TryGetComponent(out WhiteWillow.Agent agent))
                agent.Kill();
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
}
