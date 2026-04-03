using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Desk2", menuName = "Scriptable Objects/Desk2")]
public class Desk2 : ScriptableObject
{
    public string ID;
    [SerializeField]
    public List<DeckTile> TilesPositions = new();
}

[Serializable]
public class DeckTile
{
    public Vector3 position;
    public bool IsOpenOnStart;
    public List<Vector3> UpNeighbors;
    public List<Vector3> LeftNeighbors;
    public List<Vector3> RightNeighbors;
}