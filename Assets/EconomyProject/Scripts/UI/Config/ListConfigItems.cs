using System.Collections.Generic;
using UnityEngine;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using Inventory;

namespace EconomyProject.Scripts.UI.Config
{
	public class ListConfigItems : AbstractList<List<BaseItemPrices>, UsableItem>
	{
		public ShopCraftingSystemBehaviour shopSubSystem;

		public override void SetupItems()
		{
			_items = shopSubSystem.system.shopSubSubSystem.basePrices;
		}


		public override void SetupList()
		{
			foreach (var itemDefault in _items)
			{
				GameObject newItem = item;
				newItem.GetComponent<ConfigItem>().Setup(itemDefault.item, itemDefault.price);
				Instantiate(newItem, holder.transform);
			}
		}

		public override void SetItem(UsableItem itemToModify, int newPrice)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i].item == itemToModify)
				{
					_items[i] = new BaseItemPrices{
						item = itemToModify,
						price = newPrice
					};

					return;
				}
			}
		}
	}
}
