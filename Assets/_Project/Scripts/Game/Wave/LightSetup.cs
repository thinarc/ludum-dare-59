using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Project.Scripts.Game.Wave
{
    public class LightSetup : MonoBehaviour
    {
        public static LightSetup Instance { get; private set; }
        
        [SerializeField] private float speedSetting = 1f;
        
        private Light2D _global;

        private void Awake()
        {
            Instance = this;
            
            _global = GetComponent<Light2D>();
            _global.intensity = 0f;
        }
        
        public async UniTask SetupLight(float intensity)
        {
            while (!Mathf.Approximately(_global.intensity, intensity))
            {
                _global.intensity = Mathf.MoveTowards(_global.intensity, intensity, speedSetting * Time.deltaTime);
                await UniTask.Yield();
            }
        }
    }
}