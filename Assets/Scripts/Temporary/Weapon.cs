using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;
//using UnityEngine.UI;

public enum WEAPONTYPE
{
    RIFLE,
    PISTOL,
    DUAL,
    NONE
}
public class Weapon : MonoBehaviour
{
    public WEAPONTYPE m_TypeTag;

    [SerializeField]
    [Tooltip("The character's camera.")]
    private Camera m_Camera;

    [SerializeField]
    [Tooltip("Damage done with each shot.")]
    public int m_BulletDamage;

    [SerializeField]
    [Tooltip("How much damage scales when weapon is used by an AI Agent.")]
    public float m_AIDamageModifier = 0.5f;

    [SerializeField]
    [Tooltip("Current ammo in magazine.")]
    private int m_RoundsInMagazine;

    [SerializeField]
    [Tooltip("How many rounds are in each magazine.")]
    private int m_MagazineSize;

    [SerializeField]
    [Tooltip("Ammo available in reserve.")]
    private int m_ReserveAmmo;

    [SerializeField]
    [Tooltip("Ammo available in reserve.")]
    private int m_MaxReserveAmmo;

    [SerializeField]
    [Tooltip("The weapon's firing speed.")]
    private float m_FireRate;

    [SerializeField]
    [Tooltip("Time it takes to reload.")]
    private float m_ReloadTime = 1.8f;

    [SerializeField]
    [Tooltip("The percentage of a full magazine required before warning to reload.")]
    private float m_LowAmmoWarningPercentage = 0.3f;

    //[SerializeField]
    //[Tooltip("Defines the position at which bullets spawn during firing.")]
    //private Transform m_FiringPosition;

    /// <summary>An accumulative value used to determine when the next round should be fired.</summary>
    private float m_NextTimeToFire = 0f;

    // Duplicate variables to handle the second gun for dual wield.
    private int m_RoundsInMagazineLeft;
    //private int m_ReserveAmmoLeft;
    private float m_NextTimeToFireLeft = 0f;
   

    // Because we're not in the HostController.cs script, we need a reference to it to access some things.
    public HostController m_Controller;

    // Because I'm trying to move away from the technique of having timers in Update(), I need a new
    // way of getting the total firing duration. I'm going to record the time when you start firing
    // and calculate the difference between the current time to find out the duration.
    [HideInInspector]
    public float m_FireStartTime = 0.0f;
    [HideInInspector]
    public float m_FireStartTimeLeft = 0.0f;

    // Stuff from my original Weapon.cs script.

    public Inventory m_Inventory;

    // temporary ui thing
    private UIManager m_UIManager;

    // temporary thing to test out semi-automatic weaponry.
    public bool m_SemiAuto = false;

    // temporary
    public bool m_DualWield = false;

    public WeaponConfiguration m_Config;

    // New structs yay.
    public WeaponTransformInfo m_TransformInfo;
    public WeaponBob m_WeaponBobInfo;
    public WeaponAction m_WeaponActions;
    public RecoilTest m_RecoilTesting;
    public RecoilInfo m_RecoilInfo;
    public ParticleEffects m_Particles;
    public Animators m_Animators;


    public FMODAudioEvent m_AudioFireEvent;
    public FMODAudioEvent m_AudioReloadEvent;
    public FMODAudioEvent m_AudioEmptyClipEvent;
    public FMODAudioEvent m_AudioEquipEvent;

    public GameObject m_CharIcon;
    public GameObject m_CharName;
    public GameObject m_WeaponIcon;
    public TextMeshProUGUI m_WeaponAmmoText;

    // To be used when initalising the skinned mesh renderers of weapons.
    // The problem with initialising on Start() or Awake() is that many of the weapons in the game are deactivated by default and only
    // Awake() when the user swaps to them.
    // But we still need to initialise the renderers even if the user hasn't held the weapon before.
    [HideInInspector]
    public bool m_InitialisedSkinnedMeshRenderers = false;

	private void Awake()
	{
        m_TransformInfo.m_OriginalLocalPosition = transform.localPosition;
        m_TransformInfo.m_OriginalGlobalPosition = transform.position;

        //m_UIManager = transform.parent.parent.parent.GetComponent<UIManager>();
        //m_UIManager?.UpdateWeaponUI(this);
        m_Config = GetComponent<WeaponConfiguration>();



        // Display a warning if reload time is less than or equal to the animators reload duration.

        // This is because the reload time needs to be slightly longer othewise the gun can become stuck in... hold this thought.

        // I'm going to try caching the original local pos and local rotation and just set it back to that everytime the player swaps weapons.

        //m_OriginalLocalRot = transform.localRotation;


        // Setting up the left gun's ammo pools and stuff to match the right gun.
        //m_ReserveAmmoLeft = m_ReserveAmmo; // Removed reserve ammo for the left gun because the left and right gun ammo pools are going to be the same.        m_RoundsInMagazineLeft = m_RoundsInMagazine;


        m_Particles.CacheBulletParticle();


    }
	private void Start()
	{
        m_UIManager = UIManager.s_Instance;

        if(!m_InitialisedSkinnedMeshRenderers)
            InitSkinnedMeshes();

        // Hide the skinned mesh renderers of the weapons and arms. We only want them to show if the player is in FPS mode.
        // We hide all weapons that are on agents who aren't being controlled by the player.
        if(!m_Inventory.Owner.Possessed)
            ToggleWeapon(false);

	}

	// Update is called once per frame
	void Update()
    {
        // I've added m_IsReloading checks to prevent shooting while reloading and also to activate recoil recovery even if m_IsFiring is still true.
        // This gives the advantage of reloading while holding down the mouse button will let you begin shooting again without having to re-press the mouse button.

        if (CanFire(m_DualWield)/*m_WeaponActions.m_IsFiring && !m_WeaponActions.m_IsReloading*/)
        {
            if (!m_DualWield)
                Fire();
            else
                Fire(true); // special is set to true since we are firing from the left gun.
            m_UIManager?.UpdateWeaponUI(this);
        }
        /*else */if (CanRecoilRecover()/*!GetFireState() || GetReloadState(true) || TotalAmmoEmpty()*/) // We want to recovery if we are reloading. This lets us set reloading to true and keep firing on true and the player wont shoot.
        {
            //bool recoilRecSuccess = false;
            //// Few more checks if we are dual wielding.
            //if (m_DualWield)
            //{
            //    if (!GetFireState(true) || GetReloadState(true)) // Both guns must be reloading to have the gun go back down.
            //        recoilRecSuccess = true;
            //}
            //else
            //    recoilRecSuccess = true;
            //
            //if(recoilRecSuccess)
                UpdateRecoilRecovery();
        }

        if (CanAim()/*m_WeaponActions.m_IsAiming && !m_WeaponActions.m_IsReloading && !m_DualWield*/)
        {
            if (!m_DualWield) // If the weapon is not dual wield, then aim like normal.
                Aim();
            else
            {
                // Otherwise, we want to treat Aim() like a second fire function that allows firing from the normal right hand weapon.
                Fire(); 
                m_UIManager?.UpdateWeaponUI(this);
            }
        }

        UpdateSway(m_Controller.LookInput.x, m_Controller.LookInput.y);
        UpdateRecoilTest();
    }

