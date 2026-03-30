using System;
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
        
        if (!isClosedTile)
        {
            if (this.tile != null)
            {
                if (this.tile == tile)
                {
                    UnselectTile();
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
                });
                UnselectTile();
                return;
                /*if (this.tile.Sprite == tile.Sprite)
                {
                    CheckWinCondition();
                    return;
                }*/
            }
            
            UnselectTile();
            this.tile = tile;
            tile.SelectedAnimation();
        }
        else
        {
            tile.ErrorAnimation();
        }
    }

    private void CheckWinCondition()
    {
        
    }

    private void UnselectTile()
    {
        if (tile != null)
        {
            tile.Unselect();
        }

        tile = null;
    }

    public bool CheckNeighbors(MajhongTileView tile)
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
    private bool ChechTilesLyingOnRight(MajhongTileView tile)
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
    private bool ChechTilesLyingOnLeft(MajhongTileView tile)
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
    private bool ChechTilesLyingOnTop(MajhongTileView tile)
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
