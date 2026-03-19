using System.Collections.Generic;
using UnityEngine;

public class TilePool : MonoBehaviour
{
    [SerializeField] private MajhongTileView prefab;
    
    public MajhongTileView Get()
    {
        MajhongTileView result = null;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf)
                continue;

            result = transform.GetChild(i).GetComponent<MajhongTileView>();
            break;
        }
        
        if (result is null)
            result = Instantiate(prefab, transform);
        
        result.gameObject.SetActive(true);
        
        return result;
    }
    
    public void Release(MajhongTileView tile)
    {
        tile.gameObject.SetActive(false);
    }
    public void ClearAll()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}