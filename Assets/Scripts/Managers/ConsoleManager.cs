using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsoleManager : MonoBehaviour
{
    public Canvas m_Console;
    public ConsoleInputHandler m_InputHandler;
    // Start is called before the first frame update

    void Start()
    {
        CustomConsole.Init(m_Console, m_InputHandler);
        m_Console.enabled = false;
    }

}
