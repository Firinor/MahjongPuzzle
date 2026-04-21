using UnityEngine;
using UnityEngine.UI;

public class DeskToggle : MonoBehaviour
{
    public string ID;
    public Image Image;
    public Button Button;
    public Image Checkmark;
    public GameObject UnlockButton;

    public void Unlock(Sprite image)
    {
        Image.enabled = true;
        Image.sprite = image;
        UnlockButton.SetActive(false);
    }
    public void Lock()
    {
        Image.enabled = false;
        UnlockButton.SetActive(true);
    }
}