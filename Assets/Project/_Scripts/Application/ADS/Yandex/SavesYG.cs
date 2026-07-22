using System;
using YG;

namespace YG
{
    public partial class SavesYG
    {
        public int GoldCoins;
        public string tilesID = "ClassicTiles";
        public string deskID = "ClassicDesk";
        public int Difficulty = 1;
    }
}

public class YGSaveData : SaveData
{
    private SavesYG saves;

    public override int GoldCoins
    {
        get => saves.GoldCoins;
        set => saves.GoldCoins = value;
    }

    public override string TilesID
    {
        get => saves.tilesID;
        set => saves.tilesID = value;
    }

    public override string DeskID 
    {
        get => saves.deskID;
        set => saves.deskID = value;
    }
    public override int Difficulty 
    {
        get => saves.Difficulty;
        set => saves.Difficulty = value;
    }

    public override void FirstLoad()
    {
        saves = YG2.saves;
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

    public override void Save()
    {
        YG2.SaveProgress();
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
}