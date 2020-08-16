using System;
using EconomyProject.Scripts.Inventory.LootBoxes;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;

namespace TurnBased.Scripts
{
    [Serializable]
    public class GeneratedFighterScriptableObject : GenericLootDropItem<CraftingResources> { }
    
    [Serializable]
    public class FighterDropTable :  GenericLootDropTable<GeneratedFighterScriptableObject, CraftingResources>
    {
    }
}
