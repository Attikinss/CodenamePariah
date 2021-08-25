using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public TMP_InputField m_InputField;
    public Slider m_Slider;
    public static float m_MouseSensitivityMultiplier;   //multiply looksensitivity by these depending on gamepad or kbm.
    public static int m_ControllerSensitivityMultiplier;//multiply looksensitivity by these depending on gamepad or kbm. convert each value of 1-8 to a float??

    public void SetMasterVolume(float value)
    {
        //AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetDialogueVolume(float value)
    {
        //AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Dialogue);
    }

    public void SetSFXVolume(float value)
    {
        //AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.SFX);
    }

    public void SetMusicVolume(float value)
    {
        //AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SliderValueChanged()
    {
        //if (m_Slider) m_Slider.value = value;
        if (m_InputField) 
            m_InputField.text = m_Slider.value.ToString();
    }

    public void MouseSensitivitySliderChanged()
    {
        //if (m_Slider) m_Slider.value = value;
        if (m_InputField)
            m_InputField.text = m_Slider.value.ToString("F2");
    }

    public void VolumeInputValueChanged()
    {
        if (m_InputField.text.Length > 0 && m_InputField.text[0] == '-')//
        {
            m_InputField.text = "0";
            return;
        }
        if (m_InputField.text.Length == 0)//
        {
            m_InputField.text = "0";
        }

        int value = int.Parse(m_InputField.text);
        value = Mathf.Clamp(int.Parse(m_InputField.text), 0, 100);//
        m_InputField.text = value.ToString();//
        if (m_Slider) 
            m_Slider.value = value;
        //if (m_InputField) m_InputField.text = value;
    }

    public void ControllerSensitivityInputValueChanged()
    {
        if (m_InputField.text.Length > 0 && m_InputField.text[0] == '-')//
        {
            m_InputField.text = "1";
            return;
        }
        if (m_InputField.text.Length == 0)//
        {
            m_InputField.text = "1";
        }

        int value = int.Parse(m_InputField.text);
        value = Mathf.Clamp(int.Parse(m_InputField.text), 1, 8);//
        m_InputField.text = value.ToString();//
        if (m_Slider)
            m_Slider.value = value;
        m_ControllerSensitivityMultiplier = value;
        //if (m_InputField) m_InputField.text = value;
    }

    public void MouseSensitivityInputValueChanged()
    {
        if (m_InputField.text.Length > 0 && m_InputField.text[0] == '-')//
        {
            m_InputField.text = "0.01";
            return;
        }
        if (m_InputField.text.Length == 0)//
        {
            m_InputField.text = "0.01";
        }

        float value = float.Parse(m_InputField.text);
        value = Mathf.Clamp(float.Parse(m_InputField.text), 0.01f, 100.00f);//
        m_InputField.text = value.ToString("F2");//
        if (m_Slider)
            m_Slider.value = value;
        m_MouseSensitivityMultiplier = value;
        //if (m_InputField) m_InputField.text = value;
    }
}
