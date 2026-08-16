#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

public class SiteLockUnity : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SitelockCheck();
#endif
    
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Call the JS function
        SitelockCheck();
#endif
    }
}
