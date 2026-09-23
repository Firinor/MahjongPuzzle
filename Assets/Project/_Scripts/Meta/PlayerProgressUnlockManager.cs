using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class PlayerProgressUnlockManager : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI playerCoins;
    [SerializeField]
    private TextMeshProUGUI playerMedals;
    
    private SaveData player;

    [SerializeField]
    private Unlocks unlocks;
    
    [SerializeField] 
    private List<TileToggle> tiles;
    [SerializeField] 
    public Button difficulty;
    [SerializeField] 
    public Sprite difficultyEasyButtonSprite;
    [SerializeField] 
    public Sprite difficultyHardButtonSprite;
    [SerializeField] 
    public Sprite CoinSprite;
    [SerializeField] 
    public Sprite MedalSprite;
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
    private readonly List<LevelStruct> unlockedDesks = new()
    {
        new(){ID = "ClassicDesk", 
            IsUnlocked = true}, 
        new(){ID = "ClassicDeskAlter", 
            IsUnlocked = true},
    };
    
    public void Initialize(SaveData progressData)
    {
        player = progressData;

        playerCoins.text = player.GoldCoins.ToString();
        playerMedals.text = player.MedalsCount.ToString();

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
        scrollDeskIndex = -1;
        foreach (var levelDesk in unlockedDesks)
        {
            if (string.Equals(levelDesk.ID, playerDesk))
            {
                scrollDeskIndex = unlockedDesks.IndexOf(levelDesk);
                break;
            }
        }
        
        int scrollIndex = scrollDeskIndex;
        if (scrollIndex % 2 != 0)
            scrollIndex--;
        if (scrollIndex >= deskSprites.Count - 4)
            scrollIndex = deskSprites.Count - 4;
        scrollDeskIndex = scrollIndex;
        
        FillDesks();
        var desk = desks.Find(d => d.ID == player.DeskID);
        desk.Checkmark.enabled = true;
    }

    public void DeskUp()
    {
        if(scrollDeskIndex <= 1)
            return;
        scrollDeskIndex-=2;
        
        FillDesks();
    }
    public void DeckDown()
    {
        if(scrollDeskIndex >= deskSprites.Count - 4)
            return;
        scrollDeskIndex+=2;
        
        FillDesks();
    }
    private void FillDesks()
    {
        int scrollIndex = scrollDeskIndex;
        foreach (var deskToggle in desks)
        {
            deskToggle.Unlock(unlockedDesks[scrollIndex]);
            scrollIndex++;
            deskToggle.Checkmark.enabled = deskToggle.ID.Equals(player.DeskID);
        }
    }

    private void UnlocksProgress()
    {
        unlockedDesks[0].Sprite = deskSprites[0];
        unlockedDesks[0].MedalsCount = player.MedalsCountByLevel(unlockedDesks[0].ID);
        unlockedDesks[1].Sprite = deskSprites[1];
        unlockedDesks[1].MedalsCount = player.MedalsCountByLevel(unlockedDesks[1].ID);
        
        int coins = player.GoldCoins;
        for(int i = 0; i < unlocks.KeyWords.Length; i++)
        {
            Sprite deskSprite = deskSprites.FirstOrDefault(d => string.Equals(d.name, unlocks.KeyWords[i]));
            if (deskSprite is null)
            {
                //Debug.Log(unlocks.KeyWords[i]);
                continue;
            }
            
            LevelStruct newLevel = new();
            newLevel.ID = unlocks.KeyWords[i];
            newLevel.IsUnlocked = coins >= unlocks.Levels[i];
            newLevel.UnlockCost = unlocks.Levels[i];
            newLevel.CurrencySprite = CoinSprite;
            newLevel.MedalsCount = player.MedalsCountByLevel(newLevel.ID);
            newLevel.Sprite = deskSprite;
            unlockedDesks.Add(newLevel);
        }
        int medals = player.MedalsCount;
        int indexPosition = 3;
        for(int i = 0; i < unlocks.MedalKeyWords.Length; i++)
        {
            Sprite deskSprite = deskSprites.FirstOrDefault(d => string.Equals(d.name, unlocks.MedalKeyWords[i]));
            if (deskSprite is null)
            {
                //Debug.Log(unlocks.MedalKeyWords[i]);
                continue;
            }
            
            LevelStruct newLevel = new();
            newLevel.ID = unlocks.MedalKeyWords[i];
            newLevel.IsUnlocked = medals >= unlocks.MedalLevels[i];
            newLevel.UnlockCost = unlocks.MedalLevels[i];
            newLevel.CurrencySprite = MedalSprite;
            newLevel.MedalsCount = player.MedalsCountByLevel(newLevel.ID);
            newLevel.Sprite = deskSprite;
            if (i < unlocks.MedalKeyWords.Length - 1)
            {
                unlockedDesks.Insert(indexPosition, newLevel);
                indexPosition += 4;
            }
            else
            {
                unlockedDesks.Add(newLevel);
            }
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
        player.Difficulty = (player.Difficulty + 1) % 5;
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
            0 => "Trifle",
            2 => "Normal",
            3 => "Hard",
            4 => "Exam",
            _ => "Easy"
        };
        string localizedText = LocalizationSettings.StringDatabase
            .GetLocalizedString("Perevodi", key);
        difficultyText.text = localizedText;
        difficultyText.GetComponent<LocalizeStringEvent>().StringReference.SetReference("Perevodi", key);
        difficulty.GetComponent<Image>().sprite =
            player.Difficulty < 3 ? difficultyEasyButtonSprite : difficultyHardButtonSprite;
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

public class LevelStruct
{
    public string ID;
    public Sprite Sprite;
    public bool IsUnlocked;
    public int UnlockCost;
    public Sprite CurrencySprite;
    public int MedalsCount;
}