using UnityEngine;

namespace _Project.Scripts.Upgrades
{
    [CreateAssetMenu(menuName = "Upgrades/Upgrade")]
    public class UpgradeData : ScriptableObject
    {
        public UpgradeType type;
        public string labelEn;
        public string descEn;
        public string labelRu;
        public string descRu;
        public float value;
    }
    
    public enum UpgradeType
    {
        Damage,
        Cooldown,
        Radius,
        HealHp
    }
}