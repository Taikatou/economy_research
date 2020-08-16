using System;
using System.Collections.Generic;
using EconomyProject.Scripts.Inventory.LootBoxes;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;

namespace TurnBased.Scripts
{
    [Serializable]
    public class BattleDrop
    {
        public CraftingResources resource;

        public int minDrop;

        public int maxDrop;

        public int GenerateNumber()
        {
            var offset = maxDrop - minDrop;
            var generator = new Random();
            return minDrop + (int)(generator.NextDouble() * offset);
        }
    }
    
    [Serializable]
    public class GeneratedFighterScriptableObject : GenericLootDropItem<BattleDrop> { }
    
    [Serializable]
    public class FighterDropTable :  GenericLootDropTable<GeneratedFighterScriptableObject, BattleDrop>
    {
        public CraftingDropReturn GenerateItems()
        {
            var toReturn = new List<CraftingResources>();
            var item = PickLootDropItem().item;

            var count = item.GenerateNumber();
            return new CraftingDropReturn { Resource = item.resource, Count=count };
        }
    }
    
    public struct CraftingDropReturn
    {
        public CraftingResources Resource;
        public int Count;
    }
}
