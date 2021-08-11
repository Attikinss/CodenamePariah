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
    private List<Weapon> m_Weapons;

    /// <summary>
    /// 
    /// </summary>
    private Weapon m_CurrentWeapon;

    // Update is called once per frame
    void Update()
    {
        DisplayHealth();

        if (m_Health > 100)
        {
            m_Health = 100;
            m_HealthSprite.SetActive(true);
        }
        if (m_Health < 0)
        {
            m_Health = 0;
            m_HealthSprite.SetActive(false);
        }
    }

    /// <summary>Displays the current health.</summary>
    void DisplayHealth() //move to ui Manager
    {
        //m_HealthText.text = "";
        m_HealthText.text = "";
        //m_HealthSprite.SetActive(true);
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
