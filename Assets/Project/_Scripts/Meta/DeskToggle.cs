using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeskToggle : MonoBehaviour
{
    public string ID;
    public Image ImageBackground;
    public Image Image;
    public Image Medal1;
    public Image Medal2;
    public TextMeshProUGUI UnlockCostText;
    public Image UnlockCostImage;
    public Button Button;
    public Image Checkmark;
    public GameObject UnlockButton;

    public void Unlock(LevelStruct level)
    {
        ImageBackground.enabled = level.IsUnlocked;
        ID = level.ID;
        Image.enabled = level.IsUnlocked;
        Image.sprite = level.Sprite;
        Medal1.enabled = level.MedalsCount > 0;
        Medal2.enabled = level.MedalsCount > 1;
        UnlockCostText.text = level.UnlockCost.ToString();
        UnlockCostImage.sprite = level.CurrencySprite;
        UnlockButton.SetActive(!level.IsUnlocked);
    }
}