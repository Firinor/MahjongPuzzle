using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(EditorTools))]
public static class EditorTools
{
    private const float TileDelta = .2f;
    private const float TileWidth = 2.2f;
    private const float TileWidthZone = 1.3f;
    private const float TileHeightZone = 1.6f;
    private const float TileThickness = 1.6f;

    private static Desk2 desk;
    
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
        }
        foreach (DeckTile tile in desk.TilesPositions)
        {
            SetNeighbors(tile);
        }
        
        AssetDatabase.CreateAsset(desk, "Assets/Project/Resources/Desks/NewDesk.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = desk;
    }
    
    private static void SetNeighbors(DeckTile tile)
    {
        ChechTilesLyingOnTopRaycast(tile);
        ChechTilesLyingOnLeftRaycast(tile);
        ChechTilesLyingOnRightRaycast(tile);

        tile.IsOpenOnStart = tile.RightNeighbors.Count == 0
                             && tile.LeftNeighbors.Count == 0
                             && tile.UpNeighbors.Count == 0;

    }
    
    private static bool ChechTilesLyingOnRightRaycast(DeckTile tile)
    {
        tile.RightNeighbors = new(2);
        foreach (DeckTile otherTile in desk.TilesPositions)
        {
            if(tile == otherTile)
                continue;
            
            if (Mathf.Approximately(otherTile.position.z, tile.position.z)
                && Mathf.Abs(otherTile.position.x - (tile.position.x + TileWidth)) < TileDelta
                && Mathf.Abs(otherTile.position.y - tile.position.y) <  TileHeightZone)
            {
                
                tile.RightNeighbors.Add(otherTile.position);
            }
        }
        return tile.RightNeighbors.Count > 0;
    }
    private static bool ChechTilesLyingOnLeftRaycast(DeckTile tile)
    {
        tile.LeftNeighbors = new(2);
        foreach (DeckTile otherTile in desk.TilesPositions)
        {
            if(tile == otherTile)
                continue;
            
            if (Mathf.Approximately(otherTile.position.z, tile.position.z)
                && Mathf.Abs(otherTile.position.x - (tile.position.x - TileWidth)) < TileDelta
                && Mathf.Abs(otherTile.position.y - tile.position.y) <  TileHeightZone)
            {
                
                tile.LeftNeighbors.Add(otherTile.position);
            }
        }
        return tile.LeftNeighbors.Count > 0;
    }

    private static bool ChechTilesLyingOnTopRaycast(DeckTile tile)
    {
        tile.UpNeighbors = new(4);
        
        foreach (DeckTile otherTile in desk.TilesPositions)
        {
            if(tile == otherTile)
                continue;
            
            if (Mathf.Approximately(otherTile.position.z, tile.position.z - TileThickness)
                && Mathf.Abs(otherTile.position.x - tile.position.x) <  TileWidthZone
                && Mathf.Abs(otherTile.position.y - tile.position.y) <  TileHeightZone)
            {
                
                tile.UpNeighbors.Add(otherTile.position);
            }
        }
        return tile.UpNeighbors.Count > 0;
    }
}
