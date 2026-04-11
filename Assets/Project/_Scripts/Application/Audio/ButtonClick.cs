using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    private Toggle toggle;
    private Button button;
    
    private void Start()
    {
        toggle = GetComponent<Toggle>();

        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(v => OnClickSound());
            return;
        }

        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnClickSound);
        }
    }

    public void OnClickSound()
    {
        if(SoundManager.Instance == null)
            return;
        
        SoundManager.Instance.PlayButtonClick();
    }

    private void OnDestroy()
    {
        toggle?.onValueChanged.RemoveAllListeners();
        button?.onClick.RemoveAllListeners();
    }
}
