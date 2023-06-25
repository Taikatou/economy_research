using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public class AdventurerShopSubSystem : LocationSelect<AdventurerAgent>
    {
        public ShopCraftingSystemBehaviour shopCraftingSystem;
        public ShopChooserSubSystem shopChooserSubSystem;
        
        public static readonly int SensorCount = AgentShopSubSystem.SensorCount;

        public override int GetLimit(AdventurerAgent agent)
        {
            var shopAgent = shopChooserSubSystem.GetCurrentShop(agent);
            var shopItems = shopCraftingSystem.system.shopSubSubSystem.GetShopUsableItems(shopAgent);
            return shopItems.Count;
        }

        public void PurchaseItem(AdventurerAgent agent)
        {
            var shopAgent = shopChooserSubSystem.GetCurrentShop(agent);
            var shopItems = shopCraftingSystem.system.shopSubSubSystem.GetShopUsableItems(shopAgent);
            if (currentLocation[agent] < shopItems.Count)
            {
                var item = shopItems[currentLocation[agent]];
                var canUse = agent.adventurerInventory.CanObtainItem(item);
                if (canUse)
                {
                    shopCraftingSystem.system.shopSubSubSystem.PurchaseItem(shopAgent, item, agent.wallet, agent.inventory);   
                }
            }
        }

        public void GetObservations(AdventurerAgent agent, BufferSensorComponent bufferSensorComponent)
        {
            var shop = shopChooserSubSystem.GetCurrentShop(agent);
            shopCraftingSystem.system.shopSubSubSystem.GetItemSenses(shop, bufferSensorComponent);
        }
    }
}
