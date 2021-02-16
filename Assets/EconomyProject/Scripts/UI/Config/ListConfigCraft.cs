using System.Collections.Generic;
using UnityEngine;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using Inventory;
using System;

namespace EconomyProject.Scripts.UI.Config
{
	public class ListConfigCraft : AbstractList<List<CraftingMap>, UsableItem>
	{
		public ShopCraftingSystemBehaviour shopCraftingSystemBehaviour;

		//To Take From Elsewhere?
		public Dictionary<CraftingChoice, List<ResourceRequirement>> _defaultRequirements = new Dictionary<CraftingChoice, List<ResourceRequirement>>
		{
			{CraftingChoice.BeginnerSword, new List<ResourceRequirement>{
				new ResourceRequirement(CraftingResources.Wood, 2),
				new ResourceRequirement(CraftingResources.Metal, 2)}},
			{CraftingChoice.IntermediateSword, new List<ResourceRequirement>{
				new ResourceRequirement(CraftingResources.Wood, 3),
				new ResourceRequirement(CraftingResources.Metal, 3)}},
			{CraftingChoice.AdvancedSword, new List<ResourceRequirement>{
				new ResourceRequirement(CraftingResources.Wood, 2),
				new ResourceRequirement(CraftingResources.Metal, 2),
				new ResourceRequirement(CraftingResources.Gem, 1)}},
			{CraftingChoice.EpicSword, new List<ResourceRequirement>{
				new ResourceRequirement(CraftingResources.Wood, 2),
				new ResourceRequirement(CraftingResources.Metal, 2),
				new ResourceRequirement(CraftingResources.Gem, 2)}},
			{CraftingChoice.MasterSword, new List<ResourceRequirement>{
				new ResourceRequirement(CraftingResources.Wood, 3),
				new ResourceRequirement(CraftingResources.Metal, 3),
				new ResourceRequirement(CraftingResources.Gem, 3)}},
			{CraftingChoice.UltimateSwordOfPower, new List<ResourceRequirement>{
				new ResourceRequirement(CraftingResources.Wood, 3),
				new ResourceRequirement(CraftingResources.Metal, 3),
				new ResourceRequirement(CraftingResources.Gem, 3),
				new ResourceRequirement(CraftingResources.DragonScale, 2)}}
		};

		public override void SetupItems()
		{
			_items = shopCraftingSystemBehaviour.system.craftingSubSubSystem.craftingRequirement;

			InitRequirement();
		}

		public override void SetupList()
		{
			if (_items == null)
			{
				Debug.Log("ListResources null");
				return;
			}

			foreach (var resource in _items)
			{
				GameObject newResource = item;
				newResource.GetComponent<ConfigCraft>().Setup(resource.resource.resultingItem, 0);
				Instantiate(newResource, holder.transform);
			}
		}

		public override void SetItem(UsableItem itemToModify, int newPrice, string category = null)
		{
			//Not appropriate
		}

		/// <summary>
		/// Initialise the requirement at the beginning of the game
		/// </summary>
		public void InitRequirement()
		{
			foreach(var itemRequirements in _defaultRequirements)
			{
				SetListRequirements(GetUsableItemByCraftingChoice(itemRequirements.Key), itemRequirements.Value);
			}
		}

		/// <summary>
		/// Return UsableItem with a CraftingChoice
		/// </summary>
		public UsableItem GetUsableItemByCraftingChoice(CraftingChoice craftingChoice)
		{
			String nameToCheck = "";
			switch (craftingChoice)
			{
				case CraftingChoice.BeginnerSword:
					nameToCheck = "Beginner Sword";
					break;
				case CraftingChoice.IntermediateSword:
					nameToCheck = "Intermediate Sword";
					break;
				case CraftingChoice.AdvancedSword:
					nameToCheck = "Advanced Sword";
					break;
				case CraftingChoice.EpicSword:
					nameToCheck = "Epic Sword";
					break;
				case CraftingChoice.MasterSword:
					nameToCheck = "Master Sword";
					break;
				case CraftingChoice.UltimateSwordOfPower:
					nameToCheck = "Ultimate Sword";
					break;
				default:
					Debug.Log("Wrong craftingChoice : " + craftingChoice);
					break;
			}

			List<BaseItemPrices> basePrices = shopCraftingSystemBehaviour.system.shopSubSubSystem.basePrices;
			foreach (BaseItemPrices item in basePrices)
			{
				if (item.item.itemDetails.itemName == nameToCheck)
				{
					return item.item;
				}
			}

			Debug.Log("Not found the item : " + nameToCheck);
			return null;
		}

		/// <summary>
		/// Replace SetItem
		/// </summary>
		public void SetListRequirements(UsableItem itemToModify, List<ResourceRequirement> listRequirements)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i].resource.resultingItem == itemToModify)
				{
					_items[i].resource.resourcesRequirements = listRequirements;
					return;
				}
			}
		}
	}
}
