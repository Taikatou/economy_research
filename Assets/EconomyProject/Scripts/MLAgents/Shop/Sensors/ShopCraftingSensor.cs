using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopCraftingSensor : AgentMovementSensor<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        private readonly ShopCraftingSystem _shopCraftingSystem;
        protected override EShopScreen ValidScreen => EShopScreen.Craft;
        protected override EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices> EconomySystem => _shopCraftingSystem;
        public override int SensorCount => ShopCraftingSystem.ObservationSize + 27;
        public override string GetName() => "ShopCraftingSensor";

        public ShopCraftingSensor(ShopAgent agent, ShopCraftingSystem system) : base(agent)
        {
            _shopCraftingSystem = system;
            canViewConstant = true;
        }
    }
}
