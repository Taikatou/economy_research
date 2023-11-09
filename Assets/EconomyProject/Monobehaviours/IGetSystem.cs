using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Monobehaviours
{
    public interface IGetSystem
    {
        EconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> GetSystem { get; }
    }
}
