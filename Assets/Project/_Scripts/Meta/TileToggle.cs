using UnityEngine;
using UnityEngine.UI;

public class TileToggle : MonoBehaviour
{
    public string ID;
    public Toggle Toggle;
    public Image Image;
    public GameObject Lock;

    public void Unlock()
    {
        Toggle.interactable = true;
        Image.gameObject.SetActive(true);
        Destroy(Lock);
    }
}