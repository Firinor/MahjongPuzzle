using System;
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
        pool.ClearAll();
        DeckInitialize();
        rules.Initialize(player);
        spells.Initialize(player);
    }

    private void LoadSaves()
    {
        player = SaveLoadSystem<ProgressData>.Load("Player", Default: new());
        tileData = tiles.First(t => string.Equals(t.ID, player.tilesID));
        desk = desks.First(d => string.Equals(d.ID, player.deskID));
    }

    private void DeckInitialize()
    {
        int pairs = desk.TilesPositions.Count / 2;
        int lastTileIndex = Math.Min(tileData.Tiles.Length, pairs);

        List<Tile> listTiles = new(desk.TilesPositions.Count);
        var possibleTiles = FillListWhisTiles();

        int currentIndex = 0;
        while (listTiles.Count < desk.TilesPositions.Count)
        {
            int randomTile = possibleTiles.PullRandom();
            if (possibleTiles.Count <= 0)
                possibleTiles = FillListWhisTiles();
            
            listTiles.Add(tileData.Tiles[randomTile]);
            listTiles.Add(tileData.Tiles[randomTile]);
            currentIndex++;
        }
        
        listTiles.Shuffle();

        currentIndex = 0;
        foreach (var position in desk.TilesPositions)
        {
            MajhongTileView tile = pool.Get();
            tile.SetData(listTiles[currentIndex]);
            tile.transform.position = position;

            int floor = (int)(position.z / -1.6f);
            tile.SetDefaultMaterial(floorMaterials[floor]);
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
