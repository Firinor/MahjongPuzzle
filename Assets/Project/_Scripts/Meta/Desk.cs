using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Desk", menuName = "Scriptable Objects/Desk")]
public class Desk : ScriptableObject
{
    public string ID;
    [SerializeField]
    public List<Vector4> TilesPositions = new();
}

