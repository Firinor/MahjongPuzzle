using System;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [SerializeField]
    private PlayerInputHolder playerInput;

    public Action<MajhongTileView> OnTileClick;

    private void Start()
    {
        playerInput.onClick += FindTile;
    }

    private void FindTile(Vector2 position)
    {
        MajhongTileView tile = GetRayHitTile(position);
        
        if(tile == null)
            return;
        
        OnTileClick?.Invoke(tile);
    }

    private static MajhongTileView GetRayHitTile(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);

        MajhongTileView result = null;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.TryGetComponent(out result);
        }
        return result;
    }

    private void OnDestroy()
    {
        playerInput.onClick -= FindTile;
    }
}
