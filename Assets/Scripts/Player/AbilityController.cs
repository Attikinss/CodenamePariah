using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AbilityController : MonoBehaviour
{
    public Ability m_Ability1;
    public Ability m_Ability2;
    public Ability m_Ability3;


    public Rigidbody m_Rigidbody;


    // ============== UI ELEMENTS ============== //
    public Image m_AbilityIcon1;
    public Image m_AbilityIcon2;
    public Image m_AbilityIcon3;
    // ========================================= //


    // Start is called before the first frame update
    public void Start()
    {
        if (m_Rigidbody == null)
            m_Rigidbody = GetComponent<Rigidbody>();

        Debug.Assert(m_Rigidbody); // Make sure it's set to something now.

        m_Ability1.SetIcon(m_AbilityIcon1);
        m_Ability2.SetIcon(m_AbilityIcon2);
        m_Ability3.SetIcon(m_AbilityIcon3);

        // Initializing InputManager.
        InputManager.OnAbility1 += m_Ability1.Invoke;
        InputManager.OnAbility2 += m_Ability2.Invoke;
        InputManager.OnAbility3 += m_Ability3.Invoke;

    }

    // Update is called once per frame
    public void Update()
    {
        m_Ability1.Update();
        m_Ability2.Update();
        m_Ability3.Update();
    }
}
