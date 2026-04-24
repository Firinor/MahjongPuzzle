using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProgressUnlockManager : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI playerGold;
    
    private ProgressData player;

    [SerializeField] 
    private List<TileToggle> tiles;
    [SerializeField] 
    private List<Toggle> difficulty;
    [SerializeField] 
    private List<DeskToggle> desks;
    [SerializeField] 
    private List<Sprite> deskSprites;
    [SerializeField] 
    private Button ScrollDesksUp;
    [SerializeField] 
    private Button ScrollDesksDown;

    private int scrollDeskIndex = 0;
    private readonly List<string> unlockedDesks = new(){"ClassicDesk"};
    
    public void Initialize(ProgressData progressData)
    {
        player = progressData;

        playerGold.text = player.GoldCoins.ToString();

        UnlocksProgress();
        UnlockedDesk(player.deskID);

        tiles[0].Toggle.isOn = false;
        var toggle = tiles.Find(d => d.ID.Equals(player.tilesID));
        toggle.Toggle.isOn = true;
        
        difficulty[0].isOn = false;
        difficulty[player.Difficulty].isOn = true;
        
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
        
        var desk = desks.Find(d => d.ID == player.deskID);
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
            
            if(deskToggle.ID.Equals(player.deskID))
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
            if(index >= Unlocks.Levels.Length
               || index >= Unlocks.KeyWords.Length) 
                break;
            
            if (coins < Unlocks.Levels[index])
                break;
            
            coins -= Unlocks.Levels[index];
            string unlockKey = Unlocks.KeyWords[index];

            if (unlockKey.Equals(tiles[1].ID))
            {
                tiles[1].Unlock();
                index++;
                continue;
            }
            if (unlockKey.Equals(tiles[2].ID))
            {
                tiles[2].Unlock(); 
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
            d.onValueChanged.AddListener(v =>
            {
                if(!v)
                    return;
                SelectDifficulty(difficulty.IndexOf(d));
            });
        }
        foreach (var tileToggle in tiles)
        {
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
        SaveLoadSystem<ProgressData>.Save("Player", player);
    }

    private void SelectTiles(string ID)
    {
        player.tilesID = ID;
        SaveLoadSystem<ProgressData>.Save("Player", player);
    }
    private void SelectDesk(string ID)
    {
        player.deskID = ID;
        foreach (var desk in desks)
        {
            bool v = desk.ID.Equals(ID);
            desk.Checkmark.enabled = v;
        }
        SaveLoadSystem<ProgressData>.Save("Player", player);
    }

    private void OnDestroy()
    {
        foreach (var t in tiles)
        {
            t.Toggle.onValueChanged.RemoveAllListeners();
        }
        foreach (var d in difficulty)
        {
            d.onValueChanged.RemoveAllListeners();
        }
        foreach (var d in desks)
        {
            d.Button.onClick.RemoveAllListeners();
        }
    }
}
