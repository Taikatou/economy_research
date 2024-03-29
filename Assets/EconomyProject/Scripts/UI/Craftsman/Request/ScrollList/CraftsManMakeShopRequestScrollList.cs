﻿using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.UI.Craftsman.Request.Buttons;
using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman.Request.ScrollList
{
    public class CraftingResourceUi
    {
        public readonly ECraftingResources ResourceType;
        public readonly int InventoryNumber;
		public readonly Sprite Icon;

        public CraftingResourceUi (ECraftingResources resourceType, int inventoryNumber, Sprite icon)
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
            throw new System.NotImplementedException();
        }

        protected override void Update()
        {
            base.Update();
            
            var resource = requestShopSystem.shopMakeCraftingRequestLocationMap.GetItem(CraftsmanAgent);
            if (resource.HasValue)
            {
                foreach (var button in buttons)
                {
                    button.UpdateData(resource.Value);
                }   
            }
        }
    }
}