	private void FixedUpdate()
	{
        UpdateRecoil();
    }

    /// <summary>
    /// Fire() handles firing of all weapons. It is utilized by the AI aswell as input from the player.
    /// </summary>
    /// <param name="special">If special is true, it will perform firing for the left dual wield gun. Set to false if not for dual wield.</param>
	public void Fire(bool special = false) 
    {
        // In this function you can do a few invocations to different systems
        // For example whenever a sound needs to be played, you can call into
        // the audio manager to play the appropriate sound.

        if (ReadyToFire(special))
        {
            // Getting the correct rounds in magazine. There are two, the normal one and the one for the left gun.
            int RoundsInMag;
            if (special)
                RoundsInMag = m_RoundsInMagazineLeft;
            else
                RoundsInMag = m_RoundsInMagazine;

            if (RoundsInMag > 0/* && !m_IsReloading*/)
            {
                // testing somethhing for semi-auto
                if (m_SemiAuto)
                    m_WeaponActions.m_IsFiring = false;

                //// Play effects.
                //if (m_DualWield)
                //{
                //    if (m_Particles.m_MuzzleFlashes.Count > 0)
                //    {
                //        for (int i = 0; i < m_Particles.m_MuzzleFlashes.Count; i++)
                //            m_Particles.m_MuzzleFlashes[i].Play();
                //    }
                //    if (m_Particles.m_BulletCasings.Count > 0)
                //    {
                //        for (int i = 0; i < m_Particles.m_BulletCasings.Count; i++)
                //            m_Particles.m_BulletCasings[i].Play();
                //    }
                //    if (m_Animators.m_GunAnimators.Count > 0)
                //        for (int i = 0; i < m_Animators.m_GunAnimators.Count; i++)
                //            m_Animators.m_GunAnimators[i].SetTrigger("IsFiring");
                //    if (m_Animators.m_ArmsAnimators.Count > 0)
                //        for (int i = 0; i < m_Animators.m_ArmsAnimators.Count; i++)
                //            m_Animators.m_ArmsAnimators[i].SetTrigger("IsFiring");
                //}
                //else
                //{
                //    m_Particles.m_MuzzleFlashes[0].Play();
                //    m_Particles.m_BulletCasings[0].Play();
                //    m_Animators.m_GunAnimators[0].SetTrigger("IsFiring");
                //    m_Animators.m_ArmsAnimators[0].SetTrigger("IsFiring");
                //}

                // Playing sounds.
                PlayFireSound();
                
                PlayAnimations(special);

                // Currently gets rid of bullet sprite before UI has fully updated //
                //m_UIManager.DisableBulletSpriteInCurrentMag(m_RoundsInMagazine - 1);

                if (special) // If special, deduct from the left gun's ammo pool.
                    m_RoundsInMagazineLeft--;
                else // Otherwise, deduct from the normal right gun ammo pool.
                    m_RoundsInMagazine--;

                //m_FireSoundEmitter?.Trigger();

                // I'll add my shoot code in here.
                Ray ray;
                if (m_Inventory.Owner == null || m_Inventory.Owner.Possessed) // Added a check so that it can be used without being tied to the agent system.
                {
                    
                    ray = new Ray(m_Camera.transform.position, m_Camera.transform.forward);
                    WeaponConfiguration currentConfig = GetCurrentWeaponConfig();
                    // =========== TESTING =========== //
                    if (!currentConfig.m_DisableAllRecoil/* && m_Inventory.Owner.Possessed*/) // Redundant possessed check since the outer if statement already checks for that.
                    {
                        float ShootingDuration = Time.time - m_FireStartTime;

                        CameraRecoil cameraRecoil = m_Controller.m_AccumulatedRecoil;

                        cameraRecoil.accumulatedPatternRecoilX += currentConfig.m_VerticalRecoil.Evaluate(ShootingDuration);
                        cameraRecoil.accumulatedPatternRecoilY += currentConfig.m_HorizontalRecoil.Evaluate(ShootingDuration);
                    }
                    // =============================== //

                    //m_HasFired = true;

                    AddVisualRecoil();

                    // ========================= TEMPORARY SHOOT COLLISION ========================= //
                    if (Physics.Raycast(ray, out RaycastHit hitInfo, 500))
                    {
                        if (hitInfo.transform.gameObject != null)
                        {
                            // Only play the bullet shot effect if we are standing atleast a certain amount away from the collider.
                            if(Vector3.Distance(m_Inventory.Owner.transform.position, hitInfo.point) > 2)
                                PlayBulletEffect(special, true, hitInfo.point);

                            if (hitInfo.transform.TryGetComponent(out Inventory agentInventory))
                            {
                                float damageMod = m_Inventory.Owner.Possessed ? 1.0f : m_AIDamageModifier;
                                agentInventory.TakeDamage((int)(m_BulletDamage * damageMod));
                                //Debug.Log("BAD");
                                GameManager.s_Instance?.PlaceBulletSpray(hitInfo.point, hitInfo.transform, (transform.position - hitInfo.point).normalized);


                                return;
                            }
                            
                            GameManager.s_Instance?.PlaceDecal(hitInfo.transform, hitInfo.point, hitInfo.normal);

                            // Adding a force to the hit object.
                            if (hitInfo.rigidbody != null)
                                hitInfo.rigidbody.AddForce(m_Camera.transform.forward * GetCurrentWeaponConfig().m_BulletForce, ForceMode.Impulse);
                        }
                    }
                    else
                    {
                        // Shot did not hit gameobject.
                        Plane plane = new Plane(-m_Camera.transform.forward, 1000); // This is creating a plane for us to aim the
                                                                                    // bullet effect towards if we are in an open area without any colliders.
                        float planeEnter;
                        if (plane.Raycast(ray, out planeEnter))
                        {
                            Debug.Log("Raycast used plane instead.");
                            PlayBulletEffect(special, true, ray.GetPoint(planeEnter));
                        }

                        //PlayBulletEffect(special, false, Vector3.zero);
                    }
                    // ============================================================================= //
                }
                else
                {
                    var bullet = BulletPooler.Instance?.GetNextAvailable();
                    if (m_Inventory.Owner.Target != m_Inventory.Owner.PariahController.gameObject)
                        bullet?.SetTarget(m_Inventory.Owner.Target, (int)(m_BulletDamage * m_AIDamageModifier));

                    if(m_Inventory.Owner.m_FiringPosition) // Quick check since dual wield has no firing position currently.
                        bullet?.Fire(m_Inventory.Owner.m_FiringPosition.position, m_Inventory.Owner.Target.transform.position, transform.forward, m_Inventory.Owner.transform);
                }
            }
            else
            {
                // Do nothing / reload automatically
                if (!ReserveAmmoEmpty(special) && !m_Animators.CheckWeaponInspect())
                {
                    CombatInfo combatInfo = m_Controller.m_CombatInfo;

                    StartCoroutine(Reload(special));
                    // To prevent recoil from affecting player while reloading, we must.
                    combatInfo.m_ShootingDuration = 0;
                    
                    //m_IsFiring = false;
                }
                else
                {
                    PlayEmptyClipSound();
                }
                //else if (TotalAmmoEmpty())
                //    this.GetComponentInParent<UIManager>().DisableMagazine();
            }
        }
    }

