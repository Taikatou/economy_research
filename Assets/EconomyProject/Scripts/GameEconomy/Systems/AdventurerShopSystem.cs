using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public class AdventurerShopSystem : EconomySystem<AdventurerAgent, AgentScreen>
    {
        protected override AgentScreen ActionChoice => AgentScreen.Shop;
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }
    }
}
