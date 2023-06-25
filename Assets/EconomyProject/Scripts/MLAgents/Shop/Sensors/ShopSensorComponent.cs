using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopSensorComponent : SensorComponent
    {
        public BufferSensorComponent craftingBufferComp;
        public BufferSensorComponent requestBufferComp;
        public ShopAgent shopAgent;
        public override ISensor[] CreateSensors()
        {
            var shopCraftingSystem = FindObjectOfType<ShopCraftingSystemBehaviour>();
            var request = FindObjectOfType<RequestShopSystemBehaviour>();
            var configSystem = FindObjectOfType<ConfigSystem>();
            var locationSelect = FindObjectOfType<ShopMainLocationSelect>();

            return new ISensor[]
            {
                new ShopCraftingSensor(shopAgent, shopCraftingSystem.system, craftingBufferComp),
                new ShopRequestSensor(shopAgent, request.system, requestBufferComp),
                new ShopBaseSensor(shopAgent, locationSelect),
                new ConfigSensor(configSystem),
                new CraftingSensor(shopAgent, shopCraftingSystem.system.craftingSubSubSystem, null)
            };
        }
    }
}
