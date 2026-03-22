using UnityEngine;

[SelectionBase]
public class MajhongTileView : MonoBehaviour
{
    [SerializeField] 
    private SpriteRenderer Renderer;
    [SerializeField] 
    private MeshRenderer Cube;
    private Tile Data;

    public void SetData(Tile data)
    {
        Data = data;
        Renderer.sprite = data.Sprite;
    }

    public void SetMaterial(Material material)
    {
        Cube.material = material;
    }
}
