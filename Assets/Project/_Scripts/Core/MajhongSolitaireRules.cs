using System;
using System.Collections.Generic;
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
    private GameObject winPopup;
    [SerializeField] 
    private GameObject losePopup;
    
    [SerializeField] 
    private TextMeshProUGUI winPopupGoldText;
    [SerializeField] 
    private TextMeshProUGUI allPlayerGold;
    [SerializeField] 
    private TextMeshProUGUI roundPlayerGold;

    private ProgressData player;
    private MajhongTileView tile;

    private int roundScores;
    
    private const float TileWidth = 2.2f;
    private const float TileThickness = 1.6f;

    public event Action OnTilesChanged;

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
        tile.RaycastDisable();
    
        OnTilesChanged?.Invoke();
    
        effects.FlyTiles(tile1, tile, tile1.Data.Points, () =>
        {
            player.AddGold(tile1.Data.Points);
            roundScores += tile1.Data.Points;
            roundPlayerGold.text = "+" + roundScores;
            pool.Release(tile1);
            pool.Release(tile);
            SaveLoadSystem<ProgressData>.Save("Player", player);
            
            CheckWinCondition();
        });
        
        UnselectTile();
    }

    private void CheckWinCondition()
    {
        if (pool.transform.childCount == 0)
        {
            winPopup.SetActive(true);
            winPopupGoldText.text = roundPlayerGold.text;
            return;
        }

        HashSet<Sprite> openTiles = new();
        for (int i = 0; i < pool.transform.childCount; i++)
        {
            MajhongTileView tileView = pool.transform.GetChild(i).GetComponent<MajhongTileView>();
            bool openTile = CheckNeighbors(tileView);
            if(!openTile)
                continue;

            Sprite tileSprite = tileView.Sprite;
            if(!openTiles.Add(tileSprite))
                return;
        }
        
        losePopup.SetActive(true);
    }

    private void UnselectTile()
    {
        if (tile != null)
        {
            tile.Unselect();
        }

        tile = null;
    }

    public static bool CheckNeighbors(MajhongTileView tile)
    {
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
        if (Physics.Raycast(tile.RayPoints[2].position,
                Vector3.right,
                out RaycastHit hit,
                TileWidth))
            return true;
        if (Physics.Raycast(tile.RayPoints[3].position,
                Vector3.right,
                out RaycastHit hit2,
                TileWidth))
            return true;

        return false;
    }
    private static bool ChechTilesLyingOnLeft(MajhongTileView tile)
    {
        if (Physics.Raycast(tile.RayPoints[0].position,
                Vector3.left,
                out RaycastHit hit,
                TileWidth))
            return true;
        if (Physics.Raycast(tile.RayPoints[1].position,
                Vector3.left,
                out RaycastHit hit2,
                TileWidth))
            return true;

        return false;
    }
    private static bool ChechTilesLyingOnTop(MajhongTileView tile)
    {
        foreach (var tileRayPoint in tile.RayPoints)
        {
            if (Physics.Raycast(tileRayPoint.position,
                    Vector3.back,
                    out RaycastHit hit,
                    TileThickness))
                return true;
        }

        return false;
    }
}
