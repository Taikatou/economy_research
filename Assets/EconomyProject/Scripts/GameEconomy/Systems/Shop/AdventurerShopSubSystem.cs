using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public class AdventurerShopSubSystem : LocationSelect<AdventurerAgent>
    {
        public ShopCraftingSystemBehaviour shopCraftingSystem;
        public ShopChooserSubSystem shopChooserSubSystem;

        protected override int GetLimit(AdventurerAgent agent)
        {
            var shopAgent = shopChooserSubSystem.GetCurrentShop(agent);
            var shopItems = shopCraftingSystem.system.shopSubSubSystem.GetShopItems(shopAgent);
            return shopItems.Count;
        }

        public void PurchaseItem(AdventurerAgent agent)
        {
            var shopAgent = shopChooserSubSystem.GetCurrentShop(agent);
            var shopItems = shopCraftingSystem.system.shopSubSubSystem.GetShopItems(shopAgent);
            if (currentLocation[agent] < shopItems.Count)
            {
                var shopDetails = shopItems[currentLocation[agent]].itemDetails;
                shopCraftingSystem.system.shopSubSubSystem.PurchaseItem(shopAgent, shopDetails, agent.wallet, agent.inventory);
            }
        }

        public float[] GetObservations(AdventurerAgent agent)
        {
            var shop = shopChooserSubSystem.GetCurrentShop(agent);
            var senseA = shopCraftingSystem.system.shopSubSubSystem.GetSenses(shop);
            var output = new float [1 + AgentShopSubSystem.SenseCount + shopChooserSubSystem.SenseCount];
            output[0] = currentLocation[agent];
            senseA.CopyTo(output, 1);

            var senseB = shopChooserSubSystem.GetObservations(agent);
            senseB.CopyTo(output, 1 + AgentShopSubSystem.SenseCount);

            return output;
        }
    }
}
