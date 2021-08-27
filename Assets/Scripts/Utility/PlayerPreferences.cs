using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerPreferences", menuName = "User/Player Preferences")]
public class PlayerPreferences : ScriptableObject
{
    public GameplayConfig GameplayConfig;
    public VideoConfig VideoConfig;
    public AudioConfig AudioConfig;
}

