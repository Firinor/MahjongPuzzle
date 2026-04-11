using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerHand : MonoBehaviour
{
    private InputActionAsset action;
    [SerializeField]
    private Button playerInput;

    public Action<MajhongTileView> OnTileClick;

    private void Start()
    {
        playerInput.onClick.AddListener(FindTile);
    }

    private void FindTile()
    {
        MajhongTileView tile = GetRayHitTile();
        
        if(tile == null)
            return;
        
        OnTileClick?.Invoke(tile);
    }

    private static MajhongTileView GetRayHitTile()
    {
        Vector2 position;
        if (Touch.activeTouches.Count > 0)
            position = Touch.activeTouches[0].screenPosition;
        else
            position = Mouse.current.position.ReadValue();

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
        playerInput.onClick.RemoveAllListeners();
    }
}
