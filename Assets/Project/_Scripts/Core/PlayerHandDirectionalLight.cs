using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerHandDirectionalLight : MonoBehaviour
{
    [SerializeField] 
    private Transform[] Lights;
    
    private InputAction action;
    
    void Start()
    {
        action = InputSystem.actions.FindAction("Look");
        EnhancedTouchSupport.Enable();
        action.performed += MoveLight;
    }

    private void MoveLight(InputAction.CallbackContext obj)
    {
        Vector2 position;
        if (Touch.activeTouches.Count > 0)
            position = Touch.activeTouches[0].screenPosition;
        else
            position = Mouse.current.position.ReadValue();

        foreach (var directionalLight in Lights)
        {
            directionalLight.LookAt(Camera.main.ScreenToWorldPoint(position));
        }
    }
    
    private void OnDestroy()
    {
        if(action != null)
            action.performed -= MoveLight;
    }
}
