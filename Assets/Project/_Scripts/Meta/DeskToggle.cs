using UnityEngine;
using UnityEngine.UI;

public class DeskToggle : MonoBehaviour
{
    public string ID;
    public Image ImageBackground;
    public Image Image;
    public Button Button;
    public Image Checkmark;
    public GameObject UnlockButton;

    public void Unlock(Sprite image)
    {
        ImageBackground.enabled = true;
        Image.enabled = true;
        Image.sprite = image;
        UnlockButton.SetActive(false);
    }
    public void Lock()
    {
        ImageBackground.enabled = false;
        Image.enabled = false;
        UnlockButton.SetActive(true);
    }
}