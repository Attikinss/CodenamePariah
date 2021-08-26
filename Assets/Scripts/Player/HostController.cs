using System.Collections;
using System.Collections.Generic;
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

    
    [Header("Other References")]
    [SerializeField]
    private bool m_EnableDebug = false;
    public Transform m_Orientation;
    public Inventory m_Inventory;
    public Rigidbody Rigidbody { get; private set; }
    
    
  

    // ================== BOOKKEEPING STUFF ================== //

    public Vector2 MovementInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public Vector3 CacheMovDir { get; private set; }

    private float m_FireCounter = 0.0f; // time counter between shots.
    private bool m_HasFired = false;

    // If this variable was once public and you had set it's value in the inspector, it will still have the value you set in the inspector even if you change its initialization here.
    public float ShootingDuration { get; set; } = 1; // time tracking since started shooting.
    
    public float m_XRotation = 0;   // Made public because nowadays the Weapon.cs script needs to access it.

    private bool m_HasDoubleJumped = false;

    private bool m_HasJumped = false;

    public bool IsSliding { get; private set; }
    public Vector3 SlideDir { get; private set; }

    private Vector3 m_CacheSlideMove = Vector3.zero;

    public float SlideCounter { get; private set; }

    public Vector3 LookInput { get; private set; }

    [HideInInspector]
    public bool m_IsAiming = false;

    public Vector3 AdditionalRecoilRotation { get; set; } // Made the setter public so that I can access it in the Weapon.cs script.

    //public Vector3 WeaponRecoilRot { get; private set; }

    public float AdditionalCameraRecoilX { get; set; } // For actual recoil pattern. This will judge how much higher your camera will go while shooting.

    public float AdditionalCameraRecoilY { get; set; } // This will be how much horizontal recoil will be applied to the camera.

    public float m_DesiredX = 0; // Made public because I'm moving everything to the Weapon.cs script but I still need to access it there.

    private bool m_IsFiring = false;

    public Vector3 PreviousCameraRotation { get; private set; } // Stores rotation when the player just starts shooting. Okay, so because comparing euler angles is a terrible idea due to there being multiple numbers
                                                                // that can describe the same thing, this variable now stores the forward vector before shooting. The idea being that I can use the dot product to
                                                                // compare the difference in angle between the old forward vector and the new forward vector.

    public Vector3 CurrentCamRot { get; private set; }          // Like I mentioned above, this variable will be storing the current forward vector to be used when recovering from recoil.
    // ======================================================= //


    // Exposed variables for debugging.
    [ReadOnly]
    public float m_CurrentMoveSpeed;
    [ReadOnly]
    public bool m_IsMoving;

    


    // Temporary ground normal thing.
    Vector3 m_GroundNormal = Vector3.zero;

    Vector3 m_ModifiedRight = Vector3.zero;
    Vector3 m_ModifiedForward = Vector3.zero;
    Vector3 moveDir = Vector3.zero;

    // temporary jump deactivate cooldown. to prevent m_IsJumping from being deactivated as soon as you jump.
    float m_JumpCounter = 0;



    // Temporary host drain ability stuff.
    // These are integers because the Inventory.cs script has health stored as an integer.
    public int m_DrainDamage = 10;         // By setting them to the same value, its a 1:1 ratio of drain/restoration.
    public int m_DrainRestore = 10;
    [Range(0,2)]
    public float m_DrainInterval = 0.15f;

    private float m_DrainCounter = 0.0f;
    private bool m_IsDraining = false;

    
    [HideInInspector]
    public Vector3 m_PreviousOrientationVector = Vector3.zero;
    [HideInInspector]
    public float m_PreviousXCameraRot = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (m_Active)
            Look();
    }

    private void Update()
    {
        if (!m_Active) return;

        if (GetCurrentWeaponConfig().m_AlwaysADS) // The reason why I do this is so I don't have to check for the m_AlwaysADS bool everywhere. This way I can still just check for m_IsAiming in all functions.
            m_IsAiming = true;
        if (GetCurrentWeaponConfig().m_AlwaysFiring) // Doing same here for the reason above.
            m_IsFiring = true;


        // This is now taken care of by Lauchlan's weapon system.

        //if (m_HasFired)
        //{
        //    m_FireCounter += Time.deltaTime;
        //    //m_fireCounter = Time.time + (60.0f / GetCurrentWeaponConfig().m_FireRate);
        //    if (m_FireCounter >= 60.0f / GetCurrentWeaponConfig().m_FireRate)
        //    {
        //        m_HasFired = false;
        //        m_FireCounter = 0;
        //    }
        //}

        //if (m_IsFiring)
        //{
        //    // They are holding down the fire button.
        //    ShootingDuration += Time.deltaTime;
        //}
        //else
        //{
            
        //    // RECOIL RECOVERY STUFF MOVED TO WEAPON.CS SCRIPT.
            

        //}

        IsGrounded = CheckGrounded();
        CalculateGroundNormal();
        if (IsGrounded)
        { 
            m_HasDoubleJumped = false;
        }

        m_CurrentMoveSpeed = Rigidbody.velocity.magnitude;


        // MOVED TO WEAPON.CS SCRIPT.

        //if (m_IsAiming)
        //    Aim();
        //UpdateSway(LookInput.x, LookInput.y);



        //if (m_IsFiring)                       // CURRENTLY MOVING FUNCTIONALITY TO THE WEAPON.CS SCRIPT.
        //    Shoot(true);

        // Just for debugging purposes. This variable is only used in the CustomDebugUI script.
        CurrentCamRot = m_Camera.transform.forward;


        // ================ NOTE ================ //
        // It's really weird and bad to have a counter like this. I'll try find a way around it but
        // for now it's helping fix an issue with walking up ramps and jumping.
        // ====================================== //

        if (m_HasJumped)
        {
            m_JumpCounter += Time.deltaTime; // how to get around having a timer for something like this?
        }


        // While draining, the player is not allowed to interact with their weapons.
        // ;-;
        if (m_IsDraining)
        {
            if (m_Inventory.GetHealth() > 0)
            {
                // timed event. have adjustable drain speed.
                m_DrainCounter += Time.deltaTime;
                if (m_DrainCounter >= m_DrainInterval)
                {
                    m_DrainCounter = 0.0f;
                    m_Inventory.TakeDamage(m_DrainDamage);
                }

            }
            else
            { 
                // TODO 
                // Kill host if health less than 0.
                // eject Pariah at the same time damage Pariah.
            }
        }


        
    }

    private void FixedUpdate()
	{
        if (!m_Active) return;

        Slide();
        Move(MovementInput);
        //UpdateRecoil();           // CURRENTLY MOVING TO WEAPON.CS SCRIPT.
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
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.Cross(m_GroundNormal, -m_Orientation.right) * 100));
        }
        else
        {
            GraphicalDebugger.DrawSphereCast(transform.position + Vector3.up, (transform.position + Vector3.up) + Vector3.down * m_GroundCheckDistance, Color.red, m_GroundCheckRadius, m_GroundCheckDistance);
        }

        Gizmos.color = defaultColour;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + new Vector3(-CacheMovDir.x, CacheMovDir.y, -CacheMovDir.z));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + CacheMovDir);

        // Trying to visualise true movement forward vector.
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + m_ModifiedForward);



        //Vector3 centre = m_Camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, transform.forward.z));
        //Gizmos.DrawSphere(centre, 0.25f);
        //Gizmos.DrawSphere(m_Gun.position, 0.25f);




        Color cache = Gizmos.color;
        // ================= Camera Forward Vectors For Recoil Recovery ================= //
        Vector2 modifiedCurrent = new Vector2(m_Camera.transform.forward.y, 1);
        Vector2 modifiedPrevious = new Vector2(PreviousCameraRotation.y, 1);

        // Debug Lines:
        // When the dot product is close to 1, the two lines will be GREEN.
        // When the current forward vector is below the previous forward vector, the two lines will be PURPLE.
        // When the current forward vector is above the previous forward vector, the two lines will be YELLOW.

        float dot = Vector2.Dot(modifiedCurrent.normalized, modifiedPrevious.normalized);

        if (dot < 0.9999f)
        {
            if (PreviousCameraRotation.y > CurrentCamRot.y)
                Gizmos.color = Color.magenta;
            else
                Gizmos.color = Color.yellow;
        }
        else
            Gizmos.color = Color.green;

        // Trying to create the same forward vectors but only caring about x and z.

        Gizmos.DrawLine(m_Camera.transform.position, m_Camera.transform.position + m_Camera.transform.forward * 100);  // Current forward vector.
        Gizmos.DrawLine(m_Camera.transform.position, m_Camera.transform.position + PreviousCameraRotation * 100);    // Forward vector when they first clicked the fire trigger.

        Gizmos.color = cache;

        // ============================================================================== //
    }

    public override void Enable()
    {
        GetComponent<PlayerInput>().enabled = true;
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
        if (GetCurrentWeapon().m_IsRecoilTesting)
            return; // early out to prevent mouse movement while testing recoil.
        LookInput = value.ReadValue<Vector2>();
	}

    public override void OnJump(InputAction.CallbackContext context)
    {
        bool active = context.performed;

        if (active)
        {
            m_HasJumped = true;


            Vector3 direction = Vector3.up * ControllerMaths.CalculateJumpForce(m_JumpHeight, Rigidbody.mass, m_Gravity);
            direction.x = Rigidbody.velocity.x;
            direction.z = Rigidbody.velocity.z;

            if (IsGrounded)
            {
                CacheMovDir = direction;
                Rigidbody.velocity = direction;
            }
            else if (!IsGrounded && !m_HasDoubleJumped)
            {
                CacheMovDir = direction;
                Rigidbody.velocity = direction;

                // Have to tick m_HasDoubleJumped to false;
                m_HasDoubleJumped = true;
            }
        }
    }

    public override void OnSlide(InputAction.CallbackContext value)
    {
        if (value.performed && IsGrounded && m_IsMoving)
        {
            Debug.Log("OnSlide called.");
            SlideDir = value.performed ? m_Orientation.forward : SlideDir;
            IsSliding = true;
        }
    }

	public override void OnMovement(InputAction.CallbackContext value)
	{
        MovementInput = value.performed ? value.ReadValue<Vector2>() : Vector2.zero;
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
            PreviousCameraRotation = m_Camera.transform.forward;
            GetCurrentWeapon().SetFireTime();

        }
        else
        {
            GetCurrentWeapon().m_IsFiring = false;

            // Reset held counter happens regardless.
            ShootingDuration = 0.0f;
        }
    }

    public void OnWeaponSelect1(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            SelectWeapon(0);

            // Previously I was tracking weapon states in PlayerManager in an attempt to free up space in this controller script. However, now that we have an Inventory script that tracks weapons and
            // the players current weapon, I'll leave that stuff in there.
            //PlayerManager.SetWeapon(WeaponSlot.WEAPON1);
        }
    }

    public void OnWeaponSelect2(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            SelectWeapon(1);
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
            m_IsDraining = true;
        }
        else if (value.canceled)
        {
            m_IsDraining = false;
        }
    }

    public void OnReload(InputAction.CallbackContext value)
    { 
        
    }


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
        m_Orientation.transform.localRotation = Quaternion.Euler(0, m_DesiredX - AdditionalCameraRecoilY, 0);
        m_Camera.transform.localRotation = Quaternion.Euler(Mathf.Clamp((m_XRotation - AdditionalCameraRecoilX - AdditionalRecoilRotation.x), -90f, 90f), 0.0f - AdditionalRecoilRotation.y, 0 - AdditionalRecoilRotation.z);

    }

    private void Move(Vector2 input)
    {
        // Preserves m_Rigidbody's y velocity.
        Vector3 direction = CacheMovDir;
        if (IsGrounded/* && !m_IsMoving*/)
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
        else if (!IsGrounded)
        {
            Rigidbody.useGravity = true;

            direction.y = Rigidbody.velocity.y;
        }
        //else
        //{
        //    Rigidbody.useGravity = false;
        //}
        if (m_GroundNormal != Vector3.zero && !IsGrounded && !m_HasJumped)
        { 
            Vector3 velocityTowardsSurface = Vector3.Dot(Rigidbody.velocity, m_GroundNormal) * m_GroundNormal;
            direction -= velocityTowardsSurface;
        }
        CacheMovDir = direction;

        // Ensure the slide will never make the player move vertically.
        m_CacheSlideMove.y = 0;


        // Making sure angular velocity isn't a problem.
        Rigidbody.velocity = CacheMovDir + m_CacheSlideMove;
        //Rigidbody.angularVelocity = Vector3.zero;


        // ============================ MODIFIED FALLING ============================ //
        if (Rigidbody.velocity.y < 0)
        {
            Rigidbody.velocity += Vector3.up * Physics.gravity.y * m_JumpFallModifier * Time.deltaTime;
        }
        // ======================================================================== //

        m_IsMoving = false;
        if (input.x != 0 || input.y != 0)
            m_IsMoving = true;



        Vector3 currentVel = CacheMovDir;
        Vector3 desiredVel = CalculateMoveDirection(input.x, input.y, m_MovementSpeed);
        

        Vector3 requiredChange = desiredVel - currentVel;
        CacheMovDir += requiredChange * (IsGrounded ? m_GroundAcceleration : m_AirAcceleration);
    }

    private Vector3 CalculateMoveDirection(float x, float z, float speedMultiplier)
    {

        
        m_ModifiedForward = Vector3.Cross(m_GroundNormal, -m_Orientation.right);
        m_ModifiedRight = Vector3.Cross(m_GroundNormal, m_Orientation.forward);

        Vector3 xMov;
        Vector3 zMov;

        if (m_GroundNormal != Vector3.zero)
        {
            xMov = m_ModifiedRight * x;
            zMov = m_ModifiedForward * z;
        }
        else
        { 
            xMov = m_Orientation.transform.right * x;
            zMov = m_Orientation.transform.forward * z;
        }


        //xMov.y = 0;
        //zMov.y = 0;

        /*Vector3 */moveDir = ((xMov + zMov).normalized * speedMultiplier * Time.fixedDeltaTime) /*+ Vector3.up * Rigidbody.velocity.y*/; // i don't know why this line of code was there but without it
                                                                                                                                          // it works better.

        return moveDir;
    }

    private bool CheckGrounded()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.SphereCast(ray, m_GroundCheckRadius, out hit, m_GroundCheckDistance))
        {
            //Debug.Log(hit.transform.name);
            //m_GroundNormal = hit.normal;          Moved to its own function.

            if (m_JumpCounter >= 0.25f)
            {
                m_JumpCounter = 0.0f;
                m_HasJumped = false;
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
            m_GroundNormal = hit.normal;
        }
        else
        {
            m_GroundNormal = Vector3.zero;
        }
    }

    /// <summary>
    /// SelectWeapon() parses in an index to the weapon you want to select that is in the Inventory m_Weapons List.
    /// </summary>
    /// <param name="index">The element of the m_Weapons List you want to swap to.</param>
    private void SelectWeapon(int index)
    {
        Weapon cache = m_Inventory.m_CurrentWeapon;
        m_Inventory.m_CurrentWeapon.m_IsAiming = false;
        m_Inventory.m_CurrentWeapon.m_IsFiring = false;
        m_Inventory.m_CurrentWeapon = m_Inventory.m_Weapons[index];

        // Setting them active/inactive to display the correct weapon. Eventually this will be complimented by a weapon swapping phase where it will take some time before
        // the player can shoot after swapping weapons.
        cache.gameObject.SetActive(false);
        m_Inventory.m_CurrentWeapon.gameObject.SetActive(true);
    }

    //private Vector3 WeaponBob()
    //{
    //      // MOVED TO WEAPON.CS SCRIPT.   
    //
    //}

    private void Slide()
    {
		// do slide code.
		Vector3 currentVelocity = m_CacheSlideMove;
		Vector3 desiredVelocity = SlideDir * m_SlideSpeed * Time.deltaTime;

		Vector3 requiredChange = desiredVelocity - currentVelocity;
		m_CacheSlideMove += requiredChange * 0.5f;

		if (IsSliding)
		{
            Debug.Log("Sliding");
			// smoothly rotate backwards. todo
			SmoothMove(m_Camera.transform, new Vector3(0, -0.5f, 0), 0.25f);

			SlideCounter += Time.deltaTime;
			if (SlideCounter >= m_SlideDuration)
			{
				IsSliding = false;
				SlideCounter = 0.0f;
				SlideDir = Vector3.zero;
			}
		}
		else
		{
			SmoothMove(m_Camera.transform, new Vector3(0, 0.5f, 0), 0.25f);
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

        return m_Inventory.m_CurrentWeapon.gameObject.GetComponent<WeaponConfiguration>();
    }

    private Transform GetCurrentWeaponTransform() => m_Inventory.m_CurrentWeapon.transform;
    private Vector3 GetCurrentWeaponOriginalPos() => m_Inventory.m_CurrentWeapon.m_OriginalLocalPosition;
    private Vector3 GetCurrentWeaponOriginalGlobalPos() => m_Inventory.m_CurrentWeapon.m_OriginalGlobalPosition;
    public Weapon GetCurrentWeapon() => m_Inventory.m_CurrentWeapon;


    public void OnTestRecoil(InputAction.CallbackContext value)
    {
        if (value.performed && !GetCurrentWeapon().m_IsRecoilTesting)
        {
            GetCurrentWeapon().m_IsRecoilTesting = true;
            m_PreviousOrientationVector = m_Orientation.transform.eulerAngles;
            m_PreviousXCameraRot = m_XRotation;

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
}
