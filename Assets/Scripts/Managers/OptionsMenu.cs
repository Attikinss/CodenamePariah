using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Input field of this option.")]
    public TMP_InputField m_MasterVolumeInputField;

    [SerializeField]
    [Tooltip("Slider of this option.")]
    public Slider m_MasterVolumeSlider;

    [SerializeField]
    [Tooltip("Input field of this option.")]
    public TMP_InputField m_DialogueVolumeInputField;

    [SerializeField]
    [Tooltip("Slider of this option.")]
    public Slider m_DialogueVolumeSlider;

    [SerializeField]
    [Tooltip("Input field of this option.")]
    public TMP_InputField m_SFXVolumeInputField;

    [SerializeField]
    [Tooltip("Slider of this option.")]
    public Slider m_SFXVolumeSlider;

    [SerializeField]
    [Tooltip("Input field of this option.")]
    public TMP_InputField m_MusicVolumeInputField;

    [SerializeField]
    [Tooltip("Slider of this option.")]
    public Slider m_MusicVolumeSlider;

    [SerializeField]
    [Tooltip("Mouse sensitivity slider.")]
    public Slider m_MouseSensitivitySlider;

    [SerializeField]
    [Tooltip("Mouse sensitivity input field.")]
    public TMP_InputField m_MouseInputField;

    [SerializeField]
    [Tooltip("Controller sensitivity slider.")]
    public Slider m_ControllerSensitivitySlider;

    [SerializeField]
    [Tooltip("Controller sensitivity input field.")]
    public TMP_InputField m_ControllerInputField;

    [SerializeField]
    [Tooltip("Field of View Slider.")]
    public Slider m_FOVSlider;

    [SerializeField]
    [Tooltip("Field of View input field.")]
    public TMP_InputField m_FOVInputField;

    [SerializeField]
    private PlayerPreferences m_PlayerPrefs;

    [SerializeField]
    [ReadOnly]
    private static int m_MouseSensitivity;

    [SerializeField]
    [ReadOnly]
    private static int m_ControllerSensitivity;

    [SerializeField]
    [ReadOnly]
    private static int m_FieldOfView;

    [SerializeField]
    [ReadOnly]
    private static int m_MasterVolume;

    [SerializeField]
    [ReadOnly]
    private static int m_DialogueVolume;

    [SerializeField]
    [ReadOnly]
    private static int m_SFXVolume;

    [SerializeField]
    [ReadOnly]
    private static int m_MusicVolume;

    Resolution[] resolutions;

    public TMP_Dropdown resolutionDropdown;

    FMOD.Studio.Bus m_MasterBus;
    FMOD.Studio.Bus m_MusicBus;
    FMOD.Studio.Bus m_SFXBus;
    FMOD.Studio.Bus m_DialogueBus;

    private void Awake()
    {

        m_MasterBus = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        m_MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        m_SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
        m_DialogueBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/Dialogue");

        //THIS IS PROBABLY VERY BAD
        m_MouseSensitivity = m_PlayerPrefs.GameplayConfig.MouseSensitivity;
        m_MouseSensitivitySlider.value = m_MouseSensitivity;
        m_MouseInputField.text = m_MouseSensitivity.ToString();

        m_ControllerSensitivity = m_PlayerPrefs.GameplayConfig.ControllerSensitivity;
        m_ControllerSensitivitySlider.value = m_ControllerSensitivity;
        m_ControllerInputField.text = m_ControllerSensitivity.ToString();

        m_FieldOfView = m_PlayerPrefs.VideoConfig.FieldOfView;
        m_FOVSlider.value = m_FieldOfView;
        m_FOVInputField.text = m_FieldOfView.ToString();

        m_MasterVolume = m_PlayerPrefs.AudioConfig.MasterVolume;
        m_DialogueVolume = m_PlayerPrefs.AudioConfig.DialogueVolume;
        m_SFXVolume = m_PlayerPrefs.AudioConfig.SFXVolume;
        m_MusicVolume = m_PlayerPrefs.AudioConfig.MusicVolume;

        m_MasterVolumeSlider.value = m_MasterVolume;
        m_DialogueVolumeSlider.value = m_DialogueVolume;
        m_SFXVolumeSlider.value = m_SFXVolume;
        m_MusicVolumeSlider.value = m_MusicVolume;

        m_MasterVolumeInputField.text = m_MasterVolume.ToString();
        m_DialogueVolumeInputField.text = m_DialogueVolume.ToString();
        m_SFXVolumeInputField.text = m_SFXVolume.ToString();
        m_MusicVolumeInputField.text = m_MusicVolume.ToString();

        //CHANGE THIS
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " X " + resolutions[i].height;//TEST THIS ON OTHER MONITORS
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height) 
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        //player prefs for resolution and fullscreen.
    }

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

    /// <summary>Gets called when an audio slider value has changed. NEED TO MAKE SURE TO IGNORE ON AWAKE/START.</summary>
    public void MasterVolumeSliderValueChanged()
    {
        if (m_MasterVolumeInputField) 
            m_MasterVolumeInputField.text = m_MasterVolumeSlider.value.ToString();
        m_MasterVolume = (int)m_MasterVolumeSlider.value;
        m_PlayerPrefs.AudioConfig.MasterVolume = m_MasterVolume;

        m_MasterBus.setVolume((float)m_MasterVolume / 100);
    }
    public void DialogueVolumeSliderValueChanged()
    {
        if (m_DialogueVolumeInputField)
            m_DialogueVolumeInputField.text = m_DialogueVolumeSlider.value.ToString();
        m_DialogueVolume = (int)m_DialogueVolumeSlider.value;
        m_PlayerPrefs.AudioConfig.DialogueVolume = m_DialogueVolume;

        m_DialogueBus.setVolume((float)m_DialogueVolume / 100);
    }
    public void SFXVolumeSliderValueChanged()
    {
        if (m_SFXVolumeInputField)
            m_SFXVolumeInputField.text = m_SFXVolumeSlider.value.ToString();
        m_SFXVolume = (int)m_SFXVolumeSlider.value;
        m_PlayerPrefs.AudioConfig.SFXVolume = m_SFXVolume;

        m_SFXBus.setVolume((float)m_SFXVolume / 100);
    }
    public void MusicVolumeSliderValueChanged()
    {
        if (m_MusicVolumeInputField)
            m_MusicVolumeInputField.text = m_MusicVolumeSlider.value.ToString();
        m_MusicVolume = (int)m_MusicVolumeSlider.value;
        m_PlayerPrefs.AudioConfig.MusicVolume = m_MusicVolume;

        m_MusicBus.setVolume((float)m_MusicVolume / 100);
    }

    public void FOVSliderValueChanged()
    {
        if (m_FOVInputField)
            m_FOVInputField.text = m_FOVSlider.value.ToString();
        m_FieldOfView = (int)m_FOVSlider.value;
        m_PlayerPrefs.VideoConfig.FieldOfView = m_FieldOfView;

        // Updates Pariah's FOV without relying on putting it in PariahController Update().
        if (GameManager.s_Instance && GameManager.s_Instance.m_Pariah)
            GameManager.s_Instance.m_Pariah.Camera.fieldOfView = (Mathf.Atan(Mathf.Tan((float)(m_FieldOfView * Mathf.Deg2Rad) * 0.5f) / GameManager.s_Instance.m_Pariah.Camera.aspect) * 2) * Mathf.Rad2Deg;
    }

    /// <summary>Gets called when the controller sensitivity has been changed.</summary>
    public void ControllerSensitivitySliderValueChanged()
    {
        if (m_ControllerInputField)
            m_ControllerInputField.text = m_ControllerSensitivitySlider.value.ToString();
        m_ControllerSensitivity = (int)m_ControllerSensitivitySlider.value;
        m_PlayerPrefs.GameplayConfig.ControllerSensitivity = m_ControllerSensitivity;
    }

    /// <summary>Gets called when the mouse sensitivity has been changed.</summary>
    public void MouseSensitivitySliderChanged()
    {
        if (m_MouseInputField)
            m_MouseInputField.text = m_MouseSensitivitySlider.value.ToString();
        m_MouseSensitivity = (int)m_MouseSensitivitySlider.value;
        m_PlayerPrefs.GameplayConfig.MouseSensitivity = m_MouseSensitivity;
    }

    /// <summary>Gets called when a volume input field value has been changed.</summary>
    public void MasterVolumeInputValueChanged()
    {
        if (m_MasterVolumeInputField.text.Length > 0 && m_MasterVolumeInputField.text[0] == '-')
        {
            m_MasterVolumeInputField.text = "0";
            //return;
        }
        if (m_MasterVolumeInputField.text.Length == 0)
        {
            m_MasterVolumeInputField.text = "0";
        }

        int value = int.Parse(m_MasterVolumeInputField.text);
        value = Mathf.Clamp(int.Parse(m_MasterVolumeInputField.text), 0, 100);
        m_MasterVolumeInputField.text = value.ToString();
        if (m_MasterVolumeSlider) 
            m_MasterVolumeSlider.value = value;
        m_MasterVolume = value;
        m_PlayerPrefs.AudioConfig.MasterVolume = m_MasterVolume;

        m_MasterBus.setVolume((float)m_MasterVolume / 100);
    }

    /// <summary>Gets called when a volume input field value has been changed.</summary>
    public void DialogueVolumeInputValueChanged()
    {
        if (m_DialogueVolumeInputField.text.Length > 0 && m_DialogueVolumeInputField.text[0] == '-')
        {
            m_DialogueVolumeInputField.text = "0";
            //return;
        }
        if (m_DialogueVolumeInputField.text.Length == 0)
        {
            m_DialogueVolumeInputField.text = "0";
        }

        int value = int.Parse(m_DialogueVolumeInputField.text);
        value = Mathf.Clamp(int.Parse(m_DialogueVolumeInputField.text), 0, 100);
        m_DialogueVolumeInputField.text = value.ToString();
        if (m_DialogueVolumeSlider)
            m_DialogueVolumeSlider.value = value;
        m_DialogueVolume = value;
        m_PlayerPrefs.AudioConfig.DialogueVolume = m_DialogueVolume;

        m_DialogueBus.setVolume((float)m_DialogueVolume / 100);
    }

    /// <summary>Gets called when a volume input field value has been changed.</summary>
    public void SFXVolumeInputValueChanged()
    {
        if (m_SFXVolumeInputField.text.Length > 0 && m_SFXVolumeInputField.text[0] == '-')
        {
            m_SFXVolumeInputField.text = "0";
            //return;
        }
        if (m_SFXVolumeInputField.text.Length == 0)
        {
            m_SFXVolumeInputField.text = "0";
        }

        int value = int.Parse(m_SFXVolumeInputField.text);
        value = Mathf.Clamp(int.Parse(m_SFXVolumeInputField.text), 0, 100);
        m_SFXVolumeInputField.text = value.ToString();
        if (m_SFXVolumeSlider)
            m_SFXVolumeSlider.value = value;
        m_SFXVolume = value;
        m_PlayerPrefs.AudioConfig.SFXVolume = m_SFXVolume;

        // Changing volume.
        m_SFXBus.setVolume((float)m_SFXVolume / 100);
        

    }

    /// <summary>Gets called when a volume input field value has been changed.</summary>
    public void MusicVolumeInputValueChanged()
    {
        if (m_MusicVolumeInputField.text.Length > 0 && m_MusicVolumeInputField.text[0] == '-')
        {
            m_MusicVolumeInputField.text = "0";
            //return;
        }
        if (m_MusicVolumeInputField.text.Length == 0)
        {
            m_MusicVolumeInputField.text = "0";
        }

        int value = int.Parse(m_MusicVolumeInputField.text);
        value = Mathf.Clamp(int.Parse(m_MusicVolumeInputField.text), 0, 100);
        m_MusicVolumeInputField.text = value.ToString();
        if (m_MusicVolumeSlider)
            m_MusicVolumeSlider.value = value;
        m_MusicVolume = value;
        m_PlayerPrefs.AudioConfig.MusicVolume = m_MusicVolume;

        m_MusicBus.setVolume((float)m_MusicVolume / 100);
    }

    public void FOVInputValueChanged()
    {
        if (m_FOVInputField.text.Length > 0 && m_FOVInputField.text[0] == '-')
        {
            m_FOVInputField.text = "70";
            //return;
        }
        if (m_FOVInputField.text.Length == 0)
        {
            m_FOVInputField.text = "70";
        }

        int value = int.Parse(m_FOVInputField.text);
        value = Mathf.Clamp(int.Parse(m_FOVInputField.text), 70, 120);
        m_FOVInputField.text = value.ToString();
        if (m_FOVSlider)
            m_FOVSlider.value = value;
        m_FieldOfView = value;
        m_PlayerPrefs.VideoConfig.FieldOfView = m_FieldOfView;

        // Updates Pariah's FOV without relying on putting it in PariahController Update().
        if (GameManager.s_Instance && GameManager.s_Instance.m_Pariah)
            GameManager.s_Instance.m_Pariah.Camera.fieldOfView = (Mathf.Atan(Mathf.Tan((float)(m_FieldOfView * Mathf.Deg2Rad) * 0.5f) / GameManager.s_Instance.m_Pariah.Camera.aspect) * 2) * Mathf.Rad2Deg;
    }

    /// <summary>Gets called when the controller sensitivity input field value has been changed.</summary>
    public void ControllerSensitivityInputValueChanged()
    {
        if (m_ControllerInputField.text.Length > 0 && m_ControllerInputField.text[0] == '-')
        {
            m_ControllerInputField.text = "5";
            //return;
        }
        if (m_ControllerInputField.text.Length == 0)
        {
            m_ControllerInputField.text = "5";
        }

        int value = int.Parse(m_ControllerInputField.text);
        value = Mathf.Clamp(int.Parse(m_ControllerInputField.text), 5, 100);
        m_ControllerInputField.text = value.ToString();
        if (m_ControllerSensitivitySlider)
            m_ControllerSensitivitySlider.value = value;
        m_ControllerSensitivity = value;
        m_PlayerPrefs.GameplayConfig.ControllerSensitivity = m_ControllerSensitivity;
    }

    /// <summary>Gets called when the mouse sensitivity input field value has been changed.</summary>
    public void MouseSensitivityInputValueChanged()
    {
        if (m_MouseInputField.text.Length > 0 && m_MouseInputField.text[0] == '-')
        {
            m_MouseInputField.text = "5";
            //return;
        }
        if (m_MouseInputField.text.Length == 0)
        {
            m_MouseInputField.text = "5";
        }

        int value = int.Parse(m_MouseInputField.text);
        value = Mathf.Clamp(int.Parse(m_MouseInputField.text), 5, 100);
        m_MouseInputField.text = value.ToString();
        if (m_MouseSensitivitySlider)
            m_MouseSensitivitySlider.value = value;
        m_MouseSensitivity = value;
        m_PlayerPrefs.GameplayConfig.MouseSensitivity = m_MouseSensitivity;
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        m_PlayerPrefs.VideoConfig.Fullscreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
        m_PlayerPrefs.VideoConfig.Resolution.Width = resolution.width;
        m_PlayerPrefs.VideoConfig.Resolution.Height = resolution.height;
        m_PlayerPrefs.VideoConfig.Resolution.RefreshRate = resolution.refreshRate;
    }
}
