using System.Linq;
using UnityEngine;
using UnityEditor;

public static class EditorTools
{
    private static Desk2 desk;
    private static int mask = LayerMask.NameToLayer("Default");
    
    [MenuItem("Tool/SaveDesk")]
    public static void SaveDesk()
    {
        MajhongTileView[] tiles 
            = Object.FindObjectsByType<MajhongTileView>(
                FindObjectsInactive.Exclude, 
                FindObjectsSortMode.None);

        desk = ScriptableObject.CreateInstance<Desk2>();

        foreach (MajhongTileView tile in tiles)
        {
            DeckTile deckTile = new();
            deckTile.position = tile.transform.position;
            desk.TilesPositions.Add(deckTile);
            SetNeighbors(tile, deckTile);

        }
        desk.TilesPositions = desk.TilesPositions
            .OrderByDescending(t => t.position.z)
            .ThenByDescending(t => t.position.y)
            .ThenBy(t => t.position.x)
            .ToList();
        
        AssetDatabase.CreateAsset(desk, "Assets/Project/Resources/Desks/NewDesk.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = desk;
    }
    
    private static void SetNeighbors(MajhongTileView tileView, DeckTile tile)
    {
        Collider[] results = new Collider[4];
        Bounds bounds = tileView.FrontTrigger.bounds;
        int size = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, results, Quaternion.identity);
        if (size > 0)
            for (int i = 0; i < size; i++)
            {
                if (results[i].gameObject.layer == mask)
                {
                    Debug.Log("UpNeighbors" + results[i].name);   
                    tile.UpNeighbors.Add(results[i].transform.position);
                }
            }
        bounds = tileView.LeftTrigger.bounds;
        size = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, results, Quaternion.identity);
        if (size > 0)
            for (int i = 0; i < size; i++)
            {
                if (results[i].gameObject.layer == mask)
                {
                    Debug.Log("LeftNeighbors" + results[i].name);   
                    tile.LeftNeighbors.Add(results[i].transform.position);
                }
            }
        bounds = tileView.RightTrigger.bounds;
        size = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, results, Quaternion.identity);
        if (size > 0)
            for (int i = 0; i < size; i++)
            {
                if (results[i].gameObject.layer == mask)
                {
                    Debug.Log("RightNeighbors" + results[i].name);   
                    tile.RightNeighbors.Add(results[i].transform.position);
                }
            }

        tile.IsOpenOnStart = tile.RightNeighbors.Count == 0
                             && tile.LeftNeighbors.Count == 0
                             && tile.UpNeighbors.Count == 0;

    }
}
