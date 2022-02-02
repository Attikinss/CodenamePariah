using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Welcome to the LiftEssenceHandler, yet another temporary UI script.
/// </summary>

public class LifeEssenceHandler : MonoBehaviour
{
    public PariahController m_Pariah;
	public TextMeshProUGUI m_Text;
	public Image m_HealthBar;

	private void Start()
	{
		if (!m_Pariah) // If not set in inspector.
		{
			// Let's try get it by name.
			m_Pariah = GameObject.Find("Pariah").GetComponent<PariahController>();
			if (!m_Pariah) // If we still haven't found it.
				Debug.LogError("LifeEssenceHandler.cs could not find Pariah's controller!");
		}
	}

	// Update is called once per frame
	void Update()
    {
		m_Text.text = m_Pariah.GetHealth().ToString();//
		m_HealthBar.fillAmount = (float)m_Pariah.GetHealth() / 100.0f;
    }
}
