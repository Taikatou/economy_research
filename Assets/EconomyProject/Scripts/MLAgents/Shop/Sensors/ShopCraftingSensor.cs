using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopCraftingSensor : AgentMovementSensor<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        private readonly ShopCraftingSystem _shopCraftingSystem;
        protected override EShopScreen ValidScreen => EShopScreen.Craft;
        protected override EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices> EconomySystem => _shopCraftingSystem;
        protected override int SensorCount => 4;
        public override string GetName() => "ShopCraftingSensor";

        public ShopCraftingSensor(ShopAgent agent, ShopCraftingSystem system, BufferSensorComponent buffer, BufferSensorComponent otherShopBuffer) : base(agent, new []{buffer, otherShopBuffer})
        {
            _shopCraftingSystem = system;
            CanViewConstant = true;
        }
    }
}
