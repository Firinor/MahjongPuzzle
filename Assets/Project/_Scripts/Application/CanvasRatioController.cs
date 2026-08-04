using System;
using UnityEngine;

public class CanvasRatioController : MonoBehaviour
{
    private bool isLandscape = true;

    [SerializeField] private GameObject[] LandscapeObjects;
    [SerializeField] private GameObject[] PortraitObjects;
    
    private void Start()
    {
        CanvasRatioManager.onChange += ScreenOrientation;
        ScreenOrientation();
    }

    private void ScreenOrientation(ScreenOrientation orientation = UnityEngine.ScreenOrientation.AutoRotation)
    {
        if(Screen.height == Screen.width)
            return;
        
        bool isLandscape = Screen.height < Screen.width;

        foreach (var gameObject in LandscapeObjects)
        {
            gameObject.SetActive(isLandscape);
        }
        foreach (var gameObject in PortraitObjects)
        {
            gameObject.SetActive(!isLandscape);
        }
    }

    private void OnDestroy()
    {
        CanvasRatioManager.onChange -= ScreenOrientation;
    }
}
