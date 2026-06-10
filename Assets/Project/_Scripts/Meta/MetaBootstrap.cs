using System.Collections;
using FirAnimations;
using UnityEngine;
#if IS_YANDEX
using YG;
#endif

[DefaultExecutionOrder(-1)]
public class MetaBootstrap : MonoBehaviour
{
    public FirAnimation closeСurtain;  
    
    [SerializeField] 
    private Settings settings;

    [SerializeField] 
    private PlayerProgressUnlockManager unlocksManager;
    
    [SerializeField] 
    private Cheats cheats;
    
    private SaveData player;
    
    private IEnumerator Start()
    {
        closeСurtain.Initialize();
        
        yield return null;
        
        closeСurtain.Play();//OpenScene
        
        settings.Initialize();
        
        LoadPlayerData();
        
        unlocksManager.Initialize(player);

#if Cheats
        cheats.Initialize(player);
#endif
#if IS_YANDEX
    YG2.GameReadyAPI();
#endif
    }
    
    private void LoadPlayerData()
    {
#if IS_YANDEX
        player = new YGSaveData();
#else
        player = new PrefsSaveData();
#endif
        player.FirstLoad();
    }
}