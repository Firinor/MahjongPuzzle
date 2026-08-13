using System;
using System.Collections.Generic;
using FirAnimations;
using TMPro;
using UnityEngine;

public class CoreRulesManager : MonoBehaviour
{
    [SerializeField] 
    private PlayerHand playerHand;
    public TilesEffects effects;
    [SerializeField] 
    private TilePool pool;
    [SerializeField] 
    private FirAnimationsManager winPopup;
    [SerializeField] 
    private WinLevelUnlockAnimations winAnimations;
    [SerializeField] 
    private FirAnimationsManager losePopup;
    
    [SerializeField] 
    private TextMeshProUGUI allPlayerGold;
    public TextMeshProUGUI roundPlayerGold;
    public TextMeshProUGUI OpenPairs;

    public TilesHand TilesHand;
    
    private SaveData player;
    public MajhongTileView tile;

    public int roundScores;

    public event Action OnTilesChanged;

    public DateTime lastComboTime;
    public int defaultPoints = 10;
    public int comboBonusPoints = 5;
    public float comboTimePeriod = 10;
    public int comboCounter;
    public bool IsComboEnable;

    private static Rules rules;

    public void Initialize(SaveData player)
    {
        this.player = player;
        player.OnGoldChange += PlayerGoldChanged;

        allPlayerGold.text = player.GoldCoins.ToString();
        roundPlayerGold.text = "+" + roundScores;

        IsComboEnable = player.Difficulty > 0;
        
        if (player.GameMode == GameMode.Collecting)
        {
            OpenPairs.transform.parent.gameObject.SetActive(false);
            rules = new CollectingRules(TilesHand);
        }
        else
        {
            OpenPairs.transform.parent.gameObject.SetActive(true);
            Destroy(TilesHand.gameObject);
            rules = new SolitareRules();
        }

        rules.Manager = this;
        rules.pool = pool;
        
        playerHand.OnTileClick += rules.IsCorrectTile;
    }

    public static bool CheckNeighbors(MajhongTileView tile) => rules.CheckNeighbors(tile);
    private void PlayerGoldChanged(int count)
    {
        allPlayerGold.text = count.ToString();
        player.Save();
    }

    [ContextMenu("Win")]
    public void Win()
    {
        SoundManager.Instance.PlayWin();
        int bonus = player.Difficulty switch
        {
            1 => 1000,
            2 => 5000,
            _ => 0
        };
        
        winAnimations.Initialize(player, roundScores, bonus);
        player.AddGold(roundScores + bonus);
        winPopup.gameObject.SetActive(true);
        winPopup.ToStartPoint();
        winPopup.StartAnimations();
        winAnimations.Play();
    }
    [ContextMenu("UnlockAll")]
    public void UnlockAll()
    {
        player.AddGold(100000);
    }
    [ContextMenu("LockAll")]
    public void LockAll()
    {
        player.ResetProgress();
    }
    [ContextMenu("Lose")]
    public void Lose()
    {
        SoundManager.Instance.PlayLose();
        losePopup.gameObject.SetActive(true);
        losePopup.ToStartPoint();
        losePopup.StartAnimations();
    }

    public void UnselectTile()
    {
        if (tile != null)
        {
            tile.Unselect();
        }

        tile = null;
    }

    public void TilesChanged()
    {
        OnTilesChanged?.Invoke();
    }
    
    public bool IsHasPairs(out int pairs)
    {
        pairs = 0;
        HashSet<Sprite> openTiles = new();
        for (int i = 0; i < pool.transform.childCount; i++)
        {
            MajhongTileView tileView = pool.transform.GetChild(i).GetComponent<MajhongTileView>();
            bool isClosed = CheckNeighbors(tileView);
            if(isClosed)
                continue;

            Sprite tileSprite = tileView.Sprite;
            if (!openTiles.Add(tileSprite))
                pairs++;
        }
        
        return pairs > 0;
    }

    public void CheckWinCondition()
    {
        rules.CheckWinCondition();
    }

    private void OnDestroy()
    {
        rules?.Dispose();
    }
}