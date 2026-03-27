using UnityEngine;

[CreateAssetMenu(fileName = "TilesData", menuName = "Majhong/TilesData")]
public class TilesData : ScriptableObject
{
    public string ID;
    public Sprite TileBase;
    public Tile[] Tiles;
}
