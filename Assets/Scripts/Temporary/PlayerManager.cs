using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum WeaponSlot
{
    WEAPON1,
    WEAPON2
}


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager s_Instance;



    public static WeaponSlot s_CurrentWeapon;
    public HostController m_PlayerController;

    // Temporary health variable until I get the possession system.
    public static int s_Health = 100;
    public static int s_MaxHealth = 100;


	private void Awake()
	{
        if (PlayerManager.s_Instance != null)
            Debug.Assert(false); // Make sure there aren't more than one PlayerManager scripts.

        s_Instance = this;           
	}


	// Start is called before the first frame update
	void Start()
    {
        Debug.Assert(m_PlayerController, "Please give the PlayerManager a reference to the PlayerController.");
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public static void AddHealth(int health)
    {
        s_Health += health;
    }

    public static void RemoveHealth(int health)
    {
        s_Health -= health;
    }

    public static void SetWeapon(WeaponSlot weapon) { s_CurrentWeapon = weapon; }

    //public static Weapon GetCurrentWeapon()
    //{
    //    PlayerManager playerManager = PlayerManager.s_Instance;
    //    Debug.Assert(playerManager.m_PlayerController, "PlayerController is a null reference.");
    //
    //
    //    switch (s_CurrentWeapon)
    //    {
    //        case WeaponSlot.WEAPON1:
    //            return playerManager.m_PlayerController.m_Weapon1;
    //        case WeaponSlot.WEAPON2:
    //            return playerManager.m_PlayerController.m_Weapon2;
    //        default:
    //            return playerManager.m_PlayerController.m_Weapon1;
    //    }
    //}
}
