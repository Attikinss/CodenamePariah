using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //should probably all be inside gamemanager or something.
    [ReadOnly]
    [Tooltip("Current state of the game.")]
    public static bool m_GameIsPaused = false;

    public static bool m_GameOver = false;

    [SerializeField]
    [Tooltip("Whether this gameobject is the pausemenu.")]
    private bool m_IsPauseMenu = false;

    [Tooltip("Whether the options menu is open.")]
    private bool m_OptionsOpen = false;

    [Tooltip("Whether the player is in the process of quitting the game (quit menu).")]
    private static bool m_IsQuitting = false;

    //[Tooltip("Whether there are settings to be applied.")]
    //private static bool m_SettingsToBeApplied = false;

    //[Tooltip("Whether the player has a dialogue box active.")]
    //private static bool m_InsideDialogueBox = false;

    [Tooltip("All the gameobjects that need to be turned on or off when pausing or resuming the game.")]
    public GameObject[] m_PauseMenuUI;

    [Tooltip("All the gameobjects that need to be turned on or off when opening or closing the options menu.")]
    public GameObject[] m_OptionsMenuUI;

    public GameObject[] m_GameOverMenuUI;

    [Tooltip("All the gameobjects that need to be turned on or off when in the process of quitting the game.")]
    public GameObject[] m_QuitMenuUI;

    public GameObject[] m_GameOverQuitMenuUI;

    [Tooltip("Gameobject that needs to be turned on or off when a dialogue box should appear.")]
    public GameObject m_DialogueBoxUI;

    [Tooltip("Animator for menu transitions.")]
    public Animator m_Transition;

    [SerializeField]
    private Image m_Panel;

    [SerializeField]
    [Tooltip("Time it takes for transition between scenes.")]
    private float m_TransitionTime = 2f;

    AsyncOperation asyncOperation;

    private void Start()
    {
        Debug.Log("Loaded");
        if (!m_IsPauseMenu)
            StartCoroutine(StartLoadingLevel());
    }

    private void Awake()
    {
       if (m_IsPauseMenu)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
       else
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        //just keyboard at this stage. needs to be readjusted for inputsystem
        if (Keyboard.current.escapeKey.wasPressedThisFrame) 
        {
            //if (m_InsideDialogueBox)
            //{
            //    CloseDialogueBox();
            //}
            /*else*/ if (m_OptionsOpen)
            {
                //if (m_SettingsToBeApplied)
                //{
                //    OpenDialogueBox();
                //    SettingsChanged();
                //}
                //else
                //{
                    OptionsClose();
                    Pause();
                //}
            }
            else if (m_IsQuitting)
            {
                if (!m_GameOver)
                {
                    QuittingClose();
                    Pause();
                }
                else
                {
                    GameOverQuittingClose();
                    GameOverMenu();
                }
            }
            else if (!m_IsPauseMenu)
            {
                QuitMenu();
            }
            else if (m_GameOver)
            {

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

        if (m_GameOver && !m_GameIsPaused)
            GameOverMenu();
    }

    /// <summary>Resumes gameplay.</summary>
    public void Resume()
    {
        for (int i = 0; i < m_PauseMenuUI.Length; i++)
        {
            m_PauseMenuUI[i].SetActive(false);
        }
        //Time.timeScale = 1f;// time scale affects scene animations.
        m_GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>Starts the game.</summary>
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


        // When clicking the play button, we want to start a new game afresh. So this means wiping the checkpoint history.
        GameManager.ResetCheckpoint();
    }

    //dont know if this one should be an ienumerator in a start function and could possibly just be a normal function.
    IEnumerator StartLoadingLevel()
    {
        asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            Debug.Log($"Progress: {asyncOperation.progress * 100}%");

            yield return null;
        }
    }

    /// <summary>Loads the gameplay scene.</summary>
    /// <returns></returns>
    IEnumerator LoadLevel(/*int levelIndex*/)
    {
        //SceneManager.LoadSceneAsync("Arena_001");

        m_Transition.SetTrigger("Start");
        
        yield return new WaitForSeconds(m_TransitionTime);

        //SceneManager.LoadScene(levelIndex);
        //#if UNITY_EDITOR
        //        SceneManager.LoadScene("Test_Lauchlan_002");
        //#else
        if (asyncOperation.progress >= 0.9f)
        {
            Debug.Log("Loading");
            asyncOperation.allowSceneActivation = true;
        }
        //SceneManager.LoadScene("Level_001", LoadSceneMode.Single);
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        //#endif

        //while (!asyncLoad.isDone)
        //{
        //    yield return null;
        //}
    }

    /// <summary>Loads the main menu scene.</summary>
    /// <returns></returns>
    IEnumerator LoadMainMenu()
    {
        m_Transition.SetTrigger("Start");

        yield return new WaitForSeconds(m_TransitionTime);

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

    /// <summary>Pauses the game.</summary>
    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        for (int i = 0; i < m_PauseMenuUI.Length; i++)
        {
            m_PauseMenuUI[i].SetActive(true);
        }
        //Time.timeScale = 0f;// time scale affects scene animations.
        m_GameIsPaused = true;
        //disable a bunch of things requiring input - currently a bullet gets fired on first click inside pause menu.
        //can probably do "if(... && !m_GameIsPaused) in other scripts.
    }

    public void GameOverMenu()
    {
        //m_GameOver = false;
        m_GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        for (int i = 0; i < m_GameOverMenuUI.Length; i++)
        {
            m_GameOverMenuUI[i].SetActive(true);
        }
    }

    /// <summary>Opens the quit menu.</summary>
    public void QuitMenu()
    {
        for (int i = 1; i < m_PauseMenuUI.Length - 1; i++)
        {
            m_PauseMenuUI[i].SetActive(false);
        }

        for (int i = 0; i < m_QuitMenuUI.Length; i++)
        {
            m_QuitMenuUI[i].SetActive(true);
        }
        QuittingOpen();
    }

    /// <summary>Upon opening the options menu, sets optionsopen to true.</summary>
    public void OptionsOpen()
    {
        m_OptionsOpen = true;
    }

    /// <summary>Upon closing the options menu, sets optionsopen to false.</summary>
    public void OptionsClose()
    {
        for (int i = 0; i < m_OptionsMenuUI.Length; i++)
        {
            m_OptionsMenuUI[i].SetActive(false);
        }
        if (m_Panel)
        {
            m_Panel.enabled = false;
        }

        m_OptionsOpen = false;
        //m_SettingsToBeApplied = false;
    }
    
    /// <summary>Upon opening the quit menu, sets isquitting to true.</summary>
    public void QuittingOpen()
    {
        m_IsQuitting = true;
    }

    /// <summary>Upon closing the quit menu, sets isquitting to false.</summary>
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

    public void GameOverQuittingClose()
    {
        if (!m_IsPauseMenu)
            for (int i = 1; i < m_GameOverMenuUI.Length; i++)
            {
                m_GameOverMenuUI[i].SetActive(true);
            }

        for (int i = 0; i < m_QuitMenuUI.Length; i++)
        {
            m_GameOverQuitMenuUI[i].SetActive(false);
        }
        m_IsQuitting = false;
    }

    public void ReloadSceneCheckpoint()
    {
        m_GameOver = false;
        m_GameIsPaused = false;
        StartCoroutine(ReloadLevel());
    }

    public void ReloadScene()
    {
        GameManager.ResetCheckpoint();
        m_GameOver = false;
        m_GameIsPaused = false;
        StartCoroutine(ReloadLevel());
    }

    // ****** Highly temporary ******
    private IEnumerator ReloadLevel()
    {
        GameManager.s_IsNotFirstLoad = true; // Telling the game manager that it's not the games first load.
        yield return null;

        //Need to test for whether async or non async is better.
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            Debug.Log($"Progress: {asyncOperation.progress * 100}%");
            if (asyncOperation.progress >= 0.9f)
                asyncOperation.allowSceneActivation = true;

            yield return null;
        }


    }

    ///// <summary>Upon settings being changed, sets settings to be applied to true. NEED TO FIX.</summary>
    //public void SettingsChanged()
    //{
    //    m_SettingsToBeApplied = true;
    //}

    ///// <summary>Changes settings and disable dialogue box, then sets settings to be applied to false.</summary>
    //public void SettingsApplied()
    //{
    //    //do all these changes.
    //    m_DialogueBoxUI.SetActive(false);
    //    m_SettingsToBeApplied = false;
    //}

    ///// <summary>Opens a dialogue box and sets insidedialoguebox to true.</summary>
    //public void OpenDialogueBox()
    //{
    //    m_DialogueBoxUI.SetActive(true);
    //    m_InsideDialogueBox = true;
    //    Debug.Log("Discard Changes?");
    //}

    ///// <summary>Closes a dialogue box and sets insidedialoguebox to false.</summary>
    //public void CloseDialogueBox()
    //{
    //    m_DialogueBoxUI.SetActive(false);
    //    m_InsideDialogueBox = false;
    //    Debug.Log("Settings still to be applied so cancelled.");
    //}

    ///// <summary>Discards changes. DOESN'T CURRENTLY WORK.</summary>
    //public void DiscardChanges()
    //{
    //    m_InsideDialogueBox = false;
    //    Debug.Log("Discarded changes.");
    //}

    /// <summary>Exits the game to desktop.</summary>
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }

    /// <summary>Exits the game to main menu.</summary>
    public void ExitToMenu()
    {
        StartCoroutine(LoadMainMenu());
    }
}
