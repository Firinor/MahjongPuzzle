using System;
using System.Collections.Generic;
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
    [SerializeField] private Material darkMaterial;
    private void Awake()
    {
        spellShuffle.onClick.AddListener(Shuffle);
        spellHint.onClick.AddListener(Hint);
        spellSpotlight.onClick.AddListener(Spotlight);
    }

    private void Shuffle()
    {
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
    }

    private void Hint()
    {
        if(!CheckHint())
            return;
        
        for (int i = 0; i < pool.transform.childCount-1; i++)
        {
            MajhongTileView data1 = pool.transform.GetChild(i).GetComponent<MajhongTileView>();
            if(rules.CheckNeighbors(data1))
                continue;
            
            for (int j = i+1; j < pool.transform.childCount; j++)
            {
                MajhongTileView data2 = pool.transform.GetChild(j).GetComponent<MajhongTileView>();
                if(rules.CheckNeighbors(data2))
                    continue;
                
                if (data1.Sprite == data2.Sprite)
                {
                    effects.Hint(data1, data2);
                    return;
                }
            }
        }
    }

    private bool CheckHint()
    {
        return !effects.isHintAnimation;
    }
    
    private void Spotlight()
    {
        for (int i = 0; i < pool.transform.childCount; i++)
        {
            MajhongTileView data = pool.transform.GetChild(i).GetComponent<MajhongTileView>();
            if (!rules.CheckNeighbors(data))
                continue;
            
            data.SetMaterial(darkMaterial);
        }
    }

    private void OnDestroy()
    {
        spellShuffle.onClick.RemoveAllListeners();
        spellHint.onClick.RemoveAllListeners();
        spellSpotlight.onClick.RemoveAllListeners();
    }
}
