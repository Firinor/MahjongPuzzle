using System.Collections;
using System.Collections.Generic;
using FirAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinLevelUnlockAnimations : MonoBehaviour
{
    [SerializeField]
    private float numeratorSpeed = 200;
    [SerializeField]
    private float delay = 4;
    [SerializeField]
    private Slider currentSlider;
    [SerializeField]
    private Slider inLevelSlider;
    [SerializeField]
    private Slider bonusSlider;
    [SerializeField]
    private FirAnimation sliderZoomAnimation;
    [SerializeField]
    private TextMeshProUGUI rewardText;
    [SerializeField]
    private FirAnimation rewardTextAnimation;
    [SerializeField]
    private FirAnimation rewardTextAnimation2;
    [SerializeField]
    private TextMeshProUGUI bonusText;
    [SerializeField]
    private FirAnimation bonusTextAnimation;
    [SerializeField]
    private FirAnimation bonusTextAnimation2;
    [SerializeField]
    private TextMeshProUGUI startText;
    [SerializeField]
    private TextMeshProUGUI endText;
    [SerializeField]
    private Unlocks unlocks;
    [SerializeField]
    private UnlockView unlockView;
    
    //private ProgressData player;
    private int startGold;
    private int reward;
    private int bonus;
    
    private float currentReward;
    private float currentBonus;

    private int playerLevelIndex;

    private List<Sprite> levelRewards = new(); 
    
    public void Initialize(ProgressData player, int reward, int bonus = 0)
    {
        //this.player = player;
        this.reward = reward;
        this.bonus = bonus;
        
        playerLevelIndex = 0;
        startGold = player.GoldCoins;
        int currentPlayerGold = startGold;
        while (true)
        {
            if(playerLevelIndex >= unlocks.Levels.Length
               || playerLevelIndex >= unlocks.KeyWords.Length) 
                break;
            
            if (currentPlayerGold < unlocks.Levels[playerLevelIndex])
                break;
            
            currentPlayerGold -= unlocks.Levels[playerLevelIndex];
            playerLevelIndex++;
        }

        SlidersByLevel();
        
        currentSlider.value = startGold;
        inLevelSlider.value = 0;
        bonusSlider.value = 0;
    }

    private void SlidersByLevel()
    {
        int lastLevelGold = 0;
        if (playerLevelIndex > 0
            && playerLevelIndex < unlocks.Levels.Length)
            lastLevelGold = unlocks.Levels[playerLevelIndex-1];

        int nextLevelGold = 1;
        if (playerLevelIndex < unlocks.Levels.Length)
            nextLevelGold = unlocks.Levels[playerLevelIndex];
        
        SlidersSetValues(lastLevelGold, nextLevelGold);
    }

    private void SlidersSetValues(int min, int max)
    {
        currentSlider.maxValue = max;
        currentSlider.minValue = min;
        inLevelSlider.maxValue = max;
        inLevelSlider.minValue = min;
        bonusSlider.maxValue = max;
        bonusSlider.minValue = min;
        startText.text = min.ToString();
        endText.text = max.ToString();
    }

    public void Play()
    {
        StartCoroutine(WinAnlockAnimations());
    }

    private IEnumerator WinAnlockAnimations()
    {
        yield return new WaitForSeconds(delay);

        rewardTextAnimation.OnComplete = () => StartCoroutine(RewardCounter());
        rewardTextAnimation.Play();
        rewardTextAnimation2.Play();
    }

    private IEnumerator RewardCounter()
    {
        while (currentReward < reward)
        {
            yield return null;
            currentReward += numeratorSpeed * Time.deltaTime;
            rewardText.text = currentReward.ToString("0");
            SetInLevelSlider();
        }

        currentReward = reward;
        rewardText.text = currentReward.ToString("0");
        SetInLevelSlider();

        if (bonus > 0)
        {
            bonusTextAnimation.OnComplete = () => StartCoroutine(BonusCounter());
            bonusTextAnimation.Play();
            bonusTextAnimation2.Play();
        }
        else
        {
            yield return ShowUnlocks();
        }
    }

    private IEnumerator BonusCounter()
    {
        while (currentBonus < bonus)
        {
            yield return null;
            currentBonus += numeratorSpeed * Time.deltaTime;
            bonusText.text = currentBonus.ToString("0");
            SetBonusSlider();
        }

        currentBonus = bonus;
        bonusText.text = currentBonus.ToString("0");
        SetBonusSlider();
        
        yield return ShowUnlocks();
    }

    private IEnumerator ShowUnlocks()
    {
        if(levelRewards.Count <= 0)
            yield break;

        bool buttonClick = false;
        unlockView.CloseButton.onClick.RemoveAllListeners();
        unlockView.CloseButton.onClick.AddListener(() => buttonClick = true);
        foreach (var sprite in levelRewards)
        {
            unlockView.Content.sprite = sprite;
            unlockView.FirRotate.Play();
            unlockView.Animations.ToStartPoint();
            unlockView.Animations.StartAnimations();
            unlockView.gameObject.SetActive(true);
            yield return new WaitUntil(() => buttonClick);
            buttonClick = false;
        }
    }
    
    private void SetBonusSlider()
    {
        if (startGold + currentReward + currentBonus < inLevelSlider.maxValue)
            bonusSlider.value = startGold + currentReward + currentBonus;
        else
            NextLevel();
    }
    
    private void SetInLevelSlider()
    {
        if (startGold + currentReward < inLevelSlider.maxValue)
            inLevelSlider.value = startGold + currentReward;
        else
            NextLevel();
    }

    private void NextLevel()
    {
        levelRewards.Add(unlocks.Sprites[playerLevelIndex]);
        playerLevelIndex++;
        sliderZoomAnimation.Play();
        SlidersByLevel();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}