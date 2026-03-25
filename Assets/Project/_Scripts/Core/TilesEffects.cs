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

    [SerializeField] 
    private Transform tileCollideEffectParent;
    [SerializeField] 
    private TileCollideEffect tileCollideEffectPrefab;
    
    public bool isHintAnimation;
    
    private const float halfTile3 = 1.1f;
    public void FlyTiles(MajhongTileView tile1, MajhongTileView tile2, 
        int scores, Action callback)
    {
        StartCoroutine(FlyTilesCoroutine(tile1, tile2, scores, callback));
    }
    
    private IEnumerator FlyTilesCoroutine(MajhongTileView tile1, MajhongTileView tile2,
        int scores, Action callback)
    {
        tile1.RaycastDisable();
        tile2.RaycastDisable();

        Vector3 tile1StartPoint = tile1.transform.position;
        Vector3 tile2StartPoint = tile2.transform.position;
        Vector3 collidePoint = (tile1StartPoint + tile2StartPoint) / 2;
        collidePoint.z = collideZCoordinate;

        bool isRight = tile1StartPoint.x > tile2StartPoint.x;
        bool isUp = tile1StartPoint.y > tile2StartPoint.y;

        Vector3 tile1CollidePoint, tile2CollidePoint;
        if (isRight)
        {
            tile1CollidePoint = collidePoint + Vector3.right * halfTile3;
            tile2CollidePoint = collidePoint + Vector3.left * halfTile3;
        }
        else
        {
            tile1CollidePoint = collidePoint + Vector3.left * halfTile3;
            tile2CollidePoint = collidePoint + Vector3.right * halfTile3;
        }

        float timer = 0;

        Vector3 delta1 = tile1CollidePoint - tile1StartPoint;
        Vector3 delta2 = tile2CollidePoint - tile2StartPoint;
        
        while (timer < 1)
        {
            float path = tilePath.Evaluate(timer);
            float Zpath = tileZPath.Evaluate(timer);
            float XYpath = tileXYPath.Evaluate(timer);

            Vector3 nexPosition1 = tile1StartPoint;
            nexPosition1.x += delta1.x * path + (isRight ? XYpath : -XYpath);
            nexPosition1.y += delta1.y * path + (isUp ? -XYpath : XYpath);
            nexPosition1.z += delta1.z * Zpath;
            tile1.transform.position = nexPosition1;
            
            Vector3 nexPosition2 = tile2StartPoint;
            nexPosition2.x += delta2.x * path + (isRight ? -XYpath : XYpath);
            nexPosition2.y += delta2.y * path + (isUp ? XYpath : -XYpath);
            nexPosition2.z += delta2.z * Zpath;
            tile2.transform.position = nexPosition2;
                
            yield return null;
            
            timer += Time.deltaTime;
        }

        Vector3 effectPosition = Camera.main.WorldToScreenPoint(collidePoint);
        var effect = Instantiate(tileCollideEffectPrefab,
            collidePoint,
            Quaternion.identity,
            tileCollideEffectParent);
        effect.SetText(scores);
        callback?.Invoke();
    }

    public void Hint(MajhongTileView tile1, MajhongTileView tile2)
    {
        isHintAnimation = true;
        
        tile1.HintAnimation();
        tile2.HintAnimation();
    }
}
