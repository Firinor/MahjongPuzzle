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
        color.OnComplete += () => gameObject.SetActive(false);
    }

    public void StopAnimations()
    {
        gameObject.SetActive(false);
        position.Stop();
        color.Stop();
    }
    private void OnEnable()
    {
        position.Play();
        color.Play();
    }

    public void SetText(int scores)
    {
        text.text = "+" + scores;
    }
}
