using System;
using _Project.Scripts.Managers;
using _Project.Scripts.Upgrades;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Player
{
    public class PlayerStats : MonoBehaviour, IUpgradeTarget
    {
        public static PlayerStats Instance { get; private set; }

        [SerializeField] private Slider hpSlider;
        [SerializeField] private Image shadow;
        [SerializeField] private TextMeshProUGUI[] hpNumbers;
        
        private CanvasGroup _canvas;

        private const int MaxHp = 5;
        private int _hp;

        private void Awake()
        {
            Instance = this;
            
            _canvas = GetComponent<CanvasGroup>();
            _canvas.DOFade(0, 0).SetLink(_canvas.gameObject, LinkBehaviour.KillOnDestroy);
            hpNumbers.ForEach(n => n.DOFade(1, 0).SetLink(n.gameObject, LinkBehaviour.KillOnDestroy));
            
            _hp = MaxHp;
        }

        public void ApplyUpgrade(UpgradeData upgrade)
        {
            if (upgrade.type == UpgradeType.HealHp) _healHp += Convert.ToInt32(upgrade.value);
        }

        private int _healHp = 2;
        public void TakeHeal()
        {
            if (_hp <= 0) return;
            _hp += _healHp;
            _hp = Mathf.Clamp(_hp, 0, MaxHp);
            for (var i = _hp; i > 0; i--)
            {
                var n = hpNumbers[i - 1];
                n.DOFade(1, 0.1f).SetEase(Ease.InOutSine).SetLink(n.gameObject, LinkBehaviour.KillOnDestroy);
            }
        }

        public async void TakeDamage()
        {
            if (_hp <= 0) return;
            _hp--;
            await _canvas.DOFade(1f, 0.08f).SetEase(Ease.Flash).SetLoops(2, LoopType.Yoyo).SetLink(_canvas.gameObject, LinkBehaviour.KillOnDestroy).AsyncWaitForCompletion();
            if (_hp > 0 && shadow) shadow.DOColor(Color.black, 0.1f).SetEase(Ease.Flash).SetLink(shadow.gameObject, LinkBehaviour.KillOnDestroy).SetLoops(2, LoopType.Incremental);
            else if (shadow) shadow.DOColor(Color.red, 0.1f).SetEase(Ease.Flash).SetEase(Ease.Flash).SetLink(shadow.gameObject, LinkBehaviour.KillOnDestroy).SetLoops(2, LoopType.Incremental);
            if (_canvas) await _canvas.DOFade(1f, 0.2f).SetEase(Ease.InOutSine).SetLink(_canvas.gameObject, LinkBehaviour.KillOnDestroy).AsyncWaitForCompletion();
            hpNumbers[^(MaxHp - _hp)].DOFade(0, 0.2f).SetLink(hpNumbers[^(MaxHp - _hp)].gameObject, LinkBehaviour.KillOnDestroy).SetEase(Ease.InOutSine);
            while (!Mathf.Approximately(hpSlider.value, _hp)) hpSlider.value = Mathf.MoveTowards(hpSlider.value, _hp, Time.deltaTime * 0.6f);
            await UniTask.Delay(1000);
            if (shadow) await shadow.DOColor(Color.white, 0.2f).SetEase(Ease.InFlash).SetLink(shadow.gameObject, LinkBehaviour.KillOnDestroy).AsyncWaitForCompletion();
            var fade = Random.Range(0.2f, 0.4f);
            if (_canvas) _canvas.DOFade(0, fade).SetLink(_canvas.gameObject, LinkBehaviour.KillOnDestroy).SetEase(Ease.OutSine);
            if (_hp <= 0)
            {
                PlayerController.Instance.SetSmert(true);
                SoundManager.Instance.SoundActions.Smert();
            }
        }
    }
}