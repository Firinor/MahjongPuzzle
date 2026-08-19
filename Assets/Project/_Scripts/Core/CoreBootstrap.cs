using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FirAnimations;
using UnityEngine;

public class CoreBootstrap : MonoBehaviour
{
    public FirAnimation closeСurtain;  
    
    [SerializeField] 
    private TilesData[] tiles;
    [SerializeField] 
    private Desk2[] desks2;
    //[SerializeField, Min(9)] 
    //private int NumberOfUniqueTiles;
    [SerializeField] 
    private Settings settings;
    [SerializeField] 
    private TilePool pool;
    [SerializeField] 
    private CoreRulesManager rulesManager;
    [SerializeField] 
    private CameraMover cameraMover;
    [SerializeField] 
    private SpellManager spells;
    [SerializeField] 
    private PlayerInputHolder inputHolder;
    [SerializeField] 
    private CameraStartPosition cameraStartPosition;
    
    [SerializeField] 
    private Transform tileStartAnimationPoint;
    
    [SerializeField] 
    private Material[] floorMaterials;
    
    private SaveData player;
    private TilesData tileData;
    private Desk2 desk;

    private bool startSignal;
    
    [ContextMenu("DeckInitialize")]
    private IEnumerator Start()
    {
        closeСurtain.Initialize();
        
        yield return null;
        
        closeСurtain.Play();//OpenScene
        
        LoadPlayerData();
        
        settings.Initialize();
        pool.ClearAll(instant: true);
        StartCoroutine(DeckInitialize(EmptyDesk()));
        rulesManager.Initialize(player);
        spells.Initialize(player);
        cameraMover.Initialize();
        cameraStartPosition.Initialize(player);

        inputHolder.onClick += FastStart;
    }

    private void FastStart(Vector2 point)
    {
        startSignal = true;
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
            int floor = (int)(deckTile.position.z / -0.607f);
            tile.gameObject.name = "Tile z" + floor + "x" + (int)deckTile.position.x + "y" + (int)deckTile.position.y;
            tile.transform.position = deckTile.position;
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
                //if(dictionaryTileView.ContainsKey(tile))
                    tileView.UpNeighbors.Add(dictionaryTileView[tile]);
            }
            tileView.LeftNeighbors = new(2);
            foreach (Vector3 tile in deckTile.LeftNeighbors)
            {
                //if(dictionaryTileView.ContainsKey(tile))
                    tileView.LeftNeighbors.Add(dictionaryTileView[tile]);
            }
            tileView.RightNeighbors = new(2);
            foreach (Vector3 tile in deckTile.RightNeighbors)
            {
                //if(dictionaryTileView.ContainsKey(tile))
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

    private void LoadPlayerData()
    {
        player = SaveData.GetPlayer();
        player.FirstLoad();
        tileData = tiles.First(t => string.Equals(t.ID, player.TilesID));
        desk = desks2.First(d => string.Equals(d.ID, player.DeskID));
    }

    public void Shuffle()
    {
        StartCoroutine(DeckInitialize(pool.GetAll()));
    }
    
    private IEnumerator DeckInitialize(List<MajhongTileView> listTiles)
    {
        startSignal = false;
        
        rulesManager.UnselectTile();
        spells.DisableShuffle();
        
        foreach (var tile in listTiles)
        {
            tile.isHint = false;
        }

        yield return null;

        List<MajhongTileView> tilesToSpawn;
        int fallbackIndexer = 0;
        do {
            tilesToSpawn = (player.Difficulty) switch
            {
                0 => DifficultyShuffle.ShuffleEasy(listTiles),
                2 => DifficultyShuffle.ShuffleHard(listTiles),
                _ => DifficultyShuffle.ShuffleNormal(listTiles),
            };
            fallbackIndexer++;
        } 
        while (!rulesManager.IsHasPairs(out int pairs) && fallbackIndexer < 10);


        //Animations
        tilesToSpawn = tilesToSpawn
            .OrderByDescending(t => (int)(t.transform.position.z*10))
            .ThenByDescending(t => (int)(t.transform.position.y*10))
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
        float tileOffset = 0.003f;
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
                tile.EnableShadow();
                animationRotation.OnComplete = null;
                Destroy(animationRotation);
                SoundManager.Instance.PlayTileSelect(transform.position, 0.4f);
                tilesCounter++;
            };
            animationRotation.Curve = curveRotation;
            animationRotation.Play();
            if(!startSignal)
                yield return new WaitForSeconds(delta);
            delta *= 0.996f;
        }
        if(!startSignal)
            yield return new WaitForSeconds(1);
        
        //lastTile
        lastTile.GetComponent<FirPositionAnimation>().Play();
        var lastAnimationRotation = lastTile.gameObject.AddComponent<FirRotationAnimation>();
        lastAnimationRotation.StartZoom = Vector3.zero;
        lastAnimationRotation.EndZoom = new Vector3(0,180,180);
        lastAnimationRotation.OnComplete += () =>
        {
            lastTile.GetComponent<FirZoomAnimation>().Play();
            lastTile.EnableShadow();
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

        rulesManager.CheckWinCondition();
        spells.ButtonsOn();
        spells.EnableShuffle();
    }

    private void OnDestroy()
    {
        inputHolder.onClick -= FastStart;
    }
}
