﻿using System.Collections.Generic;
using UnityEngine;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;

namespace EconomyProject.Scripts.UI.Config
{
	public class ListConfigResources : AbstractList<Dictionary<CraftingResources, int>, CraftingResources>
	{
		public RequestShopSystemBehaviour requestShopSystemBehaviour;

		public override void SetupItems()
		{
			_items = requestShopSystemBehaviour.system.requestSystem.defaultResourcePrices;
		}

		public override void SetupList()
		{
			if(_items == null)
			{
				Debug.Log("ListResources null");
				return;
			}

			foreach (var resource in _items)
			{
				GameObject newResource = item;
				newResource.GetComponent<ConfigResource>().Setup(resource.Key, resource.Value);
				Instantiate(newResource, holder.transform);
			}
		}

		public override void SetItem(CraftingResources resourceToModify, int newPrice, string category = null)
		{
			if (_items.ContainsKey(resourceToModify))
			{
				_items[resourceToModify] = newPrice;
			}
			else
			{
				Debug.LogWarning("Wrong key resource : " + resourceToModify.ToString());
			}
			
		}
	}
}
