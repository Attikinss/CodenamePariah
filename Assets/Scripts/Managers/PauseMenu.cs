using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //should probably all be inside gamemanager or something.
    [ReadOnly]
    public static bool m_GameIsPaused = false;

    [SerializeField]
    private bool m_IsPauseMenu = false;

    private bool m_OptionsOpen = false;
    private static bool m_IsQuitting = false;
    private static bool m_SettingsToBeApplied = false;
    private static bool m_InsideDialogueBox = false;

    public GameObject[] m_PauseMenuUI;
    public GameObject[] m_OptionsMenuUI;
    public GameObject[] m_QuitMenuUI;
    public GameObject m_DialogueBoxUI;

    public Animator m_Transition;

    [SerializeField]
    private float m_TransitionTime = 2f;

    // Update is called once per frame
    void Update()
    {
        //just keyboard at this stage. needs to be readjusted for inputsystem
        if (Keyboard.current.escapeKey.wasPressedThisFrame) 
        {
            if (m_InsideDialogueBox)
            {
                CloseDialogueBox();
            }
            else if (m_OptionsOpen)
            {
                if (m_SettingsToBeApplied)
                {
                    OpenDialogueBox();
                    SettingsChanged();
                }
                else
                {
                    OptionsClose();
                    Pause();
                }
            }
            else if (m_IsQuitting)
            {
                QuittingClose();
                Pause();
            }
            else if (!m_IsPauseMenu)
            {
                QuitMenu();
            }
            else if (m_GameIsPaused)
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
        //Time.timeScale = 1f;// time scale affects scene animations.
        m_GameIsPaused = false;
    }

    public void Play()
    {
        //for (int i = 0; i < m_PauseMenuUI.Length; i++)
        //{
        //    m_PauseMenuUI[i].SetActive(false);
        //}
        //Time.timeScale = 1f;//
        m_GameIsPaused = false;//
        //StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel(/*int levelIndex*/)
    {
        //SceneManager.LoadSceneAsync("Arena_001");

        m_Transition.SetTrigger("Start");
        
        yield return new WaitForSeconds(m_TransitionTime);

        //SceneManager.LoadScene(levelIndex);
        SceneManager.LoadScene("Arena_001");
        //while (!asyncLoad.isDone)
        //{
        //    yield return null;
        //}
    }

    IEnumerator LoadMainMenu()
    {
        m_Transition.SetTrigger("Start");

        yield return new WaitForSeconds(m_TransitionTime);

        SceneManager.LoadScene("MainMenu");
    }

    public void Pause()
    {
        for (int i = 0; i < m_PauseMenuUI.Length; i++)
        {
            m_PauseMenuUI[i].SetActive(true);
        }
        //Time.timeScale = 0f;// time scale affects scene animations.
        m_GameIsPaused = true;
        //disable a bunch of things requiring input - currently a bullet gets fired on first click inside pause menu.
        //can probably do "if(... && !m_GameIsPaused) in other scripts.
    }

    public void QuitMenu()
    {
        for (int i = 1; i < m_PauseMenuUI.Length - 2; i++)
        {
            m_PauseMenuUI[i].SetActive(false);
        }

        for (int i = 0; i < m_QuitMenuUI.Length; i++)
        {
            m_QuitMenuUI[i].SetActive(true);
        }
        QuittingOpen();
    }

    public void OptionsOpen()
    {
        m_OptionsOpen = true;
    }

    public void OptionsClose()
    {
        for (int i = 0; i < m_OptionsMenuUI.Length; i++)
        {
            m_OptionsMenuUI[i].SetActive(false);
        }
        m_OptionsOpen = false;
        m_SettingsToBeApplied = false;
    }
    
    public void QuittingOpen()
    {
        m_IsQuitting = true;
    }

    public void QuittingClose()
    {
        if (!m_IsPauseMenu)
        for (int i = 1; i < m_PauseMenuUI.Length; i++)
        {
            m_PauseMenuUI[i].SetActive(true);
        }

        for (int i = 0; i < m_QuitMenuUI.Length; i++)
        {
            m_QuitMenuUI[i].SetActive(false);
        }
        m_IsQuitting = false;
    }

    public void SettingsChanged()
    {
        m_SettingsToBeApplied = true;
    }

    public void SettingsApplied()
    {
        //do all these changes.
        m_DialogueBoxUI.SetActive(false);
        m_SettingsToBeApplied = false;
    }

    public void OpenDialogueBox()
    {
        m_DialogueBoxUI.SetActive(true);
        m_InsideDialogueBox = true;
        Debug.Log("Discard Changes?");
    }

    public void CloseDialogueBox()
    {
        m_DialogueBoxUI.SetActive(false);
        m_InsideDialogueBox = false;
        Debug.Log("Settings still to be applied so cancelled.");
    }

    public void DiscardChanges()
    {
        m_InsideDialogueBox = false;
        Debug.Log("Discarded changes.");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }

    public void ExitToMenu()
    {
        StartCoroutine(LoadMainMenu());
    }
}
