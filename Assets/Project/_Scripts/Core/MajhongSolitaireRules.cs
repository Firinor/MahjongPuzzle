using System;
using UnityEngine;

public class MajhongSolitaireRules : MonoBehaviour
{
    [SerializeField] 
    private Material defaultMaterial;
    [SerializeField] 
    private Material selectedMaterial;
    [SerializeField] 
    private PlayerHand player;
    
    private MajhongTileView tile;
    
    private const float TileWidth = 2.2f;
    private const float TileThickness = 1.6f;

    private void Awake()
    {
        player.OnTileClick += IsCorrectTile;
    }

    private void IsCorrectTile(MajhongTileView tile)
    {
        bool isClosedTile = CheckNeighbors(tile);
        
        if (!isClosedTile)
        {
            UnselectTile();
            this.tile = tile;
            tile.SetMaterial(selectedMaterial);
        }
        else
        {
            UnselectTile();
        }
    }

    private void UnselectTile()
    {
        if (tile != null)
        {
            tile.SetMaterial(defaultMaterial);
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
