using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using YG;

public class BOOTSTRAP : MonoBehaviour
{
    [SerializeField]
    private SceneButton nextScene;
    [SerializeField]
    private Settings settings;
    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        settings.Initialize(bootstrap: true);
        
        nextScene.SwitchToScene();
    }
}

public static class FirYG2Service
{
    public static void Initialize()
    {
        YG2.GameReadyAPI();
    }
} 
