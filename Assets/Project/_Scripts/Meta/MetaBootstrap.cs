using UnityEngine;

[DefaultExecutionOrder(-1)]
public class MetaBootstrap : MonoBehaviour
{
    [SerializeField] 
    private Settings settings;

    [SerializeField] 
    private PlayerProgressUnlockManager unlocksManager;
    
    public CheatGoldToDestroy CHEATS;
    
    private ProgressData player;
    
    private void Awake()
    {
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