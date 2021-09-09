using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInfo
{
    //public Vector3 PreviousCameraRotation { get; private set; } // Stores rotation when the player just starts shooting. Okay, so because comparing euler angles is a terrible idea due to there being multiple numbers
    //                                                            // that can describe the same thing, this variable now stores the forward vector before shooting. The idea being that I can use the dot product to
    //                                                            // compare the difference in angle between the old forward vector and the new forward vector.

    //public Vector3 CurrentCamRot { get; private set; }          // Like I mentioned above, this variable will be storing the current forward vector to be used when recovering from recoil.
    //// ======================================================= //

    //[HideInInspector]
    //public Vector3 m_PreviousOrientationVector = Vector3.zero;
    //[HideInInspector]
    //public float m_PreviousXCameraRot = 0;

    //[HideInInspector]
    //public bool m_IsAiming = false;

    //public float ShootingDuration { get; set; } = 1; // time tracking since started shooting.

    public Vector3 m_PrevCamForward;
    public Vector3 m_camForward;

    public Vector3 m_PrevOrientationRot;
    public float m_PrevXRot;


    public bool m_IsAiming = false;
    public float m_ShootingDuration = 0;
}
