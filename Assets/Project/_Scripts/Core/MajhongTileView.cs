using System.Collections.Generic;
using FirAnimations;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class MajhongTileView : MonoBehaviour
{
    [SerializeField] 
    private SpriteRenderer Renderer;
    [SerializeField] 
    private MeshRenderer Cube;
    [SerializeField] 
    private Material defaultMaterial;
    [SerializeField] 
    private Material darkerMaterial;
    [SerializeField] 
    private Material selectedMaterial;
    [SerializeField] 
    private Material errorMaterial;
    
    [SerializeField] 
    private FirAnimation zoomAnimation;
    [SerializeField] 
    private FirAnimation rotateAnimation;
    
    public Tile Data { get; private set; }
    public Sprite Sprite => Data.Sprite;
    
    public Transform[] RayPoints;

    public void SetData(Tile data)
    {
        Data = data;
        Renderer.sprite = data.Sprite;
    }

    public void SetMaterial(Material material)
    {
        Cube.material = material;
    }

    public void RaycastDisable()
    {
        GetComponent<Collider>().enabled = false;
    }

    public void ErrorAnimation()
    {
        StopAnimation();
        SetMaterial(errorMaterial);
        rotateAnimation.OnComplete = () => SetMaterial(defaultMaterial);
        rotateAnimation.Play();
    }
    public void HintAnimation()
    {
        StopAnimation();
        SetMaterial(selectedMaterial);
        rotateAnimation.Play();
    }
    public void SelectedAnimation()
    {
        StopAnimation();
        SetMaterial(selectedMaterial);
        zoomAnimation.Play();
    }

    public void StopAnimation()
    {
        zoomAnimation.Stop();
        rotateAnimation.Stop();
        SetMaterial(defaultMaterial);
    }
}
