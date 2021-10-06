using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopCraftingSensor : AgentMovementSensor<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        public ShopCraftingSystem shopCraftingSystem;
        protected override EShopScreen ValidScreen => EShopScreen.Craft;
        protected override EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices> EconomySystem => shopCraftingSystem;
        protected override int SensorCount { get; }

        public override string GetName()
        {
            throw new System.NotImplementedException();
        }

        public ShopCraftingSensor(ShopAgent agent, ShopCraftingSystem system) : base(agent)
        {
            shopCraftingSystem = system;
        }
    }
}
