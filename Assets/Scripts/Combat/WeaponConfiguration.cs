using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConfiguration : MonoBehaviour
{
    [Header("Weapon Controls")]

    [Header("Transform References")]
    public Transform m_Gun;
    public Transform m_ScopeCentre;

    [Header("ADS")]
    public float m_GunAimZPos = 0.5f;
    public float m_GunAimHeight = 0.5f;
    public float m_GunAimSpeed = 0.25f;
    public float m_GunAimSwayStrength = 1;

    [Header("Hipfire")]
    public float m_GunSwayStrength = 1;
    public float m_GunSwayReturn = 6;

    [Header("Camera Shake Recoil")]
    public Vector3 RecoilRotationAiming = new Vector3(0.5f, 0.5f, 1.5f);
    public float m_RotationSpeed = 6;
    public float m_CameraRecoilReturnSpeed = 25;

    [Header("Sway Clamps")]
    public float m_WeaponSwayClampX = 0.5f;
    public float m_WeaponSwayClampY = 0.5f;
    public float m_WeaponSwayRotateClamp = 0.5f;
    public float m_WeaponSwayRotateSpeed = 0.05f;

    [Header("Visual Gun Recoil")]
    public float m_WeaponRecoilReturnSpeed = 1;
    public float m_WeaponRotRecoilVertStrength = 2.5f;
    public float m_WeaponTransformRecoilZStrength = 2.5f;
    [Tooltip("Can be used to reduce recoil when ADS")]
    public float m_ADSRecoilModifier = 1;
    [HideInInspector]
    public Vector3 m_WeaponRecoilTransform;

    [Header("Camera Recoil Pattern")]
    public AnimationCurve m_VerticalRecoil;
    public AnimationCurve m_HorizontalRecoil;
    [Range(0, 1)]
    public float m_RecoilRecoveryModifier;
}