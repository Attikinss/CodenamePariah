using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PROBABLY NOT NEEDED ANYMORE
public class DesktopButton : MonoBehaviour
{
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}
