using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectingRules : Rules, IDisposable
{
    private readonly TilesHand TilesHand;

    public CollectingRules(TilesHand tilesHand)
    {
        TilesHand = tilesHand;
        TilesHand.OnFlyEndAnimation += CheckPairs;
        //TilesHand.OnCollideEndAnimation += CheckWinCondition;
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
        
        if(TilesHand.Full)
            return;
        
        tile.SelectedSound();
        TilesHand.AddTile(tile);
    }

    private void CheckPairs()
    {
        bool checkLose = true;
        
        foreach (TileInHandViewFrame tile1 in TilesHand.Tiles)
        {
            if (!tile1.IsFull)
                continue;
            
            if (!tile1.TileView.InGame)
                continue;
            
            if (tile1.TileView.PositionAnimation.enabled)
                continue;

            foreach (TileInHandViewFrame tile2 in TilesHand.Tiles)
            {
                if (!tile2.IsFull)
                    continue;
                
                if (!tile2.TileView.InGame)
                    continue;
                
                if (tile2.TileView.PositionAnimation.enabled)
                    continue;
                
                if (tile1 == tile2)
                    continue;

                if (tile1.TileView.Face.sprite == tile2.TileView.Face.sprite)
                {
                    checkLose = false;
                    tile1.TileView.InGame = false;
                    tile2.TileView.InGame = false;
                    CollideEffect(tile1, tile2);
                    break;
                }
            }
        }
        
        if(checkLose)
            CheckWinCondition();
    }

    private void CollideEffect(TileInHandViewFrame tile1, TileInHandViewFrame tile2)
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

        MajhongTileView majTile1 = tile1.TileView.TileOwner;
        MajhongTileView majTile2 = tile2.TileView.TileOwner;
        Debug.Log("Tile1 "+ majTile1.Sprite.name + " Tile2 "+ majTile2.Sprite.name);
        Manager.effects.FlyTiles(tile1, tile2, scores, () =>
        {
            Manager.roundScores += scores;
            Manager.roundPlayerGold.text = "+" + Manager.roundScores;
            Debug.Log("Tile1aft "+ majTile1.Sprite.name + " Tile2aft "+ majTile2.Sprite.name);
            pool.Release(majTile1);
            pool.Release(majTile2);
            tile1.Hide();
            tile2.Hide();
            
            TilesHand.MoveTiles();
            
            CheckWinCondition();
        });
        TilesHand.MoveTiles();
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
        //TilesHand.OnCollideEndAnimation -= CheckWinCondition;
        GC.SuppressFinalize(this);
    }
}