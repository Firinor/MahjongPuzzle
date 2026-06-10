using System;

[Serializable]
public abstract class SaveData
{
    public abstract int GoldCoins { get; set; }
    public abstract string TilesID { get; set; }
    public abstract string DeskID { get; set; }
    public abstract int Difficulty { get; set; }
    
    public event Action<int> OnGoldChange;

    public abstract void FirstLoad();
    
    public abstract void AddGold(int count);
    public abstract bool TrySpendGold(int count);
    public abstract void ResetProgress();
    public abstract void Save();
    
    protected void InvokeGoldChange(int gold)
    {
        OnGoldChange?.Invoke(gold);
    }
}

[Serializable]
public class PrefsSaveData : SaveData
{
    public int goldCoins;
    public string tilesID = "ClassicTiles";
    public string deskID = "ClassicDesk";
    public int difficulty = 1;

    public override int GoldCoins
    {
        get => goldCoins;
        set => goldCoins = value;
    }
    public override string TilesID 
    {
        get => tilesID;
        set => tilesID = value;
    }
    public override string DeskID 
    {
        get => deskID;
        set => deskID = value;
    }
    public override int Difficulty 
    {
        get => difficulty;
        set => difficulty = value;
    }

    public override void FirstLoad()
    {
        var data = SaveLoadSystem<PrefsSaveData>.Load("Player", new ());
        goldCoins = data.GoldCoins;
        tilesID = data.tilesID;
        deskID = data.deskID;
        difficulty = data.difficulty;
    }

    public override void AddGold(int count)
    {
        GoldCoins += count;
        InvokeGoldChange(GoldCoins);
    }

    public override bool TrySpendGold(int count)
    {
        if (GoldCoins < count)
            return false;

        GoldCoins -= count;
        InvokeGoldChange(GoldCoins);
        return true;
    }

    public override void ResetProgress()
    {
        GoldCoins = 0;
        TilesID = "ClassicTiles";
        DeskID = "ClassicDesk";
        Difficulty = 1;
        InvokeGoldChange(GoldCoins);
        Save();
    }

    public override void Save()
    {
        SaveLoadSystem<PrefsSaveData>.Save("Player", this);
    }
}