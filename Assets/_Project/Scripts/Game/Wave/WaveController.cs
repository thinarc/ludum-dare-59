using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
        
        public void ChangeState() => battle = !battle;
        
        private void Update()
        {
            if (battle && !_battle) ToBattle();
            else if (!battle && _battle) ToIdle();
            
            if (Input.GetKeyDown(KeyCode.F)) ChangeState();
        }

        private async void ToBattle()
        {
            _battle = battle;
            CamChanger.Instance.ChangeView();

            while (_battle)
            {
                var ghostPrefab = ghosts[Random.Range(0, ghosts.Length)];
                var pos = GetRandomPosition();
                
                await UniTask.Delay(TimeSpan.FromSeconds(spawnDelay));
                Instantiate(ghostPrefab, pos, Quaternion.identity);
            }
        }
        
        private void ToIdle()
        {
            _battle = battle;
            CamChanger.Instance.ChangeView();
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