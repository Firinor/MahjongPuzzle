using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private Button spellShuffle;
    [SerializeField] private Button spellHint;
    [SerializeField] private Button spellSpotlight;
    [SerializeField] private MajhongSolitaireRules rules;
    [SerializeField] private TilePool pool;
    [SerializeField] private TilesEffects effects;
    
    [SerializeField] private GameObject losePopup;
    [SerializeField] private Button spellShuffle2;
    
    [SerializeField] private GameObject SpellSupply;

    [SerializeField] private TextMeshProUGUI ShuffleCountText;
    [SerializeField] private TextMeshProUGUI HintCountText;
    [SerializeField] private TextMeshProUGUI SpotLightCountText;

    private ProgressData player;

    private bool isSpotLightOn = false;
    
    public void Initialize(ProgressData progress)
    {
        player = progress;
        
        spellShuffle.onClick.AddListener(Shuffle);
        spellShuffle2.onClick.AddListener(Shuffle);
        spellHint.onClick.AddListener(Hint);
        spellSpotlight.onClick.AddListener(ApplySpotlight);

        rules.OnTilesChanged += TrySpotlight;

        
        ShuffleCountText.text = GetSpellCount(player.ShuffleSpell);
        HintCountText.text = GetSpellCount(player.HintSpell);
        SpotLightCountText.text = GetSpellCount(player.SpotLightSpell);
    }

    private string GetSpellCount(int count)
    {
        return count > 0 ? count.ToString() : "<color=green>+</color>";
    }
    private void Shuffle()
    {
        if (player.ShuffleSpell <= 0)
        {
            SpellSupply.SetActive(true);
            return;
        }

        player.ShuffleSpell--;
        ShuffleCountText.text = GetSpellCount(player.ShuffleSpell);
        SaveLoadSystem<ProgressData>.Save("Player", player);
        
        List<Tile> tiles = new();
        for (int i = pool.transform.childCount; i > 0; i--)
        {
            tiles.Add(pool.transform.GetChild(i-1).GetComponent<MajhongTileView>().Data);
        }
                
        tiles.Shuffle();
        
        for (int i = pool.transform.childCount; i > 0; i--)
        {
            pool.transform.GetChild(i-1).GetComponent<MajhongTileView>().SetData(tiles[i-1]);
        }
        
        losePopup.SetActive(false);
    }

    private void Hint()
    {
        if (player.HintSpell <= 0)
        {
            SpellSupply.SetActive(true);
            return;
        }
        
        for (int i = 0; i < pool.transform.childCount-1; i++)
        {
            MajhongTileView data1 = pool.transform.GetChild(i).GetComponent<MajhongTileView>();
            if(data1.isHint || MajhongSolitaireRules.CheckNeighbors(data1))
                continue;
            
            for (int j = i+1; j < pool.transform.childCount; j++)
            {
                MajhongTileView data2 = pool.transform.GetChild(j).GetComponent<MajhongTileView>();
                if(data2.isHint || MajhongSolitaireRules.CheckNeighbors(data2))
                    continue;
                
                if (data1.Sprite == data2.Sprite)
                {
                    //player.HintSpell--;//TODO
                    HintCountText.text = GetSpellCount(player.HintSpell);
                    SaveLoadSystem<ProgressData>.Save("Player", player);

                    data1.isHint = true;
                    data2.isHint = true;
                    effects.Hint(data1, data2);
                    return;
                }
            }
        }
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
        
        if (player.SpotLightSpell <= 0)
        {
            SpellSupply.SetActive(true);
            return;
        }
        
        player.SpotLightSpell--;
        SpotLightCountText.text = GetSpellCount(player.SpotLightSpell);
        SaveLoadSystem<ProgressData>.Save("Player", player);
        
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
            if (!MajhongSolitaireRules.CheckNeighbors(tileView))
            {
                tileView.DisableDarkerMaterial();
                continue;
            }
            
            tileView.SetDarkerMaterial();
        }

        isSpotLightOn = true;
    }

    public void AddHintSpell()
    {
        player.HintSpell++;
        HintCountText.text = player.HintSpell.ToString();
        SaveLoadSystem<ProgressData>.Save("Player", player);
    }
    public void AddShuffleSpell()
    {
        player.ShuffleSpell++;
        ShuffleCountText.text = player.ShuffleSpell.ToString();
        SaveLoadSystem<ProgressData>.Save("Player", player);
    }
    public void AddSpotLightSpell()
    {
        player.SpotLightSpell++;
        SpotLightCountText.text = player.SpotLightSpell.ToString();
        SaveLoadSystem<ProgressData>.Save("Player", player);
    }
    
    private void OnDestroy()
    {
        spellShuffle.onClick.RemoveAllListeners();
        spellShuffle2.onClick.RemoveAllListeners();
        spellHint.onClick.RemoveAllListeners();
        spellSpotlight.onClick.RemoveAllListeners();
        
        rules.OnTilesChanged -= TrySpotlight;
    }
}
