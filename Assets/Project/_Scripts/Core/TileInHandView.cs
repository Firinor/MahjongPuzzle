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

    public void Hide()
    {
        TileOwner = null;
        PositionAnimation.gameObject.SetActive(false);
        Debug.Log($"{name} Tile false");
    }
}