using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Audio")]
public class AudioConfig : ScriptableObject
{
    [Header("Buttons")] 
    public ClipSettings ButtonClick;
    [Header("Tiles")] 
    public ClipSettings StartCollide;
    public ClipSettings EndCollide;
    public ClipSettings TileSelect;
}