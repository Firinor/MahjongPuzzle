using System;

public class CollectingRules : Rules, IDisposable
{
    private readonly TilesHand TilesHand;

    public CollectingRules(TilesHand tilesHand)
    {
        TilesHand = tilesHand;
        TilesHand.OnFlyEndAnimation += CheckPairs;
        TilesHand.OnCollideEndAnimation += CheckWinCondition;
    }

    public override void IsCorrectTile(MajhongTileView tile)
    {
        bool isClosedTile = CheckNeighbors(tile);

        if (isClosedTile)
        {
            tile.ErrorAnimation();
            Manager.UnselectTile();
            return;
        }
        
        tile.SelectedSound();
        TilesHand.AddTile(tile);
    }

    private void CheckPairs()
    {
        foreach (TileInHandView tile1 in TilesHand.Tiles)
        {
            if (!tile1.IsFull)
                continue;

            foreach (TileInHandView tile2 in TilesHand.Tiles)
            {
                if (!tile2.IsFull)
                    continue;

                if (tile1 == tile2)
                    continue;

                if (tile1.Face.sprite == tile2.Face.sprite)
                {
                    CollideEffect(tile1, tile2);
                    return;
                }
            }
        }
    }
    private void CollideEffect(TileInHandView tile1, TileInHandView tile2)
    {
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
        
        Manager.effects.FlyTiles(tile1, tile2, scores, () =>
        {
            Manager.roundScores += scores;
            Manager.roundPlayerGold.text = "+" + Manager.roundScores;
            pool.Release(tile1.TileOwner);
            pool.Release(tile2.TileOwner);
            TilesHand.MoveTiles();
            
            CheckWinCondition();
        });
    }
    
    public override void CheckWinCondition()
    {
        if (TilesHand.HasNoSpace)
        {
            Manager.Lose();
            return;
        }
            
        if(TilesHand.TilesCount <= 0 && pool.transform.childCount <= 0)
            Manager.Win();
    }

    public override void Dispose()
    {
        TilesHand.OnFlyEndAnimation -= CheckPairs;
        TilesHand.OnCollideEndAnimation -= CheckWinCondition;
        GC.SuppressFinalize(this);
    }
}