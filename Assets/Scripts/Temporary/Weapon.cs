using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The character's camera.")]
    private Camera m_Camera;

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
    [Tooltip("Whether gun is currently reloading.")]
    private bool m_IsReloading = false;

    [SerializeField]
    [Tooltip("The percentage of a full magazine required before warning to reload.")]
    private float m_LowAmmoWarningPercentage = 0.3f;

    /// <summary>An accumulative value used to determine when the next round should be fired.</summary>
    private float m_NextTimeToFire = 0f;




    public float m_BulletDamage;
    public GameObject m_HitDecal;

    public AnimationCurve m_VerticalRecoil;

    // ============= INTERNAL BOOKKEEPING ============= //
    [HideInInspector]
    public float m_VerticalProgress = 0.0f; // To track position on the recoil curve.

    [HideInInspector]
    public Vector3 m_OriginalLocalPosition; // Public so that HostController.cs can access it when lerping back to the weapons original pos.
    [HideInInspector]
    public Vector3 m_OriginalGlobalPosition;

	// ================================================ //


	private void Awake()
	{
        m_OriginalLocalPosition = transform.localPosition;
        m_OriginalGlobalPosition = transform.position;
	}

    // Update is called once per frame
    void Update()
    {
        GetComponentInParent<UIManager>().DisplayInventory();



        // ========================= DECREMENTING AMMO WHEN SHOOTING ========================= //

        // TEMP FIX so that other characters with this script dont have their ammo decrement when the player clicks.
        //if (m_Camera.gameObject.activeSelf == true)
        //{
        //// need to make sure this is only active when possessing
        //if (Mouse.current.leftButton.isPressed && this.CompareTag("AssaultRifle"))
        //{
        //    Fire();
        //}
        //
        // need to make sure this is only active when possessing

        // =================================================================================== //


        

        if (Mouse.current.leftButton.isPressed) //waspressedthisframe for semi auto
        {
            Fire();
        }
        else if (Keyboard.current.rKey.wasPressedThisFrame) //
        {
            if (!PrimaryAmmoFull() && !ReserveAmmoEmpty() && !m_IsReloading)
                StartCoroutine(Reload());
        }
    }

    public void Fire()
    {
        // In this function you can do a few invocations to different systems
        // For example whenever a sound needs to be played, you can call into
        // the audio manager to play the appropriate sound.

        if (ReadyToFire())
        {
            if (m_RoundsInMagazine > 0)
            {
                // Currently gets rid of bullet sprite before UI has fully updated //
                this.GetComponentInParent<UIManager>().DisableBulletSpriteInCurrentMag(m_RoundsInMagazine - 1);
                m_RoundsInMagazine--;

            }
            else
            {
                // Do nothing / reload automatically
                if (!ReserveAmmoEmpty())
                    StartCoroutine(Reload());
                //else if (TotalAmmoEmpty())
                //    this.GetComponentInParent<UIManager>().DisableMagazine();
            }
        }
    }

    /// <summary>Defines whether or not the weapon can fire the next round.</summary>
    private bool ReadyToFire()
    {
        if (Time.time >= m_NextTimeToFire && !m_IsReloading)//(time.time or time.deltatime)
        {
            // Defines the firing rate as rounds per minute (hard coded 60s)
            m_NextTimeToFire = Time.time + (60.0f / m_FireRate);
            return true;
        }

        return false;
    }

    /// <summary>Reloads the weapon over time.</summary>
    public IEnumerator Reload()
    {
        m_IsReloading = true;

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
        int ammoRequired = m_MagazineSize - m_RoundsInMagazine;

        // Check the size of the reserve pool
        if (m_ReserveAmmo <= ammoRequired)
        {
            // Update UI to only show one mag
            this.GetComponentInParent<UIManager>().ModuloEqualsZero(m_RoundsInMagazine + m_ReserveAmmo);

            // Move all remaining ammo into magazine
            m_RoundsInMagazine += m_ReserveAmmo;
            m_ReserveAmmo = 0;
        }
        else
        {
            if ((m_RoundsInMagazine + m_ReserveAmmo) % m_MagazineSize == 0)
            {
                // Total ammo equals an amount that when divided by magazine size, has no remainder therefore get rid of a mag UI element
                this.GetComponentInParent<UIManager>().ModuloEqualsZero(m_MagazineSize);
            }
            else
            {
                // Removes bullet sprites total from 1 - 2 mags depending on the ammo missing from current magazine and how much ammo was already missing in the last magazine
                this.GetComponentInParent<UIManager>().RemoveAmmoFromLastAddToCurrent(m_MagazineSize);
            }

            // Move required amount from reserve to magazine
            m_RoundsInMagazine += ammoRequired;
            m_ReserveAmmo -= ammoRequired;
        }

        m_IsReloading = false;
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
    public bool PrimaryAmmoFull()
    {
        return m_RoundsInMagazine == m_MagazineSize;
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
        return m_RoundsInMagazine == (int)(m_MagazineSize * m_LowAmmoWarningPercentage);
    }

    /// <summary>Defines whether or not the weapon's magazine is low and there is no reserve ammo.</summary>
    public bool TotalAmmoLow()
    {
        // Try to find a more elegant way to handle this
        // through exposed variables that Michael can play with.
        return m_RoundsInMagazine == (int)(m_MagazineSize * m_LowAmmoWarningPercentage) * 0.5f;
    }

    /// <summary>Defines whether or not the weapon's magazine is empty and there is no reserve ammo.</summary>
    public bool TotalAmmoEmpty()
    {
        // Try to find a more elegant way to handle this
        // through exposed variables that Michael can play with.
        return m_RoundsInMagazine + m_ReserveAmmo == 0;
    }

    /// <summary>Defines whether or not the weapon's magazine is empty.</summary>
    public bool PrimaryAmmoEmpty()
    {
        return m_RoundsInMagazine == 0;
    }

    /// <summary>Defines whether or not the weapon's reserve ammo pool is empty.</summary>
    public bool ReserveAmmoEmpty()
    {
        return m_ReserveAmmo == 0;
    }

    public int GetRoundsInMagazine()
    {
        return m_RoundsInMagazine;
    }

    public int GetReserve()
    {
        return m_ReserveAmmo;
    }
}
