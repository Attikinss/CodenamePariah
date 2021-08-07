using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public abstract class InputController : MonoBehaviour
{
    [SerializeField]
    protected Camera m_Camera;

    [SerializeField]
    protected float m_MovementSpeed;

    [SerializeField]
    protected float m_LookSensitivity;

    [SerializeField]
    private PlayerInput m_PlayerInput;

    public virtual void OnDash(InputAction.CallbackContext value) { }
    public abstract void OnMovement(InputAction.CallbackContext value);
    public abstract void OnLook(InputAction.CallbackContext value);

    public void ControllerRumbleOnce(float stength)
    {
        Haptics.Rumble(stength);
        Invoke(nameof(StopRumble), 0.1f);
    }

    public void ControllerRumble(float stength, float duration)
    {
        Haptics.Rumble(stength);
        Invoke(nameof(StopRumble), duration);
        Haptics.StopRumble();
    }

    public void ControllerRumbleIncrease(float stength, float duration)
    {
        Haptics.Rumble(stength);
        Invoke(nameof(StopRumble), duration);
        Haptics.StopRumble();
    }

    public void StopRumble()
    {
        Haptics.StopRumble();
    }
}
