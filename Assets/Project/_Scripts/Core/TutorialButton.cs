using UnityEngine;
using UnityEngine.Events;

public class TutorialButton : MonoBehaviour
{
    public UnityEvent OnOpenTutorial;
    public UnityEvent OnCloseTutorial;
    public CanvasGroup Tutorial;

    public void ClickTutorialButton()
    {
        if(Tutorial.alpha > 0)
            CloseTutorial();
        else
            OpenTutorial();
    }
    
    public void OpenTutorial()
    {
        OnOpenTutorial?.Invoke();
    }

    public void CloseTutorial()
    {
        OnCloseTutorial?.Invoke();
    }
}
