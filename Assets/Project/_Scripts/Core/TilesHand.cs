using System;
using System.Linq;
using FirAnimations;
using UnityEngine;

public class TilesHand : MonoBehaviour
{
    public TileInHandViewFrame[] Tiles;
    public RectTransform AimObject;
    public Transform Canvas;

    public TileInHandView TileViewPrefab;
    
    public event Action<TileInHandViewFrame> OnFlyEndAnimation;
    
    private static int n;
    
    public int TilesCount => Tiles.Count(t => t.TileView?.TileOwner is not null);
    public bool Full => TilesCount == Tiles.Length;
    public bool HasNoSpace
    {
        get
        {
            if (!Full) return false;
            return Tiles.Length == Tiles.Select(t => t.TileView?.Face.sprite).Distinct().Count();
        }
    }
    
    public void AddTile(MajhongTileView tile)
    {
        tile.RaycastDisable();
        tile.IsPlayable = false;
        tile.gameObject.SetActive(false);

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
        zoom.ToStartPoint();
        zoom.Play();
        zoom.OnComplete += () => { Destroy(zoom); };
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
        for (int i = 0; i < Tiles.Length; i++)//Find empty
        {
            if(Tiles[i].IsFull && Tiles[i].TileView.InGame) 
                continue;
            
            for (int j = i+1; j < Tiles.Length; j++)//Find full
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
}