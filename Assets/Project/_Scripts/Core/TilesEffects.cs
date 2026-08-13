using System;
using System.Collections;
using FirAnimations;
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
    private TileCollideEffect tileCollideEffect;
    
    private const float halfTile3 = .418f;
    public void FlyTiles(MajhongTileView tile1, MajhongTileView tile2, 
        int scores, Action callback)
    {
        StartCoroutine(FlyTilesCoroutine(tile1, tile2, scores, callback));
    }
    public void FlyTiles(TileInHandView tile1, TileInHandView tile2, 
        int scores, Action callback)
    {
        SoundManager.Instance.PlayTileEndCollide(tile1.transform.position);
        
        var zoom = tile1.PositionAnimation.gameObject.AddComponent<FirZoomAnimation>();
        zoom.StartZoom = Vector3.one;
        zoom.EndZoom = Vector3.zero;
        zoom.ToStartPoint();
        zoom.Play();
        
        var zoom2 = tile2.PositionAnimation.gameObject.AddComponent<FirZoomAnimation>();
        zoom2.StartZoom = Vector3.one;
        zoom2.EndZoom = Vector3.zero;
        zoom2.ToStartPoint();
        zoom2.Play();
        zoom2.OnComplete += () =>
        {
            Debug.Log($"OnComplete 1");
            zoom2.OnComplete = null;
            zoom.Stop();
            zoom2.Stop();
            Debug.Log($"OnComplete 2");
            Destroy(zoom); 
            Destroy(zoom2); 
            Debug.Log($"OnComplete 3");
            tile1.Hide();
            tile2.Hide();
            Debug.Log($"OnComplete 4");
            zoom.ToStartPoint();
            zoom2.ToStartPoint();
            Debug.Log($"OnComplete 5");
            callback?.Invoke();
        };
    }
    
    private IEnumerator FlyTilesCoroutine(MajhongTileView tile1, MajhongTileView tile2,
        int scores, Action callback)
    {
        SoundManager.Instance.PlayTileStartCollide(tile2.transform.position);
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

        bool sound = false;
        
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

            if (!sound && path >= 0.9f)
            {
                tileCollideEffect.StopAnimations();
                tileCollideEffect.transform.position = collidePoint + Vector3.back * 2;
                tileCollideEffect.SetText(scores);
                tileCollideEffect.gameObject.SetActive(true);
                SoundManager.Instance.PlayTileEndCollide(collidePoint);
                sound = true;
            }
            
            yield return null;
            
            timer += Time.deltaTime;
        }
        
        callback?.Invoke();
    }

    public void Hint(MajhongTileView tile1, MajhongTileView tile2)
    {
        tile1.HintAnimation();
        tile2.HintAnimation();
    }
}
