using System;
using System.Collections.Generic;
using System.Linq;
using FirAnimations;
using UnityEngine;
using UnityEngine.UI;
#if IS_YANDEX
using YG;
#endif

public class TilesHand : MonoBehaviour
{
    public List<TileInHandViewFrame> Tiles;
    public RectTransform AimObject;
    public Transform Canvas;

    public TileInHandViewFrame TileFramePrefab;
    public TileInHandView TileViewPrefab;
    public int OpenHandTiles = 4;
    public int LimitHandTiles = 8;
    public GameObject losePopup;
    public Button LoseContinueButton;
    public event Action<TileInHandViewFrame> OnFlyEndAnimation;
    
    private static int n;
    
    public int TilesCount => Tiles.Count(t => t.TileView?.TileOwner is not null);
    public bool Full => TilesCount == Tiles.Count(t => !t.IsLock);
    public bool HasNoSpace
    {
        get
        {
            if (!Full) return false;
            int tilesCount = Tiles.Where(t => t.TileView != null 
                                              && t.TileView.Face.sprite != null
                                              && !t.TileView.PositionAnimation.enabled)
                                    .Select(t => t.TileView.Face.sprite)
                                    .Distinct()
                                    .Count();
            return Tiles.Count(t => !t.IsLock) == tilesCount;
        }
    }
    
    public void Initialize()
    {
        Tiles = new();
        for (int i = 0; i < OpenHandTiles; i++)
        {
            var newFrame = Instantiate(TileFramePrefab, transform);
            newFrame.Unlock();
            Tiles.Add(newFrame);
        }
        var lastFrame = Instantiate(TileFramePrefab, transform);
        Tiles.Add(lastFrame);
        lastFrame.Lock.onClick.AddListener(UnlockHandFrame);
        LoseContinueButton.onClick.AddListener(UnlockHandFrame);
    }

    private void UnlockHandFrame()
    {
#if IS_YANDEX
        YG2.RewardedAdvShow("AddHand", AddHandFrame);
#else
        AddHandFrame();
#endif
    }
    private void AddHandFrame()
    {
        if (Tiles.Count >= LimitHandTiles)
        {
            TileInHandViewFrame lastFrame = Tiles[^1];
            lastFrame.Lock.onClick.RemoveAllListeners();
            lastFrame.Unlock();
            LoseContinueButton.interactable = false;
        }
        else
        {
            var newFrame = Instantiate(TileFramePrefab, transform);
            newFrame.Unlock();
            newFrame.transform.SetSiblingIndex(Tiles.Count - 1);
            Tiles.Add(newFrame);
            (Tiles[^1], Tiles[^2]) = (Tiles[^2], Tiles[^1]);
        }
        losePopup.SetActive(false);
    }

    public void AddTile(MajhongTileView tile)
    {
        tile.RaycastDisable();
        tile.IsPlayable = false;

        TileInHandViewFrame firstOpen = Tiles.FirstOrDefault(t => !t.IsFull);

        if (firstOpen is null)
        {
            Debug.LogError("no empty space at TilesHand!");
            return;
        }

        TileInHandView newView = Instantiate(TileViewPrefab, firstOpen.transform);
        newView.gameObject.name = n++.ToString();
        newView.TileOwner = tile;
        firstOpen.TileView = newView;
        
        newView.Face.sprite = tile.Sprite;
        var posStart = Camera.main.WorldToScreenPoint(tile.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas.GetComponent<RectTransform>(),                   
            posStart,                    
            Camera.main,        
            out Vector2 localStartPos           
        );
        AimObject.anchoredPosition = localStartPos;
        
        newView.PositionAnimation.transform.SetParent(AimObject);
        newView.PositionAnimation.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        newView.PositionAnimation.transform.SetParent(firstOpen.transform, worldPositionStays: true);
        
        newView.PositionAnimation.StartPosition = newView.PositionAnimation.GetComponent<RectTransform>().anchoredPosition;
        newView.ColorAnimation.StartPosition = tile.MaterialColor;
        newView.PositionAnimation.ToStartPoint();
        newView.ZoomAnimation.ToStartPoint();
        newView.ColorAnimation.ToStartPoint();
        newView.PositionAnimation.gameObject.SetActive(true);
        
        newView.PositionAnimation.Play();
        newView.ZoomAnimation.Play();
        newView.ColorAnimation.Play();

        newView.PositionAnimation.OnComplete = () =>
        {
            newView.PositionAnimation.OnComplete = null;
            OnFlyEndAnimation?.Invoke(firstOpen);
        };
        
        var zoom = tile.gameObject.AddComponent<FirZoomAnimation>();
        zoom.StartZoom = Vector3.one;
        zoom.EndZoom = Vector3.zero;
        Keyframe[] keys = zoom.Curve.keys;
        int lastIndex = keys.Length - 1;
        keys[lastIndex].time = 0.3f;
        zoom.Curve.keys = keys;
        zoom.ToStartPoint();
        zoom.Play();
        zoom.OnComplete += () => {     
            tile.gameObject.SetActive(false);
            Destroy(zoom); 
        };
    }
    private void MoveTilesFromHand(TileInHandViewFrame fromTile, TileInHandViewFrame toTile)
    {
        var posStart = fromTile.GetComponent<RectTransform>().anchoredPosition.x - toTile.GetComponent<RectTransform>().anchoredPosition.x;
        fromTile.TileView.transform.SetParent(toTile.transform, worldPositionStays: true);
        
        toTile.TileView = fromTile.TileView;
        fromTile.TileView = null;
        
        toTile.TileView.PositionAnimation.StartPosition = new Vector2(posStart, 0);
        toTile.TileView.PositionAnimation.ToStartPoint();
        toTile.TileView.PositionAnimation.gameObject.SetActive(true);
        toTile.TileView.PositionAnimation.Play();
    }
    

    public void MoveTiles()
    {
        for (int i = 0; i < Tiles.Count; i++)//Find empty
        {
            if(Tiles[i].IsFull 
               && Tiles[i].TileView.InGame) 
                continue;
            
            for (int j = i+1; j < Tiles.Count; j++)//Find full
            {
                if(!Tiles[j].IsFull 
                   || !Tiles[j].TileView.InGame
                   || Tiles[j].TileView.PositionAnimation.enabled) 
                    continue;
                
                MoveTilesFromHand(Tiles[j], Tiles[i]);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        LoseContinueButton.onClick.RemoveListener(UnlockHandFrame);
        if(Tiles is null || Tiles.Count <= 0)
            return;
        Tiles[^1].Lock?.onClick.RemoveAllListeners();
        Tiles = null;
    }
}