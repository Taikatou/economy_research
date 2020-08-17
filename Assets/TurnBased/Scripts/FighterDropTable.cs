using System;
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
    }
    
    public struct CraftingDropReturn
    {
        public CraftingResources Resource;
        public int Count;
    }
}
