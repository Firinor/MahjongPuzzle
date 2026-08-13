using System;

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
    public override void Dispose() { }
}