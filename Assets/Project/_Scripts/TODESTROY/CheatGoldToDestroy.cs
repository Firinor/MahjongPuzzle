using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatGoldToDestroy : MonoBehaviour
{
    private ProgressData player;
    public void Initialize(ProgressData player)
    {
        this.player = player;
    }

    public void AddGold()
    {
        player.AddGold(30000000);
        SaveLoadSystem<ProgressData>.Save("Player", player);
        SceneManager.LoadScene("Meta");
    }
}
