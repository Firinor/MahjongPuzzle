using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerProgressUnlockManager : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI playerGold;
    
    private ProgressData player;

    [SerializeField] 
    private List<TileToggle> tiles;
    [SerializeField] 
    private List<DeskToggle> desks;

    public void Initialize(ProgressData progressData)
    {
        player = progressData;

        playerGold.text = player.GoldCoins.ToString();

        if (player.TilesPacks != null)
        {
            foreach (var tileID in player.TilesPacks)
            {
                tiles.Find(t => t.ID == tileID).Unlock();
            }
        }
        if (player.Desks != null)
        {
            foreach (var deckID in player.Desks)
            {
                desks.Find(d => d.ID == deckID).Unlock();
            }
        }

        Subscriptions();
    }

    private void Subscriptions()
    {
        player.OnGoldChange += GoldChanged;
        foreach (var tileToggle in tiles)
        {
            if(tileToggle.UnlockButton == null)
                continue;
                    
            tileToggle.UnlockButton.onClick.AddListener(() => TryUnlockTile(tileToggle));
        }
        foreach (var deskToggle in desks)
        {
            if(deskToggle.UnlockButton == null)
                continue;
                    
            deskToggle.UnlockButton.onClick.AddListener(() => TryUnlockDesk(deskToggle));
        }
    }

    private void GoldChanged(int coins)
    {
        playerGold.text = player.GoldCoins.ToString();
    }

    private void TryUnlockDesk(DeskToggle desk)
    {
        if(!player.TrySpendGold(desk.UnlockCost))
            return;
        
        desk.Unlock();
        List<string> newPlayerDesks = new();
        if(player.Desks != null)
            newPlayerDesks = player.Desks.ToList();
        newPlayerDesks.Add(desk.ID);
        player.Desks = newPlayerDesks.ToArray();
        SaveLoadSystem<ProgressData>.Save("Player", player);
    }

    private void TryUnlockTile(TileToggle tile)
    {
        if(!player.TrySpendGold(tile.UnlockCost))
            return;
        
        tile.Unlock();
        List<string> newPlayerTiles = new();
        if(player.TilesPacks != null)
            newPlayerTiles = player.TilesPacks.ToList();
        newPlayerTiles.Add(tile.ID);
        player.TilesPacks = newPlayerTiles.ToArray();
        SaveLoadSystem<ProgressData>.Save("Player", player);
    }

    private void OnDestroy()
    {
        player.OnGoldChange -= GoldChanged;
        foreach (var tileToggle in tiles)
        {
            if(tileToggle.UnlockButton == null)
                continue;
                    
            tileToggle.UnlockButton.onClick.RemoveAllListeners();
        }
        foreach (var deskToggle in desks)
        {
            if(deskToggle.UnlockButton == null)
                continue;
                    
            deskToggle.UnlockButton.onClick.RemoveAllListeners();
        }
    }
}
