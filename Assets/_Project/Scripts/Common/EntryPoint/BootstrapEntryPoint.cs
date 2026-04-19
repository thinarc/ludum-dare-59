using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Common.EntryPoint
{
    public class BootstrapEntryPoint : MonoBehaviour
    {
        public static BootstrapEntryPoint Instance { get; private set; }
        
        private async void Start()
        {
            await LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];

            Application.targetFrameRate = 100;
            
            Instance = this;
            DontDestroyOnLoad(this);
            
            SceneManager.LoadScene(1);
        }
    }
}