using System.Linq;
using UnityEngine;

public class TilesHand : MonoBehaviour
{
    public TileInHandView[] Tiles;

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
        posStart.z = 0;
        var posEnd = Camera.main.WorldToScreenPoint(firstOpen.Tile.transform.position);
        posEnd.z = 0;
        posStart -= posEnd;
        //posStart.z = firstOpen.Animation.transform.position.z;
        firstOpen.Animation.StartPosition = posStart;
        firstOpen.Animation.ToStartPoint();
        firstOpen.Animation.gameObject.SetActive(true);
        firstOpen.Animation.Play();
    }

    public void HasCollect()
    {
        
    }
}