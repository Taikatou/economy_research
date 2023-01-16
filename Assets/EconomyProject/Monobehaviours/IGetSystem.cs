using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Monobehaviours
{
    public interface IGetSystem
    {
        EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> GetSystem { get; }
    }
}
