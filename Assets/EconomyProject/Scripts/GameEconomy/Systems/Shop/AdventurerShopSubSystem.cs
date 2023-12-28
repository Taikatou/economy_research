using System;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using Inventory;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public class AdventurerShopSubSystem : LocationSelect<Agent>
    {
        public ShopCraftingSystemBehaviour shopCraftingSystem;
        
        public static readonly int SensorCount = AgentShopSubSystem.SensorCount;

        public override int GetLimit(Agent agent)
        {
            var shopItems = shopCraftingSystem.system.shopSubSubSystem.GetAllUsableItems();
            return shopItems.Count;
        }
        
        public void PurchaseItem<T>(T agent, ECraftingChoice craftingChoice) where T : BaseAdventurerAgent
        {
            var item = shopCraftingSystem.system.shopSubSubSystem.GetLowestPriceOfItem(craftingChoice);
            if (item != null)
            {
                shopCraftingSystem.system.shopSubSubSystem.PurchaseItem(item.Item2, item.Item1, agent.Wallet, agent.Inventory);    
            }
        }

        public void GetObservations(BufferSensorComponent bufferSensorComponent)
        {
            shopCraftingSystem.system.shopSubSubSystem.GetItemSenses(bufferSensorComponent, null);
        }
    }
}
