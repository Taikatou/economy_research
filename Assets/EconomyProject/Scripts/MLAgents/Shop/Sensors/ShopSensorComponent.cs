using EconomyProject.Monobehaviours;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopSensorComponent : SensorComponent
    {
        public ShopAgent shopAgent;
        public override ISensor[] CreateSensors()
        {
            var shopCraftingSystem = FindObjectOfType<ShopCraftingSystemBehaviour>();
            var request = FindObjectOfType<RequestShopSystemBehaviour>();
            var configSystem = FindObjectOfType<ConfigSystem>();
            return new ISensor[]
            {
                new ShopCraftingSensor(shopAgent, shopCraftingSystem.system),
                new ShopRequestSensor(shopAgent, request.system),
                new ShopBaseSensor(shopAgent),
                new ConfigSensor(configSystem)
            };
        }
    }
}
