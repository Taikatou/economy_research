using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using Unity.MLAgents;

namespace EconomyProject.Scripts.UI.Craftsman.Crafting
{
    public class CraftsmanCraftingScrollList : AbstractScrollList<CraftingInfo, CraftingRequestButton>
    {
        public GetCurrentShopAgent getCurrentAgent;
        private ShopAgent Agent => getCurrentAgent.CurrentAgent;
        public override LastUpdate LastUpdated => GetComponent<CraftingLastUpdate>();

        public override List<CraftingInfo> ItemList
        {
            get
            {
                var itemList = new List<CraftingInfo>();
                foreach (var item in Agent.CraftingAbility.craftingRequirement)
                {
                    var craftInfo = new CraftingInfo(item, Agent.CraftingInventory);
                    itemList.Add(craftInfo);
                }
                return itemList;
            }
        }
        public override void SelectItem(CraftingInfo item, int number = 1)
        {
            Agent.CraftingAbility.SetCraftingItem(item.craftingMap.choice);
            LastUpdated.Refresh();
        }
    }
}
