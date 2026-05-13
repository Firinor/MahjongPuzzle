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
        yield return YG2.onGetSDKData;
        
        settings.Initialize(bootstrap: true);
        
        nextScene.SwitchToScene();
    }
}