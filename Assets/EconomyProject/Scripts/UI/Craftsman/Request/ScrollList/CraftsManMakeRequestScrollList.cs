using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public class CraftingResourceUi
    {
        public readonly CraftingResources ResourceType;
        public readonly int InventoryNumber;

        public CraftingResourceUi (CraftingResources resourceType, int inventoryNumber)
        {
            ResourceType = resourceType;
            InventoryNumber = inventoryNumber;
        }
    }
    public class CraftsManMakeRequestScrollList : CraftingScrollList<CraftingResourceUi, CraftingMakeRequestButton>
    {
        public GetCurrentShopAgent getCurrentAgent;

        private ShopAgent CraftsmanAgent => getCurrentAgent.CurrentAgent;

        // Start is called before the first frame update
        protected override List<CraftingResourceUi> GetItemList()
        {
            if (CraftsmanAgent)
            {
                var items = new List<CraftingResourceUi>();
                var resources = CraftingUtils.CraftingResources;
                foreach (var resource in resources)
                {
                    var inventoryNumber = CraftsmanAgent.CraftingInventory.GetResourceNumber(resource);
                    var resourceUi = new CraftingResourceUi(resource, inventoryNumber);
                    items.Add(resourceUi);
                }
                return items;
            }
            return null;
        } 

        public override void SelectItem(CraftingResourceUi item, int number = 1)
        {
            requestSystem.MakeChoice(CraftsmanAgent, item.ResourceType);
        }
    }
}
