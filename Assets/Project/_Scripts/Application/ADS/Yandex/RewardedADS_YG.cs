using System;
using UnityEngine;
#if  IS_YANDEX
using YG;
#endif

public class RewardedADS_YG : MonoBehaviour
{
#if !IS_YANDEX
    private void Awake()
    {
        Destroy(gameObject);
    }
#else
    private void OnEnable()
    {
        YG2.onRewardAdv += OnReward;
    }
    
    public void ShuffleRewardAdvShow()
    {
        YG2.RewardedAdvShow("Shuffle");
    }
    public void AddHandRewardAdvShow()
    {
        YG2.RewardedAdvShow("AddHand");
    }
    
    private void OnReward(string id)
    {
        if(string.Equals(id, "Shuffle"))
        {
            
        }
        else if(string.Equals(id, "AddHand"))
        {
            
        }
    }
    private void OnDisable()
    {
        YG2.onRewardAdv -= OnReward;
    }
#endif
}
