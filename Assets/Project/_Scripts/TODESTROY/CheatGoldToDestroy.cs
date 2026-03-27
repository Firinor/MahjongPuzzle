using UnityEngine;

public class CheatGoldToDestroy : MonoBehaviour
{
    private ProgressData player;
    public void Initialize(ProgressData player)
    {
        this.player = player;
    }

    public void AddGold()
    {
        player.AddGold(3000);
    }
}
