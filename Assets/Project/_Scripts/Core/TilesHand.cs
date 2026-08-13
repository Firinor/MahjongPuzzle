using System;
using System.Linq;
using FirAnimations;
using UnityEngine;

public class TilesHand : MonoBehaviour
{
    public TileInHandView[] Tiles;
    public RectTransform AimObject;
    public Transform Canvas;

    public event Action OnFlyEndAnimation;
    public event Action OnCollideEndAnimation;
    
    public int TilesCount => Tiles.Count(t => t.TileOwner is not null);
    public bool Full => TilesCount == Tiles.Length;
    public bool HasNoSpace
    {
        get
        {
            if (!Full) return false;
            return Tiles.Length == Tiles.Select(t => t.Face.sprite).Distinct().Count();
        }
    }

    private void Awake()
    {
        foreach (var tileInHandView in Tiles)
        {
            tileInHandView.PositionAnimation.OnComplete = () => OnFlyEndAnimation?.Invoke();
        }
    }

    public void AddTile(MajhongTileView tile)
    {
        tile.RaycastDisable();
        tile.IsPlayable = false;
        tile.gameObject.SetActive(false);

        TileInHandView firstOpen = Tiles.FirstOrDefault(t => !t.IsFull);

        if (firstOpen is null)
        {
            Debug.LogError("no empty space at TilesHand!");
            return;
        }
        
        firstOpen.TileOwner = tile;
        
        firstOpen.Face.sprite = tile.Sprite;
        var posStart = Camera.main.WorldToScreenPoint(tile.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas.GetComponent<RectTransform>(),                   
            posStart,                    
            Camera.main,        
            out Vector2 localStartPos           
        );
        AimObject.anchoredPosition = localStartPos;
        firstOpen.PositionAnimation.transform.SetParent(AimObject);
        firstOpen.PositionAnimation.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        firstOpen.PositionAnimation.transform.SetParent(firstOpen.transform, worldPositionStays: true);
        firstOpen.PositionAnimation.StartPosition = firstOpen.PositionAnimation.GetComponent<RectTransform>().anchoredPosition;
        firstOpen.ColorAnimation.StartPosition = tile.MaterialColor;
        firstOpen.PositionAnimation.ToStartPoint();
        firstOpen.ZoomAnimation.ToStartPoint();
        firstOpen.ColorAnimation.ToStartPoint();
        firstOpen.PositionAnimation.gameObject.SetActive(true);
        Debug.Log($"{firstOpen.name} Tile true");
        firstOpen.PositionAnimation.Play();
        firstOpen.ZoomAnimation.Play();
        firstOpen.ColorAnimation.Play();
        var zoom = tile.gameObject.AddComponent<FirZoomAnimation>();
        zoom.StartZoom = Vector3.one;
        zoom.EndZoom = Vector3.zero;
        zoom.ToStartPoint();
        zoom.Play();
        zoom.OnComplete += () => { Destroy(zoom); };
    }

    public void MoveTiles()
    {
        for (int i = 0; i < Tiles.Length; i++)//Find empty
        {
            if(Tiles[i].IsFull) 
                continue;
            for (int j = i+1; j < Tiles.Length; j++)//Find full
            {
                if(!Tiles[j].IsFull) continue;
                Debug.Log($"Need move {i} and {j}");
                AddTile(Tiles[j].TileOwner);
                Tiles[j].Hide();
                break;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var tileInHandView in Tiles)
        {
            tileInHandView.PositionAnimation.OnComplete = null;
        }
    }
}