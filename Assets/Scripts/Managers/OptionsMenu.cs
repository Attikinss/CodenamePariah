using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public TMP_InputField m_InputField;
    public Slider m_Slider;
    [SerializeField]
    private PlayerPreferences m_PlayerPrefs;

    [SerializeField]
    [ReadOnly]
    private static float m_MouseSensitivity;   //multiply looksensitivity by this

    [SerializeField]
    [ReadOnly]
    private static float m_ControllerSensitivity;//or multiply looksensitivity by this. convert each value of 1-8 to a float??

    //private void Awake()
    //{
    //    m_MouseSensitivity = InputController.m_LookSensitivity;
    //}

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
        //controller sensitivity - may need an override as this is also used for audio volumes.
    }

    public void ControllerSensitivitySliderValueChanged()
    {
        if (m_InputField)
            m_InputField.text = m_Slider.value.ToString("F2");
        m_ControllerSensitivity = m_Slider.value;
        m_PlayerPrefs.GameplayConfig.ControllerSensitivity = m_ControllerSensitivity;
    }

    public void MouseSensitivitySliderChanged()
    {
        //if (m_Slider) m_Slider.value = value;
        if (m_InputField)
            m_InputField.text = m_Slider.value.ToString("F2");
        m_MouseSensitivity = m_Slider.value;
        m_PlayerPrefs.GameplayConfig.MouseSensitivity = m_MouseSensitivity;
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

        float value = float.Parse(m_InputField.text);
        value = Mathf.Clamp(float.Parse(m_InputField.text), 5.00f, 100.00f);//
        m_InputField.text = value.ToString();//
        if (m_Slider)
            m_Slider.value = value;
        m_ControllerSensitivity = value;
        m_PlayerPrefs.GameplayConfig.ControllerSensitivity = m_ControllerSensitivity;
        //if (m_InputField) m_InputField.text = value;
    }

    public void MouseSensitivityInputValueChanged()
    {
        if (m_InputField.text.Length > 0 && m_InputField.text[0] == '-')//
        {
            m_InputField.text = "5.00";
            return;
        }
        if (m_InputField.text.Length == 0)//
        {
            m_InputField.text = "5.00";
        }

        float value = float.Parse(m_InputField.text);
        value = Mathf.Clamp(float.Parse(m_InputField.text), 5.00f, 100.00f);//
        m_InputField.text = value.ToString("F2");//
        if (m_Slider)
            m_Slider.value = value;
        m_MouseSensitivity = value;
        m_PlayerPrefs.GameplayConfig.MouseSensitivity = m_MouseSensitivity;
        //if (m_InputField) m_InputField.text = value;
    }
}
