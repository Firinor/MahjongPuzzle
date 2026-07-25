using MirraGames.SDK;
using UnityEngine;

public class MirraSaveData : SaveData
{
    private const string key = "playerData";
    private PrefsSaveData data;
    
    public override void Save()
    {
        if (!MirraSDK.IsInitialized)
            return;

        string saveData = JsonUtility.ToJson(data);
        MirraSDK.Data.SetString(key, saveData);
        MirraSDK.Data.Save();
    }

    private bool HasData()
    {
        return MirraSDK.Data.HasKey(key);
    }

    public override int GoldCoins
    {
        get => data.GoldCoins;
        set => data.GoldCoins = value;
    }
    public override string TilesID 
    {
        get => data.tilesID;
        set => data.tilesID = value;
    }
    public override string DeskID 
    {
        get => data.deskID;
        set => data.deskID = value;
    }
    public override int Difficulty 
    {
        get => data.difficulty;
        set => data.difficulty = value;
    }
    
    public override void FirstLoad()
    {
        if (HasData())
            data = JsonUtility.FromJson<PrefsSaveData>(MirraSDK.Data.GetString(key));
        else
            data = new();
    }

    public override void AddGold(int count)
    {
        data.GoldCoins += count;
        Save();
        InvokeGoldChange(data.GoldCoins);
    }

    public override bool TrySpendGold(int count)
    {
        if (data.GoldCoins < count)
            return false;

        data.GoldCoins -= count;
        Save();
        InvokeGoldChange(data.GoldCoins);
        return true;
    }

    public override void ResetProgress()
    {
        MirraSDK.Data.DeleteAll();
        data.GoldCoins = 0;
        data.TilesID = "ClassicTiles";
        data.DeskID = "ClassicDesk";
        data.Difficulty = 1;
        InvokeGoldChange(data.GoldCoins);
        Save();
    }
}