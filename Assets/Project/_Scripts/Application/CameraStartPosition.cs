using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraStartPosition : MonoBehaviour
{
    public float YpositionSoliter = 0;
    public float YpositionCollecting = 2;
    public float solitarePortraitOrthographicSize = 5;
    public float solitareLandscapeOrthographicSize = 10;
    public float collectingPortraitOrthographicSize = 5;
    public float collectingLandscapeOrthographicSize = 10;
    
    void Awake()
    {
        bool isLandscape = Screen.width > Screen.height;
        Vector3 cameraPosition = transform.position;
        
#if IS_Solitare
        cameraPosition.y = YpositionSoliter;
        if (isLandscape)
            Camera.main.orthographicSize = solitareLandscapeOrthographicSize;
        else
            Camera.main.orthographicSize = solitarePortraitOrthographicSize;
#elif IS_Collecting
        cameraPosition.y = YpositionCollecting;
        if (isLandscape)
            Camera.main.orthographicSize = collectingLandscapeOrthographicSize;
        else
            Camera.main.orthographicSize = collectingPortraitOrthographicSize;
#endif
        
        transform.position = cameraPosition;
    }
}
