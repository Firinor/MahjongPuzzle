using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    private SaveData player;
    
    public void Initialize(SaveData player)
    {
        this.player = player;
    }

    public void AddGold()
    {
        player.AddGold(30000000);
        player.Save();
        SceneManager.LoadScene("Bootstrap");
    }
    
    public void DestroySaves()
    {
        player.ResetProgress();
        SceneManager.LoadScene("Bootstrap");
    }
}
