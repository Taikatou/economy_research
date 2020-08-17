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
            var generator = new System.Random();
            return generator.Next(minDrop, maxDrop);
        }

        public BattleDrop(BattleDrop original)
        {
            resource = original.resource;
            minDrop = original.minDrop;
            maxDrop = original.maxDrop;
        }
    }

    [Serializable]
    public class GeneratedFighterScriptableObject : GenericLootDropItem<BattleDrop>
    {
        public static GeneratedFighterScriptableObject Clone(GeneratedFighterScriptableObject original)
        {
            var fighterObject = new GeneratedFighterScriptableObject
            {
                item = original.item,
                probabilityWeight = original.probabilityWeight,
                probabilityPercent = original.probabilityPercent,
                probabilityRangeFrom = original.probabilityRangeFrom,
                probabilityRangeTo = original.probabilityRangeTo
            };
            return fighterObject;
        }
    }
    
    [Serializable]
    public class FighterDropTable :  GenericLootDropTable<GeneratedFighterScriptableObject, BattleDrop>
    {
        public CraftingDropReturn GenerateItems()
        {
            var item = PickLootDropItem().item;

            var count = item.GenerateNumber();
            return new CraftingDropReturn { Resource = item.resource, Count=count };
        }
        
        public static FighterDropTable CloneData(FighterDropTable original)
        {
            var clone = new FighterDropTable();
            foreach (var lootDrop in original.lootDropItems)
            {
                var newFighter = GeneratedFighterScriptableObject.Clone(lootDrop);
                clone.lootDropItems.Add(newFighter);
            }
            
            clone.ValidateTable();

            return clone;
        }
    }
    
    public struct CraftingDropReturn
    {
        public CraftingResources Resource;
        public int Count;
    }
}
