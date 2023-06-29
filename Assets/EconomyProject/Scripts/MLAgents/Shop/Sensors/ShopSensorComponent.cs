using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Sensors;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Shop.Sensors
{
    public class ShopSensorComponent : SensorComponent
    {
        public BufferSensorComponent craftingBufferComp;
        public BufferSensorComponent requestBufferComp;
        public BufferSensorComponent ownShopBufferComp;
        public ShopAgent shopAgent;
        public override ISensor[] CreateSensors()
        {
            var shopCraftingSystem = FindObjectOfType<ShopCraftingSystemBehaviour>();
            var request = FindObjectOfType<RequestShopSystemBehaviour>();
            var locationSelect = FindObjectOfType<ShopMainLocationSelect>();

            return new ISensor[]
            {
                new ShopCraftingSensor(shopAgent, shopCraftingSystem.system, craftingBufferComp, ownShopBufferComp),
                new ShopRequestSensor(shopAgent, request.system, requestBufferComp),
                new CraftingSensor(shopAgent, shopCraftingSystem.system.craftingSubSubSystem),
                new ShopBaseSensor(shopAgent, locationSelect),
            };
        }
    }
}
