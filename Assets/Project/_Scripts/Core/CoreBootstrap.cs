using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoreBootstrap : MonoBehaviour
{
    [SerializeField] 
    private TilesData[] tiles;
    [SerializeField] 
    private Desk[] desks;
    //[SerializeField, Min(9)] 
    //private int NumberOfUniqueTiles;
    [SerializeField] 
    private TilePool pool;
    [SerializeField] 
    private MajhongSolitaireRules rules;
    [SerializeField] 
    private SpellManager spells;
    
    [SerializeField] 
    private Material[] floorMaterials;
    
    private ProgressData player;
    private TilesData tileData;
    private Desk desk;
    
    [ContextMenu("DeckInitialize")]
    private void Awake()
    {
        LoadSaves();
        pool.ClearAll(instant: true);
        StartCoroutine(DeckInitialize());
        rules.Initialize(player);
        spells.Initialize(player);
    }

    private void LoadSaves()
    {
        player = SaveLoadSystem<ProgressData>.Load("Player", Default: new());
        tileData = tiles.First(t => string.Equals(t.ID, player.tilesID));
        desk = desks.First(d => string.Equals(d.ID, player.deskID));
    }

    private IEnumerator DeckInitialize()
    {
        int pairs = desk.TilesPositions.Count / 2;
        int lastTileIndex = Math.Min(tileData.Tiles.Length, pairs);

        List<Tile> listTiles = new(desk.TilesPositions.Count);
        var possibleTiles = FillListWhisTiles();
        
        while (listTiles.Count < desk.TilesPositions.Count)
        {
            int randomTile = possibleTiles.PullRandom();
            if (possibleTiles.Count <= 0)
                possibleTiles = FillListWhisTiles();
            
            listTiles.Add(tileData.Tiles[randomTile]);
            listTiles.Add(tileData.Tiles[randomTile]);
        }

        //Empty Desk
        List<MajhongTileView> tilesView = new();
        foreach (var position in desk.TilesPositions)
        {
            MajhongTileView tile = pool.Get();
            tile.gameObject.name = "Tile" + tilesView.Count;
            tile.transform.position = position;
            int floor = (int)(position.z / -1.6f);
            tile.SetDefaultMaterial(floorMaterials[floor]);
            tilesView.Add(tile);
        }

        yield return null;
        
        List<MajhongTileView> tilesToSpawn = new();
        
        //Decomposition
        int Count = tilesView.Count;
        while (tilesToSpawn.Count < Count)
        {
            List<MajhongTileView> tilesToCheck = new(tilesView);
            MajhongTileView randomTile1 = null;
            while(randomTile1 == null)
            {
                MajhongTileView randomTile = tilesToCheck.PullRandom();
                if (MajhongSolitaireRules.CheckNeighbors(randomTile))
                    continue;
                
                randomTile1 = randomTile;
                tilesToSpawn.Add(randomTile1);
                tilesView.Remove(randomTile1);
            }
            MajhongTileView randomTile2 = null;
            while(randomTile2 == null)
            {
                MajhongTileView randomTile = tilesToCheck.PullRandom();
                if (MajhongSolitaireRules.CheckNeighbors(randomTile))
                    continue;
                
                randomTile2 = randomTile;
                tilesToSpawn.Add(randomTile2);
                tilesView.Remove(randomTile2);
            }
            randomTile1.gameObject.SetActive(false);
            randomTile2.gameObject.SetActive(false);
        }
        
        //Initialization
        int currentIndex = 0;
        //tilesToSpawn.Reverse();
        foreach (var tile in tilesToSpawn)
        {
            tile.gameObject.SetActive(true);
            tile.SetData(listTiles[currentIndex]);
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
