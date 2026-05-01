using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

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
