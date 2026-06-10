using UnityEngine;

namespace FirYandexService
{
    public class YandexInternADSTileHintEvent : MonoBehaviour
    {
        [SerializeField] 
        private float timer = 60f;
        private float _timer;
        
        public MajhongSolitaireRules Rules;
        private void Awake()
        {
            if(FirYG2Service.instance == null)
                return;
            
            _timer = timer;
            Rules.OnTilesChanged += CheckTimer;
        }

        private void CheckTimer()
        {
            if(_timer > 0 || !FirYG2Service.instance.AdReady())
                return;

            FirYG2Service.instance.CheckTimerAd();
            _timer = timer;
        }
        
        private void Update()
        {
            _timer -= Time.deltaTime;
        }

        private void OnDestroy()
        {
            if(FirYG2Service.instance == null)
                return;
            Rules.OnTilesChanged -= CheckTimer;
        }
    }
}