using System;
using System.Collections.Generic;
using _Project.Scripts.Game.Upgrades;
using _Project.Scripts.Managers;
using _Project.Scripts.Player;
using _Project.Scripts.Upgrades;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Game.Wave
{
    public class WaveController : MonoBehaviour, IUpgradeTarget
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
        private static bool _restart;

        public static int Repeats;
        public GameObject cherep;
        public Vector2 cherepPos;

        private void Start()
        {
            PlayerController.Instance.PlayerDeath += OnPlayerDeath;
            ToIdle(true);
            _disable = false;

            var repeats = Repeats;
            while (repeats > 0)
            {
                repeats--;
                var pos = cherepPos;
                pos.x += Random.Range(-2f, 2f);
                pos.y += Random.Range(-0.5f, 0.5f);
                Instantiate(cherep, pos, Quaternion.identity);
            }
        }

        public void ApplyUpgrade(UpgradeData upgrade)
        {
            if (upgrade.type == UpgradeType.Radius)
            {
                // _healHp += Convert.ToInt32(upgrade.value);
                var rate = GameObject.FindWithTag("RadioSystem").GetComponent<ParticleSystem>().emission.rateOverTime;
                rate.constant += upgrade.value;
                print(GameObject.FindWithTag("RadioSystem").GetComponent<ParticleSystem>().emission.rateOverTime.constant);
            }
        }

        private async void OnPlayerDeath()
        {
            print("игрокумер");
            _disable = true;
            _restart = true;
            GameObject.FindWithTag("HandleSystem").SetActive(false);
            GameObject.FindWithTag("RadioSystem").SetActive(false);
            CamChanger.Instance.ChangeView(true);
            await LightSetup.Instance.SetupLight(0f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Repeats++;
        }

        private int _waves;
        private async void Pass()
        {
            PlayerController.Instance.Pass();
            _disable = true;
            _pass = true;
            GameObject.FindWithTag("HandleSystem")?.SetActive(false);
            GameObject.FindWithTag("RadioSystem")?.SetActive(false);
            SoundManager.Instance.SoundActions.Final();
            await LightSetup.Instance.SetupLight(0f);
        }
        
        private void Update()
        {
            if (_disable) return;
            if (battle && !_battle) ToBattle();
            else if (!battle && _battle) ToIdle();
            spawnDelay -= Time.deltaTime / 60f;
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
            if (_disable) return;
            _battle = battle;
            CamChanger.Instance.ChangeView();
            var emit = GameObject.FindWithTag("HandleSystem").GetComponent<ParticleSystem>().emission;
            emit.enabled = true;
            var emit2 = GameObject.FindWithTag("RadioSystem").GetComponent<ParticleSystem>().emission;
            emit2.enabled = true;
            PlayerController.Instance.ToBattle();
            SoundManager.Instance.SoundActions.RadioOn();
            LightSetup.Instance.SetupLight(0.3f).Forget();
            crow.SetBool("hide", false);

            StartTimer();
            while (_battle)
            {
                _restart = false;
                var ghostPrefab = ghosts[Random.Range(0, ghosts.Length)];
                var pos = GetRandomPosition();
                
                await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));
                if (!_battle || _restart) break;
                var ghost = Instantiate(ghostPrefab, pos, Quaternion.identity);
                _ghosts.Add(ghost);
            }
        }

        private bool _pass;
        
        private async void ToIdle(bool first = false)
        {
            if (_disable) return;
            _waves++;
            if (_waves >= 5)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(2));
                Pass();
            }
            
            _battle = battle;
            CamChanger.Instance.ChangeView();
            if (!_disable && !_pass)
            {
                var emit = GameObject.FindWithTag("HandleSystem").GetComponent<ParticleSystem>().emission;
                emit.enabled = false;
                var emit2 = GameObject.FindWithTag("RadioSystem").GetComponent<ParticleSystem>().emission;
                emit2.enabled = false;
            }
            PlayerController.Instance.ToIdle();
            SoundManager.Instance.SoundActions.RadioOff();
            LightSetup.Instance.SetupLight(0.4f).Forget();
            crow.SetBool("hide", true);
            PlayerStats.Instance.TakeHeal();
            
            _ghosts.ForEach(g => { if (g) Destroy(g); });
            _ghosts.Clear();
            StartTimer();
            
            if (first) return;
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            if (!_disable) UpgradeManager.Instance.UpdateViews();
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