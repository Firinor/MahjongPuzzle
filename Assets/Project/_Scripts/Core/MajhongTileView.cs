using System;
using System.Collections.Generic;
using FirAnimations;
using UnityEngine;

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

    [SerializeField] 
    private GameObject selectionFramel;
    
#if UNITY_EDITOR
    public Transform Center => cube.transform;
    public BoxCollider FrontTrigger;
    public BoxCollider LeftTrigger;
    public BoxCollider RightTrigger;
#endif
    
    private TileStatuses statuses;//flags

    [Flags]
    private enum TileStatuses
    {
        Default = 0,
        Darker = 1,
        Hint = 2,
        Selected = 4,
        Error = 8,
    }
    
    [SerializeField] 
    private FirAnimation zoomAnimation;
    [SerializeField] 
    private FirAnimation rotateAnimation;
    
    public bool isHint;
    
    public Sprite Sprite => face.sprite;
    
    public bool IsPlayable = true;
    public bool IsOpenOnStart;
    public List<MajhongTileView> UpNeighbors;
    public List<MajhongTileView> LeftNeighbors;
    public List<MajhongTileView> RightNeighbors;
    
    private void Awake()
    {
        statuses = TileStatuses.Default;
    }
    
    public void SetData(Sprite tile)
    {
        face.sprite = tile;
    }

    private void ResetMaterial()
    {
        selectionFramel.SetActive(false);
        if (statuses.HasFlag(TileStatuses.Error))
        {
            cube.material = errorMaterial;
            return;
        }
        if (statuses.HasFlag(TileStatuses.Selected))
        {
            cube.material = selectedMaterial;
            return;
        }
        if (statuses.HasFlag( TileStatuses.Hint))
        {
            selectionFramel.SetActive(true);
            cube.material = defaultMaterial;
            return;
        }
        if (statuses.HasFlag(TileStatuses.Darker))
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
    }
    public void EnableShadow()
    {
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
        statuses |= TileStatuses.Darker;
        ResetMaterial();
    }
    public void DisableDarkerMaterial()
    {
        statuses &= ~TileStatuses.Darker;
        ResetMaterial();
    }
    public void ErrorAnimation()
    {
        StopAnimation();
        statuses |= TileStatuses.Error;
        SoundManager.Instance.PlayTileError(transform.position);
        rotateAnimation.OnComplete = () =>
        {
            statuses &= ~TileStatuses.Error;
            ResetMaterial();
        };
        rotateAnimation.Play();
        ResetMaterial();
    }
    public void HintAnimation()
    {
        StopAnimation();
        statuses |= TileStatuses.Hint;
        rotateAnimation.Play();
        ResetMaterial();
    }
    public void SelectedAnimation()
    {
        StopAnimation();
        statuses |= TileStatuses.Selected;
        SoundManager.Instance.PlayTileSelect(transform.position);
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
        statuses &= ~(TileStatuses.Selected|TileStatuses.Hint);
        ResetMaterial();
    }
    public void ClickUnselect()
    {
        StopAnimation();
        statuses &= ~TileStatuses.Selected;
        SoundManager.Instance.PlayTileSelect(transform.position);
        zoomAnimation.Play();
        ResetMaterial();
    }
}
