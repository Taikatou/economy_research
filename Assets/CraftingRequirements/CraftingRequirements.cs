using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Inventory;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Craftsman.Requirements
{
    public static class CraftingUtils
    {
        public static List<ECraftingResources> GetCraftingResources()
        {
            var resources = Enum.GetValues(typeof(ECraftingResources)).Cast<ECraftingResources>();
            var list = resources.ToList();
            list.Remove(ECraftingResources.Nothing);
            return list;
        }
    }

    public enum ECraftingResources { Nothing, Wood, Metal, Gem, DragonScale }

    [Serializable]
    public struct ResourceRequirement
    {
        public ECraftingResources type;
        public int number;

		public ResourceRequirement(ECraftingResources newType, int newNumber)
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

        public float TimeToCreation => TrainingConfig.IgnoreCraftTime? 0 : timeToCreation;

        public List<ResourceRequirement> resourcesRequirements = new();

        public string ResultingItemName => resultingItem ? resultingItem.itemDetails.itemName : "";
    }
}
