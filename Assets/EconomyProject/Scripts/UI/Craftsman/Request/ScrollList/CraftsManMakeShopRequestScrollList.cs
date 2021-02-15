using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public class CraftingResourceUi
    {
        public readonly CraftingResources ResourceType;
        public readonly int InventoryNumber;
		public readonly Sprite Icon;

        public CraftingResourceUi (CraftingResources resourceType, int inventoryNumber, Sprite icon)
        {
            ResourceType = resourceType;
            InventoryNumber = inventoryNumber;
			Icon = icon;
        }
    }

    public class CraftsManMakeShopRequestScrollList : ShopRequestScrollList<CraftingResourceUi, CraftingMakeRequestButton>
    {
        public RequestShopSystemBehaviour requestShopSystem;
        public GetCurrentShopAgent getCurrentAgent;
        private ShopAgent CraftsmanAgent => getCurrentAgent.CurrentAgent;

		// Start is called before the first frame update
		protected override List<CraftingResourceUi> GetItemList()
        {
            if (CraftsmanAgent)
            {
                var items = new List<CraftingResourceUi>();
                var resources = CraftingUtils.GetCraftingResources();
                foreach (var resource in resources)
                {
                    var inventoryNumber = CraftsmanAgent.craftingInventory.GetResourceNumber(resource);
					Sprite iconImage = requestShopSystem.system.requestSystem.GetIconByResource(resource);

					var resourceUi = new CraftingResourceUi(resource, inventoryNumber, iconImage);
                    items.Add(resourceUi);
                }
                return items;
            }
            return null;
        } 

        public override void SelectItem(CraftingResourceUi item, int number = 1)
        {
			CraftsmanAgent.SetAction(EShopAgentChoices.MakeResourceRequest, item);
		}
	}
}
