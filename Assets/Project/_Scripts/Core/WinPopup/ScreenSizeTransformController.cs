using UnityEngine;

public class ScreenSizeTransformController : MonoBehaviour
{
    public float widthRatio;
    //public float cameraRatio = 5f;

    private void Start()
    {
        CanvasRatioManager.onChange += UpdateScale;
        UpdateScale();
    }

    private void UpdateScale()
    {
        float ratio = (float)Screen.height / (float)Screen.width;
        if(ratio > widthRatio)
            transform.localScale = Vector3.one * (ratio / widthRatio);
        else
            transform.localScale = Vector3.one;
    }
    
    private void Update()
    {
        float ratio = (float)Screen.height / (float)Screen.width;
        if(ratio > widthRatio)
            transform.localScale = Vector3.one * (ratio / widthRatio);
        else
            transform.localScale = Vector3.one;
    }
    

    private void OnDestroy()
    {
        CanvasRatioManager.onChange -= UpdateScale;
    }
}