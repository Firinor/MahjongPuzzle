using UnityEngine;

public class CanvasRatioManager : MonoBehaviour
{
    void Start()
    {
        if (!SystemInfo.supportsGyroscope) 
            Destroy(gameObject);
        
        ApplyOrientation(Screen.orientation);
    }
    /*void OnOrientationChange()
    {
        ApplyOrientation(Screen.orientation);
    }*/
    
    private void ApplyOrientation(ScreenOrientation orientation)
    {
       
    }
}
