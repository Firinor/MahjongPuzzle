using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Desk", menuName = "Scriptable Objects/Desk")]
public class Desk : ScriptableObject
{
    [SerializeField]
    public List<Vector3> TilesPositions = new();
}

