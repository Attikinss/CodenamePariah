using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Tweening;
using WhiteWillow;

[RequireComponent(typeof(Rigidbody))]
public class PariahController : InputController
{
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_Acceleration = 0.75f;

    

    [Header("Inputs")]

    [ReadOnly]
    [SerializeField]
    private Vector3 m_MovementInput;

    [ReadOnly]
    [SerializeField]
    private Vector2 m_LookInput;

    [Header("Debug")]

    [ReadOnly]
    [SerializeField]
    private Vector2 m_Rotation;

    [ReadOnly]
    [SerializeField]
    private Vector3 m_MoveVelocity;

    [ReadOnly]
    [SerializeField]
    private Vector2 m_AccumulatedLook;

    [ReadOnly]
    [SerializeField]
    private float m_CameraTilt = 0.0f;

    [ReadOnly]
    [SerializeField]
    private bool m_Dashing = false;

    [ReadOnly]
    [SerializeField]
    private bool m_Possessing = false;

    [ReadOnly]
    [SerializeField]
    private Agent m_CurrentPossessed;

    private Rigidbody m_Rigidbody;

    private void Awake() => m_Rigidbody = GetComponent<Rigidbody>();

    private void Start()
    {
        // Crude fix for allowing player to face any direction at start of runtime
        var euler = transform.rotation.eulerAngles;
        m_Rotation = new Vector2(euler.y, euler.x);
    }

    private void FixedUpdate()
    {
        if (m_Active)
            Move();
        else
        {
            if (m_CurrentPossessed != null)
            {
                transform.position = m_CurrentPossessed.transform.position + Vector3.up * 1.75f;
                transform.rotation = Quaternion.Euler(m_CurrentPossessed.FacingDirection);
            }
        }
    }

    private void LateUpdate()
    {
        if (m_Active)
            Look();
        else
        {

        }
    }

    public override void Enable()
    {
        GetComponent<PlayerInput>().enabled = true;
        GetComponent<Collider>().enabled = true;
        m_Active = true;
        m_Camera.enabled = true;
    }

    public override void Disable()
    {
        GetComponent<PlayerInput>().enabled = false;
        GetComponent<Collider>().enabled = false;
        m_Active = false;
        m_Camera.enabled = false;
    }

    public override void OnLook(InputAction.CallbackContext value)
    {
        m_LookInput = value.ReadValue<Vector2>();
    }

    public override void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 input = value.ReadValue<Vector2>();
        m_MovementInput.x = input.x;
        m_MovementInput.z = input.y;
    }

    public override void OnDash(InputAction.CallbackContext value)
    {
        if (value.performed && !m_Dashing && !m_Possessing)
        {
            if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out RaycastHit hitInfo, m_DashDistance))
                StartCoroutine(Dash(hitInfo.point, - m_Camera.transform.forward * 0.5f, m_DashDuration));
            else
                StartCoroutine(Dash(m_Camera.transform.position + m_Camera.transform.forward * m_DashDistance, Vector3.zero, m_DashDuration));
        }
    }

    public override void OnJump(InputAction.CallbackContext value)
    {
        m_MovementInput.y += value.canceled ? -1.0f : 1.0f;
        m_MovementInput.y = Mathf.Clamp(m_MovementInput.y, -1.0f, 1.0f);
    }

    public override void OnSlide(InputAction.CallbackContext value)
    {
        m_MovementInput.y += value.canceled ? 1.0f : -1.0f;
        m_MovementInput.y = Mathf.Clamp(m_MovementInput.y, -1.0f, 1.0f);
    }

    public override void OnPossess(InputAction.CallbackContext value)
    {
        if (value.performed && !m_Possessing)
        {
            if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out RaycastHit hitInfo, m_DashDistance))
            {
                if (hitInfo.transform.TryGetComponent(out Agent agent))
                {
                    StartCoroutine(Possess(agent));
                }
            }
        }
    }

    private void Move()
    {
        Vector3 moveDirection = (transform.right * m_MovementInput.x + transform.up * m_MovementInput.y + transform.forward * m_MovementInput.z)
            * m_MovementSpeed * Time.fixedDeltaTime;

        m_MoveVelocity += (moveDirection - m_MoveVelocity) * m_Acceleration;

        if (!m_Dashing && !m_Possessing)
            m_Rigidbody.velocity = m_MoveVelocity;

        Telemetry.TracePosition("Pariah-Movement", transform.position, 0.05f, 150);
    }

    private void Look()
    {
        CameraTilt();

        m_Rotation += m_LookInput * m_LookSensitivity * Time.deltaTime;
        m_Rotation.y = Mathf.Clamp(m_Rotation.y, -90.0f, 90.0f);
        transform.rotation = Quaternion.Euler(-m_Rotation.y, m_Rotation.x, 0.0f);
    }

    private void CameraTilt()
    {
        float half = m_LookInput.x / 2;
        float mouseDelta = half + m_LookInput.x / 2.0f;

        m_Camera.transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, m_CameraTilt);
    }

    private IEnumerator Possess(Agent target)
    {
        // Early out
        if (target == null) yield return null;

        float currentTime = 0.0f;
        Vector3 targetEyes = target.transform.position + Vector3.up * 1.75f;

        // TODO: Use epsilon and replace distance check with non sqrt function
        while (Vector3.Distance(transform.position, targetEyes) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetEyes, Tween.EaseInOut5(currentTime / m_DashDuration));

            float distToTarget = Vector3.Distance(transform.position, targetEyes);
            if (distToTarget <= 1.0f)
                transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, Tween.EaseOut3(distToTarget));

            currentTime += Time.deltaTime;
            yield return null;
        }

        target.Possess();
        m_CurrentPossessed = target;
        m_Possessing = false;
    }
}
