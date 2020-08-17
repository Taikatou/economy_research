using System;
using EconomyProject.Scripts.Inventory.LootBoxes;
using TurnBased.Scripts;

namespace EconomyProject.Scripts.GameEconomy.Systems.TravelSystem
{
    [Serializable]
    public class GeneratedBattleScriptableObject : GenericLootDropItem<FighterObject> { }
    
    [Serializable]
    public class BattleLootBox : GenericLootDropTable<GeneratedBattleScriptableObject, FighterObject>
    {
        
    }
}
