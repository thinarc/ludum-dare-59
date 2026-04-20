using System;
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
        
        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            
            _target1 = GameObject.FindWithTag("GhostTarget1").transform;
            _target2 = GameObject.FindWithTag("GhostTarget2").transform;
            _target = _target1;
            
            _hp = maxHp;
            _sprite.DOFade(0, 0);
            
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
            }
        }

        private async void ShowSelf()
        {
            await _sprite.DOFade(1f, 1f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            await _sprite.DOFade(Random.Range(0.6f, 0.8f), 0.4f).SetEase(Ease.InBounce).SetLoops(2, LoopType.Restart).AsyncWaitForCompletion();
            await _sprite.DOFade(Random.Range(0.8f, 1f), 0.2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).AsyncWaitForCompletion();
        }

        private async void AbsorbSelf()
        {
            _absorb = true;
            _target = _target2;
            speed *= Random.Range(1.2f, 2f);

            _sprite.DOColor(Color.gray2, 2f).SetEase(Ease.InOutSine);
            await _sprite.DOFade(0, 2f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            try
            {
                Destroy(gameObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async void HideSelf()
        {
            _target = null;
            
            _sprite.DOColor(Color.white, 1f).SetEase(Ease.InOutSine);
            await _sprite.DOFade(0, 1f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            try
            {
                Destroy(gameObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public async void TakeDamage(int damage, float delay)
        {
            GetComponent<Collider2D>().enabled = false;
            while (_hp > 0)
            {
                _hp -= damage;
                var wait = _sprite.DOColor(Color.darkRed, Random.Range(0.02f, 0.08f)).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
                if (_hp <= 0)
                {
                    HideSelf();
                    break;
                }
                await wait;
                _sprite.DOColor(Color.white, 0.2f).SetEase(Ease.InOutSine);
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            }
        }
    }
}