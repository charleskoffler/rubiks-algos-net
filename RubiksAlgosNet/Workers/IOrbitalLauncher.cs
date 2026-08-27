using Clprolf.ArchUnitNet.Attributes;

namespace RubiksAlgosNet.Workers
{
    [ClInfrastructure]
    [ClFamily]
    public interface IOrbitalLauncher
    {
        void launchOrbital();
    }
}