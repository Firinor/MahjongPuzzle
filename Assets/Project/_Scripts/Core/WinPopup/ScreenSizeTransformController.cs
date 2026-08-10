using UnityEngine;

public class ScreenSizeTransformController : MonoBehaviour
{
    public float widthRatio;

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
#if UNITY_EDITOR
    public bool isDebug;
    private void Update()
    {
        if(!isDebug) return;
        
        float ratio = (float)Screen.height / (float)Screen.width;
        if(ratio > widthRatio)
            transform.localScale = Vector3.one * (ratio / widthRatio);
        else
            transform.localScale = Vector3.one;
    }
#endif

    private void OnDestroy()
    {
        CanvasRatioManager.onChange -= UpdateScale;
    }
}