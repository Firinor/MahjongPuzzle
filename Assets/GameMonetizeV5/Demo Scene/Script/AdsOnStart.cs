using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsOnStart : MonoBehaviour
{
#if UNITY_WEBGL || UNITY_EDITOR
#if IS_GAMEMONETIZE
    void Start()
    {
        GameMonetize.OnResumeGame += OnResumeGame;
        GameMonetize.OnPauseGame += OnPauseGame;

        if (GameMonetize.Instance != null)
            GameMonetize.Instance.ShowAd();
    }

    public void ShowAd()
    {
        GameMonetize.Instance.ShowAd();
    }

    public void OnResumeGame()
    {
        AudioListener.volume = 1f;
        Time.timeScale = 1f;
    }

    public void OnPauseGame()
    {
        Time.timeScale = 0.01f;
        AudioListener.volume = 0f;
    }
#else
    private void Awake()
    {
        enabled = false;
        Destroy(gameObject);
    }
#endif
#else
    private void Awake()
    {
        enabled = false;
        Destroy(gameObject);
    }
#endif
}
