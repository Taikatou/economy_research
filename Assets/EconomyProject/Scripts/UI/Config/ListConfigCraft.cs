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
		public Dictionary<ECraftingChoice, List<ResourceRequirement>> _defaultRequirements = new Dictionary<ECraftingChoice, List<ResourceRequirement>>
		{
		/*	{ECraftingChoice.BeginnerSword, new List<ResourceRequirement>{
				new ResourceRequirement(ECraftingResources.Wood, 2),
				new ResourceRequirement(ECraftingResources.Metal, 2)}},
			{ECraftingChoice.IntermediateSword, new List<ResourceRequirement>{
				new ResourceRequirement(ECraftingResources.Wood, 3),
				new ResourceRequirement(ECraftingResources.Metal, 3)}},
			{ECraftingChoice.AdvancedSword, new List<ResourceRequirement>{
				new ResourceRequirement(ECraftingResources.Wood, 2),
				new ResourceRequirement(ECraftingResources.Metal, 2),
				new ResourceRequirement(ECraftingResources.Gem, 1)}},
			{ECraftingChoice.EpicSword, new List<ResourceRequirement>{
				new ResourceRequirement(ECraftingResources.Wood, 2),
				new ResourceRequirement(ECraftingResources.Metal, 2),
				new ResourceRequirement(ECraftingResources.Gem, 2)}},
			{ECraftingChoice.MasterSword, new List<ResourceRequirement>{
				new ResourceRequirement(ECraftingResources.Wood, 3),
				new ResourceRequirement(ECraftingResources.Metal, 3),
				new ResourceRequirement(ECraftingResources.Gem, 3)}},
			{ECraftingChoice.UltimateSwordOfPower, new List<ResourceRequirement>{
				new ResourceRequirement(ECraftingResources.Wood, 3),
				new ResourceRequirement(ECraftingResources.Metal, 3),
				new ResourceRequirement(ECraftingResources.Gem, 3),
				new ResourceRequirement(ECraftingResources.DragonScale, 2)}}`*/
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
		public UsableItem GetUsableItemByCraftingChoice(ECraftingChoice craftingChoice)
		{
			String nameToCheck = "";
	/*		switch (craftingChoice)
			{
				case ECraftingChoice.BeginnerSword:
					nameToCheck = "Beginner Sword";
					break;
				case ECraftingChoice.IntermediateSword:
					nameToCheck = "Intermediate Sword";
					break;
				case ECraftingChoice.AdvancedSword:
					nameToCheck = "Advanced Sword";
					break;
				case ECraftingChoice.EpicSword:
					nameToCheck = "Epic Sword";
					break;
				case ECraftingChoice.MasterSword:
					nameToCheck = "Master Sword";
					break;
				case ECraftingChoice.UltimateSwordOfPower:
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

			Debug.Log("Not found the item : " + nameToCheck);*/
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
