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

    /// <summary>Disables the last magazine gameObject.</summary>
    public void DisableMagazine()
    {
        m_Magazines[m_Magazines.Count - 1].gameObject.SetActive(false);
    }

    /// <summary>Enables bullets in current magazine.</summary>
    /// <param name="i"></param>
    public void EnableBulletSprite(int i)
    {
        m_Magazines[0].BulletSprites[i].SetActive(true);
    }

    /// <summary>Disables bullets in last magazine.</summary>
    /// <param name="i"></param>
    public void DisableBulletSpriteInLastMag(int i)
    {
        m_Magazines[m_Magazines.Count - 1].BulletSprites[i].SetActive(false);
    }

    /// <summary>Disables bullets in current magazine.</summary>
    /// <param name="lastBullet"></param>
    public void DisableBulletSpriteInCurrentMag(int lastBullet)
    {
        m_Magazines[0].BulletSprites[lastBullet].SetActive(false);
    }

    /// <summary>Removes last magazine from the array.</summary>
    public void RemoveMagazine()
    {
        m_Magazines.RemoveAt(m_Magazines.Count - 1);
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
        return m_Magazines[0].BulletSprites[i].activeSelf;
    }

    /// <summary>Checks if bullet sprites in last magazine are active.</summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public bool LastMagBulletSpriteActive(int i)
    {
        return m_Magazines[m_Magazines.Count - 1].BulletSprites[i].activeSelf;
    }

    /// <summary>Activates if (total ammo % magazineSize = 0).</summary>
    /// <param name="magazineSize"></param>
    public void ModuloEqualsZero(int magazineSize)
    {
        DisableMagazine();
        for (int i = 0; i < magazineSize; i++)
        {
            EnableBulletSprite(i);
        }
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
            {
                currentMagazineMissingAmmoCount++;
            }
            if (LastMagBulletSpriteActive(i) == false)
            {
                lastMagazineMissingAmmoCount++;
            }
        }

        for (int i = 0; i < magazineSize; i++)
        {
            EnableBulletSprite(i);
        }
        Debug.Log(magazineSize - (currentMagazineMissingAmmoCount + lastMagazineMissingAmmoCount) - 1);
        if (magazineSize - (currentMagazineMissingAmmoCount + lastMagazineMissingAmmoCount) - 1 >= 0)
        {
            for (int i = magazineSize - 1; i > magazineSize - (currentMagazineMissingAmmoCount + lastMagazineMissingAmmoCount) - 1; i--)
            {
                DisableBulletSpriteInLastMag(i);
            }
        }
        else
        {
            DisableMagazine();
            RemoveMagazine();
            int excessBullets = lastMagazineMissingAmmoCount + currentMagazineMissingAmmoCount - magazineSize;
            Debug.Log(excessBullets);
            for (int i = magazineSize - 1; i > magazineSize - (excessBullets) - 1; i--)
            {
                DisableBulletSpriteInLastMag(i);
            }
        }
    }

    public void DisplayInventory()
    {
        if (GetComponentInChildren<Weapon>().TotalAmmoEmpty())
        {
            DisableMagazine();
        }

        m_AmmoWarning.text = "";
        if (GetComponentInChildren<Weapon>().TotalAmmoEmpty())
        {
            m_AmmoWarning.text = "";
            m_AmmoWarning.text = "No Ammo";
        }

        m_AmmoDisplay.text = "";

        m_AmmoDisplay.text += string.Format("{0:D2} / {1:D2}", GetComponentInChildren<Weapon>().GetRoundsInMagazine(), GetComponentInChildren<Weapon>().GetReserve());//set a gameobject in inspector to avoid getcomponent
    }
}