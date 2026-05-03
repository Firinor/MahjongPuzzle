using FirAnimations;
using UnityEngine;

public class WinFlashlight : MonoBehaviour
{
    [SerializeField] 
    private FirAnimation[] animations;
    [SerializeField] 
    private Transform root;
    [SerializeField] 
    private Transform target;
    [SerializeField] 
    private float zPosition;
    
    public void Play()
    {
        foreach (var firAnimation in animations)
        {
            firAnimation.Play();
        }

        enabled = true;
    }

    private void Update()
    {
        Vector3 newPosition = target.position;
        newPosition.z = zPosition;
        root.position = newPosition;
    }
}
