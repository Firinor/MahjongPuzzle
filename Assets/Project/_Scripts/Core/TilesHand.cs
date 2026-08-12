using System.Linq;
using UnityEngine;

public class TilesHand : MonoBehaviour
{
    public TileInHandView[] Tiles;
    public RectTransform AimObject;
    public Transform Canvas;

    public int TilesCount;
    public bool Full => TilesCount == Tiles.Length;

    public void AddTile(MajhongTileView tile)
    {
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
        firstOpen.PositionAnimation.Play();
        firstOpen.ZoomAnimation.Play();
        firstOpen.ColorAnimation.Play();
    }

    public void HasCollect()
    {
        
    }
}