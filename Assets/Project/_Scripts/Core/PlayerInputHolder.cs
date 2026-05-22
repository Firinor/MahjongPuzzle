using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputHolder : MonoBehaviour, IPointerClickHandler
{
    public event Action<Vector2> onClick;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(eventData.position);
    }
}
