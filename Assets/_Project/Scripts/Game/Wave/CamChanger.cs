using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Game.Wave
{
    public class CamChanger : MonoBehaviour
    {
        public static CamChanger Instance { get; private set; }

        public bool targetView = true;
        private bool _targetView;

        [SerializeField, Space(5)] private float targetLens = 5f;
        [SerializeField] private float worldLens = 12f;

        [SerializeField, Space(5)] private float speedLens = 8f;
        
        private Transform _target;
        private CinemachineCamera _vCam;
        private CinemachineConfiner2D _confiner;
        
        public void ChangeView() => targetView = !targetView;
        
        private void Start()
        {
            Instance = this;
            
            _target = GameObject.FindWithTag("PointCam").transform;
            _vCam = FindAnyObjectByType<CinemachineCamera>();
            _confiner = _vCam.GetComponent<CinemachineConfiner2D>();
        }

        private void Update()
        {
            if (targetView && !_targetView) ToTargetView();
            else if (!targetView && _targetView) ToWorldView();
        }

        private async void ToTargetView()
        {
            _targetView = targetView;
            _vCam.Follow = _target;
            
            while (_targetView == targetView)
            {
                var lens = _vCam.Lens;
                lens.OrthographicSize = Mathf.MoveTowards(lens.OrthographicSize, targetLens, speedLens * Random.Range(0.8f, 1.24f) * Time.deltaTime);
                _vCam.Lens = lens;
                _confiner.InvalidateLensCache();
                if (Mathf.Approximately(lens.OrthographicSize, targetLens)) break;
                await UniTask.Yield();
            }
        }
        
        private async void ToWorldView()
        {
            _targetView = targetView;
            _vCam.Follow = transform;

            while (_targetView == targetView)
            {
                var lens = _vCam.Lens;
                lens.OrthographicSize = Mathf.MoveTowards(lens.OrthographicSize, worldLens, speedLens * Random.Range(0.8f, 1.24f) * Time.deltaTime);
                _vCam.Lens = lens;
                _confiner.InvalidateLensCache();
                if (Mathf.Approximately(lens.OrthographicSize, worldLens)) break;
                await UniTask.Yield();
            }
        }
    }
}