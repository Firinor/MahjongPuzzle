using System;
using System.Collections;
using UnityEngine;

public class CanvasRatioManager : MonoBehaviour
{
    public static event Action onChange;
    private Vector2 lastScreen;
    private float time;
    private WaitForSeconds delay;

    private void Start()
    {
        lastScreen = new (){ x = Screen.width, y = Screen.height };
        delay = new WaitForSeconds(time);
        StartCoroutine(CheckUpdates());
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator CheckUpdates()
    {
        while (true)
        {
            yield return delay;
            if (Screen.height != lastScreen.y
                || Screen.width != lastScreen.x)
            {
                lastScreen = new (){ x = Screen.width, y = Screen.height };
                onChange?.Invoke();
            }
        }
    }
}
