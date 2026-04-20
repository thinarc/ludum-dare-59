namespace _Project.Scripts.Upgrades
{
    public interface IUpgradeTarget
    {
        void ApplyUpgrade(UpgradeData upgrade);
    }
}