using FirAnimations;
using UnityEngine;
using UnityEngine.UI;

public class TileInHandViewFrame : MonoBehaviour
{
    public bool IsFull => TileView != null;
    public TileInHandView TileView;
    
    public void Hide()
    {
        Destroy(TileView.gameObject);
        TileView = null;
    }
}