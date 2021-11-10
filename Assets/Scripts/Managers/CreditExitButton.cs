using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditExitButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void Exit()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
