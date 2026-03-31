using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(EditorTools))]
public static class EditorTools
{
    [MenuItem("Tool/SaveDesk")]
    public static void SaveDesk()
    {
        MajhongTileView[] tiles 
            = Object.FindObjectsByType<MajhongTileView>(
                FindObjectsInactive.Exclude, 
                FindObjectsSortMode.None);

        Desk desk = ScriptableObject.CreateInstance<Desk>();

        foreach (var tile in tiles)
        {
            desk.TilesPositions.Add(tile.transform.position);
        }
        
        AssetDatabase.CreateAsset(desk, "Assets/Project/Resources/Desks/NewDesk.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = desk;
    }
}
