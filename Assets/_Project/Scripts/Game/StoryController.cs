using System;
using System.Collections.Generic;
using _Project.Scripts.Player;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace _Project.Scripts.Game
{
    public class StoryController : MonoBehaviour
    {
        public static StoryController Instance { get; private set; }
        
        [SerializeField] private float storyDelay;
        private const float TextTemp = 2;

        public static bool Restart;
        public bool victoryCheck;
        
        [SerializeField, Space(10)] private StoryText[] stories;
        [SerializeField, Space(5)] private StoryText restartHello;
        [SerializeField, Space(5)] private StoryText victory;
        private readonly Queue<StoryText> _storiesQueue = new ();
        
        [SerializeField, Space(5)] private TextMeshProUGUI storyText;
        
        private float _storyDelayTimer;

        private async void Start()
        {
            Instance = this;
            
            await UniTask.Delay(600);
            UpdateText("");
            fSize = storyText.fontSize;
            
            foreach (var story in stories) _storiesQueue.Enqueue(story);
            _storyDelayTimer = storyDelay / 20f;
            _storyDelayTimer += TextTemp;
            
            PlayerController.Instance.PlayerDeath += OnPlayerDeath;
        }

        private float fSize;
        private bool _type;

        private void OnPlayerDeath() => Restart = true;

        public void OnPass()
        {
            _storyDelayTimer = 100f;
            victoryCheck = true;
            ShowStory();
        }

        private void Update()
        {
            if (_storyDelayTimer <= 0) ShowStory();
            if (!_type) _storyDelayTimer -= Time.deltaTime;
        }

        private async void ShowStory()
        {
            if (_storiesQueue.Count == 0 && !victoryCheck) return;
            
            bool ruMod;
            if (!victoryCheck)
            {
                UpdateText(_storiesQueue.Dequeue().GetText(out var ru));
                ruMod = ru;
            }
            else
            {
                await UniTask.Delay(2000);
                UpdateText(victory.GetText(out var ru));
                ruMod = ru;
            }
            if (ruMod) storyText.fontSize = fSize / 1.8f;
            else storyText.fontSize = fSize;
            if (!victoryCheck) _storyDelayTimer = storyDelay;
            _type = true;
            // if (_storiesQueue.Count > 12) _storyDelayTimer /= 1.5f;
            // else if (_storiesQueue.Count == 11) _storyDelayTimer /= 2.2f;
            // else if (_storiesQueue.Count == 10) _storyDelayTimer /= 5f;
            _storyDelayTimer += TextTemp;
        }

        private async void UpdateText(string text, float delay = 0)
        {
            if (delay != 0) await UniTask.Delay(TimeSpan.FromSeconds(delay));

            if (text == "")
            {
                storyText.text = text;
                return;
            }
            
            await UniTask.Delay(500);
            storyText.DOFade(1, 0f);
            storyText.text = text;
        }

        public async void OnFinishType()
        {
            if (storyText.text == "") return;
            await UniTask.Delay(TimeSpan.FromSeconds(TextTemp));
            storyText.DOFade(0, 0.4f).SetEase(Ease.Linear);
            _type = false;
        }
    }

    [Serializable]
    public class StoryText
    {
        [TextArea(2, 2)] public string textRu;
        [TextArea(2, 2)] public string textEn;

        public string GetText(out bool ruMod)
        {
            ruMod = LocalizationSettings.SelectedLocale.Identifier.Code != "en-GB";
            return !ruMod ? textEn : textRu;
        }
    }

}