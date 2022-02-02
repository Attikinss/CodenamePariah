using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuButton : MonoBehaviour
{
    [SerializeField]
    private Color m_DefaultColour;

    [SerializeField]
    private Color m_PressedColour;

    public GameObject[] m_OtherButtons;

    public void HighlightButton()
    {
        for (int i = 0; i < m_OtherButtons.Length; i++)
        {
            m_OtherButtons[i].GetComponent<Image>().color = m_DefaultColour;
        }
        this.gameObject.GetComponent<Image>().color = m_PressedColour;
    }
}
