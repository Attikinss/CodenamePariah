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

    [Header("Temporary Gun Controls")]
    public float m_FireRate = 0.5f;
    public float m_BulletForce = 5;

    [Header("Movement Controls")]
    public float m_JumpHeight = 5;
    public float m_SecondJumpHeight = 2.5f;
    public float m_GroundCheckHeight = 1;
    public float m_GroundCheckRadius = 1;
    public float m_Gravity = -9.8f;
    public float m_MaxSpeed = 5;
    public float m_GroundAcceleration = 0.3f;
    public float m_AirAcceleration = 0.1f;
    public float m_JumpFallModifier = 2.0f;

    [Header("Slide Controls")]
    public float m_SlideSpeed = 700;
    public float m_SlideDuration = 0.75f;
    public float m_CameraSlideHeight = -0.5f;

    [Header("Old Bobbing Controls")]
    public float m_BobSpeed = 1;
    public float m_BobDistance = 1;


    [Header("Weapons")]                    // ================ NOTE ================ //
    public Weapon m_weapon1;               // Weapons here will be replaced by another
    public Weapon m_weapon2;               // system. It is here for testing reasons.
                                           // ====================================== //


    [Header("Other References")]
    public Transform m_orientation;
    public Rigidbody m_Rigidbody;
    

    [Header("Temporary Weapon Controls")]
    public Transform m_Gun;
    

    public float m_GunAimHeight = 0.5f;
    public float m_GunAimSpeed = 0.25f;
    public float m_GunAimSwayStrength = 1;
    public float m_GunSwayStrength = 1;

    public Vector3 RecoilRotationAiming = new Vector3(0.5f, 0.5f, 1.5f);
    public float m_RotationSpeed = 6;
    public float m_CameraRecoilReturnSpeed = 25;

    public float m_GunSwayReturn = 6;

    public float m_WeaponSwayClampX = 0.5f;
    public float m_WeaponSwayClampY = 0.5f;
    public float m_WeaponSwayRotateClamp = 0.5f;
    public float m_WeaponSwayRotateSpeed = 0.05f;

    public float m_WeaponRecoilReturnSpeed = 1;

    public float m_WeaponRotRecoilVertStrength = 2.5f;
    public float m_WeaponTransformRecoilZStrength = 2.5f;

    [Tooltip("Can be used to reduce recoil when ADS")]
    public float m_ADSRecoilModifier = 1;

    // ================== BOOKKEEPING STUFF ================== //

    [HideInInspector]
    public Vector2 m_MovementInput = Vector2.zero;
    public bool IsGrounded { get; private set; }
    public Vector3 CacheMovDir = Vector3.zero;
    
    private float m_fireCounter = 0.0f;
    private bool m_hasFired = false;
    [HideInInspector]
    public bool m_HoldingFire = false;
    [HideInInspector]
    public float m_HeldCounter = 0.0f;
    
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
    public Vector3 m_WeaponRecoilTransform;

    [HideInInspector]
    public Vector3 m_GunOriginalPos;
    [HideInInspector]
    public Quaternion m_GunOriginalRot;
    // ======================================================= //


    // Exposed variables for debugging.
    [HideInInspector]
    public float m_currentMoveSpeed { get; private set; }
    [HideInInspector]
    public bool m_isMoving { get; private set; }

    

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



    // ========================== TESTING RECOIL ========================== //

    [HideInInspector]
    public float m_AdditionalVerticalRecoil = 0.0f;

    // ==================================================================== //

 
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        m_GunOriginalPos = m_Gun.localPosition;
        m_GunOriginalRot = transform.localRotation;
    }

    private void LateUpdate()
    {
        Look(lookInput.x, lookInput.y);
    }

    void Update()
    {
        if (m_hasFired)
        {
            m_fireCounter += Time.deltaTime;
            if (m_fireCounter >= m_FireRate)
            {
                m_hasFired = false;
                m_fireCounter = 0;
            }
        }

        if (m_HoldingFire)
        {
            // They are holding down the fire button.
            m_HeldCounter += Time.deltaTime;
        }
        else
        {
            m_HeldCounter = 0.0f;
        }

        IsGrounded = CheckGrounded();
        if (IsGrounded)
            m_HasDoubleJumped = false;

        m_currentMoveSpeed = m_Rigidbody.velocity.magnitude;


        // Testing recoil stuff.
        if (!m_HoldingFire)
        { 
            float requiredChange = m_AdditionalVerticalRecoil - m_AdditionalVerticalRecoil;
            m_AdditionalVerticalRecoil -= 1 * 0.1f;
            m_AdditionalVerticalRecoil = Mathf.Clamp(m_AdditionalVerticalRecoil, 0, 85f);

        }

        if (m_IsAiming)
            Aim();
        UpdateSway(lookInput.x, lookInput.y);
    }

	public override void OnLook(InputAction.CallbackContext value)
	{
        lookInput = value.ReadValue<Vector2>();
	}
	public void Look(float xDelta, float yDelta)
    {

        float mouseX = xDelta * m_LookSensitivity * Time.fixedDeltaTime;
        float mouseY = yDelta * m_LookSensitivity * Time.fixedDeltaTime;

        // Finding current look rotation
        Vector3 rot = m_orientation.transform.localRotation.eulerAngles;
        float desiredX = rot.y + mouseX;

        // Rotate
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Perform the rotations
        m_orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
        m_Camera.transform.localRotation = Quaternion.Euler(Mathf.Clamp((xRotation - m_AdditionalVerticalRecoil - m_AdditionalRecoilRotation.x), -90f, 90f), 0.0f - m_AdditionalRecoilRotation.y, 0 - m_AdditionalRecoilRotation.z);

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

        // Making sure angular velocity isn't a problem.
        m_Rigidbody.velocity = new Vector3(CacheMovDir.x, m_Rigidbody.velocity.y, CacheMovDir.z) + new Vector3(m_CacheSlideMove.x, 0, m_CacheSlideMove.z);
        m_Rigidbody.angularVelocity = Vector3.zero;


        // ============================ FASTER FALLING ============================ //

        if (m_Rigidbody.velocity.y < 0)
        {
            m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * m_JumpFallModifier * Time.deltaTime;
        }

        // ======================================================================== //

        float x = input.x;
        float z = input.y;
   

        m_isMoving = false;
        if (x != 0 || z != 0)
            m_isMoving = true;

        
        if (!IsGrounded)
        {
            // Slightly weaker movement.

            Vector3 currentVel = CacheMovDir;
            Vector3 desiredVel = CalculateMoveDirection(x, z, m_MovementSpeed);

            Vector3 requiredChange = desiredVel - currentVel;

            CacheMovDir += (requiredChange * m_AirAcceleration);

        }
        else
        {
            // Full on movement.
            
            Vector3 currentVel = CacheMovDir;
            Vector3 desiredVel = CalculateMoveDirection(x, z, m_MovementSpeed);

            Vector3 requiredChange = desiredVel - currentVel;

            CacheMovDir += requiredChange * m_GroundAcceleration;
        }

    }

    private Vector3 CalculateMoveDirection(float x, float z, float speedMultiplier)
    {
        Vector3 moveDir = new Vector3();

        moveDir = transform.right * x + transform.forward * z;

        Vector3 xMov = new Vector3(x * m_orientation.right.x, 0, x * m_orientation.right.z);
        Vector3 zMov = new Vector3(z * m_orientation.forward.x, 0, z * m_orientation.forward.z);

        moveDir = ((xMov + zMov).normalized * speedMultiplier * Time.deltaTime) + new Vector3(0, m_Rigidbody.velocity.y, 0);

        return moveDir;
    }

    public void OnShoot(InputAction.CallbackContext value)
    {
        if (value.control.IsPressed(0)) // Have to use this otherwise mouse button gets triggered on release aswell.
        {
            Shoot(true);
        }
    }
    public void Shoot(bool active)
    {
        if (active && !m_hasFired)
        {
            m_HoldingFire = true; // ----------- To keep track of a continuous fire sequence. It's just here for testing reasons right now.

            Ray ray = new Ray(m_Camera.transform.position, m_Camera.transform.forward);
            RaycastHit hit;
            Weapon currentWeapon = PlayerManager.GetCurrentWeapon();

            // =========== TESTING =========== //
            //m_AdditionalVerticalRecoil += currentWeapon.ShootRecoil(m_Camera.transform, m_HeldCounter);
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
                        hit.rigidbody.AddForce(m_Camera.transform.forward * m_BulletForce, ForceMode.Impulse);
                    }
                }
            }
            // ============================================================================= //
        }
        else if (!active) // This else if is a cheap way to track whether they let go of the fire button. To keep track of a continuous fire sequence.
            m_HoldingFire = false;
        
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

    public void WeaponSelect1(bool active)
    {
        if (active)
        {
            m_weapon1.gameObject.SetActive(true);
            m_weapon2.gameObject.SetActive(false);
            PlayerManager.SetWeapon(WeaponSlot.WEAPON1);
        }
    }
    public void WeaponSelect2(bool active)
    {
        if (active)
        {
            m_weapon1.gameObject.SetActive(false);
            m_weapon2.gameObject.SetActive(true);
            PlayerManager.SetWeapon(WeaponSlot.WEAPON2);
        }
    }

    public void WeaponBob()
    {
        Weapon currentWeapon = PlayerManager.GetCurrentWeapon();
        Vector3 localPosition = currentWeapon.transform.localPosition;
        Vector3 currentWeaponMidPoint = currentWeapon.m_MidPoint;

        if (m_isMoving)
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
            localPosition.y = currentWeaponMidPoint.y + translateChangeY;
            localPosition.x = currentWeaponMidPoint.x + translateChangeX;

            currentWeapon.transform.localPosition = localPosition;
        }
        else
        {
            m_SwayTimer = 0.0f;
            localPosition.y = currentWeaponMidPoint.y;
            localPosition.x = currentWeaponMidPoint.x;
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, currentWeapon.m_MidPoint, 0.01f);
        }

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

    }

    public void OnSlide(InputAction.CallbackContext value)
    {
        Debug.Log("OnSlide called.");
        if (value.performed && IsGrounded)
        {
            m_SlideDir = value.performed ? m_orientation.forward : m_SlideDir;
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
        
        if (!m_IsAiming)
        {
            Vector3 finalPosition = new Vector3(Mathf.Clamp(-x * 0.02f, -m_WeaponSwayClampX, m_WeaponSwayClampX), Mathf.Clamp(-y * 0.02f, -m_WeaponSwayClampY, m_WeaponSwayClampY), 0 + m_WeaponRecoilTransform.z);
            m_Gun.localPosition = Vector3.Lerp(m_Gun.localPosition, finalPosition + m_GunOriginalPos, Time.deltaTime * m_GunSwayReturn);
            Quaternion xAxis = Quaternion.AngleAxis(m_WeaponRecoilRot.x, new Vector3(1, 0, 0));
            Quaternion zAxis = Quaternion.AngleAxis(Mathf.Clamp(-x, -m_WeaponSwayRotateClamp, m_WeaponSwayRotateClamp), new Vector3(0, 0, 1));
            m_Gun.localRotation = Quaternion.Slerp(m_Gun.localRotation, zAxis * xAxis, m_WeaponSwayRotateSpeed);

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
            Quaternion zAxis = Quaternion.AngleAxis(Mathf.Clamp(-x, -m_WeaponSwayRotateClamp, m_WeaponSwayRotateClamp), new Vector3(0, 0, 1));
            Quaternion xAxis = Quaternion.AngleAxis(m_WeaponRecoilRot.x * m_ADSRecoilModifier, new Vector3(1, 0, 0));
            m_Gun.localRotation = Quaternion.Slerp(m_Gun.localRotation, zAxis * xAxis, m_WeaponSwayRotateSpeed);
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
        Vector3 centre = m_Camera.ScreenToWorldPoint(new Vector3((Screen.width / 2) + (-lookInput.x * m_GunAimSwayStrength), (Screen.height / 2) + (-lookInput.y * m_GunSwayStrength) - m_GunAimHeight, transform.forward.z + m_WeaponRecoilTransform.z));

        Vector3 currentPosition = m_Gun.position;
        Vector3 requiredChange = centre - currentPosition;

        m_Gun.position += requiredChange * m_GunAimSpeed;
    }

    private void Recoil()
    {
        m_AdditionalRecoilRotation += new Vector3(-RecoilRotationAiming.x, Random.Range(-RecoilRotationAiming.y, RecoilRotationAiming.y), Random.Range(-RecoilRotationAiming.z, RecoilRotationAiming.z));
        m_WeaponRecoilRot -= new Vector3(m_WeaponRotRecoilVertStrength, 0, 0);

        // Although I am setting the recoil transform here, I have to apply it in the WeaponSway() function since I'm setting pos directly there. I want to change this but I'm unsure how right now
        m_WeaponRecoilTransform -= new Vector3(0, 0, m_WeaponTransformRecoilZStrength);
    }

    private void UpdateRecoil()
    {
        m_AdditionalRecoilRotation = Vector3.Lerp(m_AdditionalRecoilRotation, Vector3.zero, m_CameraRecoilReturnSpeed * Time.deltaTime);
        m_WeaponRecoilRot = Vector3.Lerp(m_WeaponRecoilRot, Vector3.zero, m_WeaponRecoilReturnSpeed * Time.deltaTime);

        m_WeaponRecoilTransform = Vector3.Lerp(m_WeaponRecoilTransform, Vector3.zero, m_WeaponRecoilReturnSpeed * Time.deltaTime);
    }

    
    

}
