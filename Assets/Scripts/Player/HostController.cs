using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class HostController : InputController
{
    [Header("Settings")]

    [Header("Mouse Controls")]
    public float m_VerticalLock = 75.0f;

    [Header("Movement Controls")]
    public float m_JumpHeight = 5;
    public float m_SecondJumpHeight = 2.5f;
    public float m_GroundCheckDistance = 0.65f;
    public float m_GroundCheckRadius = 0.42f;
    public float m_Gravity = -9.8f;
    public float m_MaxSpeed = 5;
    [Range(0.0f, 1.0f)]
    public float m_GroundAcceleration = 0.3f;
    [Range(0.0f, 1.0f)]
    public float m_AirAcceleration = 0.1f;
    public float m_JumpFallModifier = 2.0f;

    [Header("Slide Controls")]
    public float m_SlideSpeed = 700;
    public float m_SlideDuration = 0.75f;
    public float m_CameraSlideHeight = -0.5f;
    public Vector3 m_SlideColliderCentre;
    [Range(0, 2)]
    public float m_SlideColliderHeight;

    [Header("Other References")]
    [SerializeField]
    private bool m_EnableDebug = false;
    public Transform m_Orientation;
    public Inventory m_Inventory;
    public Rigidbody Rigidbody { get; private set; }
    public GameObject m_HUD;
    private UIManager m_UIManager;
    private CapsuleCollider m_Collider;
    

    public Vector3 LookInput { get; private set; }
    public float m_XRotation = 0;   // Made public because nowadays the Weapon.cs script needs to access it.
    public float m_DesiredX = 0; // Made public because I'm moving everything to the Weapon.cs script but I still need to access it there.

    public CameraRecoil m_AccumulatedRecoil = new CameraRecoil();
    public MovementInfo m_MovInfo = new MovementInfo();
    public CombatInfo m_CombatInfo = new CombatInfo();

    public DrainAbility m_DrainAbility;
    public DeathIncarnateAbility m_DeathIncarnateAbility;

	private void Awake()
	{
        m_AccumulatedRecoil = new CameraRecoil();
        m_MovInfo = new MovementInfo();
        m_CombatInfo = new CombatInfo();

        m_UIManager = GetComponent<UIManager>();
        Rigidbody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;

        // Caching original collider height and center point.
        m_Collider = GetComponent<CapsuleCollider>();
        if (m_Collider)
        {
            m_MovInfo.m_OriginalColliderCenter = m_Collider.center;
            m_MovInfo.m_OriginalColliderHeight = m_Collider.height;
        }
	}
    private void Update()
    {
        if (!m_Active) return;

        if (GetCurrentWeaponConfig().m_AlwaysADS) // The reason why I do this is so I don't have to check for the m_AlwaysADS bool everywhere. This way I can still just check for m_IsAiming in all functions.
            GetCurrentWeapon().m_IsAiming = true;
        if (GetCurrentWeaponConfig().m_AlwaysFiring) // Doing same here for the reason above.
            GetCurrentWeapon().m_IsFiring = true;

        m_MovInfo.m_IsGrounded = CheckGrounded();
        CalculateGroundNormal();
        if (m_MovInfo.m_IsGrounded)
            m_MovInfo.m_HasDoubleJumped = false;
        

        m_MovInfo.m_CurrentMoveSpeed = Rigidbody.velocity.magnitude;


        // Just for debugging purposes. This variable is only used in the CustomDebugUI script. - Not true anymore, apparently I am using it now ._.
        m_CombatInfo.m_camForward = m_Camera.transform.forward;


        // ================ NOTE ================ //
        // It's really weird and bad to have a counter like this. I'll try find a way around it but
        // for now it's helping fix an issue with walking up ramps and jumping.
        // ====================================== //

        if (m_MovInfo.m_HasJumped)
        {
            m_MovInfo.m_JumpCounter += Time.deltaTime; // how to get around having a timer for something like this?
        }


        // While draining, the player is not allowed to interact with their weapons.
        // ;-;
        if (m_DrainAbility.isDraining)
        {
            if (m_Inventory.GetHealth() > 0)
            {
                // timed event. have adjustable drain speed.
                m_DrainAbility.drainCounter += Time.deltaTime;
                if (m_DrainAbility.drainCounter >= m_DrainAbility.drainInterval)
                {
                    m_Inventory.Owner?.PariahController.AddHealth(m_DrainAbility.restore);
                    m_DrainAbility.drainCounter = 0.0f;
                    m_Inventory.TakeDamage(m_DrainAbility.damage);
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (m_Active)
            Look();
    }

    private void FixedUpdate()
	{
        if (!m_Active) return;

        Slide();
        Move(m_MovInfo.MovementInput);
	}

    
    /// <summary>
    /// Enable() is called when the player jumps into this agent.
    /// </summary>
    public override void Enable()
    {
        GetComponent<PlayerInput>().enabled = true;
        m_Active = true;
        m_Camera.enabled = true;
        UnhideHUD();

        CustomDebugUI.s_Instance.SetController(this);
    }

    /// <summary>
    /// Disable() is called when the player jumps out of the agent.
    /// </summary>
    public override void Disable()
    {
        GetComponent<PlayerInput>().enabled = false;
        m_Active = false;
        m_Camera.enabled = false;
        HideHUD();

        CustomDebugUI.s_Instance.ClearController();
    }

    // ========================================================== Input Events ========================================================== //
    // ================================================================================================================================== //
    public override void OnLook(InputAction.CallbackContext value)
	{
        if (GetCurrentWeapon().m_IsRecoilTesting)
            return; // early out to prevent mouse movement while testing recoil.
        LookInput = value.ReadValue<Vector2>();
	}

    public override void OnJump(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            Vector3 direction = Vector3.up * ControllerMaths.CalculateJumpForce(m_JumpHeight, Rigidbody.mass, m_Gravity);
            direction.x = Rigidbody.velocity.x;
            direction.z = Rigidbody.velocity.z;

            if (/*m_MovInfo.m_IsGrounded*/!m_MovInfo.m_HasJumped)
            {
                m_MovInfo.m_HasJumped = true;

                m_MovInfo.m_CacheMovDirection = direction;
                Rigidbody.velocity = direction;
            }
            else if (!m_MovInfo.m_IsGrounded && !m_MovInfo.m_HasDoubleJumped)
            {
                m_MovInfo.m_CacheMovDirection = direction;
                Rigidbody.velocity = direction;

                // Have to tick m_HasDoubleJumped to false;
                m_MovInfo.m_HasDoubleJumped = true;
            }
        }
    }

    public override void OnSlide(InputAction.CallbackContext value)
    {
        if (value.performed && m_MovInfo.m_IsGrounded && m_MovInfo.m_IsMoving)
        {
            Debug.Log("OnSlide called.");
            m_MovInfo.m_SlideDir = value.performed ? m_Orientation.forward : m_MovInfo.m_SlideDir;
            m_MovInfo.m_IsSliding = true;

            TransformCapsuleCollider(m_SlideColliderHeight, m_SlideColliderCentre); // Shrink the collider down. (Temporarily.)
        }
    }

	public override void OnMovement(InputAction.CallbackContext value)
	{
        m_MovInfo.MovementInput = value.performed ? value.ReadValue<Vector2>() : Vector2.zero;
	}

    public override void OnPossess(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            if (TryGetComponent(out WhiteWillow.Agent agent))
                agent.Reliquinsh();
        }
    }

    public void OnShoot(InputAction.CallbackContext value)
    {
        if (value.control.IsPressed(0)) // Have to use this otherwise mouse button gets triggered on release aswell.
        {
            // Now we have to access the weapon script to call weapon functions.
            GetCurrentWeapon().m_IsFiring = true;
            

            // Experimental thing I'm trying.
            // I will store the original camera rotation when they first start shooting that way I can go back to this rotation when they recover from recoil.
            m_CombatInfo.m_PrevCamForward = m_Camera.transform.forward;
            GetCurrentWeapon().SetFireTime();

        }
        else
        {
            GetCurrentWeapon().m_IsFiring = false;

            // Reset held counter happens regardless.
            m_CombatInfo.m_ShootingDuration = 0.0f;
        }
    }

    public void OnWeaponSelect1(InputAction.CallbackContext value)
    {
        if (value.performed && !GetCurrentWeapon().IsReloading())
        {
            // Temporary fix for bug where if the player switches to another weapon while reloading, the former gun can no longer shoot.
            GetCurrentWeapon().ResetReload();
            //GetCurrentWeapon().ResetReloadAnimation(); dont need here because pistol doesnt have animation yet.


            SelectWeapon(0);

            // Previously I was tracking weapon states in PlayerManager in an attempt to free up space in this controller script. However, now that we have an Inventory script that tracks weapons and
            // the players current weapon, I'll leave that stuff in there.
            //PlayerManager.SetWeapon(WeaponSlot.WEAPON1);


            //m_UIManager.m_IsRifle = true;
            m_UIManager.m_CurrentWeaponType = WEAPONTYPE.RIFLE;
            m_UIManager.HideMagazine(m_UIManager.m_CurrentWeaponType);
        }
    }

    public void OnWeaponSelect2(InputAction.CallbackContext value)
    {
        if (value.performed && !GetCurrentWeapon().IsReloading())
        {
            // Temporary fix for bug where if the player switches to another weapon while reloading, the former gun can no longer shoot.
            GetCurrentWeapon().ResetReload();
            GetCurrentWeapon().ResetReloadAnimation();

            SelectWeapon(1);

            //m_UIManager.m_IsRifle = false;
            m_UIManager.m_CurrentWeaponType = WEAPONTYPE.PISTOL;
            m_UIManager.HideMagazine(m_UIManager.m_CurrentWeaponType);
        }
    }

    public void OnWeaponSelect3(InputAction.CallbackContext value)
    {
        if (value.performed && !GetCurrentWeapon().IsReloading())
        {
            // Temporary fix for bug where if the player switches to another weapon while reloading, the former gun can no longer shoot.
            GetCurrentWeapon().ResetReload();
            GetCurrentWeapon().ResetReloadAnimation();

            SelectWeapon(2);

            //m_UIManager.m_IsRifle = false;
            m_UIManager.m_CurrentWeaponType = WEAPONTYPE.DUAL;
            m_UIManager.HideMagazine(m_UIManager.m_CurrentWeaponType);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GetCurrentWeapon().m_IsAiming = true;
        }
        else if (context.canceled)
        {
            GetCurrentWeapon().m_IsAiming = false;
        }
    }

    public void OnAbility2(InputAction.CallbackContext value)
    {
        // Do ability 2 stuff.
        if (value.performed)
        {
            m_DrainAbility.isDraining = true;
        }
        else if (value.canceled)
        {
            m_DrainAbility.isDraining = false;
        }
    }

    public void OnReload(InputAction.CallbackContext value)
    {
        if (value.performed)
        { 
            GetCurrentWeapon().StartReload();
        }
    }

    public override void OnDash(InputAction.CallbackContext value)
    {
        if (value.performed && !m_Dashing)
        {
            Vector3 forwardDir = m_Camera.transform.forward;
            if (m_MovInfo.m_IsGrounded)
                forwardDir = m_Orientation.forward;
            

            if (Physics.Raycast(m_Orientation.position, forwardDir, out RaycastHit hitInfo, m_DashDistance))
                StartCoroutine(Dash(hitInfo.point, -forwardDir * 0.5f, m_DashDuration));
            else
                StartCoroutine(Dash(transform.position + forwardDir * m_DashDistance, Vector3.zero, m_DashDuration));
        }
    }

    public void OnTestRecoil(InputAction.CallbackContext value)
    {
        if (value.performed && !GetCurrentWeapon().m_IsRecoilTesting)
        {
            GetCurrentWeapon().m_IsRecoilTesting = true;
            m_CombatInfo.m_PrevOrientationRot = m_Orientation.transform.eulerAngles;
            m_CombatInfo.m_PrevXRot = m_XRotation;

            GetCurrentWeapon().SetFireTime(); // Starting fire counter.
        }
        else if (value.performed && GetCurrentWeapon().m_IsRecoilTesting)
        {
            GetCurrentWeapon().m_IsRecoilTesting = false;
            GetCurrentWeapon().m_IsTestResting = false;

            GetCurrentWeapon().m_IsFiring = false;
            GetCurrentWeapon().m_RecoilTestCounter = 0;
        }
    }

    // Experimental death incarnate ability thing
    public void OnAbility3(InputAction.CallbackContext value)
    {
        if (value.performed && !m_DeathIncarnateAbility.deathIncarnateUsed)
            m_DeathIncarnateAbility.chargeRoutine = StartCoroutine(Ability3Charge());

        else if (value.canceled)
            StopCoroutine(m_DeathIncarnateAbility.chargeRoutine); // When we let go, we stop the couritine to clear the time value in it.

    }

    public void OnDebugToggle(InputAction.CallbackContext value)
    {
        if (value.performed)
            CustomDebugUI.s_Instance.Toggle();
    }

    public void OnHUDToggle(InputAction.CallbackContext value)
    {
        if (value.performed)
        { 
            UIManager.s_Hide = !UIManager.s_Hide;
            m_UIManager?.UpdateWeaponUI(m_Inventory.m_CurrentWeapon);
        }
    }

    // ======================================================================================================================================== //
    private void Look()
    {
        

        float mouseX = LookInput.x * m_LookSensitivity * Time.deltaTime;
        float mouseY = LookInput.y * m_LookSensitivity * Time.deltaTime;

        // Finding current look rotation
        Vector3 rot = m_Orientation.transform.localRotation.eulerAngles;
        /*float*/ m_DesiredX = rot.y + mouseX;

        // Rotate
        m_XRotation -= mouseY;
        m_XRotation = Mathf.Clamp(m_XRotation, -90f, 90f);

        // Perform the rotations
        m_Orientation.transform.localRotation = Quaternion.Euler(0, m_DesiredX - m_AccumulatedRecoil.accumulatedPatternRecoilY, 0);
        m_Camera.transform.localRotation = Quaternion.Euler(Mathf.Clamp((m_XRotation - m_AccumulatedRecoil.accumulatedPatternRecoilX - m_AccumulatedRecoil.accumulatedVisualRecoil.x), -90f, 90f), 0.0f - m_AccumulatedRecoil.accumulatedVisualRecoil.y, 0 - m_AccumulatedRecoil.accumulatedVisualRecoil.z);

    }

    private void Move(Vector2 input)
    {
        // Preserves m_Rigidbody's y velocity.
        Vector3 direction = m_MovInfo.m_CacheMovDirection;
        if (m_MovInfo.m_IsGrounded/* && !m_IsMoving*/)
        {
            //direction.y = 0;
            //direction.y = direction.y;
            Rigidbody.useGravity = false;

            // ========== Idea of this was to negate upwards force properly. Because upwards force isn't always completely vertical ========== //
            //Vector3 velocityTowardsSurface = Vector3.Dot(Rigidbody.velocity, m_GroundNormal) * m_GroundNormal;
            //direction -= velocityTowardsSurface;
            // =============================================================================================================================== //
            //direction.y = direction.y;
            //Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, 0, Rigidbody.velocity.z);
        }
        else if (!m_MovInfo.m_IsGrounded)
        {
            Rigidbody.useGravity = true;

            direction.y = Rigidbody.velocity.y;
        }
        //else
        //{
        //    Rigidbody.useGravity = false;
        //}
        if (m_MovInfo.m_GroundNormal != Vector3.zero && !m_MovInfo.m_IsGrounded && !m_MovInfo.m_HasJumped)
        { 
            Vector3 velocityTowardsSurface = Vector3.Dot(Rigidbody.velocity, m_MovInfo.m_GroundNormal) * m_MovInfo.m_GroundNormal;
            direction -= velocityTowardsSurface;
        }
        m_MovInfo.m_CacheMovDirection = direction;

        // Ensure the slide will never make the player move vertically.
        m_MovInfo.m_CacheSlideMove.y = 0;


        // Making sure angular velocity isn't a problem.
        Rigidbody.velocity = m_MovInfo.m_CacheMovDirection + m_MovInfo.m_CacheSlideMove;
        //Rigidbody.angularVelocity = Vector3.zero;


        // ============================ MODIFIED FALLING ============================ //
        if (Rigidbody.velocity.y < 0)
        {
            Rigidbody.velocity += Vector3.up * Physics.gravity.y * m_JumpFallModifier * Time.deltaTime;
        }
        // ======================================================================== //

        m_MovInfo.m_IsMoving = false;
        if (input.x != 0 || input.y != 0)
            m_MovInfo.m_IsMoving = true;



        Vector3 currentVel = m_MovInfo.m_CacheMovDirection;
        Vector3 desiredVel = CalculateMoveDirection(input.x, input.y, m_MovementSpeed);
        

        Vector3 requiredChange = desiredVel - currentVel;
        m_MovInfo.m_CacheMovDirection += requiredChange * (m_MovInfo.m_IsGrounded ? m_GroundAcceleration : m_AirAcceleration);

        Telemetry.TracePosition("Host-Movement", transform.position, 0.05f, 150);
    }

    private Vector3 CalculateMoveDirection(float x, float z, float speedMultiplier)
    {

        
        m_MovInfo.m_ModifiedForward = Vector3.Cross(m_MovInfo.m_GroundNormal, -m_Orientation.right);
        m_MovInfo.m_ModifiedRight = Vector3.Cross(m_MovInfo.m_GroundNormal, m_Orientation.forward);

        Vector3 xMov;
        Vector3 zMov;

        if (m_MovInfo.m_GroundNormal != Vector3.zero)
        {
            xMov = m_MovInfo.m_ModifiedRight * x;
            zMov = m_MovInfo.m_ModifiedForward * z;
        }
        else
        { 
            xMov = m_Orientation.transform.right * x;
            zMov = m_Orientation.transform.forward * z;
        }


        //xMov.y = 0;
        //zMov.y = 0;

        /*Vector3 */m_MovInfo.m_MoveDirection = ((xMov + zMov).normalized * speedMultiplier * Time.fixedDeltaTime) /*+ Vector3.up * Rigidbody.velocity.y*/; // i don't know why this line of code was there but without it
                                                                                                                                          // it works better.

        return m_MovInfo.m_MoveDirection;
    }

    private bool CheckGrounded()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.SphereCast(ray, m_GroundCheckRadius, out hit, m_GroundCheckDistance))
        {
            //Debug.Log(hit.transform.name);
            //m_GroundNormal = hit.normal;          Moved to its own function.

            if (m_MovInfo.m_JumpCounter >= 0.25f)
            {
                m_MovInfo.m_JumpCounter = 0.0f;
                m_MovInfo.m_HasJumped = false;
            }

            return true;
        }
        //m_GroundNormal = Vector3.zero;
        return false;
    }

    /// <summary>
    /// CalculateGroundNormal() used to just be a line of code in the CheckGrounded() function, but I wanted to be able to check the ground normal before the player is offically "grounded".
    /// </summary>
    /// <returns></returns>
    private void CalculateGroundNormal()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.SphereCast(ray, m_GroundCheckRadius, out hit, m_GroundCheckDistance * 1.04f))
        {
            m_MovInfo.m_GroundNormal = hit.normal;
        }
        else
        {
            m_MovInfo.m_GroundNormal = Vector3.zero;
        }
    }

    /// <summary>
    /// SelectWeapon() parses in an index to the weapon you want to select that is in the Inventory m_Weapons List.
    /// </summary>
    /// <param name="index">The element of the m_Weapons List you want to swap to.</param>
    private void SelectWeapon(int index)
    {
        // Resetting weapon rotation and location.
        m_Inventory.m_CurrentWeapon.ClearThings();

        Weapon cache = m_Inventory.m_CurrentWeapon;
        m_Inventory.m_CurrentWeapon.m_IsAiming = false;
        m_Inventory.m_CurrentWeapon.m_IsFiring = false;
        m_Inventory.m_CurrentWeapon = m_Inventory.m_Weapons[index]; // Swapping to new weapon.

        // Setting them active/inactive to display the correct weapon. Eventually this will be complimented by a weapon swapping phase where it will take some time before
        // the player can shoot after swapping weapons.
        cache.gameObject.SetActive(false);
        m_Inventory.m_CurrentWeapon.gameObject.SetActive(true);

        m_UIManager?.UpdateWeaponUI(m_Inventory.m_CurrentWeapon);
    }

    //private Vector3 WeaponBob()
    //{
    //      // MOVED TO WEAPON.CS SCRIPT.   
    //
    //}

    private void Slide()
    {
		// do slide code.
		Vector3 currentVelocity = m_MovInfo.m_CacheSlideMove;
		Vector3 desiredVelocity = m_MovInfo.m_SlideDir * m_SlideSpeed * Time.deltaTime;

		Vector3 requiredChange = desiredVelocity - currentVelocity;
		m_MovInfo.m_CacheSlideMove += requiredChange * 0.5f;

		if (m_MovInfo.m_IsSliding)
		{
            Debug.Log("Sliding");
			// smoothly rotate backwards. todo
			SmoothMove(m_Camera.transform, new Vector3(0, -0.5f, 0), 0.25f);

			m_MovInfo.m_SlideCounter += Time.deltaTime;
			if (m_MovInfo.m_SlideCounter >= m_SlideDuration) // Slide is complete here.
			{
				m_MovInfo.m_IsSliding = false;
				m_MovInfo.m_SlideCounter = 0.0f;
				m_MovInfo.m_SlideDir = Vector3.zero;
                TransformCapsuleCollider(m_MovInfo.m_OriginalColliderHeight, m_MovInfo.m_OriginalColliderCenter); // Raise the collider back up.
			}
		}
		else
		{
			SmoothMove(m_Camera.transform, new Vector3(0, 0.5f, 0), 0.25f);
		}
	}
    /// <summary>
    /// TransformCollider() is used when sliding. It allows us to shrink the height and centre of the capsule collider. 
    /// </summary>
    /// <param name="newHeight">The new height of the collider</param>
    /// <param name="newCentre">The new centre point of the collider</param>
    private void TransformCapsuleCollider(float newHeight, Vector3 newCentre)
    {
        if (m_Collider)
        {
            m_Collider.center = newCentre;
            m_Collider.height = newHeight;
        }
        else
        {
            Debug.LogWarning("TransformCapsuleCollider() could not find a collider!");
        }

    }

	private void SmoothMove(Transform obj, Vector3 wantedLocalPos, float t)
    {
        Vector3 currentPos = obj.localPosition;
        Vector3 desiredPos = wantedLocalPos;

        Vector3 requiredChange = desiredPos - currentPos;

       
        obj.localPosition += requiredChange * t;
        
    }

    

    //private void Aim()
    //{
    //    // MOVED TO WEAPON.CS SCRIPT.
    //}


    /// <summary>
    /// GetCurrentWeaponConfig() returns the currently held weapons WeaponConfiguration script. It is public because the CustomDebugUI script needs to access it.
    /// </summary>
    /// <returns></returns>
    public WeaponConfiguration GetCurrentWeaponConfig()
    {
        // I know it's bad to use GetComponent() during runtime, but for now this does the job.
        // An alternative I though of but am unsure if is good practice would be for the Inventory.cs
        // script to have another list that compliments the m_Weapons list. This new list would match
        // each element in the m_Weapons list and store the corresponding weapons WeaponConfiguration script.

        return m_Inventory.m_CurrentWeapon.m_Config;
    }

    private Transform GetCurrentWeaponTransform() => m_Inventory.m_CurrentWeapon.transform;
    private Vector3 GetCurrentWeaponOriginalPos() => m_Inventory.m_CurrentWeapon.m_OriginalLocalPosition;
    private Vector3 GetCurrentWeaponOriginalGlobalPos() => m_Inventory.m_CurrentWeapon.m_OriginalGlobalPosition;
    public Weapon GetCurrentWeapon() => m_Inventory.m_CurrentWeapon;


    

    public void HideHUD()
    {
        m_HUD.SetActive(false);
    }

    public void UnhideHUD()
    {
        m_HUD.SetActive(true);
    }

	// remember cooldown.
	// this host dies. you get kicked out.
	// remove life essence.
	// maybe freeze player or slow them down while performing.
	// telegraph/delay at start. small timer before it actually performs.
	private void Ability3(float radius, int damage)
    {
        Collider[] collisions = Physics.OverlapSphere(m_Orientation.position, radius); // Using m_Orientation.position to be at the centre of the model.

        for (int i = 0; i < collisions.Length; i++) 
        {

            Inventory agentInv = collisions[i].GetComponent<Inventory>();
            if (agentInv) // If they had an inventory, it means they are an agent.
                agentInv.TakeDamage(damage);
            
        }

        StartCoroutine(Ability3Draw()); // Start timer for drawing.

        // Storing position of time of attack.
        m_DeathIncarnateAbility.deathIncarnatePos = m_Orientation.position;

        m_DeathIncarnateAbility.deathIncarnateUsed = true;
        StartCoroutine(Ability3Refresh());
    }

    private void Ability3Gizmo()
    {
		Color cache = Gizmos.color;
		Gizmos.color = Color.blue;

		Gizmos.DrawWireSphere(m_DeathIncarnateAbility.deathIncarnatePos, m_DeathIncarnateAbility.deathIncarnateRadius);

		Gizmos.color = cache;
	}

    IEnumerator Ability3Refresh()
    {
        float time = 0;
        while (time < m_DeathIncarnateAbility.deathIncarnateCooldown)
        {
            time += Time.deltaTime;
            yield return null;
        }

        m_DeathIncarnateAbility.deathIncarnateUsed = false;
    }
	IEnumerator Ability3Charge()
	{
		float time = 0.0f;

		while (time < m_DeathIncarnateAbility.deathIncarnateRequiredHold)
		{
			time += Time.deltaTime;


			Debug.Log(time);

			yield return null;
		}

		// This means the time has now passed the required held time.
		// We can now activate Ability3.
		StartCoroutine(Ability3Delay());
		Debug.Log("Ability3 delay started.");

		
	}

	IEnumerator Ability3Delay()
	{
		float time = 0.0f;
		while (time < m_DeathIncarnateAbility.deathIncarnateDelay)
		{
			time += Time.deltaTime;
			yield return null;
		}

		Ability3(m_DeathIncarnateAbility.deathIncarnateRadius, m_DeathIncarnateAbility.deathIncarnateDamage);
	}
	IEnumerator Ability3Draw()
	{
		float time = 0.0f;
		m_DeathIncarnateAbility.drawingDeathIncarnate = true;
		while (time < 10)
		{
			// We'll draw death incarnate for 3 seconds after it was used.

			Debug.Log("Drawing: at " + m_DeathIncarnateAbility.deathIncarnatePos + " at " + time);

			time += Time.deltaTime;
			yield return null;
		}

		m_DeathIncarnateAbility.drawingDeathIncarnate = false;
	}

    

    private void OnDrawGizmos()
    {
        if (!m_EnableDebug || !m_Active) return;

        Color defaultColour = Gizmos.color;

        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.SphereCast(ray, m_GroundCheckRadius, out hit, m_GroundCheckDistance))
        {
            Gizmos.DrawLine(transform.position, hit.point);

            GraphicalDebugger.DrawSphereCast(transform.position + Vector3.up, (transform.position + Vector3.up) + Vector3.down * m_GroundCheckDistance, Color.green, m_GroundCheckRadius, m_GroundCheckDistance);

            // Draw forward direction but relative to ground normal.
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.Cross(m_MovInfo.m_GroundNormal, -m_Orientation.right) * 100));
        }
        else
        {
            GraphicalDebugger.DrawSphereCast(transform.position + Vector3.up, (transform.position + Vector3.up) + Vector3.down * m_GroundCheckDistance, Color.red, m_GroundCheckRadius, m_GroundCheckDistance);
        }

        Gizmos.color = defaultColour;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + new Vector3(-m_MovInfo.m_CacheMovDirection.x, m_MovInfo.m_CacheMovDirection.y, -m_MovInfo.m_CacheMovDirection.z));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + m_MovInfo.m_CacheMovDirection);

        // Trying to visualise true movement forward vector.
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + m_MovInfo.m_ModifiedForward);



        //Vector3 centre = m_Camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, transform.forward.z));
        //Gizmos.DrawSphere(centre, 0.25f);
        //Gizmos.DrawSphere(m_Gun.position, 0.25f);




        Color cache = Gizmos.color;
        // ================= Camera Forward Vectors For Recoil Recovery ================= //
        Vector2 modifiedCurrent = new Vector2(m_Camera.transform.forward.y, 1);
        Vector2 modifiedPrevious = new Vector2(m_CombatInfo.m_PrevCamForward.y, 1);

        // Debug Lines:
        // When the dot product is close to 1, the two lines will be GREEN.
        // When the current forward vector is below the previous forward vector, the two lines will be PURPLE.
        // When the current forward vector is above the previous forward vector, the two lines will be YELLOW.

        float dot = Vector2.Dot(modifiedCurrent.normalized, modifiedPrevious.normalized);

        if (dot < 0.9999f)
        {
            if (m_CombatInfo.m_PrevCamForward.y > m_CombatInfo.m_camForward.y)
                Gizmos.color = Color.magenta;
            else
                Gizmos.color = Color.yellow;
        }
        else
            Gizmos.color = Color.green;

        // Trying to create the same forward vectors but only caring about x and z.

        Gizmos.DrawLine(m_Camera.transform.position, m_Camera.transform.position + m_Camera.transform.forward * 100);  // Current forward vector.
        Gizmos.DrawLine(m_Camera.transform.position, m_Camera.transform.position + m_CombatInfo.m_PrevCamForward * 100);    // Forward vector when they first clicked the fire trigger.

        Gizmos.color = cache;

        // ============================================================================== //





        // Trying to fix dash bug.
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.25f);


        // Drawing Ability3 stuff.
        if (m_DeathIncarnateAbility.drawingDeathIncarnate)
            Ability3Gizmo();
    }
}
