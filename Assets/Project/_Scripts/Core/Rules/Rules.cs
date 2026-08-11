using System;

public abstract class Rules
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
}

public class SolitareRules : Rules
{
    public override void IsCorrectTile(MajhongTileView tile)
    {
        bool isClosedTile = CheckNeighbors(tile);

        if (isClosedTile)
        {
            tile.ErrorAnimation();
            Manager.UnselectTile();
            return;
        }

        if (Manager.tile == null)
        {
            Manager.UnselectTile();
            Manager.tile = tile;
            tile.SelectedAnimation();
            return;
        }

        if (Manager.tile == tile)
        {
            tile.ClickUnselect();
            Manager.UnselectTile();
            return;
        }

        if (Manager.tile.Sprite != tile.Sprite)
        {
            Manager.UnselectTile();
            Manager.tile = tile;
            tile.SelectedAnimation();
            return;
        }
        
        MajhongTileView tile1 = Manager.tile;
        tile1.RaycastDisable();
        tile1.IsPlayable = false;
        tile.RaycastDisable();
        tile.IsPlayable = false;

        Manager.TilesChanged();

        if ((DateTime.Now - Manager.lastComboTime).TotalSeconds > Manager.comboTimePeriod)
        {
            Manager.comboCounter = 0;
        }
        else //combo
        {
            if(Manager.IsComboEnable) //on Easy-mode no combo bonus
                Manager.comboCounter++;
        }

        Manager.lastComboTime = DateTime.Now;
        int scores = Manager.defaultPoints + Manager.comboBonusPoints * Manager.comboCounter;
        
        Manager.effects.FlyTiles(tile1, tile, scores, () =>
        {
            Manager.roundScores += scores;
            Manager.roundPlayerGold.text = "+" + Manager.roundScores;
            pool.Release(tile1);
            pool.Release(tile);
            
            CheckWinCondition();
        });
        
        Manager.UnselectTile();
    }
    
    public override void CheckWinCondition()
    {
        if (pool.transform.childCount <= 0)
        {
            Manager.Win();
            return;
        }

        bool isOnGame = Manager.IsHasPairs(out int pairs);
        Manager.OpenPairs.text = pairs.ToString();

        if(isOnGame)
            return;
        
        Manager.Lose();
    }
}
public class CollectingRules : Rules
{
    public TilesHand TilesHand;
    
    public override void IsCorrectTile(MajhongTileView tile)
    {
        TilesHand.AddTile(tile);
    }

    public override void CheckWinCondition()
    {
        if (TilesHand.Full)
        {
            Manager.Lose();
            return;
        }
            
        if(TilesHand.TilesCount <= 0)
            if (pool.transform.childCount <= 0)
            {
                Manager.Win();
                return;
            }
        else
        {
            
        }

        bool isOnGame = Manager.IsHasPairs(out int pairs);
        Manager.OpenPairs.text = pairs.ToString();

        if(isOnGame)
            return;
        
        Manager.Lose();
    }
}