using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Tweening;
using WhiteWillow;
using UnityEngine.SceneManagement;
//using FMOD;
using FMODUnity;
using UnityEngine.VFX;

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

    public Agent CurrentHost { get => m_CurrentPossessed; }

    private Rigidbody m_Rigidbody;

    bool m_Dead = false;

    public int m_LowHealthThreshold = 40;
    public int m_LowHealthVoiceLineThreshold = 20;

    [ReadOnly]
    public int m_Power = 0; // Power is used for the death incarnate ability. It is gained by destroying agents.

    public FMODAudioEvent m_AudioLowHPEvent;
    private bool m_IsPlayingLowHP = false;       // Heartbeat.
    private bool m_IsPlayingExtremeLowHP = false; // Voice line.

    [HideInInspector]
    public Agent m_LookedAtAgent = null;

    public SkinnedMeshRenderer m_Arms;

    public Animator m_ArmsAnimator;



    // ==================== FMOD Audio Events ==================== //
    // Okay, so before I had a bunch of different audio events for
    // every sound Pariah would make. This led to the issue of
    // having sounds overlap each other. To prevent this, I've
    // added checks to see if the sounds are playing in the
    // GeneralSounds script. 
    // =========================================================== //

    // Particle effect for incarnate ability.
    public VisualEffect m_IncarnateParticle;

    // Smokey particle effects for Pariah's arms. We need a reference to them so we can disable them when we possess a unit.
    public VisualEffect m_SmokyArmParticle1;
    public VisualEffect m_SmokyArmParticle2;



    private void Awake() => m_Rigidbody = GetComponent<Rigidbody>();

    private void Start()
    {
        // Crude fix for allowing player to face any direction at start of runtime
        var euler = transform.rotation.eulerAngles;
        m_Rotation = new Vector2(euler.y, euler.x);
        m_Camera.fieldOfView = (Mathf.Atan(Mathf.Tan((float)(m_PlayerPrefs.VideoConfig.FieldOfView * Mathf.Deg2Rad) * 0.5f) / m_Camera.aspect) * 2) * Mathf.Rad2Deg;//

        StartCoroutine(DrainHealth(m_HealthDrainDelay));
    }

	private void Update()
	{
        // Currently I'm using this update function to handle tracking what agent Pariah is looking at.
        // When Pariah looks at an agent, we want to apply the possession shader.

        if (m_CurrentPossessed == null) // Only do this if we are not in a unit.
        {
            RaycastHit lookHit;
            if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out lookHit, m_DashDistance))
            {
                // This is the same raycast settings as used in the OnPossession function.
                if (lookHit.transform.TryGetComponent(out Agent agent))
                {
                    // We are looking at a selectable agent!

                    if (m_LookedAtAgent) // If we have a reference to a previous looked at agent, we have to reset their shader before we select a new agent.
                    {
                        m_LookedAtAgent.DeselectAgent();
                    }

                    // Now that we have reset the previously looked at agent's shader, we can work with the new agent.
                    // Set currently looked at agent to this one.
                    m_LookedAtAgent = agent;
                    // Set the new agent's shader to the possession one.
                    m_LookedAtAgent.SelectAgent();
                }
                else
                {
                    // We are looking at something, but not an agent. So let's clear the currently looked at agent if we have one.
                    if (m_LookedAtAgent)
                    {
                        m_LookedAtAgent.DeselectAgent();
                        m_LookedAtAgent = null;
                    }
                }
            }
            else
            {
                // The raycast failed to hit anything. We should deselect and reset the shader on any selected agent, if we have one.
                if (m_LookedAtAgent)
                {
                    m_LookedAtAgent.DeselectAgent();
                    m_LookedAtAgent = null;
                }
            }
        }
	}

    

	private void FixedUpdate()
    {
        if (!PauseMenu.m_GameIsPaused)
        {
            if (m_Active)
                Move();

            // Stop playing low hp sound.
            // We should stop playing the low health sound if we are playing it.
            // This stops the hearbeat.
            if (m_IsPlayingLowHP && m_Health > m_LowHealthThreshold)
            {
                StopLowHPSound();
            }
            // Now we also have to check if we have enough health to reset the voice line.
            if (m_IsPlayingExtremeLowHP && m_Health > m_LowHealthVoiceLineThreshold)
            {
                m_IsPlayingExtremeLowHP = false;
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
                // I moved this position and rotation update function from FixedUpdate() into this LateUpdate().
                // I had to move it because I wanted Pariah's arms to match the direction and position of the agent that way we can play an animation from here
                // when the user is in the agent and it will be correctly positioned.
                // The reason why its LateUpdate is because the Look() code for the HostController is in LateUpdate() and so to match the HostController's 
                // camera movement we had to put this in here.
                if(m_CurrentPossessed != null)
                {
                    transform.position = m_CurrentPossessed.transform.position + Vector3.up * 0.75f;

                    // Changed those localEulerAngles to just normal eulerAngles because the parent prefab of m_Orientation may be rotated.
                    // This is the case now because the parent of m_Orientation is where the soldier/scientist mesh is.
                    m_Rotation.x = m_CurrentPossessed.Orientation.eulerAngles.y;
                    m_Rotation.y = m_CurrentPossessed.Orientation.eulerAngles.x;

                    // Updating the rotation because now we have the animations that will sometimes play from Pariah even when the user is in an agent.
                    // We need the arms to be rotated the correct way.

                    // Old transform update.
                    //transform.rotation = Quaternion.Euler(-m_Rotation.y, m_Rotation.x, 0.0f);

                    // New transform update. // We want to make Pariah face the same direction in the X rotation as the player when they're in controlling the agent.
                    transform.rotation = Quaternion.Euler(m_CurrentPossessed.m_HostController.Camera.transform.rotation.eulerAngles.x, m_Rotation.x, 0);
                    m_Rotation.y = -m_CurrentPossessed.m_HostController.GetXRotation();
                }
            }
        }
    }

    public override void Enable()
    {
        GetComponent<PlayerInput>().enabled = true;
        GetComponent<Collider>().enabled = true;
        m_Active = true;
        m_Camera.enabled = true;

        ToggleArms(true);
    }

    public override void Disable()
    {
        GetComponent<PlayerInput>().enabled = false;
        m_Active = false;
        m_Camera.enabled = false;

        ToggleArms(false);
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
            if (value.performed && !m_Dashing && !m_Possessing && m_CurrentDashCharges > 0)
            {

                // This is an additional "layer" of dash code to have a delay before the actual dash begins.
                // This snipper of code is copied from the HostController, there are more detailed comments located there too.

                //if (m_IsDelayedDashing) // This means we already have a dash in the process of being done.
                //{
                //    if (!m_Dashing && !m_DashCoolingDown) // If we aren't dashing, and the dash has cooled down, we can reset the m_IsDelayedDashing.
                //        m_IsDelayedDashing = false;
                //}
                //else
                //{
                    //m_IsDelayedDashing = true;
                    m_Dashing = true;
                    StartCoroutine(DelayedDash(m_DashDelay));
                //}


                // This peice of code is now moved into the DelayedDash() function.
                 
                //if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out RaycastHit hitInfo, m_DashDistance))
                //    StartCoroutine(Dash(hitInfo.point, -m_Camera.transform.forward * 0.5f, m_DashDuration));
                //else
                //    StartCoroutine(Dash(m_Camera.transform.position + m_Camera.transform.forward * m_DashDistance, Vector3.zero, m_DashDuration));
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
                    { 
                        StartCoroutine(Possess(agent));
                        PlayArmAnim("OnDash");
                    }
                }
            }
        }
    }
    private void Move()
    {
        Vector3 moveDirection = (transform.right * m_MovementInput.x + transform.up * m_MovementInput.y + transform.forward * m_MovementInput.z)
            * m_MovementSpeed * Time.fixedDeltaTime;

        m_MoveVelocity += (moveDirection - m_MoveVelocity) * m_Acceleration;

        if (/*!m_Dashing && */!m_Possessing) // Commented out m_Dashing here to allow Pariah to move while dashing.
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

        // Play heartbeat if our life essence is beyond the threshold.
        if (m_Health <= m_LowHealthThreshold && !m_IsPlayingLowHP)
        {
            m_IsPlayingLowHP = true;
            PlayLowHPSound();
            //GeneralSounds.s_Instance.PlayLowHealthPariahSound(transform); // This is the voice line sound effect.
        }

        // Play voice line if we are even lower health.
        if (m_Health <= m_LowHealthVoiceLineThreshold && !m_IsPlayingExtremeLowHP)
        {
            m_IsPlayingExtremeLowHP = true;
            GeneralSounds.s_Instance.PlayLowHealthPariahSound(transform);
        }

        if (m_Health == 0)
        {
            // TODO: Kill pariah
            // Temporary: Reloads the current scene
            m_Dead = true;
            FMOD.Studio.Bus allBussess = RuntimeManager.GetBus("bus:/");
            allBussess.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
            PauseMenu.m_GameOver = true;
            //StartCoroutine(ReloadLevel());
        }
    }

    private IEnumerator DrainHealth(float delay)
    {
        while (true)
        {
            if (!m_Dead && m_Active && !m_Possessing && !PauseMenu.m_GameIsPaused)
            {
                // This added on check is to only lose health after we have entered a unit for the first time.
                if (GameManager.s_Instance && GameManager.s_Instance.m_Achievements.hasEnteredUnit)
                    m_Health -= m_HealthDrainAmount;

                // Only play the sound if we're not already playing it.
                if (m_Health <= m_LowHealthThreshold && !m_IsPlayingLowHP)
                {
                    m_IsPlayingLowHP = true;
                    PlayLowHPSound();
                    //GeneralSounds.s_Instance.PlayLowHealthPariahSound(transform); // This is the voice line sound effect.
                }
                // Play voice line if we are even lower health.
                if (m_Health <= m_LowHealthVoiceLineThreshold && !m_IsPlayingExtremeLowHP)
                {
                    m_IsPlayingExtremeLowHP = true;
                    GeneralSounds.s_Instance.PlayLowHealthPariahSound(transform);
                }



                if (m_Health <= 0)
                {
                    m_Health = 0;
                    m_Dead = true;
                    FMOD.Studio.Bus allBussess = RuntimeManager.GetBus("bus:/");
                    allBussess.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    PauseMenu.m_GameOver = true;
                    //StartCoroutine(ReloadLevel());
                }

                yield return new WaitForSeconds(delay);
            }

            yield return null;
        }
    }

    // ****** Highly temporary ******
    private IEnumerator ReloadLevel()
    {
        GameManager.s_IsNotFirstLoad = true; // Telling the game manager that it's not the games first load.
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

    private void PlayLowHPSound()
    {
        if (m_AudioLowHPEvent)
        {
            m_AudioLowHPEvent.Trigger();
        }
       
    }

    private void StopLowHPSound()
    {
        if (m_AudioLowHPEvent)
        {
            m_IsPlayingLowHP = false;
            m_AudioLowHPEvent.StopSound(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        
    }


    /// <summary>
    /// Made this function so I wouldn't have to make m_CurrentPossessed a public variable. I needed to be able to clear this back to null.
    /// </summary>
    public void ClearCurrentPossessed() { m_CurrentPossessed = null; }

    /// <summary>
    /// Used to hide and unhide Pariah's arms.
    /// </summary>
    private void ToggleArms(bool mode)
    {
        if (m_Arms)
        {
            if (mode)
            {
                m_Arms.enabled = true;
                m_SmokyArmParticle1.enabled = true;
                m_SmokyArmParticle2.enabled = true;
            }
            else
            { 
                m_Arms.enabled = false;
                m_SmokyArmParticle1.enabled = false;
                m_SmokyArmParticle2.enabled = false;
            }
        }
        else
            Debug.LogWarning("Pariah's arms have not been set!");
    }

    /// <summary>
    /// Pass in the name of the trigger and this will call .SetTrigger(). Optionally argument to allow hiding of arms
    /// after animation is complete.
    /// </summary>
    /// <param name="triggerName">Name of animation trigger.</param>
    /// <param name="m_HideArmsAfterwards">Bool to set if the arms hide or not after the animation is complete.</param>
    public void PlayArmAnim(string triggerName, bool m_HideArmsAfterwards = true)
    {
        if (m_ArmsAnimator == null)
        {
            Debug.LogWarning("Tried to play Pariah arm animation but PariahController is missing a reference to the arms animation controller.");
            return;
        }
        else // Otherwise, we could find the arms animator.
        {
            if (triggerName == "OnDash")
            {
                if (m_Arms.enabled == false) // If the arms are hidden, unhide them for the duration of the dash animation. This is so the hosts can use this animation.
                {
                    m_Arms.enabled = true;
                    StartCoroutine(HideArms(0.30f)); // 0.30f is around about the time it takes for the animation to complete.

                }
                m_ArmsAnimator.SetTrigger(triggerName);
            }
            else if (triggerName == "OnIncarnate")
            {
                if (m_Arms.enabled == false )
                {
                    m_Arms.enabled = true;
                    if(m_HideArmsAfterwards)
                        StartCoroutine(HideArms(6f));
                }
                m_ArmsAnimator.SetTrigger(triggerName);
            }
        }
    }

    /// <summary>
    /// Will hide Pariah's arms in time amount of seconds.
    /// </summary>
    /// <param name="time">Elapsed time required before hiding Pariah's arms.</param>
    /// <returns></returns>
    IEnumerator HideArms(float time)
    {
        float elapsedTime = 0;

        while (elapsedTime <= time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_Arms.enabled = false; // Hide Pariah's arms after this counter has completed.
    }

    /// <summary>
    /// DelayedDash() acts as a wrapper around the old dash code to cause a delay coroutine to start before the actual dash can begin.
    /// This is used because we want the dash to perform a few milliseconds after the animations plays, to make it look like Pariah is pulling the
    /// unit forward.
    /// </summary>
    /// <param name="t">Amount of delay.</param>
    /// <returns></returns>
    IEnumerator DelayedDash(float t)
    {
        m_IsDelayedDashing = true;
        // Play dash animation.
        GameManager.s_Instance?.m_Pariah.PlayArmAnim("OnDash");

        float delayTime = 0.0f;
        // Adding a start delay before actual dash is performed.
        while (delayTime <= t)
        {
            delayTime += Time.deltaTime;
            yield return null;
        }


        // Getting the correct forward vector. If we are in the air, we want to be able to move in any direction, however, when we are grounded, we want to
        // ignore the camera being able to look up and down (rotation on the x axis).
        Vector3 forwardDir = m_Camera.transform.forward;
        //if (m_MovInfo.m_IsGrounded) This was copied from the HostController, but seeing as Pariah can't be grounded, I'll ignore this.
        //    forwardDir = m_Orientation.forward;



        // When we get to this point, we just continue with the ordinary dash.

        if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out RaycastHit hitInfo, m_DashDistance))
        {
            StartCoroutine(Dash(hitInfo.point, -m_Camera.transform.forward * 0.5f, m_DashDuration, true));
        }
        else
        { 
            StartCoroutine(Dash(m_Camera.transform.position + m_Camera.transform.forward * m_DashDistance, Vector3.zero, m_DashDuration, true));
        }
    }

    
}
