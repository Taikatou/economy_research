using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.Inventory;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.Craftsman.Requirements
{
    public static class CraftingUtils
    {
        public static IEnumerable<CraftingResources> CraftingResources => Enum.GetValues(typeof(CraftingResources)).Cast<CraftingResources>();
    }
    public enum CraftingResources { Wood, Metal, Gem, DragonScale }

    [Serializable]
    public struct ResourceRequirement
    {
        public CraftingResources type;
        public int number;
    }

    [CreateAssetMenu]
    public class CraftingRequirements : ScriptableObject
    {
        public InventoryItem resultingItem;

        public float timeToCreation = 3;

        public List<ResourceRequirement> resourcesRequirements = new List<ResourceRequirement>();

        public string ResultingItemName => resultingItem ? resultingItem.itemName : "";
    }
}
