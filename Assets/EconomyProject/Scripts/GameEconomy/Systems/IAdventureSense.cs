using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public interface IAdventureSense
    {
        float [] GetObservations(AdventurerAgent agent, int limit);
    }
}
