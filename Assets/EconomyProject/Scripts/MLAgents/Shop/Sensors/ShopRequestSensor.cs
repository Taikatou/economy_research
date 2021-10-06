using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopRequestSensor : AgentMovementSensor<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        private readonly RequestShopSystem requestShopSystem;
        public override string GetName() => "ShopRequestSensor";
        protected override EShopScreen ValidScreen => EShopScreen.Request;
        protected override EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices> EconomySystem => requestShopSystem;

        protected override int SensorCount { get; }

        public ShopRequestSensor(ShopAgent agent, RequestShopSystem system) : base(agent)
        {
            requestShopSystem = system;
        }
    }
}
