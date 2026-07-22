using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if IS_GAMEMONETIZE
using System.Runtime.InteropServices;
using UnityEngine.UI;
#endif

public class GameMonetize : MonoBehaviour
{
    public string GAME_ID = "YOUR_GAME_ID_HERE";
    
    [SerializeField] private float Timer;
    [SerializeField] private float timeLeft;
    
#if UNITY_WEBGL || UNITY_EDITOR
#if IS_GAMEMONETIZE
    public static GameMonetize Instance;
    
    private Button[] buttons;

    public static Action OnResumeGame;
    public static Action OnPauseGame;


    [DllImport("__Internal")]
    private static extern void InitApi(string gameKey);

    [DllImport("__Internal")]
    private static extern void ShowBanner();

    public void Awake()
    {
        if(Instance != null)
            Debug.LogError("Duo instance!");
        Instance = this;

        DontDestroyOnLoad(this);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetButtons(Button[] _buttons)
    {
        if (buttons != null)
        {
            foreach (var button in buttons)
            {
                if(button != null)
                    button.onClick.RemoveListener(CheckTimerAd);
            }
        }

        buttons = _buttons;
        foreach (var button in buttons)
        {
            button.onClick.AddListener(CheckTimerAd);
        }
    }
    
    public void Init()
    {
        try
        {
            InitApi(GAME_ID);
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("Initialization failed. Make sure you are running a WebGL build in a browser. Error: " + e.Message);
        }
    }
    
    public void CheckTimerAd()
    {
        if (timeLeft > 0) 
            return;
        
        ShowAd();
    }
    
    internal void ShowAd()
    {
        try
        {
            ShowBanner();
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogWarning("ShowBanner failed. Make sure you are running a WebGL build in a browser. Error: " + e.Message);
        }
    }

    [ContextMenu("Pause")]
    public void UnpauseGame()
    {
        AudioListener.volume = 1f;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        Time.timeScale = Mathf.Epsilon;
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
