using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput
{
    private InputMap m_Input;

    // Player actions
    private Action m_Dash;
    private Action m_Possession;
    private Action m_Melee;
    private Action m_ADS;
    private Action m_Fire;

    public Vector2 MovementAxis { get; private set; }
    public Vector2 LookAxis { get; private set; }

    public bool ADS { get; private set; } = false;
    public bool Dash { get; private set; } = false;
    public bool Fire { get; private set; } = false;
    public bool Melee { get; private set; } = false;
    public bool Possess { get; private set; } = false;

    private UserInput()
    {
        m_Input = new InputMap();

        // Movement
        m_Input.Host.Movement.performed += ctx => MovementAxis = ctx.ReadValue<Vector2>();
        m_Input.Pariah.Movement.performed += ctx => MovementAxis = ctx.ReadValue<Vector2>();
        m_Input.Host.Movement.canceled += ctx => MovementAxis = Vector2.zero;
        m_Input.Pariah.Movement.canceled += ctx => MovementAxis = Vector2.zero;

        // Look
        m_Input.Host.Look.performed += ctx => LookAxis = ctx.ReadValue<Vector2>();
        m_Input.Pariah.Look.performed += ctx => LookAxis = ctx.ReadValue<Vector2>();
        m_Input.Host.Look.canceled += ctx => LookAxis = Vector2.zero;
        m_Input.Pariah.Look.canceled += ctx => LookAxis = Vector2.zero;

        // ADS
        m_Input.Host.ADS.performed += ctx => ADS = true;
        m_Input.Host.ADS.canceled += ctx => ADS = false;

        // Dash
        m_Input.Host.Dash.performed += ctx => Dash = true;
        m_Input.Pariah.Dash.performed += ctx => Dash = true;
        m_Input.Host.Dash.canceled += ctx => Dash = false;
        m_Input.Pariah.Dash.canceled += ctx => Dash = false;

        // Fire
        m_Input.Host.Fire.performed += ctx => Fire = true;
        m_Input.Host.Fire.canceled += ctx => Fire = false;

        // Melee
        m_Input.Host.Melee.performed += ctx => Melee = true;
        m_Input.Host.Melee.canceled += ctx => Melee = false;

        // Possess
        m_Input.Host.Possession.performed += ctx => Possess = true;
        m_Input.Pariah.Possession.performed += ctx => Possess = true;
        m_Input.Host.Possession.canceled += ctx => Possess = false;
        m_Input.Pariah.Possession.canceled += ctx => Possess = false;
    }

    public void Enable() => m_Input.Enable();
    public void Disable() => m_Input.Disable();
}