    public void FireAt(GameObject target)
    {
        // TODO: Clean up this duped code

        if (ReadyToFire())
        {
            // Getting the correct rounds in magazine. There are two, the normal one and the one for the left gun.
            int RoundsInMag = m_RoundsInMagazine;

            if (RoundsInMag > 0)
            {
                if (m_SemiAuto)
                    m_WeaponActions.m_IsFiring = false;

                PlayFireSound();
                PlayAnimations();

                m_RoundsInMagazine--;

                if (m_Inventory.Owner == null || m_Inventory.Owner.Possessed)
                {
                    //Vector3 randOffset = new Vector3(Random.Range(-0.75f, 0.75f), Random.Range(-0.75f, 0.75f), Random.Range(-0.75f, 0.75f));
                    Vector3 direction = ((target.transform.position /*+ randOffset*/)
                        - m_Camera.transform.position);

                    Ray ray = new Ray(m_Camera.transform.position, direction);
                    WeaponConfiguration currentConfig = GetCurrentWeaponConfig();

                    // =========== TESTING =========== //
                    if (!currentConfig.m_DisableAllRecoil)
                    {
                        float ShootingDuration = Time.time - m_FireStartTime;

                        CameraRecoil cameraRecoil = m_Controller.m_AccumulatedRecoil;

                        cameraRecoil.accumulatedPatternRecoilX += currentConfig.m_VerticalRecoil.Evaluate(ShootingDuration);
                        cameraRecoil.accumulatedPatternRecoilY += currentConfig.m_HorizontalRecoil.Evaluate(ShootingDuration);
                    }
                    // =============================== //

                    AddVisualRecoil();

                    // ========================= TEMPORARY SHOOT COLLISION ========================= //

                    if (Physics.Raycast(ray, out RaycastHit hitInfo, 500))
                    {
                        if (hitInfo.transform.gameObject != null)
                        {
                            if (hitInfo.transform.TryGetComponent(out Inventory agentInventory))
                            {
                                float damageMod = m_Inventory.Owner.Possessed ? 1.0f : m_AIDamageModifier;
                                agentInventory.TakeDamage((int)(m_BulletDamage * damageMod));

                                GameManager.s_Instance?.PlaceBulletSpray(hitInfo.point, hitInfo.transform, (transform.position - hitInfo.point).normalized);

                                return;
                            }

                            PlayBulletEffect(false, true, hitInfo.point);
                            GameManager.s_Instance?.PlaceDecal(hitInfo.transform, hitInfo.point, hitInfo.normal);

                            // Adding a force to the hit object.
                            if (hitInfo.rigidbody != null)
                                hitInfo.rigidbody.AddForce(m_Camera.transform.forward * GetCurrentWeaponConfig().m_BulletForce, ForceMode.Impulse);
                        }
                    }
                    else
                    {
                        // Shot did not hit gameobject.
                        PlayBulletEffect(false, false, Vector3.zero);
                    }
                    // ============================================================================= //
                }
                else
                {
                    var bullet = BulletPooler.Instance?.GetNextAvailable();
                    if (m_Inventory.Owner.Target != m_Inventory.Owner.PariahController.gameObject)
                        bullet?.SetTarget(m_Inventory.Owner.Target, (int)(m_BulletDamage * m_AIDamageModifier));

                    // Quick check since dual wield has no firing position currently.
                    if (m_Inventory.Owner.m_FiringPosition)
                    {
                        bullet?.Fire(m_Inventory.Owner.m_FiringPosition.position, m_Inventory.Owner.Target.transform.position, transform.forward, m_Inventory.Owner.transform);
                    }
                }
            }
            else
            {
                // Do nothing / reload automatically
                if (!ReserveAmmoEmpty() && !m_Animators.CheckWeaponInspect())
                {
                    CombatInfo combatInfo = m_Controller.m_CombatInfo;

                    StartCoroutine(Reload());

                    // To prevent recoil from affecting player while reloading, we must.
                    combatInfo.m_ShootingDuration = 0;
                }
                else
                    PlayEmptyClipSound();
            }
        }
    }

    public void ForceReload()
    {
        StartCoroutine(Reload());
    }

    public void StartReload(bool dualWield)
    {
        if (!PrimaryAmmoFull(dualWield) && !ReserveAmmoEmpty() && !GetReloadState(dualWield) && !m_Animators.CheckWeaponInspect())
        {
            CombatInfo combatInfo = m_Controller.m_CombatInfo;
            StartCoroutine(Reload(dualWield));
            combatInfo.m_ShootingDuration = 0;
        }
    }

    private void Aim()
    {
        WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();
        Transform gunTransform = GetCurrentWeaponTransform();

        Vector3 aimPosition = Vector3.zero;
        aimPosition.x = (Screen.width / 2) + (-m_Controller.LookInput.x * weaponConfig.m_GunAimSwayStrength);
        aimPosition.y = (Screen.height / 2) + (-m_Controller.LookInput.y * weaponConfig.m_GunSwayStrength) - (m_Controller.transform.up.y * weaponConfig.m_GunAimHeight);
        //aimPosition.z = (m_Controller.transform.forward.z * weaponConfig.m_GunAimZPos) + weaponConfig.m_WeaponRecoilTransform.z * weaponConfig.m_ADSRecoilModifier;
        aimPosition.z = (Vector3.forward.z * weaponConfig.m_GunAimZPos) + weaponConfig.m_WeaponRecoilTransform.z * weaponConfig.m_ADSRecoilModifier;
        
        Vector3 centre = m_Camera.ScreenToWorldPoint(aimPosition);

        //Vector3 currentPosition = m_Gun.position;
        Vector3 currentPosition = weaponConfig.m_ScopeCentre.position;
        Vector3 requiredChange = centre - currentPosition;

        gunTransform.position += requiredChange * weaponConfig.m_GunAimSpeed * Time.deltaTime;
    }

