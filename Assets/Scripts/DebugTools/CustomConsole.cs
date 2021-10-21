using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomConsole
{
    public static Canvas m_Canvas;
    public static ConsoleInputHandler m_Handler;

    public static bool m_Activated = false;

    public static void Init(Canvas canvas, ConsoleInputHandler handler)
    {
        m_Canvas = canvas;
        m_Handler = handler;
    }

    public static void Toggle()
    {
        if (m_Activated)
            Deactivate();
        else
            Activate();
    }
    public static void Activate()
    {
        // Turn on the console.
        if (m_Canvas)
        { 
            m_Activated = true;
            m_Canvas.enabled = true;
            m_Handler.Activate();
        }
    }
    public static void Deactivate()
    {
        if (m_Canvas)
        { 
            // Turn off the console.
            m_Activated = false;
            m_Canvas.enabled = false;
            m_Handler.Deactivate();
        }
    }

    public static void ParseCommand(string cmd)
    {
        if (cmd.ToUpper() == "GET ASSAULTRIFLE")
        {
            if (GameManager.s_CurrentHost)
            {
                HostController host = GameManager.s_CurrentHost;
                host.m_Inventory.AddWeapon(Resources.Load<GameObject>("AssaultRifle"));
            }
        }
        else if (cmd.ToUpper() == "GET PISTOL")
        {
            if (GameManager.s_CurrentHost)
            {
                HostController host = GameManager.s_CurrentHost;
                host.m_Inventory.AddWeapon(Resources.Load<GameObject>("Pistol"));
            }
        }
        else if (cmd.ToUpper() == "GET DUALWIELD")
        {
            if (GameManager.s_CurrentHost)
            {
                HostController host = GameManager.s_CurrentHost;
                host.m_Inventory.AddWeapon(Resources.Load<GameObject>("DualRifle"));
            }
        }
    }
}
