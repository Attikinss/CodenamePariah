using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class HostController : InputController
{
    [Header("Settings")]
    //public float m_sensitivity = 1000.0f;
    public float m_verticalLookLock = 75.0f;
    //public float m_moveSpeed = 1;
    public float m_fireRate = 0.5f;
    public float m_bulletForce = 5;
    public float m_JumpHeight = 5;
    public float m_SecondJumpHeight = 2.5f;
    public float m_groundCheckHeight = 1;
    public float m_groundCheckRadius = 1;
    public float m_Gravity = -9.8f;
    public float m_maxSpeed = 5;
    public float m_BobSpeed = 1;
    public float m_BobDistance = 1;
    public float m_GroundAcceleration = 0.3f;
    public float m_AirAcceleration = 0.1f;
    public float m_JumpFallModifier = 2.0f;
    public float m_SlideSpeed = 700;
    public float m_SlideDuration = 0.75f;

    [HideInInspector]
    public Vector2 m_MovementInput = Vector2.zero;


    

    [Header("Weapons")]                    // ================ NOTE ================ //
    public Weapon m_weapon1;               // Weapons here will be replaced by another
    public Weapon m_weapon2;               // system. It is here for testing reasons.
                                           // ====================================== //
    [Header("Other References")]
    public PlayerManager m_PlayerManager;
    //public Camera m_mainCamera;
    public Transform m_orientation;
    public Rigidbody m_Rigidbody;


    


    // ================== BOOKKEEPING STUFF ================== //

    // Public bookkeeping.
    public bool IsGrounded { get; private set; }
    public Vector3 CacheMovDir = Vector3.zero;
    

    // Private bookkeeping.
    private float m_fireCounter = 0.0f;
    private bool m_hasFired = false;
    [HideInInspector]
    public bool m_HoldingFire = false;
    [HideInInspector]
    public float m_HeldCounter = 0.0f;
    
    private float xRotation = 0;

    private bool m_HasDoubleJumped = false;
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

    public InputActionAsset test;

    [HideInInspector]
    public bool m_Sliding = false;
    [HideInInspector]
    public Vector3 m_SlideDir = Vector2.zero;
    private Vector3 m_CacheSlideMove = Vector3.zero;

    [HideInInspector]
    public float m_SlideCounter = 0.0f;


    public float m_CameraCrouchHeight = -0.5f;



    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_PlayerManager);

        //InputManager.OnLook += Look;
        //InputManager.OnMove += Move;
        //InputManager.OnFire += Shoot;
        //InputManager.OnJump += Jump;
        //InputManager.OnSelect1 += WeaponSelect1;
        //InputManager.OnSelect2 += WeaponSelect2;

        m_Rigidbody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

	

	// Update is called once per frame
	void Update()
    {
        if (m_hasFired)
        {
            m_fireCounter += Time.deltaTime;
            if (m_fireCounter >= m_fireRate)
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

        //WeaponBob();



        // Testing recoil stuff.
        if (!m_HoldingFire)
        { 
            float requiredChange = m_AdditionalVerticalRecoil - m_AdditionalVerticalRecoil;
            m_AdditionalVerticalRecoil -= 1 * 0.1f;
            m_AdditionalVerticalRecoil = Mathf.Clamp(m_AdditionalVerticalRecoil, 0, 85f);

        }
    }

	public override void OnLook(InputAction.CallbackContext value)
	{
        Vector2 lookInput = value.ReadValue<Vector2>();
        Look(lookInput.x, lookInput.y);
	}
	public void Look(float xDelta, float yDelta)
    {
        //float mouseX = Input.GetAxis("Mouse X") * m_sensitivity * Time.fixedDeltaTime;
        //float mouseY = Input.GetAxis("Mouse Y") * m_sensitivity * Time.fixedDeltaTime;

        float mouseX = xDelta * m_LookSensitivity * Time.fixedDeltaTime;
        float mouseY = yDelta * m_LookSensitivity * Time.fixedDeltaTime;

        // Finding current look rotation
        Vector3 rot = m_Camera.transform.localRotation.eulerAngles;
        float desiredX = rot.y + mouseX;

        // Rotate
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Perform the rotations
        m_Camera.transform.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation - m_AdditionalVerticalRecoil, -90f, 90f), desiredX, 0);
        m_orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);

    }
	private void FixedUpdate()
	{
        Slide();
        Move(m_MovementInput);

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



        Debug.Log("Move called.");
        
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

    public void Shoot(bool active)
    {
        if (active && !m_hasFired)
        {
            m_HoldingFire = true; // ----------- To keep track of a continuous fire sequence. It's just here for testing reasons right now.

            Ray ray = new Ray(m_Camera.transform.position, m_Camera.transform.forward);
            RaycastHit hit;
            Weapon currentWeapon = PlayerManager.GetCurrentWeapon();

            // =========== TESTING =========== //
            m_AdditionalVerticalRecoil += currentWeapon.ShootRecoil(m_Camera.transform, m_HeldCounter);
            // =============================== //

            m_hasFired = true;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject != null)
                {
                    Decal newDecal = new Decal(hit.transform, hit.point, currentWeapon.m_HitDecal, hit.normal);
                    GameManager.Instance.AddDecal(newDecal);


                    // Adding a force to the hit object.
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(m_Camera.transform.forward * m_bulletForce, ForceMode.Impulse);
                    }
                }
            }
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
        if (Physics.SphereCast(ray, m_groundCheckRadius, out hit, m_groundCheckHeight))
        {
            Debug.Log(hit.transform.name);
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
        if (Physics.SphereCast(ray, m_groundCheckRadius, out hit, m_groundCheckHeight))
        {
            Gizmos.DrawLine(transform.position, hit.point);

            GraphicalDebugger.DrawSphereCast(transform.position, hit.point, Color.green, m_groundCheckRadius);
        }
        else
        {
            GraphicalDebugger.DrawSphereCast(transform.position, transform.position + Vector3.down, Color.red, m_groundCheckRadius);
        }

        Gizmos.color = defaultColour;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-CacheMovDir.x, CacheMovDir.y, -CacheMovDir.z));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(CacheMovDir.x, CacheMovDir.y, CacheMovDir.z));

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
			SmoothMove(m_Camera.transform, new Vector3(0, -0.5f, 0), 0.45f);

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
			SmoothMove(m_Camera.transform, new Vector3(0, 0.5f, 0), 0.45f);
		}

		//elseif(slideRecovery)
		// smoothly rotate back to normal.

	}

	private void SmoothMove(Transform obj, Vector3 wantedLocalPos, float t)
    {
        Vector3 currentPos = obj.localPosition;
        Vector3 desiredPos = wantedLocalPos;

        Vector3 requiredChange = desiredPos - currentPos;

       
        obj.localPosition += requiredChange * t;
    }

}
