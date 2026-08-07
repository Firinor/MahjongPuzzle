using UnityEngine;

public class ScreenSizeTransformController : MonoBehaviour
{
    public float width;
    public float cameraRatio = 5f;

    private void Start()
    {
        CanvasRatioManager.onChange += UpdateScale;
        UpdateScale();
    }

    private void UpdateScale()
    {
        Debug.Log($"Screen width: {Screen.width}");
        if(Screen.width < width)
            transform.localScale = Vector3.one * (Screen.width / width) * (Camera.main.orthographicSize/cameraRatio);
        else
            transform.localScale = Vector3.one * (Camera.main.orthographicSize/cameraRatio);
    }

    private void OnDestroy()
    {
        CanvasRatioManager.onChange -= UpdateScale;
    }
}