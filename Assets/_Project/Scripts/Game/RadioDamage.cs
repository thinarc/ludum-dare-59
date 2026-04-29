using System;
using _Project.Scripts.Game.Ghost;
using _Project.Scripts.Managers;
using _Project.Scripts.Upgrades;
using UnityEngine;

namespace _Project.Scripts.Game.Radio
{
    public class RadioDamage : MonoBehaviour, IUpgradeTarget
    {
        [SerializeField] private int damage = 5;
        [SerializeField] private float delay = 0.2f;

        [SerializeField] private float speedRot = 100f;
        private float _currentRot;

        [SerializeField, Space(5)] private ParticleSystem handle;

        private ParticleSystem _waveSystem;

        private void Start()
        {
            _waveSystem = GetComponent<ParticleSystem>();
        }

        public void ApplyUpgrade(UpgradeData upgrade)
        {
            if (upgrade.type == UpgradeType.Damage) damage += Convert.ToInt32(upgrade.value);
            if (upgrade.type == UpgradeType.Cooldown)
            {
                delay -= upgrade.value;
                var rate = _waveSystem.emission.rateOverTime;
                rate.constant += upgrade.value * 3f;
            }
        }

        private void Update()
        {
            var input = 0f;
            if (Input.GetKey(KeyCode.D)) input = -1f;
            if (Input.GetKey(KeyCode.A)) input = 1f;
            if (input != 0f) SoundManager.Instance.SoundActions.SearchRadio(Time.deltaTime / 10f);

            _currentRot += input * speedRot * Time.deltaTime;
            _currentRot = Mathf.Clamp(_currentRot, -240f, 240f);
            
            // var shape = _waveSystem.shape;
            // shape.rotation = new Vector3(0f, 0f, _currentRot);
            var shape = _waveSystem.transform;
            shape.rotation = Quaternion.Euler(0f, 0f, _currentRot);
            
            var handleRot = _currentRot / 4f;
            handle.transform.rotation = Quaternion.Euler(0f, 0f, handleRot);
        }
        
        private void OnParticleCollision(GameObject other)
        {
            var ghost = other.GetComponent<GhostController>();
            ghost.TakeDamage(damage, delay);
        }
    }
}