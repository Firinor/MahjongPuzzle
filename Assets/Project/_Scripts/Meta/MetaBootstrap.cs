using System.Collections;
using FirAnimations;
using UnityEngine;
using YG;

[DefaultExecutionOrder(-1)]
public class MetaBootstrap : MonoBehaviour
{
    public FirAnimation closeСurtain;  
    
    [SerializeField] 
    private Settings settings;

    [SerializeField] 
    private PlayerProgressUnlockManager unlocksManager;
    
    private SavesYG player;
    
    private IEnumerator Start()
    {
        closeСurtain.Initialize();
        
        yield return null;
        
        closeСurtain.Play();//OpenScene
        
        settings.Initialize();
        
        LoadPlayerData();
        
        unlocksManager.Initialize(player);

        YG2.GameReadyAPI();
    }
    
    private void LoadPlayerData()
    {
        player = YG2.saves;
    }
}