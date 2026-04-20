using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Player
{
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        [SerializeField] private Slider hpSlider;
        [SerializeField] private Image shadow;
        [SerializeField] private TextMeshProUGUI[] hpNumbers;
        
        private CanvasGroup _canvas;

        private const int MaxHp = 5;
        private int _hp;

        private void Start()
        {
            Instance = this;
            
            _canvas = GetComponent<CanvasGroup>();
            _canvas.DOFade(0, 0);
            hpNumbers.ForEach(n => n.DOFade(1, 0));
            
            _hp = MaxHp;
        }

        public async void TakeDamage()
        {
            if (_hp <= 0) return;
            _hp--;
            await _canvas.DOFade(1f, 0.08f).SetEase(Ease.Flash).SetLoops(2, LoopType.Yoyo).AsyncWaitForCompletion();
            if (_hp > 0) shadow.DOColor(Color.black, 0.1f).SetEase(Ease.Flash).SetLoops(2, LoopType.Incremental);
            else shadow.DOColor(Color.red, 0.1f).SetEase(Ease.Flash).SetEase(Ease.Flash).SetLoops(2, LoopType.Incremental);
            await _canvas.DOFade(1f, 0.2f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            hpNumbers[^(MaxHp - _hp)].DOFade(0, 0.2f).SetEase(Ease.InOutSine);
            while (!Mathf.Approximately(hpSlider.value, _hp)) hpSlider.value = Mathf.MoveTowards(hpSlider.value, _hp, Time.deltaTime * 0.6f);
            await UniTask.Delay(1000);
            await shadow.DOColor(Color.white, 0.2f).SetEase(Ease.InFlash).AsyncWaitForCompletion();
            var fade = Random.Range(0.2f, 0.4f);
            _canvas.DOFade(0, fade).SetEase(Ease.OutSine);
            if (_hp <= 0) PlayerController.Instance.SetSmert(true);
        }
    }
}