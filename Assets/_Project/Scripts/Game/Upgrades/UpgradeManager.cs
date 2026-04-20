using System.Collections.Generic;
using _Project.Scripts.Upgrades;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace _Project.Scripts.Game.Upgrades
{
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }
        
        [SerializeField] private List<UpgradeData> allUpgrades;
        [SerializeField] private RectTransform[] views;

        private void Start()
        {
            Instance = this;
            
            foreach (var view in views) view.transform.DOScale(0, 0);
            
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<Image>().DOFade(0, 0f);
        }

        public void UpdateViews()
        {
            Time.timeScale = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            foreach (var other in views) other.GetComponent<Button>().interactable = true;
            
            try
            {
                GetComponent<Image>().DOFade(0.5f, 0.8f).SetEase(Ease.OutExpo).SetUpdate(true);
            }
            catch
            {
                // ignored
            }

            var updates = GetRandomUpgrades(views.Length);
            var queue = new Queue<UpgradeData>();
            foreach (var u in updates) queue.Enqueue(u);

            for (var i = 0; i < views.Length; i++)
            {
                var update = queue.Dequeue();
                var view = views[i];
                var label = "";
                var desc = "";
                var lang = LocalizationSettings.SelectedLocale;
                if (lang.Identifier.Code == "en-GB")
                {
                    label = update.labelEn;
                    desc = update.descEn;
                }
                else
                {
                    label = update.labelRu;
                    desc = update.descRu;
                }
                
                if (view == null) return;
                var n = view.Find("name").GetComponent<TextMeshProUGUI>();
                var d = view.Find("desc").GetComponent<TextMeshProUGUI>();
                n.text = label;
                d.text = desc;

                view.transform.DOScale(0, 0).SetUpdate(true);
                view.transform.DOScale(1, 0.5f).SetDelay(i * 0.2f).SetEase(Ease.OutBack).SetUpdate(true);

                view.GetComponent<Button>().onClick.AddListener(() =>
                {
                    
                    foreach (var vd in views)
                    {
                        vd.GetComponent<HoverZoomTrigger>().enabled = false;
                    }
                    
                    view.DOScale(0, 0.3f).SetEase(Ease.InBack).SetUpdate(true);
                    foreach (var other in views)
                    {
                        other.GetComponent<Button>().onClick.RemoveAllListeners();
                        other.GetComponent<Button>().interactable = false;
                        if (other != view)
                            other.DOScale(0, 0.4f * Random.Range(0.8f, 1.5f)).SetEase(Ease.InBack).SetUpdate(true);
                    }
                    
                    GetComponent<Image>().DOFade(0, 0.65f).SetEase(Ease.InExpo).SetUpdate(true);
                    Time.timeScale = 1f;
                    GetComponent<CanvasGroup>().blocksRaycasts = false;
                    
                    ApplyUpgrade(update);
                });
            }
        }

        private List<UpgradeData> GetRandomUpgrades(int count)
        {
            var pool = new List<UpgradeData>(allUpgrades);
            var result = new List<UpgradeData>();
            var usedDesc = new HashSet<string>();

            while (result.Count < count)
            {
                var index = Random.Range(0, pool.Count);
                var selected = pool[index];

                if (usedDesc.Contains(selected.descEn))
                {
                    pool.RemoveAt(index);
                    continue;
                }

                result.Add(selected);
                usedDesc.Add(selected.descEn);
                
                pool.RemoveAll(u => u.descEn == selected.descEn);
            }

            return result;
        }

        public void ApplyUpgrade(UpgradeData upgrade)
        {
            var all = FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );
            var targets = new List<IUpgradeTarget>();
            foreach (var mb in all)
            {
                if (mb is IUpgradeTarget target)
                    targets.Add(target);
            }
            
            foreach (var target in targets)
                target.ApplyUpgrade(upgrade);
        }
    }
}