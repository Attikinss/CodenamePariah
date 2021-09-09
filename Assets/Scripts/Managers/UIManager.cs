using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Numerical Ammo Count UI")]
    private TextMeshProUGUI m_AmmoDisplay;

    [SerializeField]
    [Tooltip("On Screen Warning for Low Ammo and No Ammo")]
    private TextMeshProUGUI m_AmmoWarning;

    [SerializeField]
    [Tooltip("All the Magazine UI elements active on this character")]
    private List<Magazine> m_Magazines;

    [SerializeField]
    [Tooltip("All the Magazine UI elements active on this character")]
    private List<Magazine> m_PistolMagazines;

    [HideInInspector]
    public bool m_IsRifle = true;

    public static bool s_Hide = false;
    public Canvas m_Canvas;

    //private static UIManager s_Instance;
    //private void Awake()
    //{
    //       if (!s_Instance)
    //       {
    //           s_Instance = this;
    //       }
    //       else
    //       {
    //           Debug.LogWarning("UIManager already exists in scene!");
    //           Destroy(this);
    //       }
    //}

    /// <summary>
    /// HideMagazine() is a veeery temporary function just so the apporpriate magazines appear when the player switches weapons.
    /// </summary>
    /// <param name="rifle"></param>
    public void HideMagazine(bool rifle)
    {
        // When rifle is held, hide the pistol magazines.
        if (rifle)
        {
            for (int i = 0; i < m_Magazines.Count; i++)
            {
                m_Magazines[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < m_PistolMagazines.Count; i++)
            {
                m_PistolMagazines[i].gameObject.SetActive(false);
            }
        }
        // When pistol is held, hide the rifle magazines.
        else
        {
            for (int i = 0; i < m_Magazines.Count; i++)
            {
                m_Magazines[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < m_PistolMagazines.Count; i++)
            {
                m_PistolMagazines[i].gameObject.SetActive(true);
            }
        }
    }

    /// <summary>Disables the last magazine gameObject.</summary>
    public void DisableMagazine()
    {
        if (m_IsRifle)
            m_Magazines[m_Magazines.Count - 1].gameObject.SetActive(false);
        else
            m_PistolMagazines[m_PistolMagazines.Count - 1].gameObject.SetActive(false);
    }

    /// <summary>Enables bullets in current magazine.</summary>
    /// <param name="i"></param>
    public void EnableBulletSprite(int i)
    {
        if (m_IsRifle)
            m_Magazines[0].BulletSprites[i].SetActive(true);
        else
            m_PistolMagazines[0].BulletSprites[i].SetActive(true);
    }

    /// <summary>Disables bullets in last magazine.</summary>
    /// <param name="i"></param>
    public void DisableBulletSpriteInLastMag(int i)
    {
        if(m_IsRifle)
            m_Magazines[m_Magazines.Count - 1].BulletSprites[i].SetActive(false);
        else
            m_PistolMagazines[m_PistolMagazines.Count - 1].BulletSprites[i].SetActive(false);
    }

    /// <summary>Disables bullets in current magazine.</summary>
    /// <param name="lastBullet"></param>
    public void DisableBulletSpriteInCurrentMag(int lastBullet)
    {
        if(m_IsRifle)
            m_Magazines[0].BulletSprites[lastBullet].SetActive(false);
        else
            m_PistolMagazines[0].BulletSprites[lastBullet].SetActive(false);
    }

    /// <summary>Removes last magazine from the array.</summary>
    public void RemoveMagazine()
    {
        if(m_IsRifle)
            m_Magazines.RemoveAt(m_Magazines.Count - 1);
        else
            m_PistolMagazines.RemoveAt(m_PistolMagazines.Count - 1);
    }

    //public void TotalAmmoGreaterThanMagazine(int magazineSize)
    //{
    //    RemoveAmmoFromLastAddToCurrent(magazineSize);
    //
    //    int lastMagazineMissingAmmoCount = 0;
    //
    //    for (int i = 0; i < magazineSize; i++)
    //    {
    //        if (LastMagBulletSpriteActive(i) == false)
    //        {
    //            lastMagazineMissingAmmoCount++;
    //        }
    //    }
    //
    //    if (lastMagazineMissingAmmoCount == magazineSize)
    //    {
    //        RemoveMagazine();
    //    }
    //}

    /// <summary>Checks if bullet sprites in first magazine are active.</summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public bool FirstMagBulletSpriteActive(int i)
    {
        if(m_IsRifle)
            return m_Magazines[0].BulletSprites[i].activeSelf;
        else
            return m_PistolMagazines[0].BulletSprites[i].activeSelf;
    }

    /// <summary>Checks if bullet sprites in last magazine are active.</summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public bool LastMagBulletSpriteActive(int i)
    {
        if(m_IsRifle)
            return m_Magazines[m_Magazines.Count - 1].BulletSprites[i].activeSelf;
        else
            return m_PistolMagazines[m_PistolMagazines.Count - 1].BulletSprites[i].activeSelf;
    }

    /// <summary>Activates if (total ammo % magazineSize = 0).</summary>
    /// <param name="magazineSize"></param>
    public void ModuloEqualsZero(int magazineSize)
    {
        DisableMagazine();

        for (int i = 0; i < magazineSize; i++)
            EnableBulletSprite(i);

        RemoveMagazine();
    }

    /// <summary>Activates when reserve ammo is greater than magazine size and modulo =/= 0.</summary>
    /// <param name="magazineSize"></param>
    public void RemoveAmmoFromLastAddToCurrent(int magazineSize)
    {
        int currentMagazineMissingAmmoCount = 0;
        int lastMagazineMissingAmmoCount = 0;

        for (int i = 0; i < magazineSize; i++)
        {
            if (FirstMagBulletSpriteActive(i) == false)
                currentMagazineMissingAmmoCount++;

            if (LastMagBulletSpriteActive(i) == false)
                lastMagazineMissingAmmoCount++;
        }

        for (int i = 0; i < magazineSize; i++)
            EnableBulletSprite(i);

        //Debug.Log(magazineSize - (currentMagazineMissingAmmoCount + lastMagazineMissingAmmoCount) - 1);
        if (magazineSize - (currentMagazineMissingAmmoCount + lastMagazineMissingAmmoCount) - 1 >= 0)
        {
            for (int i = magazineSize - 1; i > magazineSize - (currentMagazineMissingAmmoCount + lastMagazineMissingAmmoCount) - 1; i--)
                DisableBulletSpriteInLastMag(i);
        }
        else
        {
            DisableMagazine();
            RemoveMagazine();

            int excessBullets = lastMagazineMissingAmmoCount + currentMagazineMissingAmmoCount - magazineSize;
            //Debug.Log(excessBullets);

            for (int i = magazineSize - 1; i > magazineSize - (excessBullets) - 1; i--)
                DisableBulletSpriteInLastMag(i);
        }
    }

    public void UpdateWeaponUI(Weapon weapon)
    {
        if (s_Hide)
            m_Canvas.enabled = false;
        else
            m_Canvas.enabled = true;


        if (!m_AmmoDisplay) return;

        if (weapon.TotalAmmoEmpty())
            DisableMagazine();

        m_AmmoWarning?.SetText("");
        if (weapon.TotalAmmoEmpty())
            m_AmmoWarning?.SetText("No Ammo");

        int currentRounds = weapon.GetRoundsInMagazine();
        int reserveRounds = weapon.GetReserve();

        string first = currentRounds >= 10 ? currentRounds.ToString() : $"0{currentRounds}";
        string second = reserveRounds >= 10 ? reserveRounds.ToString() : $"0{reserveRounds}";
        
        m_AmmoDisplay.SetText($"{first} / {second}");
    }
}
