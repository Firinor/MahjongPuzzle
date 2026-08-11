using FirAnimations;
using UnityEngine;
using UnityEngine.UI;

public class TileInHandView : MonoBehaviour
{
    public GameObject Tile;
    public Image Face;
    public FirPositionAnimation Animation;
    public MajhongTileView TileOwner;
    
    public bool IsFull => TileOwner != null;
}