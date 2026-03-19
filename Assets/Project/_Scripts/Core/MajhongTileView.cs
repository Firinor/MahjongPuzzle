using UnityEngine;

public class MajhongTileView : MonoBehaviour
{
    [SerializeField] 
    private SpriteRenderer Renderer;
    private Tile Data;

    public void SetData(Tile data)
    {
        Data = data;
        Renderer.sprite = data.Sprite;
    }
}
