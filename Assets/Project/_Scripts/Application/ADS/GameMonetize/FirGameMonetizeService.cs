using System;
using UnityEngine;
using UnityEngine.UI;

public class FirGameMonetizeService : MonoBehaviour
{
    public static FirGameMonetizeService instance;
    
    [SerializeField] private float Timer;
    
    private Coroutine timerAdShowCoroutine;

    
#if IS_GAMEMONETIZE
    private void Awake()
    {
        if(instance != null)
            Debug.LogError("Duo instance!");
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        GameMonetize.OnPauseGame += PauseGame;
        GameMonetize.OnResumeGame  += UnpauseGame;
    }
    
    public void Initialize()
    {
        YG2.GameReadyAPI();
    }


    
    private void Update()
    {
        Timer = YG2.timerInterAdv;
    }
    
    /// <summary>
    /// InterstitialAdv
    /// </summary>


    public bool AdReady()
    {
        return YG2.isTimerAdvCompleted && !YG2.nowAdsShow;
    }

    


    public void ShowAd()
    {
        GameMonetize.Instance.ShowAd();
    }
    
    [ContextMenu("ResetAdsTimer")]
    void ResetAdsTimer()
    {
        YGInsides.ResetTimerInterAdv();
    }
    
    /// <summary>
    /// Purchase
    /// </summary>
    private void SuccessPurchased(string ID)
    {
        switch (ID)
        {
            case "NoAds":
            {
                
                break;
            }
            default:
                throw new Exception("Unknown Purchase ID!");
        }
    }

    private void OnDestroy()
    {
        GameMonetize.OnPauseGame -= PauseGame;
        GameMonetize.OnResumeGame -= UnpauseGame;
    }
#else
    private void Awake()
    {
        enabled = false;
        Destroy(gameObject);
    }
#endif
}