using System;

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