using UnityEngine;
using UnityEngine.UI;

public class DeskToggle : MonoBehaviour
{
    public string ID;
    public int UnlockCost;

    public Toggle Toggle;
    public Button UnlockButton;

    public void Unlock()
    {
        if(UnlockButton == null)
            return;
        
        Destroy(UnlockButton.gameObject);
    }
}