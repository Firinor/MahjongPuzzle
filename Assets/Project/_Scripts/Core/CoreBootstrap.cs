using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CoreBootstrap : MonoBehaviour
{
    [SerializeField] 
    private TilesData data;
    [SerializeField] 
    private Desk desk;
    [SerializeField, Min(9)] 
    private int NumberOfUniqueTiles;
    [SerializeField] 
    private TilePool pool;
    
    [ContextMenu("DeckInitialize")]
    private void Awake()
    {
        //LoadSaves();
        pool.ClearAll();
        DeckInitialize();
    }

    private void DeckInitialize()
    {
        int pairs = desk.TilesPositions.Count / 2;
        int lastTileIndex = Math.Min(NumberOfUniqueTiles, pairs);

        List<Tile> tiles = new(desk.TilesPositions.Count);
        var possibleTiles = FillListWhisTiles();

        int currentIndex = 0;
        while (tiles.Count < desk.TilesPositions.Count)
        {
            int randomTile = possibleTiles.PullRandom();
            if (possibleTiles.Count <= 0)
                possibleTiles = FillListWhisTiles();
            
            tiles.Add(data.Tiles[randomTile]);
            tiles.Add(data.Tiles[randomTile]);
            currentIndex++;
        }
        
        tiles.Shuffle();

        currentIndex = 0;
        foreach (var position in desk.TilesPositions)
        {
            MajhongTileView tile = pool.Get();
            tile.SetData(tiles[currentIndex]);
            tile.transform.position = position;
            currentIndex++;
        }

        List<int> FillListWhisTiles()
        {
            List<int> ints = new(lastTileIndex);
            for (int i = 0; i < lastTileIndex; i++)
            {
                int index = i;
                ints.Add(index);
            }

            return ints;
        }
    }
}
