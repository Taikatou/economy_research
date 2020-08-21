using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public interface IAdventureSense
    {
        float [] GetSenses(AdventurerAgent agent);
    }
}
