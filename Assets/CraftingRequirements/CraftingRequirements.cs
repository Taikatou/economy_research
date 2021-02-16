using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Craftsman.Requirements
{
    public static class CraftingUtils
    {
        public static List<CraftingResources> GetCraftingResources()
        {
            var resources = Enum.GetValues(typeof(CraftingResources)).Cast<CraftingResources>();
            var list = resources.ToList();
            list.Remove(CraftingResources.Nothing);
            return list;
        }
    }

    public enum CraftingResources { Nothing, Wood, Metal, Gem, DragonScale }

    [Serializable]
    public struct ResourceRequirement
    {
        public CraftingResources type;
        public int number;

		public ResourceRequirement(CraftingResources newType, int newNumber)
		{
			type = newType;
			number = newNumber;
		}
	}

    [CreateAssetMenu]
    public class CraftingRequirements : ScriptableObject
    {
        public UsableItem resultingItem;

        public float timeToCreation = 3;

        public List<ResourceRequirement> resourcesRequirements = new List<ResourceRequirement>();

        public string ResultingItemName => resultingItem ? resultingItem.itemDetails.itemName : "";
    }
}
