using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CustomDebugUI : MonoBehaviour
{
    // To make this temporary debug class work with the now multiple host agents, I'm going to make it a singleton so I can
    // easily re-assign which m_playerController we are talking about.
    public static CustomDebugUI s_Instance;
    




    [Header("References")]
    public HostController m_playerController;
    //public AbilityController m_AbilityController;
    public Canvas m_Canvas;

    [Header("Jump")]
    public Text m_isGroundedText;

    [Header("Movement")]
    public Text m_moveSpeedText;
    public Text m_isMovingText;
    public Text m_YVelocityText;
    public Text m_CacheMovDirText;
    public Text m_XAxisText;
    public Text m_ZAxisText;
    public Text m_IsSlidingText;
    public Text m_SlideDirText;
    public Text m_SlideCounterText;

    public Text m_LookInputText;

    private bool m_Toggle = false; // set this to false so it doesn't appear on start.



    //[Header("Weapon Sway")]
    //public Text m_SwayTimerText;
    //public Text m_WaveSliceText;

    //[Header("Weapon General")]
    //public Text m_CurrentWeaponSlotText;

    [Header("Combat General")]
    public Text m_CurrentHealthText;
    public Text m_CurrentMaxHealthText;

    public Text m_AdditionalRecoilText;

    public Text m_WeaponRecoilRotText;
    public Text m_WeaponRecoilTransfText;

    public Text m_FireHeldCounterText;


    [Header("Recoil Patterns")]
    public Text m_VerticalCameraRecoilText;
    public Text m_HorizontalCameraRecoilText;

    public Text m_PreviousCamXRotText;
    public Text m_CurrentCamXRotText;

    public Text m_CameraRecoilDotProductText;

    //public Text m_ShootHeldText;
    //public Text m_ShootHeldCounterText;
    //public Text m_VertRecoilText;

    //[Header("Combat Abilities")]
    //public Text m_Ability1ActiveText;
    //public Text m_Ability1CounterText;

    //public Text m_Ability2ActiveText;
    //public Text m_Ability2CounterText;

    //public Text m_Ability3ActiveText;
    //public Text m_Ability3CounterText;
    private void HideOnPlay()
    {
        m_Canvas.enabled = false;
    }
    private void Awake()
    {
        if (!s_Instance)
            s_Instance = this;
        else
        {
            Debug.LogError("You have added more than one CustomDebugUI to the scene. There can only be one!");
            Destroy(this);
        }

        //Debug.Assert(m_playerController);
        //Debug.Assert(m_AbilityController);
        Debug.Assert(m_Canvas);

        HideOnPlay();
        m_Canvas.gameObject.SetActive(false);
    }

    public void SetController(HostController controller) 
    {
        m_Canvas.gameObject.SetActive(true);
        m_playerController = controller; 
        
    }
    public void ClearController()
    {
        m_Canvas.gameObject.SetActive(false);
        m_playerController = null; 
    }

    // Start is called before the first frame update
    void Start()
    {
        //InputManager.OnDebugToggle += Toggle;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerController)
        {
            GraphicalDebugger.Assign<bool>(m_playerController.IsGrounded, "IsGrounded", m_isGroundedText);
            GraphicalDebugger.Assign<float>(m_playerController.m_CurrentMoveSpeed, "MoveSpeed", m_moveSpeedText);
            GraphicalDebugger.Assign<bool>(m_playerController.m_IsMoving, "IsMoving", m_isMovingText);
            GraphicalDebugger.Assign<float>(m_playerController.Rigidbody.velocity.y, "YVelocity", m_YVelocityText);
            GraphicalDebugger.Assign<Vector3>(m_playerController.CacheMovDir, "CacheMovDir", m_CacheMovDirText);
            GraphicalDebugger.Assign<float>(m_playerController.MovementInput.x, "XAxis", m_XAxisText);
            GraphicalDebugger.Assign<float>(m_playerController.MovementInput.y, "ZAxis", m_ZAxisText);

            GraphicalDebugger.Assign<bool>(m_playerController.IsSliding, "IsSliding", m_IsSlidingText);
            GraphicalDebugger.Assign<Vector3>(m_playerController.SlideDir, "SlideDir", m_SlideDirText);
            GraphicalDebugger.Assign<float>(m_playerController.SlideCounter, "SlideCounter", m_SlideCounterText);

            GraphicalDebugger.Assign<Vector3>(m_playerController.AdditionalRecoilRotation, "AdditionalRecoilRotation", m_AdditionalRecoilText);

            GraphicalDebugger.Assign<Vector3>(m_playerController.LookInput, "LookInput", m_LookInputText);

            // Because there are multiple weapons, I have to get the current weapons configuration.
            WeaponConfiguration weaponConfig = m_playerController.GetCurrentWeaponConfig();

            //GraphicalDebugger.Assign<Vector3>(m_playerController.WeaponRecoilRot, "WeaponRecoilRot", m_WeaponRecoilRotText);
            GraphicalDebugger.Assign<Vector3>(weaponConfig.m_WeaponRecoilTransform, "WeaponRecoilTransf", m_WeaponRecoilTransfText);

            GraphicalDebugger.Assign<float>(m_playerController.AdditionalCameraRecoilX, "VerticalCameraRecoil", m_VerticalCameraRecoilText);
            GraphicalDebugger.Assign<float>(m_playerController.AdditionalCameraRecoilY, "HorizontalCameraRecoil", m_HorizontalCameraRecoilText);

            GraphicalDebugger.Assign<float>(Time.time - m_playerController.GetCurrentWeapon().m_FireStartTime, "FireHeldCounter", m_FireHeldCounterText);

            GraphicalDebugger.Assign<float>(m_playerController.CurrentCamRot.x, "CurrentCamXRot", m_CurrentCamXRotText);
            GraphicalDebugger.Assign<float>(m_playerController.PreviousCameraRotation.x, "PreviousCamXRot", m_PreviousCamXRotText);

            // I know it's super inefficient to recalculate the dot product here but this wont be in the builds of the game so I think its okay, plus it saved me like 1 minute.
            Vector3 modifiedCurrent = new Vector2(m_playerController.CurrentCamRot.y, 1);
            Vector3 modifiedPrevious = new Vector2(m_playerController.PreviousCameraRotation.y, 1);
            float dot = Vector2.Dot(modifiedCurrent.normalized, modifiedPrevious.normalized);
            GraphicalDebugger.Assign<float>(dot, "CameraDotProduct", m_CameraRecoilDotProductText);
        }
        //else // Because I'm too lazy to set all the variables to 0 and stuff, I'm just going to hide the canvas when m_playerController is null.
        //{
        //    m_Canvas.gameObject.SetActive(false);
        //}

        // Okay ignore that else statement. Now on ResetController() I set the canvas to false. This has the benefit of not being called every single frame.

    }

    public void Toggle()
    {
        if (m_Toggle)
        {
            m_Canvas.enabled = false;
            m_Toggle = false;
        }
        else
        { 
            m_Canvas.enabled = true;
            m_Toggle = true;
        }
    }
    
}
