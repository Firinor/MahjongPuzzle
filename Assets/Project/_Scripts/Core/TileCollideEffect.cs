using System;
using FirAnimations;
using TMPro;
using UnityEngine;

public class TileCollideEffect : MonoBehaviour
{
    [SerializeField] 
    private FirAnimation position;
    [SerializeField] 
    private FirAnimation color;
    [SerializeField] 
    private TextMeshProUGUI text;
    
    private void Awake()
    {
        color.OnComplete += () => Destroy(gameObject);
        position.Play();
        color.Play();
    }

    public void SetText(int scores)
    {
        text.text = "+" + scores;
    }
}
