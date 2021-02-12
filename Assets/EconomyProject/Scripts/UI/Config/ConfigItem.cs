using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Config
{
	public class ConfigItem : AbstractConfigComponent<List<BaseItemPrices>, UsableItem>
	{
		[Header("Durability")]
		public int durability;
		public Text durabilityText;
		public InputField inputTextDurability;

		[Header("Damage")]
		public int damage;
		public Text damageText;
		public InputField inputTextDamage;

		public override void SetupAbstractComponent()
		{
			abstractList = GameObject.FindObjectOfType<ListConfigItems>();
		}

		//Hide parent Setup() because has more field to fill in ConfigItem than the other components, then call the parent Setup function
		public new void Setup(UsableItem newItem, int newPrice)
		{
			base.Setup(newItem, newPrice);
			SetupOthersParameters();
		}

		public void SetupOthersParameters()
		{
			durability = ItemData.GetDefaultDurability(item.itemDetails.itemName);
			damage = ItemData.GetDefaultDamage(item.itemDetails.itemName);

			durabilityText.text = durability.ToString();
			damageText.text = damage.ToString();
		}

		/********************************************Durability**********************************/
		/// <summary>
		/// Increment Durability by +1
		/// </summary>
		public void IncreaseDurability()
		{
			durability++;
			SetDurability();
		}

		/// <summary>
		/// Increment price by -1. Minimum price = 1
		/// </summary>
		public void DecreaseDurability()
		{
			if (durability <= 1)
			{
				return;
			}

			durability--;
			SetDurability();
		}
		/// <summary>
		/// Set Durability in the inputField and call the ListConfigItems
		/// </summary>
		public void SetDurabilityInputField()
		{
			if (inputTextDurability.text == null || inputTextDurability.text == "")
			{
				inputTextDurability.text = durability.ToString();
				return;
			}
			else
			{
				durability = int.Parse(inputTextDurability.text);
			}
			SetDurability();
		}

		/// <summary>
		/// Set the Durability in the _itemPrices in ListConfigItems.cs
		/// </summary>
		public void SetDurability()
		{
			inputTextDurability.text = durability.ToString();
			abstractList.SetItem(item, durability, "Durability");
		}

		/********************************************Damage**********************************/
		/// <summary>
		/// Increment damage by +1
		/// </summary>
		public void IncreaseDamage()
		{
			damage++;
			SetDamage();
		}

		/// <summary>
		/// Increment damage by -1. Minimum damage = 1
		/// </summary>
		public void DecreaseDamage()
		{
			if (damage <= 1)
			{
				return;
			}

			damage--;
			SetDamage();
		}
		/// <summary>
		/// Set damage in the inputField and call the ListConfigItems
		/// </summary>
		public void SetDamageInputField()
		{
			if (inputTextDamage.text == null || inputTextDamage.text == "")
			{
				inputTextDamage.text = damage.ToString();
				return;
			}
			else
			{
				damage = int.Parse(inputTextDamage.text);
			}
			SetDamage();
		}

		/// <summary>
		/// Set the Durability in the _itemPrices in ListConfigItems.cs
		/// </summary>
		public void SetDamage()
		{
			inputTextDamage.text = damage.ToString();
			abstractList.SetItem(item, damage, "Damage");
		}
	}
}
