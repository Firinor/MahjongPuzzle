using System;

[Serializable]
public class ProgressData
{
    public int GoldCoins;
    public string tilesID = "Classic";
    public string[] TilesPacks;
    public string deskID = "Classic";
    public string[] Desks;

    public event Action<int> OnGoldChange;

    public void AddGold(int count)
    {
        GoldCoins += count;
        OnGoldChange?.Invoke(GoldCoins);
    }

    public bool TrySpendGold(int count)
    {
        if (GoldCoins < count)
            return false;

        GoldCoins -= count;
        OnGoldChange?.Invoke(GoldCoins);
        return true;
    }
}