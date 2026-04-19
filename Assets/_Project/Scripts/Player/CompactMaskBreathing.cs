using UnityEngine;

namespace _Project.Scripts.Player
{
    [RequireComponent(typeof(SpriteMask))]
    public class SpriteMaskBreathing : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private float min = 0.5f;
        [SerializeField] private float max = 0.8f;

        private SpriteMask mask;

        private void Awake()
        {
            mask = GetComponent<SpriteMask>();
        }

        private void Update()
        {
            float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
            float value = Mathf.Lerp(min, max, t);

            mask.alphaCutoff = value;
        }
    }
}