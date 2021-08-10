using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// This enum helps keep track of what weapon the player is currently using.
/// </summary>
public enum WEAPON
{ 
    WEAPON1,
    WEAPON2
}


[RequireComponent(typeof(Rigidbody))]
public class HostController : InputController
{
    [Header("Settings")]

    [Header("Mouse Controls")]
    public float m_VerticalLock = 75.0f;

    [Header("Movement Controls")]
    public float m_JumpHeight = 5;
    public float m_SecondJumpHeight = 2.5f;
    public float m_GroundCheckHeight = 0.65f;
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
    public Transform m_Orientation;
    public Rigidbody m_Rigidbody;
    public Inventory m_Inventory;

    public Transform m_Weapon1;
    public Transform m_Weapon2;

    // These are references to the weapons that the controller has.
    public WeaponConfiguration m_WeaponConfig1;
    public WeaponConfiguration m_WeaponConfig2;
   
    //[Header("Temporary Weapon 1 Controls")]
    //public float m_FireRate = 0.5f;
    //public float m_BulletForce = 5;

    // ================== BOOKKEEPING STUFF ================== //

    [HideInInspector]
    public Vector2 m_MovementInput = Vector2.zero;
    public bool IsGrounded { get; private set; }
    public Vector3 CacheMovDir = Vector3.zero;
    
    private float m_fireCounter = 0.0f;
    private bool m_hasFired = false;

    [HideInInspector] // If this variable was once public and you had set it's value in the inspector, it will still have the value you set in the inspector even if you change its initialization here.
    public float m_HeldCounter = 1;
    
    private float xRotation = 0;

    private bool m_HasDoubleJumped = false;

    [HideInInspector]
    public bool m_Sliding = false;
    [HideInInspector]
    public Vector3 m_SlideDir = Vector2.zero;
    private Vector3 m_CacheSlideMove = Vector3.zero;

    [HideInInspector]
    public float m_SlideCounter = 0.0f;

    [HideInInspector]
    public Vector3 lookInput = Vector3.zero;

    [HideInInspector]
    public bool m_IsAiming = false;

    [HideInInspector]
    public Vector3 m_AdditionalRecoilRotation;
    [HideInInspector]
    public Vector3 m_WeaponRecoilRot;

    [HideInInspector]
    public float m_AdditionCameraRecoilX; // For actual recoil pattern. This will judge how much higher your camera will go while shooting.

    [HideInInspector]
    public float m_AdditionalCameraRecoilY; // This will be how much horizontal recoil will be applied to the camera.

    float desiredX = 0;


    // Storing there original positions and rotations for lerping purposes. Here I'm limiting us to two weapons however this can be replaced.
    [HideInInspector]
    public Vector3 m_GunOriginalPos1;
    [HideInInspector]
    public Vector3 m_GunOriginalPos2;
    [HideInInspector]
    public Quaternion m_GunOriginalRot;
    [HideInInspector]
    public Quaternion m_GunOriginalRot2;

    private bool m_IsFiring = false;

    [HideInInspector]
    public Vector3 m_PreviousCameraRotation; // Stores rotation when the player just starts shooting. Okay, so because comparing euler angles is a terrible idea due to there being multiple numbers
                                             // that can describe the same thing, this variable now stores the forward vector before shooting. The idea being that I can use the dot product to
                                             // compare the difference in angle between the old forward vector and the new forward vector.
    [HideInInspector]
    public Vector3 m_CurrentCamRot;          // Like I mentioned above, this variable will be storing the current forward vector to be used when recovering from recoil.
    // ======================================================= //


    // Exposed variables for debugging.
    [HideInInspector]
    public float m_CurrentMoveSpeed { get; private set; }
    [HideInInspector]
    public bool m_IsMoving { get; private set; }

    

    // ========================== TEMPORARY WEAPON SWAY ========================== //
    // This weapon sway stuff is here for now since we haven't got animations in yet.
    // It will be replaced soon.

    [HideInInspector]
    public float m_SwayTimer = 0.0f;
    [HideInInspector]
    public float m_WaveSlice = 0.0f;
    [HideInInspector]
    public float m_WaveSliceX = 0.0f;
    // ========================================================================== //

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        m_GunOriginalPos1 = m_Weapon1.localPosition;
        m_GunOriginalRot = transform.localRotation; // I don't remember why I store this, I think the original rotation will always be 0, 0, 0. I'll leave it here for now but will probably remove later.

