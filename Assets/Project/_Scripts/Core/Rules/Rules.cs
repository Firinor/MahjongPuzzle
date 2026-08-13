using System;

public abstract class Rules: IDisposable
{
    public CoreRulesManager Manager;
    public TilePool pool;
    
    public bool CheckNeighbors(MajhongTileView tile)
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
    private bool ChechTilesLyingOnRight(MajhongTileView tile)
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
    private bool ChechTilesLyingOnLeft(MajhongTileView tile)
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
    private bool ChechTilesLyingOnTop(MajhongTileView tile)
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

    public abstract void IsCorrectTile(MajhongTileView tile);
    public abstract void CheckWinCondition();
    public abstract void Dispose();
}