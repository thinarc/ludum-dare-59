using _Project.Scripts.Game;
using _Project.Scripts.Game.Wave;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Common.EntryPoint
{
    public class BootstrapEntryPoint : MonoBehaviour
    {
        public static BootstrapEntryPoint Instance { get; private set; }

        private float _timer;
        public TextMeshProUGUI text;
        private float fontSize;
        
        private async void Start()
        {
            await LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];

            Application.targetFrameRate = 100;

            StoryController.Restart = false;
            
            Instance = this;
            DontDestroyOnLoad(this);
            WaveController.Repeats = 0;
            FMODUnity.RuntimeManager.StudioSystem.getBankList(out var banks);
            print(banks.Length);

            fontSize = text.fontSize;
            _timer = 0f;
            while (true)
            {
                if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space)) _timer -= Time.deltaTime;
                if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Space)) _timer = 2f;
                if (_timer <= 0f)
                {
                    SceneManager.LoadScene(1);
                    break;
                }

                text.fontSize = fontSize / (1f + 0.5f * (2f - _timer) / 10f);
                await UniTask.Yield();
            }
            
            while(true)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
                }
                await UniTask.Yield();
            }
        }
    }
}