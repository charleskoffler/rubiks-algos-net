using Clprolf.ArchUnitNet.Attributes;

namespace RubiksAlgos.Workers
{
    [ClInfrastructure]
    [ClFamily]
    public interface IOrbitalLauncher
    {
        void launchOrbital();
    }
}