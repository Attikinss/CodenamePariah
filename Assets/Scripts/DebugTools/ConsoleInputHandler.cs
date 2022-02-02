using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ConsoleInputHandler : MonoBehaviour
{
    public TMP_InputField m_Input;
    public TMP_Text m_Log;
    string completeLog;
    // Start is called before the first frame update
   
    public void Activate()
    {
        m_Input.enabled = true;
        m_Input.ActivateInputField();
        Cursor.lockState = CursorLockMode.None;
    }
    public void Deactivate()
    {
        m_Input.enabled = false;
        m_Input.DeactivateInputField(true);
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    public void OnSubmit()
    {
        m_Input.ActivateInputField();
        CustomConsole.ParseCommand(m_Input.text);
        completeLog += "$ " + m_Input.text + '\n';
        m_Input.text = "";
        m_Log.text = completeLog;
    }

    
}
