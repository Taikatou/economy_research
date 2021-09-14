using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public interface IShopSubSystem
    {
        void SetInput(AdventurerAgent agent, EAdventureShopChoices choice);
    }
}
