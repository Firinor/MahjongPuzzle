using System;
using System.Collections;
using UnityEngine;

public class TilesEffects : MonoBehaviour
{
    [SerializeField] 
    private float collideZCoordinate = -5.5f;
    [SerializeField] 
    private AnimationCurve tilePath;
    [SerializeField] 
    private AnimationCurve tileZPath;
    [SerializeField] 
    private AnimationCurve tileXYPath;
    
    private const float halfTile3 = 1.1f;
    public void FlyTiles(MajhongTileView tile1, MajhongTileView tile2, Action callback)
    {
        StartCoroutine(FlyTilesCoroutine(tile1, tile2, callback));
    }

    private IEnumerator FlyTilesCoroutine(MajhongTileView tile1, MajhongTileView tile2, Action callback)
    {
        tile1.RaycastDisable();
        tile2.RaycastDisable();

        Vector3 tile1StartPoint = tile1.transform.position;
        Vector3 tile2StartPoint = tile2.transform.position;
        Vector3 collidePoint = (tile1StartPoint + tile2StartPoint) / 2;
        collidePoint.z = collideZCoordinate;
        Vector3 tile1CollidePoint = collidePoint + Vector3.left * halfTile3;
        Vector3 tile2CollidePoint = collidePoint + Vector3.right * halfTile3;

        float timer = 0;

        Vector3 delta1 = tile1CollidePoint - tile1StartPoint;
        Vector3 delta2 = tile2CollidePoint - tile2StartPoint;
        
        while (true)
        {
            if(timer >= 1)
                break;

            float path = tilePath.Evaluate(timer);

            tile1.transform.position = tile1StartPoint + delta1 * path;
                
            yield return null;
            
            timer += Time.deltaTime;
        }
        callback?.Invoke();
    }
}
