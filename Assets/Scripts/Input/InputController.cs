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

    [SerializeField]
    protected float m_MovementSpeed;

    [SerializeField]
    protected float m_LookSensitivity;

    [Min(0.0f)]
    [SerializeField]
    protected float m_DashDuration = 0.5f;

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

    public abstract void Enable();
    public abstract void Disable();

    public abstract void OnDash(InputAction.CallbackContext value);
    public abstract void OnMovement(InputAction.CallbackContext value);
    public abstract void OnLook(InputAction.CallbackContext value);
    public abstract void OnJump(InputAction.CallbackContext value);
    public abstract void OnSlide(InputAction.CallbackContext value);
    public abstract void OnPossess(InputAction.CallbackContext value);

    protected IEnumerator Dash(Vector3 destination, Vector3 offset, float duration)
    {
        float currentTime = 0.0f;

        // TODO: Use epsilon and replace distance check with non sqrt function
        while (currentTime < duration && Vector3.Distance(transform.position, destination + offset) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, destination + offset, Tween.EaseInOut5(currentTime / duration));

            currentTime += Time.deltaTime;
            yield return null;
        }

        m_Dashing = false;
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
