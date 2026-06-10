using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
#if IS_YANDEX
using YG;
#endif

public class BOOTSTRAP : MonoBehaviour
{
    [SerializeField]
    private SceneButton nextScene;
    [SerializeField]
    private Settings settings;
    
    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
#if IS_YANDEX
        yield return YG2.onGetSDKData;
#endif
        
        settings.Initialize(bootstrap: true);
        
        nextScene.SwitchToScene();
    }
}