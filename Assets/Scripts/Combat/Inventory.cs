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
    [Tooltip("Health UI.")]//
    public TextMeshProUGUI m_HealthText;

    [SerializeField]
    [Tooltip("Weapons this character has.")]
    public List<Weapon> m_Weapons;

    /// <summary>
    /// 
    /// </summary>
    
    [HideInInspector]
    public Weapon m_CurrentWeapon; // I know this wasn't public by default, but I'm going to make it publically accessible so that I can swap weapons around form the HostController.cs script.


    /// <summary>
    /// I've added a Awake() function here because m_CurrentWeapon was always unintialised. I'm going to initialise it here.
    /// </summary>
	private void Awake()
	{
        m_CurrentWeapon = m_Weapons[0]; // For now, m_CurrentWeapon will always start off as the first element in the m_Weapons list.
	}

	// Update is called once per frame
	void Update()
    {
        DisplayHealth();

        if (m_Health > 100)
        {
            m_Health = 100;
        }
        if (m_Health < 0)
        {
            m_Health = 0;
        }
    }

    /// <summary>Displays the current health.</summary>
    void DisplayHealth()
    {
        //m_HealthText.text = "";
        m_HealthText.text = "";
        //enable sprite (possibly only at start - until health is 0)
        m_HealthText.text += m_Health;
    }

    /// <summary>Takes health away equal to the damage value.</summary>
    /// <param name="damage"></param>
    void TakeDamage(int damage)
    {
        m_Health -= damage;
    }
}
