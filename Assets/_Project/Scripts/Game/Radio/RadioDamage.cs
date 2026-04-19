using _Project.Scripts.Game.Ghost;
using UnityEngine;

namespace _Project.Scripts.Game.Radio
{
    public class RadioDamage : MonoBehaviour
    {
        [SerializeField] private int damage = 5;
        [SerializeField] private float delay = 0.2f;
        
        private void OnParticleCollision(GameObject other)
        {
            var ghost = other.GetComponent<GhostController>();
            ghost.TakeDamage(damage, delay);
        }
    }
}