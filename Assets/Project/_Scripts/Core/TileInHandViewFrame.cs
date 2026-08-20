using UnityEngine;
using UnityEngine.UI;

public class TileInHandViewFrame : MonoBehaviour
{
    public bool IsFull => TileView != null;
    public bool IsLock = true;
    public TileInHandView TileView;
    public Button Lock;

    public void Unlock()
    {
        IsLock = false;
        Lock.onClick.RemoveAllListeners();
        Destroy(Lock.gameObject);
    }
    public void Hide()
    {
        Destroy(TileView.gameObject);
        TileView = null;
    }
}