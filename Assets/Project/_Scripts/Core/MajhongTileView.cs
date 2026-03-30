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

    private Dictionary<Material, bool> statuses;
    
    [SerializeField] 
    private FirAnimation zoomAnimation;
    [SerializeField] 
    private FirAnimation rotateAnimation;
    
    public Tile Data { get; private set; }
    public Sprite Sprite => Data.Sprite;
    
    public Transform[] RayPoints;

    private void Awake()
    {
        statuses = new();
        statuses.Add(defaultMaterial, true);
        statuses.Add(darkerMaterial, false);
        //statuses.Add(selectedMaterial, false);//Hint spell
        statuses.Add(selectedMaterial, false);//Player click
        statuses.Add(errorMaterial, false);
    }
    
    public void SetData(Tile data)
    {
        Data = data;
        Renderer.sprite = data.Sprite;
    }

    private void ResetMaterial()
    {
        if (statuses[errorMaterial])
        {
            Cube.material = errorMaterial;
            return;
        }
        if (statuses[selectedMaterial])
        {
            Cube.material = selectedMaterial;
            return;
        }
        if (statuses[darkerMaterial])
        {
            Cube.material = darkerMaterial;
            return;
        }
        Cube.material = defaultMaterial;
    }

    public void RaycastDisable()
    {
        DestroyImmediate(GetComponent<Collider>());
    }

    public void SetDarkerMaterial()
    {
        statuses[darkerMaterial] = true;
        ResetMaterial();
    }
    public void DisableDarkerMaterial()
    {
        statuses[darkerMaterial] = false;
        ResetMaterial();
    }
    public void ErrorAnimation()
    {
        StopAnimation();
        statuses[errorMaterial] = true;
        rotateAnimation.OnComplete = () =>
        {
            statuses[errorMaterial] = false;
            ResetMaterial();
        };
        rotateAnimation.Play();
        ResetMaterial();
    }
    public void HintAnimation()
    {
        StopAnimation();
        statuses[selectedMaterial] = true;
        rotateAnimation.Play();
        ResetMaterial();
    }
    public void SelectedAnimation()
    {
        StopAnimation();
        statuses[selectedMaterial] = true;
        zoomAnimation.Play();
        ResetMaterial();
    }

    public void StopAnimation()
    {
        zoomAnimation.Stop();
        rotateAnimation.Stop();
        ResetMaterial();
    }

    public void SetDefaultMaterial(Material floorMaterial)
    {
        defaultMaterial = floorMaterial;
        ResetMaterial();
    }

    public void Unselect()
    {
        statuses[selectedMaterial] = false;
        ResetMaterial();
    }
}
