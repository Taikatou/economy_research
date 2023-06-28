using System;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public class AdventurerShopSubSystem : LocationSelect<AdventurerAgent>
    {
        public ShopCraftingSystemBehaviour shopCraftingSystem;
        
        public static readonly int SensorCount = AgentShopSubSystem.SensorCount;

        public override int GetLimit(AdventurerAgent agent)
        {
            var shopItems = shopCraftingSystem.system.shopSubSubSystem.GetAllUsableItems();
            return shopItems.Count;
        }

        public void PurchaseItem(AdventurerAgent agent)
        {
            var shopItems = shopCraftingSystem.system.shopSubSubSystem.GetAllUsableItems();
            
            if (!CurrentLocation.ContainsKey(agent))
            {
                return;
            }
            
            var location = CurrentLocation[agent];
            if (location < shopItems.Count && location >= 0)
            {
                var item = shopItems[location];
                var canUse = agent.adventurerInventory.CanObtainItem(item.Item1);
                if (canUse)
                {
                    shopCraftingSystem.system.shopSubSubSystem.PurchaseItem(item.Item2, item.Item1, agent.wallet, agent.inventory);   
                }
            }
        }

        public void GetObservations(BufferSensorComponent bufferSensorComponent)
        {
            shopCraftingSystem.system.shopSubSubSystem.GetItemSenses(bufferSensorComponent, null);
        }
    }
}
