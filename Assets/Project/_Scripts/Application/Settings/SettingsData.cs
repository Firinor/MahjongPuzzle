using System;
using UnityEngine.Serialization;

[Serializable]
public class SettingsData
{
    public float MusicValue = .5f;
    public bool IsMusicOn = true;
    public float SFXValue = .5f;
    public bool IsSFXOn = true;
    [FormerlySerializedAs("PlayerLanguage")] public bool isPlayerLanguage = false;
    public string Language = "en";
}