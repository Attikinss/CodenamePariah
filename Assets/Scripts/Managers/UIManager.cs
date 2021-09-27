using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Numerical Ammo Count UI")]
    private TextMeshProUGUI m_AmmoDisplay;

    [SerializeField]
    [Tooltip("Numerical Left Gun Ammo Count UI")]
    private TextMeshProUGUI m_AmmoDisplayLeft;

    [SerializeField]
    [Tooltip("On Screen Warning for Low Ammo and No Ammo")]
    private TextMeshProUGUI m_AmmoWarning;

    //[SerializeField]
    //[Tooltip("All the Magazine UI elements active on this character")]
    //private List<Magazine> m_Magazines;

    //[SerializeField]
    //[Tooltip("All the Magazine UI elements active on this character")]
    //private List<Magazine> m_PistolMagazines;

    //[SerializeField]
    //[Tooltip("The dual wielding Magazine UI elements.")]
    //private List<Magazine> m_DualMagazines;

    //[HideInInspector]
    ////public bool m_IsRifle = true;
    //// Because I'm adding dual weapons I can't rely on this temporary bool, so I'll make a new temporary enum instead!
    //public WEAPONTYPE m_CurrentWeaponType = WEAPONTYPE.RIFLE;

    public static bool s_Hide = false;
    public Canvas m_Canvas;


    //// These are super temporary. We really should rework the ui system.
    //// I'm only bothering for assault rifle because the pistol is semi automatic and doesn't really call set active that much.
    //// Might have to do it for the dual wield weapon but that's still being developed anyway.
    //public List<Image> m_AssaultMag1Bullets = new List<Image>();
    //public List<Image> m_AssaultMag2Bullets = new List<Image>();
    //public List<Image> m_AssaultMag3Bullets = new List<Image>();
    //// same for dual wielded weapon.
    //public List<Image> m_DualMag1Bullets = new List<Image>();
    //public List<Image> m_DualMag2Bullets = new List<Image>();
    //public List<Image> m_DualMag3Bullets = new List<Image>();

    public static UIManager s_Instance;

    [HideInInspector]
    public Inventory m_Inv;

    [SerializeField]
    [Tooltip("Health Text UI.")]
    private TextMeshProUGUI m_HealthText;

	[SerializeField]
	[Tooltip("Health Sprite UI")]
	private GameObject m_HealthSprite;

    [SerializeField]
    [Tooltip("Death Incarnate Bar UI")]
    private Image m_DeathIncarnateBar;

	public void Awake()
	{
        if (!s_Instance)
        {
            s_Instance = this;
        }
        else
        {
            Debug.LogWarning("UIManager already exists in scene!");
            Destroy(this);
        }


        //// Temporarily getting all the Bullet Image components because I've discovered setting game objects to false is slow if you do it a lot of times.
        //// Also I'm getting the components through code so I don't change the player prefab.
        //for (int i = 0; i < m_Magazines[0].BulletSprites.Length; i++) // Very risky only checking the first magazine and trusting all the other magazines will have the same amount
        //{                                                             // of bullet sprites but this is temporary so whatever.

        //    m_AssaultMag1Bullets.Add(m_Magazines[0].BulletSprites[i].GetComponent<Image>());
        //    m_AssaultMag2Bullets.Add(m_Magazines[1].BulletSprites[i].GetComponent<Image>());
        //    m_AssaultMag3Bullets.Add(m_Magazines[2].BulletSprites[i].GetComponent<Image>());
        //}

        //// Same temporary fix for dual wielded weapon.
        //for (int i = 0; i < m_DualMagazines[0].BulletSprites.Length; i++)
        //{
        //    m_DualMag1Bullets.Add(m_DualMagazines[0].BulletSprites[i].GetComponent<Image>());
        //    m_DualMag2Bullets.Add(m_DualMagazines[1].BulletSprites[i].GetComponent<Image>());
        //    m_DualMag3Bullets.Add(m_DualMagazines[2].BulletSprites[i].GetComponent<Image>());
        //}
	}

	/// <summary>
	/// HideMagazine() is a veeery temporary function just so the apporpriate magazines appear when the player switches weapons.
	/// </summary>
	/// <param name="rifle"></param>
	//public void HideMagazine(WEAPONTYPE type)
 //   {
 //       //// When rifle is held, hide the pistol magazines.
 //       //if (type == WEAPONTYPE.RIFLE)
 //       //{
 //       //    for (int i = 0; i < m_Magazines.Count; i++)
 //       //    {
 //       //        m_Magazines[i].gameObject.SetActive(true);
 //       //    }
 //       //    for (int i = 0; i < m_PistolMagazines.Count; i++)
 //       //    {
 //       //        m_PistolMagazines[i].gameObject.SetActive(false);
 //       //    }
 //       //    for (int i = 0; i < m_DualMagazines.Count; i++)
 //       //    {
 //       //        m_DualMagazines[i].gameObject.SetActive(false);
 //       //    }
 //       //}
 //       //// When pistol is held, hide the rifle magazines.
 //       //else if (type == WEAPONTYPE.PISTOL)
 //       //{
 //       //    for (int i = 0; i < m_Magazines.Count; i++)
 //       //    {
 //       //        m_Magazines[i].gameObject.SetActive(false);
 //       //    }
 //       //    for (int i = 0; i < m_PistolMagazines.Count; i++)
 //       //    {
 //       //        m_PistolMagazines[i].gameObject.SetActive(true);
 //       //    }
 //       //    for (int i = 0; i < m_DualMagazines.Count; i++)
 //       //    {
 //       //        m_DualMagazines[i].gameObject.SetActive(false);
 //       //    }
 //       //}
 //       //else if (type == WEAPONTYPE.DUAL)
 //       //{
 //       //    for (int i = 0; i < m_Magazines.Count; i++)
 //       //        m_Magazines[i].gameObject.SetActive(false);
 //       //    for (int i = 0; i < m_PistolMagazines.Count; i++)
 //       //        m_PistolMagazines[i].gameObject.SetActive(false);
 //       //    for (int i = 0; i < m_DualMagazines.Count; i++)
 //       //        m_DualMagazines[i].gameObject.SetActive(true);
 //       //}
 //   }

    /// <summary>Disables the last magazine gameObject.</summary>
    //public void DisableMagazine()
    //{
    //    //if (m_CurrentWeaponType == WEAPONTYPE.RIFLE)
    //    //    m_Magazines[m_Magazines.Count - 1].gameObject.SetActive(false);
    //    //else if (m_CurrentWeaponType == WEAPONTYPE.PISTOL)
    //    //    m_PistolMagazines[m_PistolMagazines.Count - 1].gameObject.SetActive(false);
    //    //else if (m_CurrentWeaponType == WEAPONTYPE.DUAL)
    //    //    m_DualMagazines[m_DualMagazines.Count - 1].gameObject.SetActive(false);

    //}

    /// <summary>Enables bullets in current magazine.</summary>
    /// <param name="i"></param>
    //public void EnableBulletSprite(int i)
    //{
    //    if (m_CurrentWeaponType == WEAPONTYPE.RIFLE)
    //    {
    //        // Because of my lag fix of using .enabled rather than SetActive, we have to .enable = true here just incase.
    //        m_Magazines[0].BulletSprites[i].SetActive(true);
    //        m_AssaultMag1Bullets[i].enabled = true;
    //    }
    //    else if (m_CurrentWeaponType == WEAPONTYPE.PISTOL)
    //        m_PistolMagazines[0].BulletSprites[i].SetActive(true);
    //    else if (m_CurrentWeaponType == WEAPONTYPE.DUAL)
    //    { 
    //        m_DualMagazines[0].BulletSprites[i].SetActive(true);
    //        // Same temporary fix like for the rifle.
    //        m_DualMag1Bullets[i].enabled = true;
    //    }
    //}

    /// <summary>Disables bullets in last magazine.</summary>
    /// <param name="i"></param>
    //public void DisableBulletSpriteInLastMag(int i)
    //{
    //    if (m_CurrentWeaponType == WEAPONTYPE.RIFLE)
    //    {
    //        m_Magazines[m_Magazines.Count - 1].BulletSprites[i].SetActive(false);
    //        m_AssaultMag3Bullets[i].enabled = false;
    //    }
    //    else if (m_CurrentWeaponType == WEAPONTYPE.PISTOL)
    //        m_PistolMagazines[m_PistolMagazines.Count - 1].BulletSprites[i].SetActive(false);
    //    else if (m_CurrentWeaponType == WEAPONTYPE.DUAL)
    //    { 
    //        m_DualMagazines[m_DualMagazines.Count - 1].BulletSprites[i].SetActive(false);
    //        m_DualMag3Bullets[i].enabled = false;
    //    }
    //}

    /// <summary>Disables bullets in current magazine.</summary>
    /// <param name="lastBullet"></param>
    //public void DisableBulletSpriteInCurrentMag(int lastBullet)
    //{
    //    if (m_CurrentWeaponType == WEAPONTYPE.RIFLE)
    //    //m_Magazines[0].BulletSprites[lastBullet].SetActive(false);
    //    {
    //        m_AssaultMag1Bullets[lastBullet].enabled = false; // Much faster than SetActive(), atleast in the editor.
    //    }
    //    else if (m_CurrentWeaponType == WEAPONTYPE.PISTOL)
    //        m_PistolMagazines[0].BulletSprites[lastBullet].SetActive(false);
    //    else if (m_CurrentWeaponType == WEAPONTYPE.DUAL)
    //    //    m_DualMagazines[0].BulletSprites[lastBullet].SetActive(false);
    //    {
    //        // Same fix like for rifle. .enabled is much faster than SetActive().
    //        m_DualMag1Bullets[lastBullet].enabled = false;
    //    }
    //}

    /// <summary>Removes last magazine from the array.</summary>
    //public void RemoveMagazine()
    //{
    //    if (m_CurrentWeaponType == WEAPONTYPE.RIFLE)
    //        m_Magazines.RemoveAt(m_Magazines.Count - 1);
    //    else if (m_CurrentWeaponType == WEAPONTYPE.PISTOL)
    //        m_PistolMagazines.RemoveAt(m_PistolMagazines.Count - 1);
    //    else if (m_CurrentWeaponType == WEAPONTYPE.DUAL)
    //        m_DualMagazines.RemoveAt(m_DualMagazines.Count - 1);
    //}

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
    //public bool FirstMagBulletSpriteActive(int i)
    //{
    //    if (m_CurrentWeaponType == WEAPONTYPE.RIFLE)
    //    {
    //        return m_AssaultMag1Bullets[i].enabled;
    //        //return m_Magazines[0].BulletSprites[i].activeSelf;
    //    }
    //    else if (m_CurrentWeaponType == WEAPONTYPE.PISTOL)
    //        return m_PistolMagazines[0].BulletSprites[i].activeSelf;
    //    else if (m_CurrentWeaponType == WEAPONTYPE.DUAL)
    //    {
    //        return m_DualMag1Bullets[i].enabled;
    //        //return m_DualMagazines[0].BulletSprites[i].activeSelf;
    //    }
    //    else
    //        return false; // eh not really error checking but whateva!
    //}

    /// <summary>Checks if bullet sprites in last magazine are active.</summary>
    /// <param name="i"></param>
    /// <returns></returns>
    //public bool LastMagBulletSpriteActive(int i)
    //{
    //    if (m_CurrentWeaponType == WEAPONTYPE.RIFLE)
    //    {
    //        return m_AssaultMag3Bullets[i].enabled;
    //        //return m_Magazines[m_Magazines.Count - 1].BulletSprites[i].activeSelf;
    //    }
    //    else if (m_CurrentWeaponType == WEAPONTYPE.PISTOL)
    //        return m_PistolMagazines[m_PistolMagazines.Count - 1].BulletSprites[i].activeSelf;
    //    else if (m_CurrentWeaponType == WEAPONTYPE.DUAL)
    //    {
    //        return m_DualMag3Bullets[i].enabled;
    //        //return m_DualMagazines[m_DualMagazines.Count - 1].BulletSprites[i].activeSelf;
    //    }
    //    else
    //        return false;
    //}

    /// <summary>Activates if (total ammo % magazineSize = 0).</summary>
    /// <param name="magazineSize"></param>
    //public void ModuloEqualsZero(int magazineSize)
    //{
    //    DisableMagazine();

    //    for (int i = 0; i < magazineSize; i++)
    //        EnableBulletSprite(i);

    //    RemoveMagazine();
    //}

    /// <summary>Activates when reserve ammo is greater than magazine size and modulo =/= 0.</summary>
    /// <param name="magazineSize"></param>
    //public void RemoveAmmoFromLastAddToCurrent(int magazineSize)
    //{
    //    int currentMagazineMissingAmmoCount = 0;
    //    int lastMagazineMissingAmmoCount = 0;

    //    for (int i = 0; i < magazineSize; i++)
    //    {
    //        if (FirstMagBulletSpriteActive(i) == false)
    //            currentMagazineMissingAmmoCount++;

    //        if (LastMagBulletSpriteActive(i) == false)
    //            lastMagazineMissingAmmoCount++;
    //    }

    //    for (int i = 0; i < magazineSize; i++)
    //        EnableBulletSprite(i);

    //    //Debug.Log(magazineSize - (currentMagazineMissingAmmoCount + lastMagazineMissingAmmoCount) - 1);
    //    if (magazineSize - (currentMagazineMissingAmmoCount + lastMagazineMissingAmmoCount) - 1 >= 0)
    //    {
    //        for (int i = magazineSize - 1; i > magazineSize - (currentMagazineMissingAmmoCount + lastMagazineMissingAmmoCount) - 1; i--)
    //            DisableBulletSpriteInLastMag(i);
    //    }
    //    else
    //    {
    //        DisableMagazine();
    //        RemoveMagazine();

    //        int excessBullets = lastMagazineMissingAmmoCount + currentMagazineMissingAmmoCount - magazineSize;
    //        //Debug.Log(excessBullets);

    //        for (int i = magazineSize - 1; i > magazineSize - (excessBullets) - 1; i--)
    //            DisableBulletSpriteInLastMag(i);
    //    }
    //}

    public void UpdateWeaponUI(Weapon weapon)
    {
        if (!weapon)
        {
            // There is no weapon to display, so hide the canvas.
            HideCanvas();
        }
        else
        {
            if (!m_Canvas.gameObject.activeSelf)
                UnhideCanvas(); // If it was hidden for some reason, unhide it.
            if (!m_AmmoDisplay) return;

            //if (weapon.TotalAmmoEmpty())
            //    DisableMagazine();

            m_AmmoWarning?.SetText("");
            if (weapon.TotalAmmoEmpty())
                m_AmmoWarning?.SetText("No Ammo");

            int currentRounds = weapon.GetRoundsInMagazine();
            int reserveRounds = weapon.GetReserve();

            string first = currentRounds >= 10 ? currentRounds.ToString() : $"0{currentRounds}";
            string second = reserveRounds >= 10 ? reserveRounds.ToString() : $"0{reserveRounds}";
            
            //m_AmmoDisplay.SetText($"{first} / {second}");
            m_AmmoDisplay.SetText(first + " / " + second);

            // ==================================================== For left gun in dual wield. ==================================================== //
            // Hiding if unneccessary.
            ToggleDualWield(weapon.m_DualWield);

            int currentRoundsLeftGun = weapon.GetRoundsInMagazine(true);
            int reserveRoundsLeftGun = weapon.GetReserve(true);

            string firstExtraGun = currentRoundsLeftGun >= 10 ? currentRoundsLeftGun.ToString() : $"0{currentRoundsLeftGun}";
            string secondExtraGun = reserveRoundsLeftGun >= 10 ? reserveRoundsLeftGun.ToString() : $"0{reserveRoundsLeftGun}";

            //m_AmmoDisplay.SetText($"{first} / {second}");
            m_AmmoDisplayLeft.SetText(firstExtraGun + " / " + secondExtraGun);
        }

    }

    /// <summary>Displays the current health.</summary>
    public void UpdateHealthUI() //move to ui Manager
    {
        // Doesn't matter that much given agents are
        // destroyed along with everything attached
        if (m_Inv)
        { 
            m_HealthSprite.SetActive(GetHealth() > 0);
            m_HealthText?.SetText(GetHealth().ToString());
        }
    }

    public void UpdateAllUI(Weapon currentWeapon)
    {
        UpdateHealthUI();
        UpdateWeaponUI(currentWeapon);
    }
    private void ToggleDualWield(bool toggle)
    {
        if (!toggle)
            m_AmmoDisplayLeft.enabled = false;
        else
            m_AmmoDisplayLeft.enabled = true;
    }

    public void SetDeathIncarnateBar(float percentage)
    {
        

        m_DeathIncarnateBar.fillAmount = percentage;
    }
    public void UnhideCanvas() { m_Canvas.gameObject.SetActive(true); }
    public void HideCanvas() { m_Canvas.gameObject.SetActive(false); }
    public void HideHealth() { m_HealthSprite.SetActive(false); m_HealthText.gameObject.SetActive(false); }
    public void UnhideHealth() { m_HealthSprite.SetActive(true); m_HealthText.gameObject.SetActive(true); }
    public void SetInventory(Inventory inv) { m_Inv = inv; }
    public int GetHealth() { return m_Inv.GetHealth(); }
    

}
