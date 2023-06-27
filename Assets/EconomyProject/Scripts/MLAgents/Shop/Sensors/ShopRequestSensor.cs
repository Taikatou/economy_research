using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopRequestSensor : AgentMovementSensor<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        private readonly RequestShopSystem _requestShopSystem;
        public override string GetName() => "ShopRequestSensor";
        protected override EShopScreen ValidScreen => EShopScreen.Request;
        protected override EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices> EconomySystem => _requestShopSystem;
        protected override int SensorCount => RequestShopSystem.ObservationSize;

        public ShopRequestSensor(ShopAgent agent, RequestShopSystem system, BufferSensorComponent buffer) : base(agent, buffer)
        {
            _requestShopSystem = system;
        }
    }
}
