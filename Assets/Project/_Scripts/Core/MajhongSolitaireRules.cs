using System;
using System.Collections.Generic;
using FirAnimations;
using TMPro;
using UnityEngine;

public class MajhongSolitaireRules : MonoBehaviour
{
    [SerializeField] 
    private PlayerHand playerHand;
    [SerializeField] 
    private TilesEffects effects;
    [SerializeField] 
    private TilePool pool;
    [SerializeField] 
    private FirAnimationsManager winPopup;
    [SerializeField] 
    private WinLevelUnlockAnimations winAnimations;
    [SerializeField] 
    private float winAnimationsDelay;
    [SerializeField] 
    private FirAnimationsManager losePopup;
    
    [SerializeField] 
    private TextMeshProUGUI winPopupGoldText;
    [SerializeField] 
    private TextMeshProUGUI allPlayerGold;
    [SerializeField] 
    private TextMeshProUGUI roundPlayerGold;
    [SerializeField] 
    private TextMeshProUGUI openPairs;
    
    private ProgressData player;
    [SerializeField]
    private MajhongTileView tile;

    private int roundScores;

    public event Action OnTilesChanged;

    private DateTime lastComboTime;
    [SerializeField] 
    private int defaultPoints = 10;
    [SerializeField] 
    private int comboBonusPoints = 5;
    [SerializeField] 
    private float comboTimePeriod = 10;
    private int comboCounter;
    

    public void Initialize(ProgressData player)
    {
        this.player = player;
        player.OnGoldChange += PlayerGoldChanged;
        playerHand.OnTileClick += IsCorrectTile;

        allPlayerGold.text = player.GoldCoins.ToString();
        roundPlayerGold.text = "+" + roundScores;
    }

    private void PlayerGoldChanged(int count)
    {
        allPlayerGold.text = count.ToString();
        SaveLoadSystem<ProgressData>.Save("Player", player);
    }

    private void IsCorrectTile(MajhongTileView tile)
    {
        bool isClosedTile = CheckNeighbors(tile);

        if (isClosedTile)
        {
            tile.ErrorAnimation();
            UnselectTile();
            return;
        }

        if (this.tile == null)
        {
            UnselectTile();
            this.tile = tile;
            tile.SelectedAnimation();
            return;
        }

        if (this.tile == tile)
        {
            tile.ClickUnselect();
            UnselectTile();
            return;
        }

        if (this.tile.Sprite != tile.Sprite)
        {
            UnselectTile();
            this.tile = tile;
            tile.SelectedAnimation();
            return;
        }
        
        MajhongTileView tile1 = this.tile;
        tile1.RaycastDisable();
        tile1.IsPlayable = false;
        tile.RaycastDisable();
        tile.IsPlayable = false;
        
        OnTilesChanged?.Invoke();

        if ((DateTime.Now - lastComboTime).TotalSeconds > comboTimePeriod)
        {
            comboCounter = 0;
        }
        else //combo
        {
            comboCounter++;
        }

        lastComboTime = DateTime.Now;
        int scores = defaultPoints + comboBonusPoints * comboCounter;
        
        effects.FlyTiles(tile1, tile, scores, () =>
        {
            roundScores += scores;
            roundPlayerGold.text = "+" + roundScores;
            pool.Release(tile1);
            pool.Release(tile);
            SaveLoadSystem<ProgressData>.Save("Player", player);
            
            CheckWinCondition();
        });
        
        UnselectTile();
    }

    public void CheckWinCondition()
    {
        if (pool.transform.childCount <= 0)
        {
            Win();
            winPopupGoldText.text = roundPlayerGold.text;
            return;
        }

        int pairs = 0;
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

        openPairs.text = pairs.ToString();
        if (pairs > 0)
            return;

        Lose();
    }

    [ContextMenu("Win")]
    public void Win()
    {
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
        winAnimations.Play(delay: winAnimationsDelay);
    }
    [ContextMenu("Lose")]
    public void Lose()
    {
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

    public static bool CheckNeighbors(MajhongTileView tile)
    {
        if (tile.IsOpenOnStart)
            return false;
        
        if (ChechTilesLyingOnTop(tile))
            return true;

        bool isNeighborLeft = ChechTilesLyingOnLeft(tile);
        if (!isNeighborLeft)
            return false;

        bool isNeighborRight = ChechTilesLyingOnRight(tile);
        if (!isNeighborRight)
            return false;
        
        return true;
    }
    private static bool ChechTilesLyingOnRight(MajhongTileView tile)
    {
        if (tile.RightNeighbors == null)
            return false;
        
        foreach (var tileToCheck in tile.RightNeighbors)
        {
            if (tileToCheck.IsPlayable)
                return true;
        }

        return false;
    }
    private static bool ChechTilesLyingOnLeft(MajhongTileView tile)
    {
        if (tile.LeftNeighbors == null)
            return false;
        
        foreach (var tileToCheck in tile.LeftNeighbors)
        {
            if (tileToCheck.IsPlayable)
                return true;
        }

        return false;
    }
    private static bool ChechTilesLyingOnTop(MajhongTileView tile)
    {
        if (tile.UpNeighbors == null)
            return false;
        
        foreach (var tileToCheck in tile.UpNeighbors)
        {
            if (tileToCheck.IsPlayable)
                return true;
        }

        return false;
    }
}
