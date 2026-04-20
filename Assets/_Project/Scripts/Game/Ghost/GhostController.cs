using System;
using _Project.Scripts.Managers;
using _Project.Scripts.Player;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Game.Ghost
{
    public class GhostController : MonoBehaviour
    {
        [SerializeField] private int maxHp = 100;
        [SerializeField] private float speed = 2f;

        private SpriteRenderer _sprite;
        private float _hp;
        
        private Transform _target1;
        private Transform _target2;
        
        private Transform _target;
        private bool _absorb;
        private bool _damaged;

        public int typ = 1;
        
        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            
            _target1 = GameObject.FindWithTag("GhostTarget1").transform;
            _target2 = GameObject.FindWithTag("GhostTarget2").transform;
            _target = _target1;
            
            _hp = maxHp;
            if (_sprite) _sprite.DOFade(0, 0).SetLink(_sprite.gameObject, LinkBehaviour.KillOnDestroy);
            
            ShowSelf();
        }

        private void Update()
        {
            if (!_target) return;
            
            speed += Time.deltaTime * Random.Range(0.6f, 2f);
            transform.position = Vector2.MoveTowards(transform.position, _target.position, Time.deltaTime * speed);
            
            if (!_absorb && (transform.position - _target.position).sqrMagnitude < 0.01f) AbsorbSelf();
            else if (!_damaged && _absorb && (transform.position - _target.position).sqrMagnitude < 0.01f)
            {
                PlayerStats.Instance.TakeDamage();
                _damaged = true;
                SoundManager.Instance.SoundActions.GhostAttack();
            }
        }

        private async void ShowSelf()
        {
            if (!_sprite || !this) return;
            await _sprite.DOFade(1f, 1f).SetEase(Ease.InOutSine).SetLink(_sprite.gameObject, LinkBehaviour.KillOnDestroy).AsyncWaitForCompletion();
            if (!_sprite || !this) return;
            await _sprite.DOFade(Random.Range(0.6f, 0.8f), 0.4f).SetEase(Ease.InBounce).SetLink(_sprite.gameObject, LinkBehaviour.KillOnDestroy).SetLoops(2, LoopType.Restart).AsyncWaitForCompletion();
            if (!_sprite || !this) return;
            await _sprite.DOFade(Random.Range(0.8f, 1f), 0.2f).SetEase(Ease.InOutSine).SetLink(_sprite.gameObject, LinkBehaviour.KillOnDestroy).SetLoops(-1, LoopType.Yoyo).AsyncWaitForCompletion();
        }

        private async void AbsorbSelf()
        {
            _absorb = true;
            _target = _target2;
            speed *= Random.Range(1.2f, 2f);

            if (_sprite) _sprite.DOColor(Color.gray1, 2f).SetLink(_sprite.gameObject, LinkBehaviour.KillOnDestroy).SetEase(Ease.InOutSine);
            if (_sprite) await _sprite.DOFade(0, 2f).SetEase(Ease.InOutSine).SetLink(_sprite.gameObject, LinkBehaviour.KillOnDestroy).AsyncWaitForCompletion();
            if (this) Destroy(gameObject);
        }

        private async void HideSelf()
        {
            _target = null;
            
            _sprite.DOColor(Color.white, 1f).SetLink(_sprite.gameObject, LinkBehaviour.KillOnDestroy).SetEase(Ease.InOutSine);
            await _sprite.DOFade(0, 1f).SetEase(Ease.InOutSine).SetLink(_sprite.gameObject, LinkBehaviour.KillOnDestroy).AsyncWaitForCompletion();
            if (!_damaged) SoundManager.Instance.SoundActions.KillGhost(typ);
            if (this) Destroy(gameObject);
        }
        
        public async void TakeDamage(int damage, float delay)
        {
            GetComponent<Collider2D>().enabled = false;
            while (_hp > 0 && !_absorb)
            {
                _hp -= damage;
                if (!_sprite || !this && _hp > 0) continue;
                var wait = _sprite.DOColor(Color.indianRed, Random.Range(0.02f, 0.08f)).SetEase(Ease.InOutSine).SetLink(_sprite.gameObject, LinkBehaviour.KillOnDestroy).AsyncWaitForCompletion();
                if (_hp <= 0)
                {
                    HideSelf();
                    break;
                }
                await wait;
                if (_sprite && !_absorb)
                {
                    await _sprite.DOColor(Color.white, 0.2f).SetEase(Ease.InOutSine)
                        .SetLink(_sprite.gameObject, LinkBehaviour.KillOnDestroy).AsyncWaitForCompletion();
                }
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            }
        }
    }
}