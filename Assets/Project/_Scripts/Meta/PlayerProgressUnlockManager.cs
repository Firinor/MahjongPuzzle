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
        var selectedTiles = tiles.Find(t => t.ID == player.tilesID);
        tiles[0].Toggle.isOn = false;
        selectedTiles.Toggle.isOn = true;
        if (player.Desks != null)
        {
            foreach (var deckID in player.Desks)
            {
                desks.Find(d => d.ID == deckID).Unlock();
            }
        }
        desks.Find(d => d.ID == player.deskID).Toggle.isOn = true;

        Subscriptions();
    }

    private void Subscriptions()
    {
        player.OnGoldChange += GoldChanged;
        foreach (var tileToggle in tiles)
        {
            tileToggle.Toggle.onValueChanged.AddListener(v =>
            {
                if(!v)
                    return;
                SelectTiles(tileToggle.ID);
            });
            if(tileToggle.UnlockButton == null)
                continue;
                    
            tileToggle.UnlockButton.onClick.AddListener(() => TryUnlockTile(tileToggle));
        }
        foreach (var deskToggle in desks)
        {
            deskToggle.Toggle.onValueChanged.AddListener(v =>
            {
                if(!v)
                    return;
                SelectDesk(deskToggle.ID);
            });
            if(deskToggle.UnlockButton == null)
                continue;
                    
            deskToggle.UnlockButton.onClick.AddListener(() => TryUnlockDesk(deskToggle));
        }
    }

    private void SelectTiles(string ID)
    {
        player.tilesID = ID;
        SaveLoadSystem<ProgressData>.Save("Player", player);
    }
    private void SelectDesk(string ID)
    {
        player.deskID = ID;
        SaveLoadSystem<ProgressData>.Save("Player", player);
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
            tileToggle.Toggle.onValueChanged.RemoveAllListeners();
            if(tileToggle.UnlockButton == null)
                continue;
                    
            tileToggle.UnlockButton.onClick.RemoveAllListeners();
        }
        foreach (var deskToggle in desks)
        {
            deskToggle.Toggle.onValueChanged.RemoveAllListeners();
            if(deskToggle.UnlockButton == null)
                continue;
                    
            deskToggle.UnlockButton.onClick.RemoveAllListeners();
        }
    }
}
