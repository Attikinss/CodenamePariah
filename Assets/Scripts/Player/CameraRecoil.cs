using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CameraRecoil is a struct to organise variables from HostController.cs.
/// </summary>
public class CameraRecoil
{
    // Don't have to be seen in inspector since these are accumulators that the designers shouldn't care about.

    public Vector3 accumulatedVisualRecoil = Vector3.zero;

    public float accumulatedPatternRecoilX = 0;
    public float accumulatedPatternRecoilY = 0;



    // Copied these from host controller just to reference while thinking of variable names.

    //public Vector3 AdditionalRecoilRotation { get; set; } // Made the setter public so that I can access it in the Weapon.cs script.

    ////public Vector3 WeaponRecoilRot { get; private set; }

    //public float AdditionalCameraRecoilX { get; set; } // For actual recoil pattern. This will judge how much higher your camera will go while shooting.

    //public float AdditionalCameraRecoilY { get; set; } // This will be how much horizontal recoil will be applied to the camera.
}
