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

		public Dictionary<string, int> defaultDurability;
		public Dictionary<string, int> defaultDamage;

		public override void SetupItems()
		{
			_items = shopSubSystem.system.shopSubSubSystem.basePrices;
			defaultDurability = ItemData.BaseDurabilities;
			defaultDamage = ItemData.BaseDamages;
		}


		public override void SetupList()
		{
			if(_items == null)
			{
				Debug.Log("basePrices null");
				return;
			}
			foreach (var itemDefault in _items)
			{
				GameObject newItem = item;
				newItem.GetComponent<ConfigItem>().Setup(itemDefault.item, itemDefault.price);
				Instantiate(newItem, holder.transform);
			}
		}

		public override void SetItem(UsableItem itemToModify, int newValue, string category = null)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i].item == itemToModify)
				{
					switch (category)
					{
						case null:
							_items[i] = new BaseItemPrices
							{
								item = itemToModify,
								price = newValue
							};
							break;
						case "Durability":
							if(defaultDurability.ContainsKey(itemToModify.itemDetails.itemName) == false)
							{
								Debug.Log(itemToModify.itemDetails.itemName);
							}
							defaultDurability[itemToModify.itemDetails.itemName] = newValue;
							break;
						case "Damage":
							if (defaultDamage.ContainsKey(itemToModify.itemDetails.itemName) == false)
							{
								Debug.Log(itemToModify.itemDetails.itemName);
							}
							defaultDamage[itemToModify.itemDetails.itemName] = newValue;
							break;
						default:
							Debug.Log("Wrong category : " + category);
							break;
					}

					return;
				}
			}
		}

		public Dictionary<string,int> GetDefaultDamages()
		{
			return defaultDamage;
		}
		public Dictionary<string, int> GetDefaultDurabilities()
		{
			return defaultDurability;
		}
	}
}
