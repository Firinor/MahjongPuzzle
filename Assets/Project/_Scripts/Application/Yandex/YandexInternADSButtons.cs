using UnityEngine;
using UnityEngine.UI;

namespace FirYandexService
{
    public class YandexInternADSButtons : MonoBehaviour
    {
        [SerializeField] 
        private Button[] buttons;

        private void Awake()
        {
            if(FirYG2Service.instance == null)
                return;
            FirYG2Service.instance.SetButtons(buttons);
        }
    }
}