    private Vector3 WeaponBob()
    {
        //Weapon currentWeapon = PlayerManager.GetCurrentWeapon();
        Vector3 localPosition = GetCurrentWeaponTransform().transform.position;
        Vector3 currentWeaponMidPoint = GetCurrentWeaponOriginalPos();

        if (m_Controller.m_MovInfo.m_IsMoving)
        {
            // Do weapon sway stuff.
            m_WeaponBobInfo.m_SwayTimer += Time.deltaTime;
            m_WeaponBobInfo.m_WaveSlice = -(Mathf.Sin(m_WeaponBobInfo.m_SwayTimer * m_WeaponBobInfo.m_BobSpeed) + 1) / 2;
            m_WeaponBobInfo.m_WaveSliceX = Mathf.Cos(m_WeaponBobInfo.m_SwayTimer * m_WeaponBobInfo.m_BobSpeed);

            if (m_WeaponBobInfo.m_WaveSlice >= -0.5f)
            {
                m_WeaponBobInfo.m_WaveSlice = -1 - -(Mathf.Sin(m_WeaponBobInfo.m_SwayTimer * m_WeaponBobInfo.m_BobSpeed) + 1) / 2;
            }

            float translateChangeX = m_WeaponBobInfo.m_WaveSliceX * m_WeaponBobInfo.m_BobDistance;
            float translateChangeY = m_WeaponBobInfo.m_WaveSlice * m_WeaponBobInfo.m_BobDistance;
            localPosition.y = /*currentWeaponMidPoint.y + */translateChangeY;
            localPosition.x = /*currentWeaponMidPoint.x + */translateChangeX;

            return localPosition;
            //currentWeapon.transform.localPosition = localPosition;
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// AddRecoil() adds to the visual recoil's. There is a visual recoil for the camera, 
    /// (this is seperate from the camera's recoil the causes a pattern), and a visual recoil for the gun model.
    /// </summary>
    private void AddVisualRecoil()
    {
        if (!GetCurrentWeaponConfig().m_DisableAllRecoil)
        {
            WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();

            CameraRecoil cameraRecoil = m_Controller.m_AccumulatedRecoil;

            Vector3 camVisRecoil = Vector3.zero;
            camVisRecoil.x = -weaponConfig.RecoilRotationAiming.x;
            camVisRecoil.y = Random.Range(-weaponConfig.RecoilRotationAiming.y, weaponConfig.RecoilRotationAiming.y);
            camVisRecoil.z = Random.Range(-weaponConfig.RecoilRotationAiming.z, weaponConfig.RecoilRotationAiming.z);

            //cameraRecoil.accumulatedVisualRecoil += new Vector3(-weaponConfig.RecoilRotationAiming.x, Random.Range(-weaponConfig.RecoilRotationAiming.y, weaponConfig.RecoilRotationAiming.y), Random.Range(-weaponConfig.RecoilRotationAiming.z, weaponConfig.RecoilRotationAiming.z));
            cameraRecoil.accumulatedVisualRecoil += camVisRecoil;

            Vector3 gunVisRecoil = Vector3.zero;
            gunVisRecoil.x = weaponConfig.m_WeaponRotRecoilVertStrength;
            gunVisRecoil.y = 0;
            gunVisRecoil.z = 0;
            //m_WeaponRecoilRot -= new Vector3(weaponConfig.m_WeaponRotRecoilVertStrength, 0, 0);
            m_RecoilInfo.m_WeaponRecoilRot -= gunVisRecoil;

            // Clamping because the gun recoils a bit to high currently.
            m_RecoilInfo.m_WeaponRecoilRot.x = Mathf.Clamp(m_RecoilInfo.m_WeaponRecoilRot.x, -weaponConfig.m_WeaponVisualRecoilClamp, weaponConfig.m_WeaponVisualRecoilClamp);

            Vector3 weaponPosChange = Vector3.zero;
            weaponPosChange.x = 0;
            weaponPosChange.y = 0;
            weaponPosChange.z = weaponConfig.m_WeaponTransformRecoilZStrength;
            weaponConfig.m_WeaponRecoilTransform -= weaponPosChange;

            // Although I am setting the recoil transform here, I have to apply it in the WeaponSway() function since I'm setting pos directly there. I want to change this but I'm unsure how right now
            //weaponConfig.m_WeaponRecoilTransform -= new Vector3(0, 0, weaponConfig.m_WeaponTransformRecoilZStrength);


        }
    }
    /// <summary>
    /// UpdateRecoil() should be called every frame and exponetially returns the visual recoils back to 0.
    /// </summary>
    private void UpdateRecoil()
    {
        WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();

        CameraRecoil cameraRecoil = m_Controller.m_AccumulatedRecoil;

        cameraRecoil.accumulatedVisualRecoil = Vector3.Lerp(cameraRecoil.accumulatedVisualRecoil, Vector3.zero, weaponConfig.m_CameraRecoilReturnSpeed * Time.deltaTime);
        m_RecoilInfo.m_WeaponRecoilRot = Vector3.Lerp(m_RecoilInfo.m_WeaponRecoilRot, Vector3.zero, weaponConfig.m_WeaponRecoilReturnSpeed * Time.deltaTime);

        weaponConfig.m_WeaponRecoilTransform = Vector3.Lerp(weaponConfig.m_WeaponRecoilTransform, Vector3.zero, weaponConfig.m_WeaponRecoilReturnSpeed * Time.deltaTime);
    }

    /// <summary>
    /// UpdateRecoilRecovery() handles returning the gun back to it's position before firing.
    /// </summary>
    private void UpdateRecoilRecovery()
    {
        CameraRecoil cameraRecoil = m_Controller.m_AccumulatedRecoil;
        CombatInfo combatInfo = m_Controller.m_CombatInfo;

        if (cameraRecoil.accumulatedPatternRecoilX > 0) // We only want to decrement AdditionCameraRecoilX if it has accumuluated recoil still in it.
        {
            // If we just keep decreasing the additional recoil until it reaches 0, it results in the camera going further down then what feels right.
            // This is because as the player is shooting, they are compensating and making the gun stand in place. While this is happening, the additional recoil could
            // build up to a high number and when the player stops shooting, the recoil will take a long time to get back to 0.

            // An experimental method I'd like to try is to either decrease it back to 0, or until the camera rotation is back to where it when they just started shooting.
            Vector2 currentCamX = Vector2.one;
            currentCamX.x = combatInfo.m_camForward.y;

            // I know I'm using the new keyword here and that's bad. But for now I'm trying to see if this system will work.
            Vector2 previousCamX = Vector2.one;
            previousCamX.x = combatInfo.m_PrevCamForward.y;


            float dot = Vector3.Dot(currentCamX.normalized, previousCamX.normalized);
            if (dot < 0.9999f || dot > 1.0001f) // Such a small difference in numbers still gives quite a generous margin for error.
            {
                // This means the current forward vector's y does not much the previous forward vector's y.
                // We have to do one of two things.
                // Either bring the gun down, so that the previous and current y components match.
                // Or if the gun is already below the previous y component, we just leave the gun alone because they've over compensated for the recoil.

                // If previous rotation's y is greater, it means they are looking further down then when they started firing.
                if (combatInfo.m_PrevCamForward.y > combatInfo.m_camForward.y)
                {
                    // We want to incorporate the additional camera recoil into the rotation of the camera, that way we can set the variable to 0 without worrying that later we will be moving the camera downwards.

                    // Because I'm setting the local rotation of the camera in the Look() function, it makes it kind of annoying to try and add/remove things to and from the rotation.
                    // Instead I will add the AdditionalCameraRecoilX into xRotation and then set AdditionalCameraRecoilX to 0. This way I don't have to directly touch the cameras local rotation
                    // here.

                    m_Controller.m_XRotation -= cameraRecoil.accumulatedPatternRecoilX;
                    cameraRecoil.accumulatedPatternRecoilX = 0;
                }
                else
                {
                    // Otherwise, they are aiming higher than when they started, so we'll bring the gun down to where it was.
                    cameraRecoil.accumulatedPatternRecoilX -= 1 * GetCurrentWeaponConfig().m_RecoilRecoveryModifier;
                    cameraRecoil.accumulatedPatternRecoilX = Mathf.Clamp(cameraRecoil.accumulatedPatternRecoilX, 0, 85f);
                }

            }
            else
            {
                // Since the forward vectors match, we'll clear the m_AdditionalCameraRecoilX variable just to keep things clean.
                m_Controller.m_XRotation -= cameraRecoil.accumulatedPatternRecoilX;
                cameraRecoil.accumulatedPatternRecoilX = 0;
            }
            // The rotation on the Y Axis. So this is if the player gets turned horizontally from the recoil.
            if (cameraRecoil.accumulatedPatternRecoilY != 0)
            {
                // I've decided not to lerp the additional horizontal recoil to 0 since it feels disorientating.

                // If we have accumulated horizontal recoil.
                //m_AdditionalCameraRecoilY -= 1 * GetCurrentWeaponConfig().m_RecoilRecoveryModifier;

                // Just incorporating the accumulated recoil into m_DesiredX to clean up AdditionalCameraRecoilY.
                m_Controller.m_DesiredX -= cameraRecoil.accumulatedPatternRecoilY;
                cameraRecoil.accumulatedPatternRecoilY = 0;
            }
        }
    }

    /// <summary>Defines whether or not the weapon can fire the next round.</summary>
    private bool ReadyToFire(bool dual = false)
    {
        float nextTime;
        if (dual)
            nextTime = m_NextTimeToFireLeft;
        else
            nextTime = m_NextTimeToFire;

        if (Time.time >= nextTime && !GetReloadState(dual))//(time.time or time.deltatime)
        {
            if (m_Animators.CheckWeaponInspect()) // If we are inspecting our weapon, the first time we shoot should cancel the animation and the second time should
            {                                     // allow us to shoot.
                if (!m_Animators.IsCancellingEquip) // Will prevent us from starting the coroutine when it's already started.
                    StartCoroutine(m_Animators.CancelWeaponInspect(0.4f, m_AudioEquipEvent));

                return false; // Not ready to fire until weapon inspect has been cancelled.
            }


            if (m_SemiAuto) // If the gun is semi auto, we have one other check to do.
            {
                // I've commented out this code so that the pistol can shoot as fast as the player can click.
                // Okay, nevermind apparently we need this animation cap.
                // Actually, nevermind that nevermind. I've added an any state transition to the firing for the pistol which
                // allows us to spam the animation without getting any desync.


                // To prevent people from being able to spam semi automatic guns really fast, I'm going to prevent them from firing unless the animation is complete.
                //if (!m_Animators.m_GunAnimators[0].GetCurrentAnimatorStateInfo(0).IsName("Idle")) // Only semi automatic weapons in the game are not dual wielded so we don't have to check the whole list of gun animators.
                //{
                //    return false;
                //}
               
            }
            

            // Defines the firing rate as rounds per minute (hard coded 60s)
            if(dual)
                m_NextTimeToFireLeft = Time.time + (60.0f / m_FireRate);
            else
                m_NextTimeToFire = Time.time + (60.0f / m_FireRate);
            return true;
        }

        return false;
    }

    /// <summary>Reloads the weapon over time.</summary>
    public IEnumerator Reload(bool special = false)
    {
        if (!GetReloadState(special))
        {
            StartReloadAnimation(special);
            PlayReloadSound();

            if (special)
                m_WeaponActions.m_IsReloadingLeft = true;
            else
                m_WeaponActions.m_IsReloading = true;

            // Before waiting you could invoke an animator here
            // to play a reload animation and an audio manager
            // to play a sound to go with it.

            // When triggering an animation it would also be pretty neat
            // to modify its playback speed to match the reload time.

            yield return new WaitForSeconds(m_ReloadTime);

            // I don't want to strip your hard work with the modulo
            // operations that you took the time to design so if you
            // wish to use your method then please do that.
            // The code below is just an example of how to simplify
            // the process so that there's little brain power needed
            // to remember what is happening behind the scenes.

            // Get how many rounds are needed to top up
            //int ammoRequired = m_MagazineSize - m_RoundsInMagazine;
            int ammoRequired;
            int reservePool;
            if (special)
            {
                ammoRequired = m_MagazineSize - m_RoundsInMagazineLeft;
                //reservePool = m_ReserveAmmoLeft;
                reservePool = m_ReserveAmmo;
            }
            else
            { 
                ammoRequired = m_MagazineSize - m_RoundsInMagazine;
                reservePool = m_ReserveAmmo;
            }

        

            // Check the size of the reserve pool
            if (reservePool <= ammoRequired)
            {
                // Update UI to only show one mag
                //m_UIManager.ModuloEqualsZero(m_RoundsInMagazine + m_ReserveAmmo);

                // Move all remaining ammo into magazine
                if (special)
                {
                    //m_RoundsInMagazineLeft += m_ReserveAmmoLeft;
                    m_RoundsInMagazineLeft += m_ReserveAmmo;
                    //m_ReserveAmmoLeft = 0;
                    m_ReserveAmmo = 0;
                }
                else 
                {
                    m_RoundsInMagazine += m_ReserveAmmo;
                    m_ReserveAmmo = 0;
                }
            }
            else
            {
                if ((m_RoundsInMagazine + m_ReserveAmmo) % m_MagazineSize == 0)
                {
                    // Total ammo equals an amount that when divided by magazine size, has no remainder therefore get rid of a mag UI element
                    //m_UIManager.ModuloEqualsZero(m_MagazineSize);
                }
                else
                {
                    // Removes bullet sprites total from 1 - 2 mags depending on the ammo missing from current magazine and how much ammo was already missing in the last magazine
                    //m_UIManager.RemoveAmmoFromLastAddToCurrent(m_MagazineSize);
                }

                // Move required amount from reserve to magazine
                if (special)
                {
                    m_RoundsInMagazineLeft += ammoRequired;
                    //m_ReserveAmmoLeft -= ammoRequired;
                    m_ReserveAmmo -= ammoRequired;
                }
                else
                { 
                    m_RoundsInMagazine += ammoRequired;
                    m_ReserveAmmo -= ammoRequired;
                }
            }

            SetFireTime(special); // Added so that if the player is holding down fire while reloading, they will begin firing at t=0. Without this the fire time is what is what when they
                           // originally started firing.
            if(m_Inventory.Owner.Possessed)
                m_UIManager?.UpdateWeaponUI(this);

            if (special)
                m_WeaponActions.m_IsReloadingLeft = false;
            else
                m_WeaponActions.m_IsReloading = false;
        }
    }

    private void UpdateSway(float x, float y)
    {
        // xAxis Quaternion is for the recoil kick upwards.

        WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();
        Transform gunTransform = GetCurrentWeaponTransform();
        if (!GetAimState() || GetReloadState())
        {
            Vector3 gunOriginalPos = GetCurrentWeaponOriginalPos();

            Vector3 bobStuff = Vector3.zero;
            if (m_Inventory.Owner.m_HostController.m_MovInfo.m_IsGrounded)
                bobStuff = WeaponBob();
            // Attempting to have minecraft hand like movement when falling. For now though, I'll leave it as I don't have much time left.

            //else
            //{
            //    if (m_Inventory.Owner.m_HostController.Rigidbody.velocity.y < 0) // This means we are falling.
            //    { 
            //        //bobStuff = -m_Inventory.Owner.m_HostController.Rigidbody.velocity;
            //        //bobStuff.x = Mathf.Clamp(bobStuff.x, -0.05f, 0.05f);
            //        bobStuff.y = Mathf.Clamp(-m_Inventory.Owner.m_HostController.Rigidbody.velocity.y, -0.05f, 0.05f);
            //        Debug.Log("We are falling.");
            //    }
            //}
            

            Vector3 finalPosition = Vector3.zero;
            finalPosition.x = Mathf.Clamp(-x * 0.02f, -weaponConfig.m_WeaponSwayClampX, weaponConfig.m_WeaponSwayClampX) + bobStuff.x;
            finalPosition.y = Mathf.Clamp((-y * 0.02f), -weaponConfig.m_WeaponSwayClampY, weaponConfig.m_WeaponSwayClampY) + bobStuff.y;
            finalPosition.z = weaponConfig.m_WeaponRecoilTransform.z;

            gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, finalPosition + gunOriginalPos, Time.deltaTime * weaponConfig.m_GunSwayReturn);
            Quaternion xAxis = Quaternion.AngleAxis(m_RecoilInfo.m_WeaponRecoilRot.x, Vector3.right);
            Quaternion zAxis = Quaternion.AngleAxis(Mathf.Clamp(-x, -weaponConfig.m_WeaponSwayRotateClamp, weaponConfig.m_WeaponSwayRotateClamp), Vector3.forward);
            gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, zAxis * xAxis, weaponConfig.m_WeaponSwayRotateSpeed);

            float currentFOV = m_Camera.fieldOfView;
            float desiredFOV = 60;

            if (!m_DualWield)
            { 
                float requiredChange = desiredFOV - currentFOV;
                m_Camera.fieldOfView += requiredChange * 0.45f;
            }

        }
        else if (GetAimState() && CanAim())
        {
            // Had to put the sway code with the Aim() function since it was easier to just add the neccessary values to the calculations over there rather than try and split up the equations.

            float currentFOV = m_Camera.fieldOfView;
            float desiredFOV = 40;

            float requiredChange = desiredFOV - currentFOV;

            if(!GetReloadState() && !m_DualWield) // Wont zoom in if we are reloading or if we are using a dual wielded weapon.
                m_Camera.fieldOfView += requiredChange * 0.45f * Time.deltaTime;



            // Quaternion rotate
            Quaternion zAxis = Quaternion.AngleAxis(Mathf.Clamp(-x, -weaponConfig.m_WeaponSwayRotateClamp, weaponConfig.m_WeaponSwayRotateClamp), Vector3.forward);
            Quaternion xAxis = Quaternion.AngleAxis(m_RecoilInfo.m_WeaponRecoilRot.x * weaponConfig.m_ADSRecoilModifier, Vector3.right);
            gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, zAxis * xAxis, weaponConfig.m_WeaponSwayRotateSpeed);
        }


    }

    private void UpdateRecoilTest()
    {
        // ============== EXPERIMENTAL RECOIL TESTING STUFF ============== //
        if (GetRecoilTestState())
        {
            if (!m_RecoilTesting.m_IsTestResting)
                m_WeaponActions.m_IsFiring = true;
            else
                m_WeaponActions.m_IsFiring = false;


            m_RecoilTesting.m_RecoilTestCounter += Time.deltaTime;
            if (!m_RecoilTesting.m_IsTestResting && m_RecoilTesting.m_RecoilTestCounter >= m_RecoilTesting.m_RecoilTestIntervals)
            {
                m_RecoilTesting.m_IsTestResting = true;
                
                m_RecoilTesting.m_RecoilTestCounter = 0.0f;
            }
            else if (m_RecoilTesting.m_IsTestResting && m_RecoilTesting.m_RecoilTestCounter >= m_RecoilTesting.m_RecoilTestRestTime) // This means were now counting the rest time.
            {
                CombatInfo combatInfo = m_Controller.m_CombatInfo;

                m_RecoilTesting.m_IsTestResting = false;
                m_RecoilTesting.m_RecoilTestCounter = 0.0f;
                m_Controller.m_XRotation = combatInfo.m_PrevXRot; // this might be unnessessary since the guns camera rotation goes back down through the recoil recovery system.
                m_Controller.m_Orientation.transform.eulerAngles = combatInfo.m_PrevOrientationRot;

                SetFireTime(); // This sequence of firing should be reset. It normally gets cancelled on mouse button up after firing.
            }
        }
    }

    /// <summary>
    /// SetFireTime() is a helper function that allows the HostController script to set the time when the player
    /// starts to hold down the mouse button.
    /// </summary>
    public void SetFireTime(bool special = false)
    {
        if (special)
            m_FireStartTimeLeft = Time.time;
        else
            m_FireStartTime = Time.time;
    }
    private void StartReloadAnimation(bool special)
    {
        if (special)
        {
            if (m_Animators.m_GunAnimators.Count > 0)
                m_Animators.m_GunAnimators[1].SetTrigger("OnReload");
            if (m_Animators.m_ArmsAnimators.Count > 0)
                m_Animators.m_ArmsAnimators[1].SetTrigger("OnReload");
        }

        else
        {
            // If we are not holding a dual wield and our weapon is the assault rifle, there
            // is a chance for the rare Pariah reload to happen. For testing I'll make it so
            // rifles only reload with Pariah's reload for now.
            //if (m_TypeTag == WEAPONTYPE.RIFLE)                                        
            //{
            //    m_Animators.m_GunAnimators[0].SetTrigger("OnPariahReload");
            //    m_Animators.m_ArmsAnimators[0].SetTrigger("OnPariahReload");                  // ============== NOTE ============== //
            //    GameManager.s_Instance?.m_Pariah.PlayArmAnim("OnPariahReload", true);         // This is commented out due to the animation
            //}                                                                                 // not working currently.
                                                                                                // ================================== //
            //else // We are reloading the pistol or one of the dual wield rifles.
            //{ 
            m_Animators.m_GunAnimators[0].SetTrigger("OnReload");
                m_Animators.m_ArmsAnimators[0].SetTrigger("OnReload");
            //}
        }
         
            
        
    }

    public void SetWeaponLayerRecursively(int layer)
    {
        // I'm not allowed to edit the agent prefab right now, so to get around the issue of
        // m_Config being set in Awake(), and the second and third guns not being Awoken.. until after the player
        // swaps to them, I'll just set them here. This is pretty bad but as soon as I can edit the Agent prefab I'll fix this properly.
        if (!m_Config)
        {
            m_Config = GetComponent<WeaponConfiguration>();
        }

        for (int i = 0; i < m_Config.m_Gun.Count; i++)
        { 
            Transform parent = m_Config.m_Gun[i];
            foreach (Transform trans in parent.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layer;
            }
        }
        for (int i = 0; i < m_Config.m_Arms.Count; i++)
        {
            Transform parent = m_Config.m_Arms[i];
            foreach (Transform trans in parent.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layer;
            }
        }
    }

    /// <summary>Refills both the primary and reserve ammo pools.</summary>
    public void RefillAmmo()
    {
        // This would be more of a debug function than
        // anything that would actually be use in game

        m_RoundsInMagazine = m_MagazineSize;
        m_ReserveAmmo = m_MaxReserveAmmo;
    }

    /// <summary>
    /// Sets the number of rounds available in the primary ammo pool.
    /// <br>NOTE: Will not exceed the specified magazine size.</br>
    /// </summary>
    public void SetPrimaryAmmoCount(int count)
    {
        m_RoundsInMagazine = Mathf.Clamp(count, 0, m_MagazineSize);
    }

    /// <summary>
    /// Sets the number of rounds available in the reserve ammo pool.
    /// <br>NOTE: Will not exceed the specified max reserve ammo.</br>
    /// </summary>
    public void SetReserveAmmoCount(int count)
    {
        m_ReserveAmmo = Mathf.Clamp(count, 0, m_MaxReserveAmmo);
    }

    /// <summary>Defines whether or not the weapon's magazine is full.</summary>
    public bool PrimaryAmmoFull(bool dual)
    {
        if (!dual)
            return m_RoundsInMagazine == m_MagazineSize;
        else
            return m_RoundsInMagazineLeft == m_MagazineSize;

    }

    /// <summary>Defines whether or not the weapon's reserve ammo pool is full.</summary>
    public bool ReserveAmmoFull()
    {
        // This function serves little purpose but it good to have for edge cases
        return m_ReserveAmmo == m_MaxReserveAmmo;
    }

    /// <summary>Defines whether or not the weapon's magazine is low.</summary>
    public bool PrimaryAmmoLow()
    {
        // Try to find a more elegant way to handle this
        // through exposed variables that Michael can play with.
        return m_RoundsInMagazine <= (int)(m_MagazineSize * m_LowAmmoWarningPercentage);
    }

    /// <summary>Defines whether or not the weapon's magazine is low and there is no reserve ammo.</summary>
    public bool TotalAmmoLow()
    {
        // Try to find a more elegant way to handle this
        // through exposed variables that Michael can play with.
        return m_RoundsInMagazine <= (int)(m_MagazineSize * m_LowAmmoWarningPercentage) * 0.5f;
    }

    /// <summary>Defines whether or not the weapon's magazine is empty and there is no reserve ammo.</summary>
    public bool TotalAmmoEmpty()
    {
        // Try to find a more elegant way to handle this
        // through exposed variables that Michael can play with.
        return m_RoundsInMagazine + m_ReserveAmmo <= 0;
    }

    /// <summary>Defines whether or not the weapon's magazine is empty.</summary>
    public bool PrimaryAmmoEmpty()
    {
        return m_RoundsInMagazine <= 0;
    }

    /// <summary>Defines whether or not the weapon's reserve ammo pool is empty.</summary>
    public bool ReserveAmmoEmpty(bool special = false)
    {
        if (special) // If special is true, check the left gun reserve ammo.
        { 
            //return m_ReserveAmmoLeft == 0;
            return m_ReserveAmmo <= 0;

        }

        return m_ReserveAmmo <= 0;
    }

    public int GetRoundsInMagazine(bool special = false)
    {
        if (special) // If special is true, return the rounds for the left gun.
            return m_RoundsInMagazineLeft;

        return m_RoundsInMagazine;
    }

    public int GetReserve(bool special = false)
    {
        if (special)
        { 
            //return m_ReserveAmmoLeft;
            return m_ReserveAmmo;
        }

        return m_ReserveAmmo;
    }

    private void PlayAnimations(bool special = false)
    {
        // Play effects.

        if (m_Inventory.Owner.Possessed) // If we're the ones in the agent.
        {
            if (special)
            {
                if (m_Particles.m_MuzzleFlashes.Count > 0)
                    m_Particles.m_MuzzleFlashes[1].Play();
                if (m_Particles.m_BulletCasings.Count > 0)
                    m_Particles.m_BulletCasings[1].Play();
                if (m_Animators.m_GunAnimators.Count > 0)
                    m_Animators.m_GunAnimators[1].SetTrigger("IsFiring");
                if (m_Animators.m_ArmsAnimators.Count > 0)
                    m_Animators.m_ArmsAnimators[1].SetTrigger("IsFiring");
            }
            else
            {
                m_Particles.m_MuzzleFlashes[0].Play();
                m_Particles.m_BulletCasings[0].Play();
                m_Animators.m_GunAnimators[0].SetTrigger("IsFiring");
                m_Animators.m_ArmsAnimators[0].SetTrigger("IsFiring");
            }
        }
        else // Otherwise, this is an AI agent.
        {
            m_Inventory.Owner.m_AIMuzzleFlash.Play(); // Play the AI's muzzle flash instead of the FPS one.
        }

        m_Inventory.Owner.PlayAnimation("Shoot", true);
    }

    private bool CanRecoilRecover()
    {
        if (m_DualWield)
        {
            // In the dual wield scenario, both guns must not be shooting, both must be reloading to enable recoil recovery.
            if ((!DualWieldIsShooting() && !DualWieldIsShooting(true)) || (GetReloadState() && GetReloadState(true)))
                return true;
            else
                return false;
        }
        else 
        {
            if (!GetFireState() || GetReloadState() || TotalAmmoEmpty())
                return true;
            else
                return false;
        }
    }

    private WeaponConfiguration GetCurrentWeaponConfig() { return m_Inventory.m_CurrentWeapon.m_Config; }
    private Transform GetCurrentWeaponTransform() => m_Inventory.m_CurrentWeapon.transform;
    private Vector3 GetCurrentWeaponOriginalPos() => m_Inventory.m_CurrentWeapon.m_TransformInfo.m_OriginalLocalPosition;
    public bool GetFireState(bool dual = false) 
    {
        if (dual)
            return m_WeaponActions.m_IsAiming; // m_IsAiming tracks whether the extra dual gun is firing.
        else
            return m_WeaponActions.m_IsFiring; 
    }
    /// <summary>
    /// IsShooting() is different from GetFireState() because GetFireState() returns whether the fire button is being held down whereas IsShooting() returns
    /// if the player is actively firing. Reloading is an example of where GetFireState() may return true but IsShooting() will return false.
    /// </summary>
    /// <param name="dual">If you are asking about the extra dual wield gun.</param>
    /// <returns></returns>
    public bool DualWieldIsShooting(bool dual = false)
    {
        if (dual)
        {
            if (GetFireState() && !GetReloadState(true))
                return true;
            else
                return false;
        }
        else
        {
            if (GetFireState(true) && !GetReloadState())
                return true;
            else
                return false;
        }
    }
    public void SetFireState(bool state) { m_WeaponActions.m_IsFiring = state; }
    public void ResetFire() { m_WeaponActions.m_IsFiring = false; }
    public bool GetReloadState(bool dual = false) 
    {
        if (dual)
            return m_WeaponActions.m_IsReloadingLeft;
        else
            return m_WeaponActions.m_IsReloading; 
    }
    public void ResetReload() { m_WeaponActions.m_IsReloading = false; }
    public bool IsReloading() { return m_WeaponActions.m_IsReloading; }
    public bool IsReloadingLeft() { return m_WeaponActions.m_IsReloadingLeft; }
    public bool GetAimState() { return m_WeaponActions.m_IsAiming; }
    public void ResetAim() { m_WeaponActions.m_IsAiming = false; }
    public bool GetRecoilTestState() { return m_RecoilTesting.m_IsRecoilTesting; }
    public bool CanFire(bool dual = false) 
    {
        if (dual)
        {
            return GetFireState() && !GetReloadState(dual)/* && !m_Animators.CheckWeaponInspect()*/; // The normal fire mode is the left gun in a dual wield scenario, so we must check if the left gun is not reloading.
        }
        else
            return (GetFireState() && !GetReloadState()/* && !m_Animators.CheckWeaponInspect()*/);
    }
    public bool CanAim() { return (GetAimState() && !GetReloadState() && !m_Animators.m_WeaponInspectAnimation); }
    public void SetCamera(Camera camera) { m_Camera = camera; }

    private void PlayFireSound()
    {
        if (m_AudioFireEvent)
            m_AudioFireEvent.Trigger();
    }

    private void PlayReloadSound()
    {
        if (m_AudioReloadEvent)
            m_AudioReloadEvent.Trigger();
    }

    private void PlayEmptyClipSound()
    {
        if (m_AudioEmptyClipEvent)
            m_AudioEmptyClipEvent.Trigger();
    }

    /// <summary>
    /// This gets called from GameManager when the player enters the unit for the first time.
    /// </summary>
    public void PlayEquipSound()
    {
        if (m_AudioEquipEvent)
            m_AudioEquipEvent.Trigger();
    }

    /// <summary>
    /// Used to stop all playing sounds. This is primarily to stop sounds from playing after the agent has been destroyed.
    /// </summary>
    public void StopSounds()
    {
        m_AudioEmptyClipEvent.StopSound(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if(m_AudioEquipEvent)
            m_AudioEquipEvent.StopSound(FMOD.Studio.STOP_MODE.IMMEDIATE);
        m_AudioReloadEvent.StopSound(FMOD.Studio.STOP_MODE.IMMEDIATE);
        m_AudioFireEvent.StopSound(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void PlayBulletEffect(bool isDualWield, bool hasHit, Vector3 direction)
    {
        m_Particles.PlayBulletEffect(isDualWield, hasHit, direction);
        
    }

	public void OnDrawGizmos()
	{
        // Only draw if we are in this agent.
        if (m_Inventory && m_Inventory.Owner && m_Inventory.Owner.Possessed)
        { 
            m_Particles.DrawBulletPFXGizmo();
            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_Camera.transform.position, m_Camera.transform.forward * 1000);
            Ray cameraRay = new Ray(m_Camera.transform.position, m_Camera.transform.forward);

            RaycastHit hit;
            if (Physics.Raycast(cameraRay, out hit, 1000))
            {

                Gizmos.DrawSphere(hit.point, 0.25f);
            }
        }

        if(m_Inventory.Owner && m_Inventory.Owner.Target)
            DrawAgentTarget(m_Inventory.Owner.Target.transform.position);
	}

    /// <summary>
    /// Used to turn the FPS gun on and off.
    /// </summary>
    /// <param name="toggle">Set to true if you want to show the arms and false if you want to hide them.</param>
    public void ToggleWeapon(bool toggle)
    {
        for (int i = 0; i < m_Animators.m_SkinnedMeshes.Count; i++)
        {
            // First we will hide all the arms.
            m_Animators.m_SkinnedMeshes[i].enabled = toggle;
        }
    }

    /// <summary>
    /// Temporary function to grab all the arm and gun skinned mesh renderers so that we have a reference to them.
    /// The reference helps us enable/disable these meshes when the player leaves and enters the host.
    /// This is temporary until I can work on the scientist/soldier prefabs.
    /// </summary>
    public void InitSkinnedMeshes()
    {
        // This whole function is relying on the first child of these animators be the skinned mesh renderer we're looking for.
        // Really bad but I can't change the soldier/scientist prefabs right now so this will have to do.

        m_InitialisedSkinnedMeshRenderers = true;

        for (int i = 0; i < m_Animators.m_ArmsAnimators.Count; i++)
        {
            // First we'll grab all the arms.
            SkinnedMeshRenderer mesh = m_Animators.m_ArmsAnimators[i].transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
            if (mesh == null) // If we couldn't find the skinned mesh renderer, log an error.
            {
                Debug.LogError("InitSkinnedMeshes could not find the skinned mesh renderer!");
                return;
            }
            else // Otherwise, if we have found it, add it to the collection.
            {
                m_Animators.m_SkinnedMeshes.Add(mesh);
            }
        }

        // Now we'll do the same for the guns.
        for (int i = 0; i < m_Animators.m_GunAnimators.Count; i++)
        {
            SkinnedMeshRenderer mesh = m_Animators.m_GunAnimators[i].transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();

            if (mesh == null)
            {
                Debug.LogError("InitSkinnedMeshes could not find the skinned mesh renderer!");
                return;
            }
            else // Otherwise, if we have found it, add it to the collection.
            {
                m_Animators.m_SkinnedMeshes.Add(mesh);
            }
        }

        // Now, presumably we have found all the required skinned mesh renderers.

    }

    /// <summary>
    /// Used to debug and visually see where agents are attempting to shoot at.
    /// </summary>
    /// <param name="targetPos">Target of the agent.</param>
    void DrawAgentTarget(Vector3 targetPos)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPos, 0.25f);
        Gizmos.DrawSphere(m_Inventory.Owner.m_FiringPosition.position, 0.25f);
        Gizmos.DrawLine(targetPos, m_Inventory.Owner.m_FiringPosition.position);
    }

    public void AddDamageCameraShake()
    {
        WeaponConfiguration weaponConfig = GetCurrentWeaponConfig();
        Vector3 cameraRot = m_Inventory.Owner.m_HostController.m_OnHitCameraShakeRotation;

        CameraRecoil cameraRecoil = m_Controller.m_AccumulatedRecoil;

        Vector3 camVisRecoil = Vector3.zero;
        camVisRecoil.x = -cameraRot.x;
        camVisRecoil.y = Random.Range(-cameraRot.y, -cameraRot.y);
        camVisRecoil.z = Random.Range(-cameraRot.z, -cameraRot.z);

        //cameraRecoil.accumulatedVisualRecoil += new Vector3(-weaponConfig.RecoilRotationAiming.x, Random.Range(-weaponConfig.RecoilRotationAiming.y, weaponConfig.RecoilRotationAiming.y), Random.Range(-weaponConfig.RecoilRotationAiming.z, weaponConfig.RecoilRotationAiming.z));
        cameraRecoil.accumulatedVisualRecoil += camVisRecoil;
    }
}
