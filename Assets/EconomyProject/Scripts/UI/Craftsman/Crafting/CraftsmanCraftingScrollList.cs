using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using Unity.MLAgents;

namespace EconomyProject.Scripts.UI.Craftsman.Crafting
{
    public class CraftsmanCraftingScrollList : AbstractScrollList<CraftingInfo, CraftingRequestButton>
    {
        public ShopCraftingSystemBehaviour shopCraftingSystem;
        public GetCurrentShopAgent getCurrentAgent;
        private ShopAgent Agent => getCurrentAgent.CurrentAgent;
        protected override ILastUpdate LastUpdated => GetComponent<CraftingLastUpdate>();

        protected override List<CraftingInfo> GetItemList()
        {
            var itemList = new List<CraftingInfo>();
            foreach (var item in shopCraftingSystem.system.craftingSubSubSystem.craftingRequirement)
            {
                var craftInfo = new CraftingInfo(item, Agent.craftingInventory);
                itemList.Add(craftInfo);
            }
            return itemList;
        }
        public override void SelectItem(CraftingInfo item, int number = 1)
        {
			Agent.SetAction(EShopAgentChoices.CraftItem, null, item);

            LastUpdated.Refresh();
        }
    }
}
