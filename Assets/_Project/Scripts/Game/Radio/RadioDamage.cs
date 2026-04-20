using _Project.Scripts.Game.Ghost;
using UnityEngine;

namespace _Project.Scripts.Game.Radio
{
    public class RadioDamage : MonoBehaviour
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

        private void Update()
        {
            var input = 0f;
            if (Input.GetKey(KeyCode.D)) input = -1f;
            if (Input.GetKey(KeyCode.A)) input = 1f;

            _currentRot += input * speedRot * Time.deltaTime;
            _currentRot = Mathf.Clamp(_currentRot, -90f, 90f);
            
            var shape = _waveSystem.shape;
            shape.rotation = new Vector3(0f, 0f, _currentRot);
            
            handle.transform.rotation = Quaternion.Euler(0f, 0f, _currentRot);
        }
        
        private void OnParticleCollision(GameObject other)
        {
            var ghost = other.GetComponent<GhostController>();
            ghost.TakeDamage(damage, delay);
        }
    }
}