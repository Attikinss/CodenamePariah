using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool m_GameIsPaused = false;

    public GameObject[] m_PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) // inputsystem (gamepad system)
        {
            if (m_GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        for (int i = 0; i < m_PauseMenuUI.Length; i++)
        {
            m_PauseMenuUI[i].SetActive(false);
        }
        Time.timeScale = 1f;
        m_GameIsPaused = false;
    }

    void Pause()
    {
        for (int i = 0; i < m_PauseMenuUI.Length; i++)
        {
            m_PauseMenuUI[i].SetActive(true);
        }
        Time.timeScale = 0f;
        m_GameIsPaused = true;
        //disable a bunch of things requiring input
    }
}
