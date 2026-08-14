using FirAnimations;
using UnityEngine;
using UnityEngine.UI;

public class TileInHandView : MonoBehaviour
{
    public bool InGame = true;
    
    public RectTransform RectTransform;
    public Image Face;
    public FirPositionAnimation PositionAnimation;
    public FirZoomAnimation ZoomAnimation;
    public FirColorAnimation ColorAnimation;
    public MajhongTileView TileOwner;
    public FirZoomAnimation SecondZoomAnimation;
}