using System.Collections;
using FirAnimations;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class MetaBootstrap : MonoBehaviour
{
    public FirAnimation closeСurtain;  
    
    [SerializeField] 
    private Settings settings;

    [SerializeField] 
    private PlayerProgressUnlockManager unlocksManager;
    
    public CheatGoldToDestroy CHEATS;
    
    private ProgressData player;
    
    private IEnumerator Start()
    {
        closeСurtain.Initialize();
        
        yield return null;
        
        closeСurtain.Play();//OpenScene
        
        settings.Initialize();
        
        LoadPlayerData();
        
        unlocksManager.Initialize(player);
        CHEATS.Initialize(player);
    }
    
    private void LoadPlayerData()
    {
        player = SaveLoadSystem<ProgressData>.Load("Player", Default: new());
    }
}