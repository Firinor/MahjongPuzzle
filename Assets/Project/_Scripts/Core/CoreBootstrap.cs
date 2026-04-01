using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FirAnimations;
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
    private Transform tileStartAnimationPoint;
    
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
        foreach (var tile in tilesToSpawn)
        {
            tile.SetData(listTiles[currentIndex]);
            currentIndex++;
        }

        //Animations
        tilesToSpawn = tilesToSpawn
            .OrderByDescending(t => t.transform.position.z)
            .ThenByDescending(t => t.transform.position.y)
            .ThenBy(t => t.transform.position.x)
            .ToList();
        
        AnimationCurve curve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(1f, 1f, 2f, 2f)
        );
        AnimationCurve curveRotation = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(1f, 2f, 2f, 2f)
        );
        
        foreach (var tile in tilesToSpawn)
        {
            tile.RaycastDisableEditor();
            Vector3 startPosition = tile.transform.position;
            var animation = tile.gameObject.AddComponent<FirPositionAnimation>();
            animation.OnComplete += () =>
            {
                animation.OnComplete = null;
                Destroy(animation);
            };
            animation.Curve = curve;
            animation.enabled = false;
            animation.StartPosition = tileStartAnimationPoint.position;
            animation.EndPosition = startPosition;
            tile.transform.position = tileStartAnimationPoint.position;
            tile.gameObject.SetActive(true);
        }
        
        float delta = 0.04f;
        int tilesCounter = 0;
        MajhongTileView lastTile = tilesToSpawn[^1];
        foreach (var tile in tilesToSpawn)
        {
            if(tile == lastTile)
                continue;
            
            tile.GetComponent<FirPositionAnimation>().Play();
            var animationRotation = tile.gameObject.AddComponent<FirRotationAnimation>();
            animationRotation.StartZoom = Vector3.zero;
            animationRotation.EndZoom = new Vector3(0,0,180);
            animationRotation.OnComplete += () =>
            {
                animationRotation.OnComplete = null;
                Destroy(animationRotation);
                tilesCounter++;
            };
            animationRotation.Curve = curveRotation;
            animationRotation.Play();
            yield return new WaitForSeconds(delta);
            delta *= 0.995f;
        }
        yield return new WaitForSeconds(1);
        
        //lastTile
        lastTile.GetComponent<FirPositionAnimation>().Play();
        var lastAnimationRotation = lastTile.gameObject.AddComponent<FirRotationAnimation>();
        lastAnimationRotation.StartZoom = Vector3.zero;
        lastAnimationRotation.EndZoom = new Vector3(0,0,180);
        lastAnimationRotation.OnComplete += () =>
        {
            lastAnimationRotation.OnComplete = null;
            Destroy(lastAnimationRotation);
            tilesCounter++;
        };
        lastAnimationRotation.Curve = curveRotation;
        lastAnimationRotation.Play();
        
        yield return new WaitUntil(() => tilesCounter == tilesToSpawn.Count);
        
        foreach (var tile in tilesToSpawn)
        {
            tile.RaycastEnableEditor();
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
