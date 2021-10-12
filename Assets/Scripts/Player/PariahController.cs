using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Tweening;
using WhiteWillow;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PariahController : InputController
{
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_Acceleration = 0.75f;

    // TODO: Move these somewhere more appropriate
    [Header("<[Temporary]>")]
    [Min(0)]
    [SerializeField]
    private int m_Health = 100;

    [Min(0)]
    [SerializeField]
    private int m_MaxHealth = 100;

    [Min(0)]
    [SerializeField]
    private int m_HealthDrainAmount = 1;

    [Min(0.0f)]
    [SerializeField]
    private float m_HealthDrainDelay = 0.1f;

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
    private bool m_Possessing = false;

    [ReadOnly]
    [SerializeField]
    private Agent m_CurrentPossessed;

    private Rigidbody m_Rigidbody;

    bool m_Dead = false;

    [ReadOnly]
    public int m_Power = 0; // Power is used for the death incarnate ability. It is gained by destroying agents.

    private void Awake() => m_Rigidbody = GetComponent<Rigidbody>();

    private void Start()
    {
        // Crude fix for allowing player to face any direction at start of runtime
        var euler = transform.rotation.eulerAngles;
        m_Rotation = new Vector2(euler.y, euler.x);
        m_Camera.fieldOfView = (Mathf.Atan(Mathf.Tan((float)(m_PlayerPrefs.VideoConfig.FieldOfView * Mathf.Deg2Rad) * 0.5f) / m_Camera.aspect) * 2) * Mathf.Rad2Deg;//

        StartCoroutine(DrainHealth(m_HealthDrainDelay));
    }

    private void FixedUpdate()
    {
        if (!PauseMenu.m_GameIsPaused)
        {
            if (m_Active)
                Move();
            else
            {
                if (m_CurrentPossessed != null)
                {
                    transform.position = m_CurrentPossessed.transform.position + Vector3.up * 0.75f;
                    m_Rotation.x = m_CurrentPossessed.Orientation.localEulerAngles.y;
                    m_Rotation.y = m_CurrentPossessed.Orientation.localEulerAngles.x;
                }
            }
        }
    }

    private void LateUpdate()
    {
        m_Camera.fieldOfView = (Mathf.Atan(Mathf.Tan((float)(m_PlayerPrefs.VideoConfig.FieldOfView * Mathf.Deg2Rad) * 0.5f) / m_Camera.aspect) * 2) * Mathf.Rad2Deg;//
        if (!PauseMenu.m_GameIsPaused)
        {
            if (m_Active)
                Look();
            else
            {

            }
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
        m_Active = false;
        m_Camera.enabled = false;
    }

    public override void OnLook(InputAction.CallbackContext value)
    {
        if (!PauseMenu.m_GameIsPaused)
        {
            m_LookInput = value.ReadValue<Vector2>();
        }
    }

    public override void OnMovement(InputAction.CallbackContext value)
    {
        if (!PauseMenu.m_GameIsPaused)
        {
            Vector2 input = value.ReadValue<Vector2>();
            m_MovementInput.x = input.x;
            m_MovementInput.z = input.y;
        }
    }

    public override void OnDash(InputAction.CallbackContext value)
    {
        if (!PauseMenu.m_GameIsPaused)
        {
            if (value.performed && !m_Dashing && !m_DashCoolingDown && !m_Possessing)
            {
                if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out RaycastHit hitInfo, m_DashDistance))
                    StartCoroutine(Dash(hitInfo.point, -m_Camera.transform.forward * 0.5f, m_DashDuration));
                else
                    StartCoroutine(Dash(m_Camera.transform.position + m_Camera.transform.forward * m_DashDistance, Vector3.zero, m_DashDuration));
            }
        }
    }

    public override void OnJump(InputAction.CallbackContext value)
    {
        if (!PauseMenu.m_GameIsPaused)
        {
            m_MovementInput.y += value.canceled ? -1.0f : 1.0f;
            m_MovementInput.y = Mathf.Clamp(m_MovementInput.y, -1.0f, 1.0f);
        }
    }

    public override void OnSlide(InputAction.CallbackContext value)
    {
        if (!PauseMenu.m_GameIsPaused)
        {
            m_MovementInput.y += value.canceled ? 1.0f : -1.0f;
            m_MovementInput.y = Mathf.Clamp(m_MovementInput.y, -1.0f, 1.0f);
        }
    }

    public override void OnPossess(InputAction.CallbackContext value)
    {
        if (!PauseMenu.m_GameIsPaused)
        {
            if (value.performed && !m_Possessing)
            {
                if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out RaycastHit hitInfo, m_DashDistance))
                {
                    if (hitInfo.transform.TryGetComponent(out Agent agent))
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

        m_Rotation += m_LookInput * m_PlayerPrefs.GameplayConfig.MouseSensitivity * Time.deltaTime;
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
        Vector3 targetEyes = target.transform.position + Vector3.up * 0.75f;
        GetComponent<Collider>().enabled = false;

        // TODO: Use epsilon and replace distance check with non sqrt function
        while (Vector3.Distance(transform.position, targetEyes) > 0.01f)
        {
            if (!PauseMenu.m_GameIsPaused)
            {
                transform.position = Vector3.Lerp(transform.position, targetEyes, Tween.EaseInOut5(currentTime / m_DashDuration));

                float distToTarget = Vector3.Distance(transform.position, targetEyes);
                if (distToTarget <= 1.0f)
                    transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, Tween.EaseOut3(distToTarget));

                currentTime += Time.deltaTime;
            }

            yield return null;
        }

        target.Possess();
        m_CurrentPossessed = target;
        m_Possessing = false;
    }

    //TODO: Move all functions below into a more suitable location

    public void AddHealth(int amount)
    {
        m_Health = Mathf.Clamp(m_Health + amount, 0, m_MaxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (m_Dead) return;

        m_Health = Mathf.Clamp(m_Health - amount, 0, m_MaxHealth);
        if (m_Health == 0)
        {
            // TODO: Kill pariah
            // Temporary: Reloads the current scene
            m_Dead = true;
            StartCoroutine(ReloadLevel());
        }
    }

    private IEnumerator DrainHealth(float delay)
    {
        while (true)
        {
            if (!m_Dead && m_Active && !m_Possessing && !PauseMenu.m_GameIsPaused)
            {
                m_Health -= m_HealthDrainAmount;
                if (m_Health <= 0)
                {
                    m_Health = 0;
                    m_Dead = true;
                    StartCoroutine(ReloadLevel());
                }

                yield return new WaitForSeconds(delay);
            }

            yield return null;
        }
    }

    // ****** Highly temporary ******
    private IEnumerator ReloadLevel()
    {
        yield return null;

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

    /// <summary>
    /// GetHealth() gets around the private health.
    /// Sorry for adding code into this script. It's just that health was private and I needed to access it from a UI script.
    /// </summary>
    /// <returns></returns>
    public int GetHealth() { return m_Health; }

    /// <summary>
    /// ForceInstantPossess() is used to start the player in an agent. It is called from GameManager.
    /// </summary>
    /// <param name="target">Agent you want to control.</param>
    public void ForceInstantPossess(Agent target)
    {
        //target.Possess();
        //m_CurrentPossessed = target;
        //m_Possessing = false;

        StartCoroutine(Possess(target));
    }
}
