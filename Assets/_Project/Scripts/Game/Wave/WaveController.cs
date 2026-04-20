using System;
using System.Collections.Generic;
using _Project.Scripts.Game.Upgrades;
using _Project.Scripts.Player;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Game.Wave
{
    public class WaveController : MonoBehaviour
    {
        [SerializeField] private GameObject[] ghosts;

        [SerializeField, Space(5)] private float spawnDelay = 2f;
        
        [SerializeField, Space(5)] private Vector2 size;
        [SerializeField] private float heigth;
        
        [Space(5)] public bool battle;
        private bool _battle;

        [SerializeField, Space(5)] private float battleTime = 10;
        [SerializeField, Space(5)] private float idleTime = 5;

        private List<GameObject> _ghosts = new();

        [SerializeField, Space(5)] private Animator crow;
        
        public void ChangeState() => battle = !battle;
        private bool _disable;

        private void Start()
        {
            PlayerController.Instance.PlayerDeath += OnPlayerDeath;
            ToIdle(true);
            _disable = false;
        }

        private async void OnPlayerDeath()
        {
            print("игрокумер");
            _disable = true;
            await LightSetup.Instance.SetupLight(0f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        private void Update()
        {
            if (_disable) return;
            if (battle && !_battle) ToBattle();
            else if (!battle && _battle) ToIdle();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F)) ChangeState();
#endif
        }

        private async void StartTimer()
        {
            float timer;
            if (_battle) timer = battleTime;
            else timer = idleTime;
            await UniTask.Delay(TimeSpan.FromSeconds(timer));
            battle = !battle;
        }

        private async void ToBattle()
        {
            _battle = battle;
            CamChanger.Instance.ChangeView();
            var emit = GameObject.FindWithTag("HandleSystem").GetComponent<ParticleSystem>().emission;
            emit.enabled = true;
            var emit2 = GameObject.FindWithTag("RadioSystem").GetComponent<ParticleSystem>().emission;
            emit2.enabled = true;
            PlayerController.Instance.ToBattle();
            LightSetup.Instance.SetupLight(0.3f).Forget();
            crow.SetBool("hide", false);

            StartTimer();
            while (_battle)
            {
                var ghostPrefab = ghosts[Random.Range(0, ghosts.Length)];
                var pos = GetRandomPosition();
                
                await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));
                var ghost = Instantiate(ghostPrefab, pos, Quaternion.identity);
                _ghosts.Add(ghost);
            }
        }
        
        private async void ToIdle(bool first = false)
        {
            _battle = battle;
            CamChanger.Instance.ChangeView();
            var emit = GameObject.FindWithTag("HandleSystem").GetComponent<ParticleSystem>().emission;
            emit.enabled = false;
            var emit2 = GameObject.FindWithTag("RadioSystem").GetComponent<ParticleSystem>().emission;
            emit2.enabled = false;
            PlayerController.Instance.ToIdle();
            LightSetup.Instance.SetupLight(0.4f).Forget();
            crow.SetBool("hide", true);
            
            _ghosts.ForEach(g => { if (g) Destroy(g); });
            _ghosts.Clear();
            StartTimer();
            
            if (first) return;
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            UpgradeManager.Instance.UpdateViews();
        }
        
        private Vector3 GetRandomPosition()
        {
            var x = Random.Range(-size.x / 2, size.x / 2);
            var y = Random.Range(-size.y / 2, size.y / 2);

            return transform.position + new Vector3(x, y + heigth, 0f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + new Vector3(0, heigth), size);
        }
    }
}