using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProgressUnlockManager : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI playerGold;
    
    private SaveData player;

    [SerializeField]
    private Unlocks unlocks;
    
    [SerializeField] 
    private List<Wrapper<TileToggle>> tiles;
    [SerializeField] 
    public List<Wrapper<Toggle>> difficulty;
    [SerializeField] 
    private List<DeskToggle> desks;
    [SerializeField] 
    private List<Sprite> deskSprites;
    [SerializeField] 
    private Button ScrollDesksUp;
    [SerializeField] 
    private Button ScrollDesksDown;

    private int scrollDeskIndex = 0;
    private readonly List<string> unlockedDesks = new(){"ClassicDesk", "ClassicDeskAlter"};
    
    public void Initialize(SaveData progressData)
    {
        player = progressData;

        playerGold.text = player.GoldCoins.ToString();

        UnlocksProgress();
        UnlockedDesk(player.DeskID);

        foreach (var tileToggle in tiles[0].List)
            tileToggle.Toggle.isOn = false;
        var toggles = tiles.Find(d => d.List[0].ID.Equals(player.TilesID));
        foreach (var tileToggle in toggles.List)
            tileToggle.Toggle.isOn = true;
        
        foreach (var diffToggle in difficulty[0].List)
            diffToggle.isOn = false;
        foreach (var diffToggle in difficulty[player.Difficulty].List)
            diffToggle.isOn = true;
        
        Subscriptions();
    }

    private void UnlockedDesk(string playerDesk)
    {
        scrollDeskIndex = unlockedDesks.IndexOf(playerDesk);
        int scrollIndex = scrollDeskIndex;
        if (scrollIndex % 2 != 0)
            scrollIndex--;
        if (scrollIndex >= deskSprites.Count - 4)
            scrollIndex = deskSprites.Count - 4;
        scrollDeskIndex = scrollIndex;

        foreach (var deskToggle in desks)
        {
            if(scrollIndex >= unlockedDesks.Count)
                break;

            deskToggle.ID = unlockedDesks[scrollIndex];
            deskToggle.Unlock(deskSprites[scrollIndex]);
            scrollIndex++;
        }
        
        var desk = desks.Find(d => d.ID == player.DeskID);
        desk.Checkmark.enabled = true;
    }

    public void DeskUp()
    {
        if(scrollDeskIndex <= 1)
            return;
        scrollDeskIndex-=2;
        
        UnselectAll();
        FillDesks();
    }
    public void DeckDown()
    {
        if(scrollDeskIndex >= deskSprites.Count - 4)
            return;
        scrollDeskIndex+=2;
        
        UnselectAll();
        FillDesks();
    }
    private void FillDesks()
    {
        int scrollIndex = scrollDeskIndex;
        foreach (var deskToggle in desks)
        {
            if(scrollIndex >= unlockedDesks.Count)
                break;
            
            deskToggle.ID = unlockedDesks[scrollIndex];
            deskToggle.Unlock(deskSprites[scrollIndex]);
            scrollIndex++;
            
            if(deskToggle.ID.Equals(player.DeskID))
                deskToggle.Checkmark.enabled = true;
        }
    }
    private void UnselectAll()
    {
        foreach (var desk in desks)
        {
            desk.Lock();
            desk.Checkmark.enabled = false;
        }
    }

    private void UnlocksProgress()
    {
        int coins = player.GoldCoins;
        int index = 0;
        while (true)
        {
            if(index >= unlocks.Levels.Length
               || index >= unlocks.KeyWords.Length) 
                break;
            
            if (coins < unlocks.Levels[index])
                break;
            
            //coins -= unlocks.Levels[index];
            string unlockKey = unlocks.KeyWords[index];

            if (unlockKey.Equals(tiles[1].List[0].ID))
            {
                foreach (var tileToggle in tiles[1].List)
                    tileToggle.Unlock();
                index++;
                continue;
            }
            if (unlockKey.Equals(tiles[2].List[0].ID))
            {
                foreach (var tileToggle in tiles[2].List)
                    tileToggle.Unlock(); 
                index++;
                continue;
            }
            
            unlockedDesks.Add(unlockKey);
            index++;
        }
    }

    private void Subscriptions()
    {
        foreach (var d in difficulty)
        {
            foreach (var d1 in d.List)
                d1.onValueChanged.AddListener(v =>
                {
                    if(!v)
                        return;
                    SelectDifficulty(difficulty.IndexOf(d));
                });
        }
        foreach (var tileToggles in tiles)
        {
            foreach (var tileToggle in tileToggles.List)
                tileToggle.Toggle.onValueChanged.AddListener(v =>
                {
                    if(!v)
                        return;
                    SelectTiles(tileToggle.ID);
                });
        }
        foreach (var desk in desks)
        {
            desk.Button.onClick.AddListener(() =>
            {
                SelectDesk(desk.ID);
            });
        }
    }

    private void SelectDifficulty(int value)
    {
        player.Difficulty = value;
        player.Save();
    }

    private void SelectTiles(string ID)
    {
        player.TilesID = ID;
        player.Save();
    }
    private void SelectDesk(string ID)
    {
        player.DeskID = ID;
        foreach (var desk in desks)
        {
            bool v = desk.ID.Equals(ID);
            desk.Checkmark.enabled = v;
        }
        player.Save();
    }

    private void OnDestroy()
    {
        foreach (var t in tiles)
            foreach (var t2 in t.List)
                t2.Toggle.onValueChanged.RemoveAllListeners();
        
        foreach (var d in difficulty)
            foreach (var d2 in d.List)
                d2.onValueChanged.RemoveAllListeners();
        
        foreach (var d in desks)
        {
            d.Button.onClick.RemoveAllListeners();
        }
    }
}

[Serializable]
public class Wrapper<T>
{
    public List<T> List;
}