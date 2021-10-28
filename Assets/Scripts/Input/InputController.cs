using System;
using System.Collections;
using Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public abstract class InputController : MonoBehaviour
{
    [SerializeField]
    protected Camera m_Camera;
    public Camera Camera { get => m_Camera; protected set => m_Camera = value; }

    [SerializeField]
    protected float m_MovementSpeed;

    [SerializeField]
    protected float m_LookSensitivity;

    [Min(0.0f)]
    [SerializeField]
    protected float m_DashDuration = 0.5f;

    [Min(0.0f)]
    [SerializeField]
    protected float m_DashCooldown = 1.0f;

    [Min(0.0f)]
    [SerializeField]
    protected float m_DashDistance = 5.0f;

    [SerializeField]
    protected PlayerPreferences m_PlayerPrefs;

    [SerializeField]
    private PlayerInput m_PlayerInput;

    [SerializeField]
    protected bool m_Active = false;

    [SerializeField]
    [ReadOnly]
    protected bool m_Dashing = false;

    [SerializeField]
    [ReadOnly]
    protected bool m_DashCoolingDown = false;

    public abstract void Enable();
    public abstract void Disable();

    public abstract void OnDash(InputAction.CallbackContext value);
    public abstract void OnMovement(InputAction.CallbackContext value);
    public abstract void OnLook(InputAction.CallbackContext value);
    public abstract void OnJump(InputAction.CallbackContext value);
    public abstract void OnSlide(InputAction.CallbackContext value);
    public abstract void OnPossess(InputAction.CallbackContext value);

    public virtual void OnHeal(InputAction.CallbackContext value)
    {
        if (!PauseMenu.m_GameIsPaused)
        {
            if (value.control.IsPressed())
                GameManager.s_Instance.IsHoldingHeal = true;
            else if (value.canceled)
            { 
                GameManager.s_Instance.IsHoldingHeal = false;
                if (GameManager.s_Instance.m_HealingRoutineActive)
                { 
                    GameManager.s_Instance.m_HealingRoutineActive = false;
                    GameManager.s_Instance.StopCoroutine(GameManager.s_Instance.m_HealingRoutine);
                }
            }
        }
    }
    protected IEnumerator Dash(Vector3 destination, Vector3 offset, float duration, bool delayed = false) // Extra bool is so I can call the animation for the host
    {                                                                                                     // differently.
        if (!m_DashCoolingDown)
        {
            // Play Pariah's arms dash animation.
            if(!delayed) // Only play animation here if this is Pariah's dash, if it is the host's dash it will be delayed.
                GameManager.s_Instance.m_Pariah.PlayArmAnim("OnDash");

            m_DashCoolingDown = true;
            float currentTime = 0.0f;

            // TODO: Use epsilon and replace distance check with non sqrt function
            while (currentTime < duration)
            {
                if (Vector3.Distance(transform.position, destination + offset) < 0.5f)
                {
                    m_Dashing = false;
                    break;
                }

                // TODO: Replace with rigidbody movement to address
                // the halting that occurs at the end of a dash
                transform.position = Vector3.Lerp(transform.position, destination + offset, Tween.EaseInOut5(currentTime / duration));

                currentTime += Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(m_DashCooldown);
        m_DashCoolingDown = false;
        
    }

    public void OnConsoleToggle(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            CustomConsole.Toggle();
        }
    }
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
