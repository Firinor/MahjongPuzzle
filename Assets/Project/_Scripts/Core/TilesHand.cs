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
        firstOpen.Animation.transform.SetParent(AimObject);
        firstOpen.Animation.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        firstOpen.Animation.transform.SetParent(firstOpen.transform, worldPositionStays: true);
        firstOpen.Animation.StartPosition = firstOpen.Animation.GetComponent<RectTransform>().anchoredPosition;
        firstOpen.Animation.ToStartPoint();
        firstOpen.Animation.gameObject.SetActive(true);
        firstOpen.Animation.Play();
    }

    public void HasCollect()
    {
        
    }
}