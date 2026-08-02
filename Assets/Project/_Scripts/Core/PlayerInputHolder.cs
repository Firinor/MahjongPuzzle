using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputHolder : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public event Action<Vector2> onClick;
    public event Action<Vector2> onDrag;
    public event Action<float> onZoom;
    public float mouseScrollCoef = -0.3f;
    public float touchScrollCoef = 0.01f;

    public bool isDebug;
    
    private float _initialDistance;
    private bool isDrag;
    private bool isZoom;
    
    void Update()
    {
        if (Input.touchCount == 2)
        {
            isZoom = true;
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            
            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                _initialDistance = Vector2.Distance(touch1.position, touch2.position);
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                float delta = currentDistance - _initialDistance;
                _initialDistance = currentDistance;
                if (isDebug)
                    Debug.Log($"eventData: {delta}, coef: {touchScrollCoef}");
                onZoom?.Invoke(delta * touchScrollCoef);
            }
        }

        if (Input.touchCount == 0)
        {
            isZoom = false;
        }
    }
    
    public void OnScroll(PointerEventData eventData)
    {
        float scrollDelta = eventData.scrollDelta.y;
        if (isDebug)
            Debug.Log($"eventData: {scrollDelta}, coef: {mouseScrollCoef}");
        onZoom?.Invoke(scrollDelta*mouseScrollCoef);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(isDrag || isZoom) return;
        onClick?.Invoke(eventData.position);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
    }
}
