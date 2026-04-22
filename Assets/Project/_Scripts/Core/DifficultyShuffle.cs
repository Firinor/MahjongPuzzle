using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DifficultyShuffle
{
    public static List<MajhongTileView> ShuffleEasy(List<MajhongTileView> listTiles)
    {
        return null;
    }
    public static List<MajhongTileView> ShuffleNormal(List<MajhongTileView> listTiles)
    {
        List<MajhongTileView> tilesToSpawn = new();
        
        List<Sprite> tilesShuffled = GetShuffeledDatas(listTiles);
        
        //Decomposition
        int Count = listTiles.Count;
        while (tilesToSpawn.Count < Count)
        {
            List<MajhongTileView> tilesToCheck = new(listTiles);
            MajhongTileView randomTile1 = null;
            while(randomTile1 == null)
            {
                if (tilesToCheck.Count > 0)
                {
                    MajhongTileView randomTile = tilesToCheck.PullRandom();
                    if (MajhongSolitaireRules.CheckNeighbors(randomTile))
                        continue;
                    randomTile1 = randomTile;
                    listTiles.Remove(randomTile1);
                }
                else
                {
                    MajhongTileView randomTile = listTiles.PullRandom();
                    randomTile1 = randomTile;
                }
                tilesToSpawn.Add(randomTile1);
            }
            MajhongTileView randomTile2 = null;
            while(randomTile2 == null)
            {
                if (tilesToCheck.Count > 0)
                {
                    MajhongTileView randomTile = tilesToCheck.PullRandom();
                    if (MajhongSolitaireRules.CheckNeighbors(randomTile))
                        continue;
                    randomTile2 = randomTile;
                    listTiles.Remove(randomTile2);
                }
                else
                {
                    MajhongTileView randomTile = listTiles.PullRandom();
                    randomTile2 = randomTile;
                }
                tilesToSpawn.Add(randomTile2);
            }
            randomTile1.gameObject.SetActive(false);
            randomTile2.gameObject.SetActive(false);
        }
        
        //Initialization
        int currentIndex = 0;
        foreach (var tile in tilesToSpawn)
        {
            tile.EnableVisual();
            tile.SetData(tilesShuffled[currentIndex]);
            tile.Unselect();
            currentIndex++;
        }

        return tilesToSpawn;
    }
    public static List<MajhongTileView> ShuffleHard(List<MajhongTileView> listTiles)
    {
        return null;
    }
    
    private static List<Sprite> GetShuffeledDatas(List<MajhongTileView> listTiles)
    {
        List<Sprite> result = new();
        foreach (var tileView in listTiles)
        {
            result.Add(tileView.Sprite);
        }

        var sorted = result
            .OrderBy(t => t.GetHashCode())
            .ToList();
        
        var pairs = new List<List<Sprite>>();
        for (int i = 0; i < sorted.Count; i += 2)
        {
            pairs.Add(new(){sorted[i], sorted[i+1]});
        }
        
        pairs.Shuffle();
        
        return pairs.SelectMany(p => p).ToList();
    }
}