using FirAnimations;
using UnityEngine;
using UnityEngine.UI;

public class TileInHandView : MonoBehaviour
{
    public GameObject Tile;
    public Image Face;
    public FirPositionAnimation PositionAnimation;
    public FirZoomAnimation ZoomAnimation;
    public FirColorAnimation ColorAnimation;
    public MajhongTileView TileOwner;
    
    public bool IsFull => TileOwner != null;
}