using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Achievements is not what is sounds. It's a struct to store special events like the first time the player controls a soldier, scientist, ect...
/// </summary>
public struct Achievements
{
    public bool hasEnteredUnit { get; private set; }
    public bool hasEnteredScientist { get; private set; }
    public bool hasEnteredSoldier { get; private set; }

    public void FirstTimeEnemy(EnemyTypes enemy, Animators weaponAnimators, MonoBehaviour weaponMonobehaviour, Weapon weapon)
    {
        switch (enemy)
        {
            case EnemyTypes.SCIENTIST:
                if (!hasEnteredScientist)
                {
                    weaponMonobehaviour.StartCoroutine(weaponAnimators.RunWeaponInspect(3));
                    //weaponAnimators.RunWeaponInspect(5); // Hardcoded time of weapon inspect animation.
                    Animator arms = weaponAnimators.m_ArmsAnimators[0];
                    Animator gun = weaponAnimators.m_GunAnimators[0];
                    hasEnteredScientist = true;
                    arms.SetTrigger("Equip");
                    gun.SetTrigger("Equip");

                    weapon.PlayEquipSound();

                    hasEnteredUnit = true;

                    break;
                }
                else
                    break;
            case EnemyTypes.SOLDIER:
                if (!hasEnteredSoldier)
                {
                    weaponMonobehaviour.StartCoroutine(weaponAnimators.RunWeaponInspect(5));
                    Animator arms = weaponAnimators.m_ArmsAnimators[0];
                    Animator gun = weaponAnimators.m_GunAnimators[0];
                    hasEnteredSoldier = true;
                    arms.SetTrigger("Equip");
                    gun.SetTrigger("Equip");

                    weapon.PlayEquipSound();

                    hasEnteredUnit = true;

                    break;
                }
                else
                    break;
        }
    }
}
