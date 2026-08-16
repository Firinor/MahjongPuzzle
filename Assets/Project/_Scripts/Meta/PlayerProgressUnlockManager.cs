using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class PlayerProgressUnlockManager : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI playerGold;
    
    private SaveData player;

    [SerializeField]
    private Unlocks unlocks;
    
    [SerializeField] 
    private List<TileToggle> tiles;
    [SerializeField] 
    public Button difficulty;
    [SerializeField] 
    public TextMeshProUGUI difficultyText;
    [SerializeField] 
    public Button gameMode;
    [SerializeField] 
    public TextMeshProUGUI gameModeText;
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

        foreach (var tileToggle in tiles)
            tileToggle.Toggle.isOn = false;
        var toggle = tiles.Find(d => d.ID.Equals(player.TilesID));
        toggle.Toggle.isOn = true;
        
        RefreshDifficultyText();
#if IS_YANDEX
       Destroy(gameMode.gameObject);
#else
        RefreshGameModeText();
#endif
        
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

        difficulty.onClick.AddListener(SelectDifficulty);
#if !IS_YANDEX
        gameMode.onClick.AddListener(SelectGameMode);
#endif

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

    private void SelectDifficulty()
    {
        player.Difficulty = (player.Difficulty + 1) % 3;
        RefreshDifficultyText();
        player.Save();
    }
    
    private void SelectGameMode()
    {
        if (player.GameMode == GameMode.Collecting)
            player.GameMode = GameMode.Solitare;
        else
            player.GameMode = GameMode.Collecting;
        
        RefreshGameModeText();
        player.Save();
    }

    private void RefreshGameModeText()
    {
        string localizedText = LocalizationSettings.StringDatabase
            .GetLocalizedString("Perevodi", player.GameMode.ToString());
        gameModeText.text = localizedText;
        gameModeText.GetComponent<LocalizeStringEvent>().StringReference.SetReference("Perevodi", player.GameMode.ToString());
    }

    private void RefreshDifficultyText()
    {
        string key = player.Difficulty switch
        {
            0 => "Easy",
            2 => "Hard",
            _ => "Normal"
        };
        string localizedText = LocalizationSettings.StringDatabase
            .GetLocalizedString("Perevodi", key);
        difficultyText.text = localizedText;
        difficultyText.GetComponent<LocalizeStringEvent>().StringReference.SetReference("Perevodi", key);
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
            t.Toggle.onValueChanged.RemoveAllListeners();

        difficulty.onClick.RemoveAllListeners();
        
        foreach (var d in desks)
        {
            d.Button.onClick.RemoveAllListeners();
        }
    }
}