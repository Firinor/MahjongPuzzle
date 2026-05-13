using UnityEngine;

namespace FirYandexService
{
    public class YandexInternADSTileHintEvent : MonoBehaviour
    {
        public MajhongSolitaireRules Rules;
        private void Awake()
        {
            if(FirYG2Service.instance == null)
                return;
            Rules.OnTilesChanged += FirYG2Service.instance.CheckTimerAd;
        }
        private void OnDestroy()
        {
            if(FirYG2Service.instance == null)
                return;
            Rules.OnTilesChanged -= FirYG2Service.instance.CheckTimerAd;
        }
    }
}