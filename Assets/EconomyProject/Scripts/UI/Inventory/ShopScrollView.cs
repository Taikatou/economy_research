using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using Inventory;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Inventory
{
   
    public class ShopScrollView : AbstractScrollList<ShopItemUi, ShopInventoryScrollButton>
    {
        public ShopCraftingSystemBehaviour shopSubSystem;
        public GetCurrentShopAgent shopAgent;
        protected override ILastUpdate LastUpdated => shopSubSystem.system.shopSubSubSystem;
        // Update is called once per frame
        protected override List<ShopItemUi> GetItemList()
        {
            var toReturn = new List<ShopItemUi>();
            var items = shopSubSystem.system.shopSubSubSystem.GetShopItems(shopAgent.CurrentAgent);
            foreach (var item in items)
            {
                toReturn.Add(new ShopItemUi
                {
                    Item = item,
                    Price = shopSubSystem.system.shopSubSubSystem.GetPrice(shopAgent.CurrentAgent, item.itemDetails),
                    Number = shopSubSystem.system.shopSubSubSystem.GetNumber(shopAgent.CurrentAgent, item.itemDetails)
                });
            }

            return toReturn;
        }

        public override void SelectItem(ShopItemUi item, int number = 1)
        {
			UnityEngine.Debug.Log("Remove from shop : " + item.Item.itemDetails.itemName);
            throw new System.NotImplementedException();
        }

		public void IncreasePrice(ShopItemUi item)
		{
			int index = GetIndexInShopList(item.Item);
			if(index == -1)
			{
				return;
			}
			shopSubSystem.system.SetChoice(shopAgent.CurrentAgent, (int)CraftingInput.IncreasePrice);
			shopSubSystem.system.MakeChoiceSetPrice(shopAgent.CurrentAgent, index);
		}

		public void DecreasePrice(ShopItemUi item)
		{
			int index = GetIndexInShopList(item.Item);
			if (index == -1)
			{
				return;
			}
			shopSubSystem.system.SetChoice(shopAgent.CurrentAgent, (int)CraftingInput.DecreasePrice);
			shopSubSystem.system.MakeChoiceSetPrice(shopAgent.CurrentAgent, index);
		}


		public int GetIndexInShopList(UsableItem item)
		{
			var items = shopSubSystem.system.shopSubSubSystem.GetShopItems(shopAgent.CurrentAgent);
			for (int i = 0; i < items.Count; i++)
			{
				if(item == items[i])
				{
					return i;
				}
			}

			Debug.LogError("Item not in shop : " + item.itemDetails.itemName);
			return -1;
		}
	}
}
