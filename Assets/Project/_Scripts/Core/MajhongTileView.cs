using System.Collections.Generic;
using FirAnimations;
using UnityEngine;

[SelectionBase]
public class MajhongTileView : MonoBehaviour
{
    [SerializeField] 
    private SpriteRenderer face;
    [SerializeField] 
    private MeshRenderer cube;
    [SerializeField] 
    private SpriteRenderer shadow;
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
    
    public bool isHint;
    
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
        face.sprite = data.Sprite;
    }

    private void ResetMaterial()
    {
        if (statuses[errorMaterial])
        {
            cube.material = errorMaterial;
            return;
        }
        if (statuses[selectedMaterial])
        {
            cube.material = selectedMaterial;
            return;
        }
        if (statuses[darkerMaterial])
        {
            cube.material = darkerMaterial;
            return;
        }
        cube.material = defaultMaterial;
    }

    public void RaycastDisable()
    {
        DestroyImmediate(GetComponent<Collider>());
    }

    public void DisableVisual()
    {
        face.enabled = false;
        cube.enabled = false;
        shadow.enabled = false;
    }
    public void EnableVisual()
    {
        face.enabled = true;
        cube.enabled = true;
        shadow.enabled = true;
    }
    
    public void RaycastDisableEditor()
    {
        GetComponent<Collider>().enabled = false;
    }
    public void RaycastEnableEditor()
    {
        GetComponent<Collider>().enabled = true;
    }

    [ContextMenu("Neighbors")]
    private void Neighbors()
    {
        Debug.Log(MajhongSolitaireRules.CheckNeighbors(this));
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

    private void StopAnimation()
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
    public void ClickUnselect()
    {
        StopAnimation();
        statuses[selectedMaterial] = false;
        zoomAnimation.Play();
        ResetMaterial();
    }
}
