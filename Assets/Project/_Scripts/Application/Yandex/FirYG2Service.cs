using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YG;
using YG.Insides;

public class FirYG2Service : MonoBehaviour
{
    public static FirYG2Service instance;

    
    [SerializeField] private float Timer;
    
    private Coroutine timerAdShowCoroutine;
    private Button[] buttons;
    
    private void Awake()
    {
        if(instance != null)
            Debug.LogError("Duo instance!");
        instance = this;
        DontDestroyOnLoad(gameObject);
        YG2.onPurchaseSuccess += SuccessPurchased;
    }
    public void Initialize()
    {
        YG2.GameReadyAPI();
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
    
    private void Update()
    {
        Timer = YG2.timerInterAdv;
    }
    
    /// <summary>
    /// InterstitialAdv
    /// </summary>
    public void CheckTimerAd()
    {
        if (!YG2.isTimerAdvCompleted || YG2.nowAdsShow) 
            return;

        timerAdShowCoroutine = StartCoroutine(AdShow());
    }

    private IEnumerator AdShow()
    {
        YG2.PauseGame(true);

        YG2.InterstitialAdvShow();

        while (!YG2.nowInterAdv)
            yield return null;
            
        YG2.PauseGame(false);
    }

    [ContextMenu("Pause")]
    void Pause()
    {
        YG2.PauseGame(!YG2.isPauseGame);   
    }
    
    [ContextMenu("ResetAdsTimer")]
    void ResetAdsTimer()
    {
        YGInsides.ResetTimerInterAdv();
    }

    /// <summary>
    /// RewardedAdv
    /// </summary>
    public void ShowRewarded()
    {
        Pause();
        YG2.RewardedAdvShow("randomBonus", RewardedBonus);
    }

    private void RewardedBonus()
    {
        YG2.SkipNextInterAdCall();
        
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
        YG2.onPurchaseSuccess -= SuccessPurchased;
    }
}