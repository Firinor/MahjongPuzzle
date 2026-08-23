using FirAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if IS_YANDEX
using YG;
#endif

public class SpellManager : MonoBehaviour
{
    [SerializeField] private Button spellShuffle;
    [SerializeField] private Button spellHint;
    [SerializeField] private Button spellSpotlight;
    [SerializeField] private CoreBootstrap bootstrap;
    [SerializeField] private CoreRulesManager rulesManager;
    [SerializeField] private TilePool pool;
    [SerializeField] private TilesEffects effects;
    [SerializeField] private FirAnimationsManager animations;
    
    [SerializeField] private GameObject losePopup;
    [SerializeField] private Button spellShuffle2;
    
    [SerializeField] private GameObject SpellSupply;

    [SerializeField] private TextMeshProUGUI ShuffleCountText;
    [SerializeField] private TextMeshProUGUI HintCountText;
    [SerializeField] private TextMeshProUGUI SpotLightCountText;

    private SaveData player;

    private bool isAnimationComplete;
    private bool isSpotLightOn = false;
    
    public void Initialize(SaveData progress)
    {
        player = progress;
        
        spellShuffle.onClick.AddListener(Shuffle);
        if (player.GameMode == GameMode.Solitare)
            spellShuffle2.onClick.AddListener(AdsShuffle);
        spellShuffle.gameObject.SetActive(false);

        spellSpotlight.onClick.AddListener(ApplySpotlight);
        spellSpotlight.gameObject.SetActive(false);

        if (progress.GameMode == GameMode.Solitare)
        {
            spellHint.onClick.AddListener(Hint);
        }
        spellHint.gameObject.SetActive(false);
        
        rulesManager.OnTilesChanged += TrySpotlight;
        
        //ShuffleCountText.text = GetSpellCount(player.ShuffleSpell);
        //HintCountText.text = GetSpellCount(player.HintSpell);
        //SpotLightCountText.text = GetSpellCount(player.SpotLightSpell);
    }

    public void ButtonsOn()
    {
        spellShuffle.gameObject.SetActive(true);
        spellSpotlight.gameObject.SetActive(true);
        if (player.GameMode == GameMode.Solitare)
        {
            spellHint.gameObject.SetActive(true);
        }

        if(isAnimationComplete)
            return;
        
        animations.Initialize();
        animations.StartAnimations();
        isAnimationComplete = true;
    }
    
    private string GetSpellCount(int count)
    {
        return count.ToString();
        //return count > 0 ? count.ToString() : "<color=green>+</color>";
    }

    private void AdsShuffle()
    {
#if IS_YANDEX
        YG2.RewardedAdvShow("Shuffle", Shuffle);
#else
        Shuffle();
#endif
    }
    private void Shuffle()
    {
        /*if (player.ShuffleSpell <= 0)
        {
            SpellSupply.SetActive(true);
            return;
        }

        player.ShuffleSpell--;
        ShuffleCountText.text = GetSpellCount(player.ShuffleSpell);
        SaveLoadSystem<ProgressData>.Save("Player", player);*/
        
        bootstrap.Shuffle();
        
        losePopup.SetActive(false);
    }

    private void Hint()
    {
        /*if (player.HintSpell <= 0)
        {
            SpellSupply.SetActive(true);
            return;
        }
        
        player.HintSpell--;
        HintCountText.text = GetSpellCount(player.HintSpell);*/
        
        for (int i = 0; i < pool.transform.childCount-1; i++)
        {
            MajhongTileView data1 = pool.transform.GetChild(i).GetComponent<MajhongTileView>();
            if(data1.isHint || CoreRulesManager.CheckNeighbors(data1))
                continue;
            
            for (int j = i+1; j < pool.transform.childCount; j++)
            {
                MajhongTileView data2 = pool.transform.GetChild(j).GetComponent<MajhongTileView>();
                if(data2.isHint || CoreRulesManager.CheckNeighbors(data2))
                    continue;
                
                if (data1.Sprite == data2.Sprite)
                {
                    //HintCountText.text = GetSpellCount(player.HintSpell);
                    player.Save();

                    data1.isHint = true;
                    data2.isHint = true;
                    effects.Hint(data1, data2);
                    return;
                }
            }
        }
    }
    public void DisableShuffle()
    {
        spellShuffle.interactable = false;
    }
    public void EnableShuffle()
    {
        spellShuffle.interactable = true;
    }
    private void ApplySpotlight()
    {
        if (isSpotLightOn)
        {
            isSpotLightOn = false;
            for (int i = 0; i < pool.transform.childCount; i++)
            {
                MajhongTileView tileView = pool.transform.GetChild(i).GetComponent<MajhongTileView>();
                tileView.DisableDarkerMaterial();
            }
            return;
        }
        
       /* if (player.SpotLightSpell <= 0)
        {
            SpellSupply.SetActive(true);
            return;
        }
        
        player.SpotLightSpell--;
        SpotLightCountText.text = GetSpellCount(player.SpotLightSpell);
        YG2.SaveProgress();*/
        
        Spotlight();
    }
    private void TrySpotlight()
    {
        if (isSpotLightOn) Spotlight();
    }
    
    private void Spotlight()
    {
        for (int i = 0; i < pool.transform.childCount; i++)
        {
            MajhongTileView tileView = pool.transform.GetChild(i).GetComponent<MajhongTileView>();
            if (!CoreRulesManager.CheckNeighbors(tileView))
            {
                tileView.DisableDarkerMaterial();
                continue;
            }
            
            tileView.SetDarkerMaterial();
        }

        isSpotLightOn = true;
    }

    /*public void AddHintSpell()
    {
        player.HintSpell++;
        HintCountText.text = player.HintSpell.ToString();
        YG2.SaveProgress();
    }
    public void AddShuffleSpell()
    {
        player.ShuffleSpell++;
        ShuffleCountText.text = player.ShuffleSpell.ToString();
        YG2.SaveProgress();
    }
    public void AddSpotLightSpell()
    {
        player.SpotLightSpell++;
        SpotLightCountText.text = player.SpotLightSpell.ToString();
        YG2.SaveProgress();
    }*/
    
    private void OnDestroy()
    {
        spellShuffle.onClick.RemoveAllListeners();
        spellShuffle2.onClick.RemoveListener(AdsShuffle);
        spellHint.onClick.RemoveAllListeners();
        spellSpotlight.onClick.RemoveAllListeners();
        
        rulesManager.OnTilesChanged -= TrySpotlight;
    }
}
