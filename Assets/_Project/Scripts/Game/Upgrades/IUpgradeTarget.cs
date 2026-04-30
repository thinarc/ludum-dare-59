using _Project.Scripts.Game.Upgrades;

namespace _Project.Scripts.Upgrades
{
    public interface IUpgradeTarget
    {
        void ApplyUpgrade(UpgradeData upgrade);
    }
}