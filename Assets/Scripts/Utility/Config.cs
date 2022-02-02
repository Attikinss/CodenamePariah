using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AudioConfig
{
    [Header("Audio")]
    public int MasterVolume;
    public int SFXVolume;
    public int MusicVolume;
    public int DialogueVolume;
}

[System.Serializable]
public struct VideoConfig
{
    [Header("Video")]
    public bool Fullscreen;
    public ScreenResolution Resolution;
    public int FieldOfView;

    [System.Serializable]
    public struct ScreenResolution
    {
        public int Width;
        public int Height;
        public int RefreshRate;
    }
}

[System.Serializable]
public struct GameplayConfig
{
    [Header("Gameplay")]
    public int MouseSensitivity;
    public int ControllerSensitivity;
}
