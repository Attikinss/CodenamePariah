using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInventory : MonoBehaviour
{
    // NOTES:
    // • If a variable is exposed to the designers via public means or serialisation, try to
    //   ensure that comments are not next to them but within the tool tip attribute field.
    //
    // • UI/GUI elements should be separated from the weapon script as they are two individual
    //   systems. Be sure to write code in a way that decouples different topics. A potential
    //   work around is to use events that trigger a UI manager to prompt ammo warnings to the
    //   player.
    //
    // • [ m_CurrentAmmo <= (int)(m_MagazineSize / 3) / 2 && m_ReserveAmmo == 0 && m_CurrentAmmo > 0; ]
    //   Lines of code like this can be pretty daunting when you have to come back to fix something
    //   when time is running short. Try to separate stuff like this into helper functions like
    //   [ bool AmmoLow ], [ bool AmmoEmpty ], etc. Some people are worried that this may bloat the
    //   code but it's the best way to guarantee readability when you need it most.
    //   Give it a try, you'll thank yourself later.
    //
    // • Nothing major but whenever you create a function or a member variable ( m_NextTimeToFire for example )
    //   try putting a /// <summary>Your description here.</summary> block above it. It allows you to hover
    //   over the variable function anywhere within the codebase and the description is shown in the popup.
    //   You don't HAVE to do this, it just makes it easier to figure out what stuff does without going down
    //   a rabbit hole.

    // Keep this for now; it's useful for raycasting stuff.
    // But this will need to be replaced for edge case reasons
    

    
    // ============================== EVERYTHING HERE HAS BEEN MOVED TO WEAPON.CS ============================== //
    

    

    

    

    
}
