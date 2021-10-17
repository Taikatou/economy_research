using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public class AdventurerShopSubSystem : LocationSelect<AdventurerAgent>
    {
        public ShopCraftingSystemBehaviour shopCraftingSystem;
        public ShopChooserSubSystem shopChooserSubSystem;
        
        public static readonly int SensorCount = AgentShopSubSystem.SensorCount;

        protected override int GetLimit(AdventurerAgent agent)
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

        public ObsData[] GetObservations(AdventurerAgent agent)
        {
            var output = new List<ObsData>();

            var shop = shopChooserSubSystem.GetCurrentShop(agent);
            var senseA = shopCraftingSystem.system.shopSubSubSystem.GetItemSenses(shop);
            output.AddRange(senseA);

            return output.ToArray();
        }
    }
}
