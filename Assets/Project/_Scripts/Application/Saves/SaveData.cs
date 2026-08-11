using System;

[Serializable]
public abstract class SaveData
{
    public abstract int GoldCoins { get; set; }
    public abstract string TilesID { get; set; }
    public abstract string DeskID { get; set; }
    public abstract int Difficulty { get; set; }
    public abstract GameMode GameMode { get; set; }
    
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

    public static SaveData GetPlayer()
    {
#if IS_YANDEX
        return new YGSaveData();
#elif IS_MIRRA
        return new MirraSaveData();
#else
        return new PrefsSaveData();
#endif
    }
}

[Serializable]
public enum GameMode
{
    Solitare,
    Collecting,
    Slide
}