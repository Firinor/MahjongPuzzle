using UnityEngine;

[CreateAssetMenu(fileName = "TilesData", menuName = "Majhong/TilesData")]
public class TilesData : ScriptableObject
{
    public Sprite TileBase;
    public Tile[] Tiles;
}
