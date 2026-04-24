using System.Collections;
using FirAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WinLevelUnlockAnimations : MonoBehaviour
{
    [SerializeField]
    private float numeratorSpeed = 1000;
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
    private TextMeshProUGUI bonusText;
    [SerializeField]
    private FirAnimation bonusTextAnimation;
    [SerializeField]
    private TextMeshProUGUI startText;
    [SerializeField]
    private TextMeshProUGUI endText;
    [SerializeField]
    private UnlockView unlockView;
    
    //private ProgressData player;
    private int startGold;
    private int reward;
    private int bonus;
    
    private float currentReward;
    private float currentBonus;

    private int playerLevelIndex;
    
    public void Initialize(ProgressData player, int reward, int bonus = 0)
    {
        //this.player = player;
        this.reward = reward;
        this.reward = 10000;
        this.bonus = bonus;
        
        playerLevelIndex = 0;
        startGold = player.GoldCoins;
        int currentPlayerGold = startGold;
        while (true)
        {
            if(playerLevelIndex >= Unlocks.Levels.Length
               || playerLevelIndex >= Unlocks.KeyWords.Length) 
                break;
            
            if (currentPlayerGold < Unlocks.Levels[playerLevelIndex])
                break;
            
            currentPlayerGold -= Unlocks.Levels[playerLevelIndex];
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
            && playerLevelIndex < Unlocks.Levels.Length)
            lastLevelGold = Unlocks.Levels[playerLevelIndex-1];

        int nextLevelGold = 1;
        if (playerLevelIndex < Unlocks.Levels.Length)
            nextLevelGold = Unlocks.Levels[playerLevelIndex];
        
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

    public void Play(float delay = 0)
    {
        StartCoroutine(WinAnlockAnimations(delay));
    }

    private IEnumerator WinAnlockAnimations(float delay)
    {
        yield return new WaitForSeconds(delay);

        rewardTextAnimation.OnComplete = () => StartCoroutine(RewardCounter());
        rewardTextAnimation.Play();
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
        throw new System.NotImplementedException();
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
        playerLevelIndex++;
        sliderZoomAnimation.Play();
        SlidersByLevel();
    }
}