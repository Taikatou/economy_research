using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman.Crafting
{
    public class CraftsmanCraftingScrollList : AbstractScrollList<CraftingInfo, CraftingRequestButton>
    {
        public CraftingRequestLocationMap craftingRequestLocationMap;
        public ShopCraftingSystemBehaviour shopCraftingSystem;
        public GetCurrentShopAgent getCurrentAgent;
        private ShopAgent Agent => getCurrentAgent.CurrentAgent;
        protected override ILastUpdate LastUpdated => GetComponent<CraftingLastUpdate>();

        protected override List<CraftingInfo> GetItemList()
        {
            var itemList = new List<CraftingInfo>();
            var shopItems = shopCraftingSystem.system.craftingSubSubSystem.craftingRequirement;
            foreach (var item in shopItems)
            {
                var craftInfo = new CraftingInfo(item, Agent.craftingInventory);
                itemList.Add(craftInfo);
            }
            return itemList;
        }
        public override void SelectItem(CraftingInfo item, int number = 1)
        {
			Agent.SetAction(EShopAgentChoices.Craft, null, item.craftingMap.choice);

            LastUpdated.Refresh();
        }

        public void FixedUpdate()
        {
            var resource = craftingRequestLocationMap.GetCraftingChoice(Agent);
            var system = shopCraftingSystem.system.GetState(Agent);
            foreach (var button in buttons)
            {
                button.UpdateData(resource, system);
            }
        }
    }
}
