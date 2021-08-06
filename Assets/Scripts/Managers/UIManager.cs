using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI m_AmmoDisplay;
    public TextMeshProUGUI m_AmmoWarning;
    public List<Magazine> m_Magazines;

    public void DisableMagazine()
    {
        m_Magazines[m_Magazines.Count - 1].gameObject.SetActive(false);
    }

    public void EnableBulletSprite(int i)
    {
        m_Magazines[0].BulletSprites[i].SetActive(true);
    }

    public void DisableBulletSpriteInLastMag(int i)
    {
        m_Magazines[m_Magazines.Count - 1].BulletSprites[i].SetActive(false);
    }

    public void DisableBulletSpriteInCurrentMag(int lastBullet)
    {
        m_Magazines[0].BulletSprites[lastBullet].SetActive(false);
    }

    public void RemoveMagazine()
    {
        m_Magazines.RemoveAt(m_Magazines.Count - 1);
    }

    public void TotalAmmoGreaterThanMagazine(int magazineSize)
    {
        RemoveAmmoFromLastAddToCurrent(magazineSize);

        int lastMagazineMissingAmmoCount = 0;

        for (int i = 0; i < magazineSize; i++)
        {
            if (LastMagBulletSpriteActive(i) == false)
            {
                lastMagazineMissingAmmoCount++;
            }
        }

        if (lastMagazineMissingAmmoCount == magazineSize)
        {
            RemoveMagazine();
        }
    }

    public bool FirstMagBulletSpriteActive(int i)
    {
        return m_Magazines[0].BulletSprites[i].activeSelf;
    }
    public bool LastMagBulletSpriteActive(int i)
    {
        return m_Magazines[m_Magazines.Count - 1].BulletSprites[i].activeSelf;
    }

    public void ModuloEqualsZero(int magazineSize)
    {
        DisableMagazine();
        for (int i = 0; i < magazineSize; i++)
        {
            EnableBulletSprite(i);
        }
        RemoveMagazine();
    }

    public void ModuloDoesNotEqualZero(int magazineSize, int ammoRequired, int lastMagMissingAmmo)
    {
        //int lastMagazineMissingAmmoCount = RemoveAmmoFromLastAddToCurrent(magazineSize);
        int lastMagazineMissingAmmoCount = lastMagMissingAmmo;
        Debug.Log(lastMagazineMissingAmmoCount);

        for (int i = magazineSize - 1; i > magazineSize - ammoRequired - 1; i--)
        {
            if (LastMagBulletSpriteActive(i) == false)
            {
                Debug.Log("this");
                lastMagazineMissingAmmoCount++;
            }
        }
        Debug.Log(lastMagazineMissingAmmoCount);
        if (lastMagazineMissingAmmoCount >= magazineSize)
        {
            DisableMagazine();
            RemoveMagazine();
            int excessBullets = lastMagazineMissingAmmoCount - magazineSize;
            Debug.Log(excessBullets);
            for (int i = magazineSize - 1; i > (magazineSize - 1 - excessBullets); i--)
            {
                DisableBulletSpriteInLastMag(i);
            }
        }
    }

    public int RemoveAmmoFromLastAddToCurrent(int magazineSize)
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
        Debug.Log(magazineSize - currentMagazineMissingAmmoCount - 1);
        for (int i = magazineSize - 1; i > magazineSize - currentMagazineMissingAmmoCount - 1; i--)
        {
            DisableBulletSpriteInLastMag(i);//
        }
        return lastMagazineMissingAmmoCount;
    }
}
