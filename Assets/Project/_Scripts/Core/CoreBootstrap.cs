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
    [SerializeField] 
    private Desk2[] desks2;
    //[SerializeField, Min(9)] 
    //private int NumberOfUniqueTiles;
    [SerializeField] 
    private Settings settings;
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
    private Desk2 desk;
    
    [ContextMenu("DeckInitialize")]
    private void Awake()
    {
        LoadSaves();
        pool.ClearAll(instant: true);
        StartCoroutine(DeckInitialize(EmptyDesk()));
        rules.Initialize(player);
        spells.Initialize(player);
        settings.Initialize();
    }

    private List<MajhongTileView> EmptyDesk()
    {
        List<Sprite> listTiles = new(desk.TilesPositions.Count);
        int pairs = desk.TilesPositions.Count / 2;
        int lastTileIndex = Math.Min(tileData.Tiles.Length, pairs);
        List<int> possibleTiles = FillListWhisTiles(lastTileIndex);
        
        while(listTiles.Count < desk.TilesPositions.Count)
        {
            int randomTile = possibleTiles.PullRandom();
            if (possibleTiles.Count <= 0)
                possibleTiles = FillListWhisTiles(lastTileIndex);
            
            listTiles.Add(tileData.Tiles[randomTile]);
            listTiles.Add(tileData.Tiles[randomTile]);
        }

        //Empty Desk
        List<MajhongTileView> tilesView = new();
        Dictionary<MajhongTileView, DeckTile> dictionaryViewTile = new();
        Dictionary<Vector3, MajhongTileView> dictionaryTileView = new();
        int index = 0;
        foreach(var deckTile in desk.TilesPositions)
        {
            MajhongTileView tile = pool.Get();
            tile.DisableVisual();
            tile.gameObject.name = "Tile" + tilesView.Count;
            tile.transform.position = deckTile.position;
            int floor = (int)(deckTile.position.z / -1.6f);
            tile.SetData(listTiles[index]);
            index++;
            tile.SetDefaultMaterial(floorMaterials[floor]);
            tilesView.Add(tile);
            dictionaryViewTile.Add(tile, deckTile);
            dictionaryTileView.Add(deckTile.position, tile);
        }
        
        foreach(MajhongTileView tileView in tilesView)
        {
            DeckTile deckTile = dictionaryViewTile[tileView];
            tileView.IsOpenOnStart = deckTile.IsOpenOnStart;
            if(tileView.IsOpenOnStart)
                continue;

            tileView.UpNeighbors = new(4);
            foreach (Vector3 tile in deckTile.UpNeighbors)
            {
                tileView.UpNeighbors.Add(dictionaryTileView[tile]);
            }
            tileView.LeftNeighbors = new(2);
            foreach (Vector3 tile in deckTile.LeftNeighbors)
            {
                tileView.LeftNeighbors.Add(dictionaryTileView[tile]);
            }
            tileView.RightNeighbors = new(2);
            foreach (Vector3 tile in deckTile.RightNeighbors)
            {
                tileView.RightNeighbors.Add(dictionaryTileView[tile]);
            }
        }
        
        return tilesView;
    }
    private List<int> FillListWhisTiles(int lastTileIndex)
    {
        List<int> ints = new(lastTileIndex);
        for (int i = 0; i < lastTileIndex; i++)
        {
            int index = i;
            ints.Add(index);
        }

        return ints;
    }

    private void LoadSaves()
    {
        player = SaveLoadSystem<ProgressData>.Load("Player", Default: new());
        tileData = tiles.First(t => string.Equals(t.ID, player.tilesID));
        desk = desks2.First(d => string.Equals(d.ID, player.deskID));
    }

    public void Shuffle()
    {
        StartCoroutine(DeckInitialize(pool.GetAll()));
    }
    
    private IEnumerator DeckInitialize(List<MajhongTileView> listTiles)
    {
        rules.UnselectTile();
        
        yield return null;
        
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
        int index = 0;
        float tileOffset = 0.001f;
        foreach (var tile in tilesToSpawn)
        {
            tile.RaycastDisableEditor();
            Vector3 startPosition = tile.GetComponent<RectTransform>().anchoredPosition3D;
            var animation = tile.gameObject.AddComponent<FirPositionAnimation>();
            animation.OnComplete += () =>
            {
                animation.OnComplete = null;
                Destroy(animation);
            };
            animation.Curve = curve;
            animation.enabled = false;
            Vector3 _startAnimationPosition = tileStartAnimationPoint.position + Vector3.forward * tileOffset * index;
            index++;
            animation.StartPosition = _startAnimationPosition;
            animation.EndPosition = startPosition;
            tile.transform.position = _startAnimationPosition;
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
            animationRotation.EndZoom = new Vector3(0,180,180);
            animationRotation.OnComplete += () =>
            {
                tile.GetComponent<FirZoomAnimation>().Play();
                animationRotation.OnComplete = null;
                Destroy(animationRotation);
                if(tilesCounter % 2 == 0)
                    SoundManager.Instance.PlayTileSelect(transform.position);
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
        lastAnimationRotation.EndZoom = new Vector3(0,180,180);
        lastAnimationRotation.OnComplete += () =>
        {
            lastTile.GetComponent<FirZoomAnimation>().Play();
            lastAnimationRotation.OnComplete = null;
            Destroy(lastAnimationRotation);
            SoundManager.Instance.PlayTileSelect(transform.position);
            tilesCounter++;
        };
        lastAnimationRotation.Curve = curveRotation;
        lastAnimationRotation.Play();
        
        yield return new WaitUntil(() => tilesCounter == tilesToSpawn.Count);
        
        foreach (var tile in tilesToSpawn)
        {
            tile.RaycastEnableEditor();
        }

        rules.CheckWinCondition();
        spells.ButtonsOn();
    }

    private List<Sprite> GetShuffeledDatas(List<MajhongTileView> listTiles)
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
