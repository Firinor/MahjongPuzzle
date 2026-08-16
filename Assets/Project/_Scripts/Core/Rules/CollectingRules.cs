using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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

    private void CheckPairs(TileInHandViewFrame tile)
    {
        bool checkLose = true;
        
        foreach (TileInHandViewFrame tilePretendent in TilesHand.Tiles)
        {
            if (!tilePretendent.IsFull)
                break;
            if (tilePretendent == tile)
                break;

            if (!tilePretendent.TileView.InGame)
                continue;

            if (tile.TileView.Face.sprite == tilePretendent.TileView.Face.sprite)
            {
                checkLose = false;
                tile.TileView.InGame = false;
                tilePretendent.TileView.InGame = false;
                CollideEffect(tilePretendent, tile);
                break;
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
        TileInHandView handTile1 = tile1.TileView;
        TileInHandView handTile2 = tile2.TileView;
        Debug.Log("Tile1 "+ majTile1.Sprite.name + " Tile2 "+ majTile2.Sprite.name);
        Manager.effects.FlyTiles(tile1, tile2, scores, () =>
        {
            Manager.roundScores += scores;
            Manager.roundPlayerGold.text = "+" + Manager.roundScores;
            Debug.Log("Tile1aft "+ majTile1.Sprite.name + " Tile2aft "+ majTile2.Sprite.name);
            pool.Release(majTile1);
            pool.Release(majTile2);
            Object.Destroy(handTile1.gameObject);
            if(tile1.TileView == handTile1)
                tile1.TileView = null;
            Object.Destroy(handTile2.gameObject);
            if(tile2.TileView == handTile2)
                tile2.TileView = null;
            
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