        m_GunOriginalPos2 = m_Weapon2.localPosition;
    }

    private void LateUpdate()
    {
        Look();
    }

    void Update()
    {
        if (m_hasFired)
        {
            m_fireCounter += Time.deltaTime;
            //m_fireCounter = Time.time + (60.0f / GetCurrentWeaponConfig().m_FireRate);
            if (m_fireCounter >= 60.0f / GetCurrentWeaponConfig().m_FireRate)
            {
                m_hasFired = false;
                m_fireCounter = 0;
            }
        }

        if (m_IsFiring)
        {
            // They are holding down the fire button.
            m_HeldCounter += Time.deltaTime;
        }
        else
        {
            if (m_AdditionCameraRecoilX > 0) // We only want to decrement AdditionCameraRecoilX if it has accumuluated recoil still in it.
            { 
                // If we just keep decreasing the additional recoil until it reaches 0, it results in the camera going further down then what feels right.
                // This is because as the player is shooting, they are compensating and making the gun stand in place. While this is happening, the additional recoil could
                // build up to a high number and when the player stops shooting, the recoil will take a long time to get back to 0.

                // An experimental method I'd like to try is to either decrease it back to 0, or until the camera rotation is back to where it when they just started shooting.
                Vector2 currentCamX = new Vector2(m_CurrentCamRot.y, 1);
                Vector2 previousCamX = new Vector2(m_PreviousCameraRotation.y, 1);          // I know I'm using the new keyword here and that's bad. But for now I'm trying to see if this system will work.
                float dot = Vector3.Dot(currentCamX.normalized, previousCamX.normalized);
                if (dot < 0.9999f || dot > 1.0001f) // Such a small difference in numbers still gives quite a generous margin for error.
                {
                    // This means the current forward vector's y does not much the previous forward vector's y.
                    // We have to do one of two things.
                    // Either bring the gun down, so that the previous and current y components match.
                    // Or if the gun is already below the previous y component, we just leave the gun alone because they've over compensated for the recoil.

                    // If previous rotation's y is greater, it means they are looking further down then when they started firing.
                    if (m_PreviousCameraRotation.y > m_CurrentCamRot.y)
                    {
                        // We want to incorporate the additional camera recoil into the rotation of the camera, that way we can set the variable to 0 without worrying that later we will be moving the camera downwards.

                        // Because I'm setting the local rotation of the camera in the Look() function, it makes it kind of annoying to try and add/remove things to and from the rotation.
                        // Instead I will add the AdditionalCameraRecoilX into xRotation and then set AdditionalCameraRecoilX to 0. This way I don't have to directly touch the cameras local rotation
                        // here.

                        xRotation -= m_AdditionCameraRecoilX;
                        m_AdditionCameraRecoilX = 0;
                    }
                    else
                    {
                        // Otherwise, they are aiming higher than when they started, so we'll bring the gun down to where it was.
                        m_AdditionCameraRecoilX -= 1 * GetCurrentWeaponConfig().m_RecoilRecoveryModifier;
                        m_AdditionCameraRecoilX = Mathf.Clamp(m_AdditionCameraRecoilX, 0, 85f);
                    }

                }
                else
                {
                    // Since the forward vectors match, we'll clear the m_AdditionalCameraRecoilX variable just to keep things clean.
                    xRotation -= m_AdditionCameraRecoilX;
                    m_AdditionCameraRecoilX = 0;
                }
            }
            if (m_AdditionalCameraRecoilY != 0)
            {
                // I've decided not to lerp the additional horizontal recoil to 0 since it feels disorientating.
                
                

                // If we have accumulated horizontal recoil.
                //m_AdditionalCameraRecoilY -= 1 * GetCurrentWeaponConfig().m_RecoilRecoveryModifier;
                
                desiredX -= m_AdditionalCameraRecoilY;
                m_AdditionalCameraRecoilY = 0;
            }
            

        }

        IsGrounded = CheckGrounded();
        if (IsGrounded)
            m_HasDoubleJumped = false;

        m_CurrentMoveSpeed = m_Rigidbody.velocity.magnitude;


        if (m_IsAiming)
            Aim();
        UpdateSway(lookInput.x, lookInput.y);

        if (m_IsFiring)
            Shoot(true);

        // Just for debugging purposes. This variable is only used in the CustomDebugUI script.
        m_CurrentCamRot = m_Camera.transform.forward;
    }

	public override void OnLook(InputAction.CallbackContext value)
	{
        lookInput = value.ReadValue<Vector2>();
	}
	private void Look()
    {

        float mouseX = lookInput.x * m_LookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * m_LookSensitivity * Time.deltaTime;

        // Finding current look rotation
        Vector3 rot = m_Orientation.transform.localRotation.eulerAngles;
        /*float*/ desiredX = rot.y + mouseX;

        // Rotate
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Perform the rotations
        m_Orientation.transform.localRotation = Quaternion.Euler(0, desiredX - m_AdditionalCameraRecoilY, 0);
        m_Camera.transform.localRotation = Quaternion.Euler(Mathf.Clamp((xRotation - m_AdditionCameraRecoilX - m_AdditionalRecoilRotation.x), -90f, 90f), 0.0f - m_AdditionalRecoilRotation.y, 0 - m_AdditionalRecoilRotation.z);

    }
	private void FixedUpdate()
	{
        Slide();
        Move(m_MovementInput);
        UpdateRecoil();
	}
	public override void OnMovement(InputAction.CallbackContext value)
	{
        m_MovementInput = value.performed ? value.ReadValue<Vector2>() : Vector2.zero;
	}
	public void Move(Vector2 input)
    {
        // Preserves m_Rigidbody's y velocity.
        CacheMovDir.y = m_Rigidbody.velocity.y;
        
        // Ensure the slide will never make the player move vertically.
        m_CacheSlideMove.y = 0;


        // Making sure angular velocity isn't a problem.
        m_Rigidbody.velocity = CacheMovDir + m_CacheSlideMove;
        m_Rigidbody.angularVelocity = Vector3.zero;


        // ============================ MODIFIED FALLING ============================ //
        if (m_Rigidbody.velocity.y < 0)
        {
            m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * m_JumpFallModifier * Time.deltaTime;
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
        Vector3 xMov = m_Orientation.right * x;
        Vector3 zMov = m_Orientation.forward * z;

        xMov.y = 0;
        zMov.y = 0;

        Vector3 moveDir = ((xMov + zMov).normalized * speedMultiplier * Time.fixedDeltaTime) + Vector3.up * m_Rigidbody.velocity.y;

        return moveDir;
    }

    public void OnShoot(InputAction.CallbackContext value)
    {

        if (value.control.IsPressed(0)) // Have to use this otherwise mouse button gets triggered on release aswell.
        {
            m_IsFiring = true;
            Debug.Log("OnShoot called.");

            // Experimental thing I'm trying.
            // I will store the original camera rotation when they first start shooting that way I can go back to this rotation when they recover from recoil.
            m_PreviousCameraRotation = m_Camera.transform.forward;

        }
        else
        {
            m_IsFiring = false;
            Debug.Log("OnShoot cancelled.");
            // Reset held counter happens regardless.
            m_HeldCounter = 0.0f;
        }
    }
    public void Shoot(bool active)
    {
        if (active && !m_hasFired)
        {

            Ray ray = new Ray(m_Camera.transform.position, m_Camera.transform.forward);
            RaycastHit hit;

            WeaponConfiguration currentConfig = GetCurrentWeaponConfig();
            // =========== TESTING =========== //
            m_AdditionCameraRecoilX += currentConfig.m_VerticalRecoil.Evaluate(m_HeldCounter);
            m_AdditionalCameraRecoilY += currentConfig.m_HorizontalRecoil.Evaluate(m_HeldCounter);
            // =============================== //

            m_hasFired = true;

            Recoil();


            // ========================= TEMPORARY SHOOT COLLISION ========================= //

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject != null)
                {
                    Decal newDecal = new Decal(hit.transform, hit.point, hit.normal);
                    GameManager.Instance.AddDecal(newDecal);
            
            
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

    public void Jump(InputAction.CallbackContext context)
    {
        bool active = context.performed;
        if (active && IsGrounded)
        {
            CacheMovDir = Vector3.up * ControllerMaths.CalculateJumpForce(m_JumpHeight, m_Rigidbody.mass, m_Gravity);
            CacheMovDir.x = m_Rigidbody.velocity.x;
            CacheMovDir.z = m_Rigidbody.velocity.z;
            m_Rigidbody.velocity = CacheMovDir;
        }
        else if ((active && !IsGrounded) && !m_HasDoubleJumped)
        {
            CacheMovDir = Vector3.up * ControllerMaths.CalculateJumpForce(m_SecondJumpHeight, m_Rigidbody.mass, m_Gravity);
            CacheMovDir.x = m_Rigidbody.velocity.x;
            CacheMovDir.z = m_Rigidbody.velocity.z;
            m_Rigidbody.velocity = CacheMovDir;

            // Have to tick m_HasDoubleJumped to false;
            m_HasDoubleJumped = true;
        }
    }

    private bool CheckGrounded()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.SphereCast(ray, m_GroundCheckRadius, out hit, m_GroundCheckHeight))
        {
            //Debug.Log(hit.transform.name);
            return true;
        }
        return false;
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

    public Vector3 WeaponBob()
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

    private void OnDrawGizmos()
    {
        Color defaultColour = Gizmos.color;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.SphereCast(ray, m_GroundCheckRadius, out hit, m_GroundCheckHeight))
        {
            Gizmos.DrawLine(transform.position, hit.point);

            GraphicalDebugger.DrawSphereCast(transform.position, hit.point, Color.green, m_GroundCheckRadius);
        }
        else
        {
            GraphicalDebugger.DrawSphereCast(transform.position, transform.position + Vector3.down, Color.red, m_GroundCheckRadius);
        }

        Gizmos.color = defaultColour;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-CacheMovDir.x, CacheMovDir.y, -CacheMovDir.z));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(CacheMovDir.x, CacheMovDir.y, CacheMovDir.z));



        //Vector3 centre = m_Camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, transform.forward.z));
        //Gizmos.DrawSphere(centre, 0.25f);
        //Gizmos.DrawSphere(m_Gun.position, 0.25f);




        Color cache = Gizmos.color;
        // ================= Camera Forward Vectors For Recoil Recovery ================= //
        Vector2 modifiedCurrent = new Vector2(m_Camera.transform.forward.y, 1);
        Vector2 modifiedPrevious = new Vector2(m_PreviousCameraRotation.y, 1);
        
        // Debug Lines:
        // When the dot product is close to 1, the two lines will be GREEN.
        // When the current forward vector is below the previous forward vector, the two lines will be PURPLE.
        // When the current forward vector is above the previous forward vector, the two lines will be YELLOW.

        float dot = Vector2.Dot(modifiedCurrent.normalized, modifiedPrevious.normalized);

        if (dot < 0.9999f)
        {
            if (m_PreviousCameraRotation.y > m_CurrentCamRot.y)
                Gizmos.color = Color.magenta;
            else
                Gizmos.color = Color.yellow;
        }
        else
            Gizmos.color = Color.green;

        // Trying to create the same forward vectors but only caring about x and z.

        Gizmos.DrawLine(m_Camera.transform.position, m_Camera.transform.position + m_Camera.transform.forward * 100);  // Current forward vector.
        Gizmos.DrawLine(m_Camera.transform.position, m_Camera.transform.position + m_PreviousCameraRotation * 100);    // Forward vector when they first clicked the fire trigger.

        Gizmos.color = cache;

        // ============================================================================== //
    }

    public void OnSlide(InputAction.CallbackContext value)
    {
        Debug.Log("OnSlide called.");
        if (value.performed && IsGrounded)
        {
            m_SlideDir = value.performed ? m_Orientation.forward : m_SlideDir;
            m_Sliding = true;
        }
    }
    private void Slide()
    {
		// do slide code.
		Vector3 currentVelocity = m_CacheSlideMove;
		Vector3 desiredVelocity = m_SlideDir * m_SlideSpeed * Time.deltaTime;

		Vector3 requiredChange = desiredVelocity - currentVelocity;
		m_CacheSlideMove += requiredChange * 0.5f;

		if (m_Sliding)
		{
			// smoothly rotate backwards. todo
			SmoothMove(m_Camera.transform, new Vector3(0, -0.5f, 0), 0.25f);

			m_SlideCounter += Time.deltaTime;
			if (m_SlideCounter >= m_SlideDuration)
			{
				m_Sliding = false;
				m_SlideCounter = 0.0f;
				m_SlideDir = Vector3.zero;
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
            Quaternion xAxis = Quaternion.AngleAxis(m_WeaponRecoilRot.x, new Vector3(1, 0, 0));
            Quaternion zAxis = Quaternion.AngleAxis(Mathf.Clamp(-x, -weaponConfig.m_WeaponSwayRotateClamp, weaponConfig.m_WeaponSwayRotateClamp), new Vector3(0, 0, 1));
            gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, zAxis * xAxis, weaponConfig.m_WeaponSwayRotateSpeed);
        
            float currentFOV = m_Camera.fieldOfView;
            float desiredFOV = 60;
        
            float requiredChange = desiredFOV - currentFOV;
            m_Camera.fieldOfView += requiredChange * 0.45f;

            Debug.Log(gunOriginalPos);
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
            Quaternion xAxis = Quaternion.AngleAxis(m_WeaponRecoilRot.x * weaponConfig.m_ADSRecoilModifier, new Vector3(1, 0, 0));
            gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, zAxis * xAxis, weaponConfig.m_WeaponSwayRotateSpeed);
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
    private void Aim()
    {
        WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();
        Transform gunTransform = GetCurrentWeapon();

        Vector3 centre = m_Camera.ScreenToWorldPoint(new Vector3((Screen.width / 2) + (-lookInput.x * weaponConfig.m_GunAimSwayStrength), (Screen.height / 2) + (-lookInput.y * weaponConfig.m_GunSwayStrength) - weaponConfig.m_GunAimHeight, (transform.forward.z * weaponConfig.m_GunAimZPos) + weaponConfig.m_WeaponRecoilTransform.z * weaponConfig.m_ADSRecoilModifier));

        //Vector3 currentPosition = m_Gun.position;
        Vector3 currentPosition = weaponConfig.m_ScopeCentre.position;
        Vector3 requiredChange = centre - currentPosition;

        gunTransform.position += requiredChange * weaponConfig.m_GunAimSpeed;
    }

    private void Recoil()
    {
        WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();
        

        m_AdditionalRecoilRotation += new Vector3(-weaponConfig.RecoilRotationAiming.x, Random.Range(-weaponConfig.RecoilRotationAiming.y, weaponConfig.RecoilRotationAiming.y), Random.Range(-weaponConfig.RecoilRotationAiming.z, weaponConfig.RecoilRotationAiming.z));
        m_WeaponRecoilRot -= new Vector3(weaponConfig.m_WeaponRotRecoilVertStrength, 0, 0);

        // Although I am setting the recoil transform here, I have to apply it in the WeaponSway() function since I'm setting pos directly there. I want to change this but I'm unsure how right now
        weaponConfig.m_WeaponRecoilTransform -= new Vector3(0, 0, weaponConfig.m_WeaponTransformRecoilZStrength);
    }

    private void UpdateRecoil()
    {
        WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();

        m_AdditionalRecoilRotation = Vector3.Lerp(m_AdditionalRecoilRotation, Vector3.zero, weaponConfig.m_CameraRecoilReturnSpeed * Time.deltaTime);
        m_WeaponRecoilRot = Vector3.Lerp(m_WeaponRecoilRot, Vector3.zero, weaponConfig.m_WeaponRecoilReturnSpeed * Time.deltaTime);

        weaponConfig.m_WeaponRecoilTransform = Vector3.Lerp(weaponConfig.m_WeaponRecoilTransform, Vector3.zero, weaponConfig.m_WeaponRecoilReturnSpeed * Time.deltaTime);
    }

    private WeaponInventory GetCurrentWeaponInv()
    {
        return null;
    }
    /// <summary>
    /// GetCurrentWeaponConfig() returns the currently held weapons WeaponConfiguration script. It is public because the CustomDebugUI script needs to access it.
    /// </summary>
    /// <returns></returns>
    public WeaponConfiguration GetCurrentWeaponConfig()
    {
        // I know it's bad to use GetComponent() during runtime, but for now this does the job. An alternative I though of but am unsure if is good practice would be for the Inventory.cs script
        // to have another list that compliments the m_Weapons list. This new list would match each element in the m_Weapons list and store the corresponding weapons WeaponConfiguration script.
        
        return m_Inventory.m_CurrentWeapon.gameObject.GetComponent<WeaponConfiguration>();
    }

    private Transform GetCurrentWeapon()
    {
        return m_Inventory.m_CurrentWeapon.transform;
    }

    private Vector3 GetCurrentWeaponOriginalPos()
    {
        return m_Inventory.m_CurrentWeapon.m_OriginalLocalPosition;
    }

    private Vector3 GetCurrentWeaponOriginalGlobalPos()
    {
        return m_Inventory.m_CurrentWeapon.m_OriginalGlobalPosition;
    }

    
    

}
