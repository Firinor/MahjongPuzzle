using System;
using UnityEngine;

public class CanvasRatioManager : MonoBehaviour
{
    public static event Action<ScreenOrientation> onChange;
#if UNITY_EDITOR
    public bool isDebug;
#endif
    
    private bool isLandscape = true;

    private void Start()
    {
        if (!SystemInfo.supportsGyroscope)
        {
#if UNITY_EDITOR
            if (isDebug)
            {
                DontDestroyOnLoad(gameObject);
                return;
            }
#endif
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (isLandscape && Screen.height > Screen.width)
        {
            isLandscape = false;
#if UNITY_EDITOR
            if(isDebug)
                Debug.Log(Screen.orientation);
#endif
            onChange?.Invoke(Screen.orientation);
        }
        if(!isLandscape && Screen.height < Screen.width)
        {
            isLandscape = true;
#if UNITY_EDITOR
            if(isDebug)
                Debug.Log(Screen.orientation);
#endif
            onChange?.Invoke(Screen.orientation);
        }
    }
}
