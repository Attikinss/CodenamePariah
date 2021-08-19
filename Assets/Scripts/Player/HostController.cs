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

    [Header("Old Bobbing Controls")]
    public float m_BobSpeed = 1;
    public float m_BobDistance = 1;

    [Header("Other References")]
    [SerializeField]
    private bool m_EnableDebug = false;
    public Transform m_Orientation;
    public Inventory m_Inventory;
    public Rigidbody Rigidbody { get; private set; }
    
    public Transform m_Weapon1;
    public Transform m_Weapon2;

    // These are references to the weapons that the controller has.
    public WeaponConfiguration m_WeaponConfig1;
    public WeaponConfiguration m_WeaponConfig2;
  

    // ================== BOOKKEEPING STUFF ================== //

    public Vector2 MovementInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public Vector3 CacheMovDir { get; private set; }

    private float m_FireCounter = 0.0f; // time counter between shots.
    private bool m_HasFired = false;

    // If this variable was once public and you had set it's value in the inspector, it will still have the value you set in the inspector even if you change its initialization here.
    public float ShootingDuration { get; private set; } = 1; // time tracking since started shooting.
    
    private float m_XRotation = 0;

    private bool m_HasDoubleJumped = false;

    private bool m_HasJumped = false;

    public bool IsSliding { get; private set; }
    public Vector3 SlideDir { get; private set; }

    private Vector3 m_CacheSlideMove = Vector3.zero;

    public float SlideCounter { get; private set; }

    public Vector3 LookInput { get; private set; }

    [HideInInspector]
    public bool m_IsAiming = false;

    public Vector3 AdditionalRecoilRotation { get; private set; }

    public Vector3 WeaponRecoilRot { get; private set; }

    public float AdditionalCameraRecoilX { get; private set; } // For actual recoil pattern. This will judge how much higher your camera will go while shooting.

    public float AdditionalCameraRecoilY { get; private set; } // This will be how much horizontal recoil will be applied to the camera.

    private float m_DesiredX = 0;

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

    // ========================== TEMPORARY WEAPON BOBBING ========================== //
    // This weapon sway stuff is here for now since we haven't got animations in yet.
    // It will be replaced soon.

    [HideInInspector]
    public float m_SwayTimer = 0.0f;
    [HideInInspector]
    public float m_WaveSlice = 0.0f;
    [HideInInspector]
    public float m_WaveSliceX = 0.0f;
    // ========================================================================== //


    // Temporary ground normal thing.
    Vector3 m_GroundNormal = Vector3.zero;

    Vector3 m_ModifiedRight = Vector3.zero;
    Vector3 m_ModifiedForward = Vector3.zero;
    Vector3 moveDir = Vector3.zero;

    // temporary jump deactivate cooldown. to prevent m_IsJumping from being deactivated as soon as you jump.
    float m_JumpCounter = 0;



    // ================== TEMPORARY RECOIL TESTING ================== //
    [Header("Recoil Testing")]
    public float m_RecoilTestIntervals = 3.0f;
    public float m_RecoilTestRestTime = 2.0f;

    private bool m_IsRecoilTesting = false;
    private bool m_IsTestResting = false;
    private float m_RecoilTestCounter = 0;

    Vector3 m_PreviousOrientationVector = Vector3.zero;
    float m_PreviousXCameraRot = 0;

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




        //if (IsGrounded)
        //{
        //    Vector3 cache = CacheMovDir;
        //    cache.y = 0;
        //    CacheMovDir = cache;
        //}





        if (m_HasFired)
        {
            m_FireCounter += Time.deltaTime;
            //m_fireCounter = Time.time + (60.0f / GetCurrentWeaponConfig().m_FireRate);
            if (m_FireCounter >= 60.0f / GetCurrentWeaponConfig().m_FireRate)
            {
                m_HasFired = false;
                m_FireCounter = 0;
            }
        }

        if (m_IsFiring)
        {
            // They are holding down the fire button.
            ShootingDuration += Time.deltaTime;
        }
        else
        {
            if (AdditionalCameraRecoilX > 0) // We only want to decrement AdditionCameraRecoilX if it has accumuluated recoil still in it.
            { 
                // If we just keep decreasing the additional recoil until it reaches 0, it results in the camera going further down then what feels right.
                // This is because as the player is shooting, they are compensating and making the gun stand in place. While this is happening, the additional recoil could
                // build up to a high number and when the player stops shooting, the recoil will take a long time to get back to 0.

                // An experimental method I'd like to try is to either decrease it back to 0, or until the camera rotation is back to where it when they just started shooting.
                Vector2 currentCamX = new Vector2(CurrentCamRot.y, 1);
                Vector2 previousCamX = new Vector2(PreviousCameraRotation.y, 1);          // I know I'm using the new keyword here and that's bad. But for now I'm trying to see if this system will work.
                float dot = Vector3.Dot(currentCamX.normalized, previousCamX.normalized);
                if (dot < 0.9999f || dot > 1.0001f) // Such a small difference in numbers still gives quite a generous margin for error.
                {
                    // This means the current forward vector's y does not much the previous forward vector's y.
                    // We have to do one of two things.
                    // Either bring the gun down, so that the previous and current y components match.
                    // Or if the gun is already below the previous y component, we just leave the gun alone because they've over compensated for the recoil.

                    // If previous rotation's y is greater, it means they are looking further down then when they started firing.
                    if (PreviousCameraRotation.y > CurrentCamRot.y)
                    {
                        // We want to incorporate the additional camera recoil into the rotation of the camera, that way we can set the variable to 0 without worrying that later we will be moving the camera downwards.

                        // Because I'm setting the local rotation of the camera in the Look() function, it makes it kind of annoying to try and add/remove things to and from the rotation.
                        // Instead I will add the AdditionalCameraRecoilX into xRotation and then set AdditionalCameraRecoilX to 0. This way I don't have to directly touch the cameras local rotation
                        // here.

                        m_XRotation -= AdditionalCameraRecoilX;
                        AdditionalCameraRecoilX = 0;
                    }
                    else
                    {
                        // Otherwise, they are aiming higher than when they started, so we'll bring the gun down to where it was.
                        AdditionalCameraRecoilX -= 1 * GetCurrentWeaponConfig().m_RecoilRecoveryModifier;
                        AdditionalCameraRecoilX = Mathf.Clamp(AdditionalCameraRecoilX, 0, 85f);
                    }

                }
                else
                {
                    // Since the forward vectors match, we'll clear the m_AdditionalCameraRecoilX variable just to keep things clean.
                    m_XRotation -= AdditionalCameraRecoilX;
                    AdditionalCameraRecoilX = 0;
                }
            }
            if (AdditionalCameraRecoilY != 0)
            {
                // I've decided not to lerp the additional horizontal recoil to 0 since it feels disorientating.
                
                

                // If we have accumulated horizontal recoil.
                //m_AdditionalCameraRecoilY -= 1 * GetCurrentWeaponConfig().m_RecoilRecoveryModifier;
                
                m_DesiredX -= AdditionalCameraRecoilY;
                AdditionalCameraRecoilY = 0;
            }
            

        }

        IsGrounded = CheckGrounded();
        CalculateGroundNormal();
        if (IsGrounded)
        { 
            m_HasDoubleJumped = false;
        }

        m_CurrentMoveSpeed = Rigidbody.velocity.magnitude;


        if (m_IsAiming)
            Aim();
        UpdateSway(LookInput.x, LookInput.y);

        if (m_IsFiring)
            Shoot(true);

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


        // ============== EXPERIMENTAL RECOIL TESTING STUFF ============== //
        if (m_IsRecoilTesting)
        {
            if (!m_IsTestResting)
                m_IsFiring = true;
            else
                m_IsFiring = false;


            m_RecoilTestCounter += Time.deltaTime;
            if (!m_IsTestResting && m_RecoilTestCounter >= m_RecoilTestIntervals)
            {
                m_IsTestResting = true;
                ShootingDuration = 0; // This sequence of firing should be cancelled. It normally gets cancelled on mouse button up after firing.
                m_RecoilTestCounter = 0.0f;
            }
            else if(m_IsTestResting && m_RecoilTestCounter >= m_RecoilTestRestTime) // This means were now counting the rest time.
            {
                m_IsTestResting = false;
                m_RecoilTestCounter = 0.0f;
                m_XRotation = m_PreviousXCameraRot; // this might be unnessessary since the guns camera rotation goes back down through the recoil recovery system.
                m_Orientation.transform.eulerAngles = m_PreviousOrientationVector;
            }
        }
    }

    private void FixedUpdate()
	{
        if (!m_Active) return;

        Slide();
        Move(MovementInput);
        UpdateRecoil();
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

        //// Input reset
        //MovementInput = Vector2.zero;
        //LookInput = Vector3.zero;
        //
        //IsGrounded = true;
        //HeldCounter = 0;
        //xRotation = 0;
        //
        //AdditionalCameraRecoilX = 0.0f;
        //AdditionalCameraRecoilY = 0.0f;
        //desiredX = 0.0f;
        //m_IsFiring = false;
        //m_IsAiming = false;
        //m_HasFired = false;
        //m_HasDoubleJumped = false;
        //m_IsMoving = false;
        //m_FireCounter = 0.0f;
        //m_CurrentMoveSpeed = 0.0f;
        //
        //m_SwayTimer = 0.0f;
        //m_WaveSlice = 0.0f;
        //m_WaveSliceX = 0.0f;
        //
        //// Slide reset
        //Sliding = false;
        //SlideDir = Vector3.zero;
        //SlideCounter = 0;
        //m_CacheSlideMove = Vector3.zero;
    }

    public override void OnLook(InputAction.CallbackContext value)
	{
        if (m_IsRecoilTesting)
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

        //if (active && IsGrounded)
        //{
        //    CacheMovDir = Vector3.up * ControllerMaths.CalculateJumpForce(m_JumpHeight, Rigidbody.mass, m_Gravity);
        //    CacheMovDir.x = Rigidbody.velocity.x;
        //    CacheMovDir.z = Rigidbody.velocity.z;
        //    Rigidbody.velocity = CacheMovDir;
        //}
        //else if ((active && !IsGrounded) && !m_HasDoubleJumped)
        //{
        //    CacheMovDir = Vector3.up * ControllerMaths.CalculateJumpForce(m_SecondJumpHeight, Rigidbody.mass, m_Gravity);
        //    CacheMovDir.x = Rigidbody.velocity.x;
        //    CacheMovDir.z = Rigidbody.velocity.z;
        //    Rigidbody.velocity = CacheMovDir;
        //
        //    // Have to tick m_HasDoubleJumped to false;
        //    m_HasDoubleJumped = true;
        //}
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
            m_IsFiring = true;
            Debug.Log("OnShoot called.");

            // Experimental thing I'm trying.
            // I will store the original camera rotation when they first start shooting that way I can go back to this rotation when they recover from recoil.
            PreviousCameraRotation = m_Camera.transform.forward;

        }
        else
        {
            m_IsFiring = false;
            Debug.Log("OnShoot cancelled.");
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
            m_IsAiming = true;
        }
        else if (context.canceled)
        {
            m_IsAiming = false;
        }
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

            Debug.Log("IsGrounded && !m_IsMoving");
            //direction.y = direction.y;
            //Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, 0, Rigidbody.velocity.z);
        }
        else if (!IsGrounded)
        {
            Rigidbody.useGravity = true;

            direction.y = Rigidbody.velocity.y;
            Debug.Log("Else");
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

    private void Shoot(bool active)
    {
        if (active && !m_HasFired)
        {

            Ray ray = new Ray(m_Camera.transform.position, m_Camera.transform.forward);
            RaycastHit hit;

            WeaponConfiguration currentConfig = GetCurrentWeaponConfig();
            // =========== TESTING =========== //
            if (!currentConfig.m_DisableAllRecoil)
            { 
                AdditionalCameraRecoilX += currentConfig.m_VerticalRecoil.Evaluate(ShootingDuration);
                AdditionalCameraRecoilY += currentConfig.m_HorizontalRecoil.Evaluate(ShootingDuration);
            }
            // =============================== //

            m_HasFired = true;

            Recoil();


            // ========================= TEMPORARY SHOOT COLLISION ========================= //

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject != null)
                {
                    //Decal newDecal = new Decal(hit.transform, hit.point, hit.normal);         // No longer need this now with the all new Object Pooling Decals! - daniel
                    GameManager.Instance?.AddDecal(hit.transform, hit.point, hit.normal);
            
            
                    // Adding a force to the hit object.
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(m_Camera.transform.forward * GetCurrentWeaponConfig().m_BulletForce, ForceMode.Impulse);
                    }
                }
            }
            // ============================================================================= //
        }  
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
        m_Inventory.m_CurrentWeapon = m_Inventory.m_Weapons[index];

        // Setting them active/inactive to display the correct weapon. Eventually this will be complimented by a weapon swapping phase where it will take some time before
        // the player can shoot after swapping weapons.
        cache.gameObject.SetActive(false);
        m_Inventory.m_CurrentWeapon.gameObject.SetActive(true);
    }

    private Vector3 WeaponBob()
    {
        //Weapon currentWeapon = PlayerManager.GetCurrentWeapon();
        Vector3 localPosition = GetCurrentWeapon().transform.position;
        Vector3 currentWeaponMidPoint = GetCurrentWeaponOriginalPos();

        if (m_IsMoving)
        {
            // Do weapon sway stuff.
            m_SwayTimer += Time.deltaTime;
            m_WaveSlice = -(Mathf.Sin(m_SwayTimer * m_BobSpeed) + 1) / 2;
            m_WaveSliceX = Mathf.Cos(m_SwayTimer * m_BobSpeed);

            if (m_WaveSlice >= -0.5f)
            {
                m_WaveSlice = -1 - -(Mathf.Sin(m_SwayTimer * m_BobSpeed) + 1) / 2;
            }

            float translateChangeX = m_WaveSliceX * m_BobDistance;
            float translateChangeY = m_WaveSlice * m_BobDistance;
            localPosition.y = /*currentWeaponMidPoint.y + */translateChangeY;
            localPosition.x = /*currentWeaponMidPoint.x + */translateChangeX;

            return localPosition;
            //currentWeapon.transform.localPosition = localPosition;
        }
        else
        {
            return Vector3.zero;
        }
        //else
        //{
        //    m_SwayTimer = 0.0f;
        //    localPosition.y = currentWeaponMidPoint.y;
        //    localPosition.x = currentWeaponMidPoint.x;
        //    currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, currentWeapon.m_MidPoint, 0.01f);
        //}

    }

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

    private void UpdateSway(float x, float y)
    {
        // xAxis Quaternion is for the recoil kick upwards.
        
        WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();
        Transform gunTransform = GetCurrentWeapon();
        if (!m_IsAiming)
        {
            Vector3 gunOriginalPos = GetCurrentWeaponOriginalPos();

            Vector3 bobStuff = WeaponBob();

            Vector3 finalPosition = new Vector3(Mathf.Clamp((-x * 0.02f), -weaponConfig.m_WeaponSwayClampX, weaponConfig.m_WeaponSwayClampX) + bobStuff.x, Mathf.Clamp((-y * 0.02f), -weaponConfig.m_WeaponSwayClampY, weaponConfig.m_WeaponSwayClampY) + bobStuff.y, 0 + weaponConfig.m_WeaponRecoilTransform.z);
            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, finalPosition + gunOriginalPos, Time.deltaTime * weaponConfig.m_GunSwayReturn);
            Quaternion xAxis = Quaternion.AngleAxis(WeaponRecoilRot.x, new Vector3(1, 0, 0));
            Quaternion zAxis = Quaternion.AngleAxis(Mathf.Clamp(-x, -weaponConfig.m_WeaponSwayRotateClamp, weaponConfig.m_WeaponSwayRotateClamp), new Vector3(0, 0, 1));
            gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, zAxis * xAxis, weaponConfig.m_WeaponSwayRotateSpeed);
        
            float currentFOV = m_Camera.fieldOfView;
            float desiredFOV = 60;
        
            float requiredChange = desiredFOV - currentFOV;
            m_Camera.fieldOfView += requiredChange * 0.45f;

        }
        else if (m_IsAiming)
        {
            // Had to put the sway code with the Aim() function since it was easier to just add the neccessary values to the calculations over there rather than try and split up the equations.

            float currentFOV = m_Camera.fieldOfView;
            float desiredFOV = 40;

            float requiredChange = desiredFOV - currentFOV;
            m_Camera.fieldOfView += requiredChange * 0.45f;



            // Quaternion rotate
            Quaternion zAxis = Quaternion.AngleAxis(Mathf.Clamp(-x, -weaponConfig.m_WeaponSwayRotateClamp, weaponConfig.m_WeaponSwayRotateClamp), new Vector3(0, 0, 1));
            Quaternion xAxis = Quaternion.AngleAxis(WeaponRecoilRot.x * weaponConfig.m_ADSRecoilModifier, new Vector3(1, 0, 0));
            gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, zAxis * xAxis, weaponConfig.m_WeaponSwayRotateSpeed);
        }

       
    }

    private void Aim()
    {
        WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();
        Transform gunTransform = GetCurrentWeapon();

        Vector3 centre = m_Camera.ScreenToWorldPoint(new Vector3(
            (Screen.width / 2) + (-LookInput.x * weaponConfig.m_GunAimSwayStrength),
            (Screen.height / 2) + (-LookInput.y * weaponConfig.m_GunSwayStrength) - (transform.up.y * weaponConfig.m_GunAimHeight),
            (transform.forward.z * weaponConfig.m_GunAimZPos) + weaponConfig.m_WeaponRecoilTransform.z * weaponConfig.m_ADSRecoilModifier));

        //Vector3 currentPosition = m_Gun.position;
        Vector3 currentPosition = weaponConfig.m_ScopeCentre.position;
        Vector3 requiredChange = centre - currentPosition;

        gunTransform.position += requiredChange * weaponConfig.m_GunAimSpeed;
    }

    private void Recoil()
    {

        if (!GetCurrentWeaponConfig().m_DisableAllRecoil)
        { 
            WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();
            

            AdditionalRecoilRotation += new Vector3(-weaponConfig.RecoilRotationAiming.x, Random.Range(-weaponConfig.RecoilRotationAiming.y, weaponConfig.RecoilRotationAiming.y), Random.Range(-weaponConfig.RecoilRotationAiming.z, weaponConfig.RecoilRotationAiming.z));
            WeaponRecoilRot -= new Vector3(weaponConfig.m_WeaponRotRecoilVertStrength, 0, 0);

            // Although I am setting the recoil transform here, I have to apply it in the WeaponSway() function since I'm setting pos directly there. I want to change this but I'm unsure how right now
            weaponConfig.m_WeaponRecoilTransform -= new Vector3(0, 0, weaponConfig.m_WeaponTransformRecoilZStrength);
        }
    }

    private void UpdateRecoil()
    {
        WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();

        AdditionalRecoilRotation = Vector3.Lerp(AdditionalRecoilRotation, Vector3.zero, weaponConfig.m_CameraRecoilReturnSpeed * Time.deltaTime);
        WeaponRecoilRot = Vector3.Lerp(WeaponRecoilRot, Vector3.zero, weaponConfig.m_WeaponRecoilReturnSpeed * Time.deltaTime);

        weaponConfig.m_WeaponRecoilTransform = Vector3.Lerp(weaponConfig.m_WeaponRecoilTransform, Vector3.zero, weaponConfig.m_WeaponRecoilReturnSpeed * Time.deltaTime);
    }

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

    private Transform GetCurrentWeapon() => m_Inventory.m_CurrentWeapon.transform;
    private Vector3 GetCurrentWeaponOriginalPos() => m_Inventory.m_CurrentWeapon.m_OriginalLocalPosition;
    private Vector3 GetCurrentWeaponOriginalGlobalPos() => m_Inventory.m_CurrentWeapon.m_OriginalGlobalPosition;


    public void OnTestRecoil(InputAction.CallbackContext value)
    {
        if (value.performed && !m_IsRecoilTesting)
        {
            // do test.
            m_IsRecoilTesting = true;
            m_PreviousOrientationVector = m_Orientation.transform.eulerAngles;
            m_PreviousXCameraRot = m_XRotation;

            Debug.Log("OnTestRecoil value performed!");
            Debug.Log(m_Camera.transform.eulerAngles + "vs" + PreviousCameraRotation);
            //m_IsFiring = true;
        }
        else if (value.performed && m_IsRecoilTesting)
        {
            m_IsRecoilTesting = false;
            m_IsTestResting = false;
            
            m_IsFiring = false;
            m_RecoilTestCounter = 0;
        }
    }
}
