using System;
using System.Collections.Generic;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.TravelSystem
{
    public enum EBattleEnvironments { Forest, Mountain, Sea, Volcano }
    
    [Serializable]
    public class EnvironmentBattleLootBox
    {
        public BattleLootBox lootBox;
        
        public EBattleEnvironments environment;
    }
    public class TravelSubSystem : MonoBehaviour
    {
        public List<EnvironmentBattleLootBox> environmentBattleLootBoxes;
        private Dictionary<EBattleEnvironments, BattleLootBox> _environmentLootTable;

        public void Start()
        {
            _environmentLootTable = new Dictionary<EBattleEnvironments, BattleLootBox>();
            foreach (var item in environmentBattleLootBoxes)
            {
                item.lootBox.ValidateTable();
                _environmentLootTable.Add(item.environment, item.lootBox);
            }
        }

        public FighterObject GetBattle(EBattleEnvironments environment)
        {
            try
            {
                if (_environmentLootTable.ContainsKey(environment))
                {
                    var battle = _environmentLootTable[environment].PickLootDropItem();

                    var newFighter = FighterObject.GenerateItem(battle.item);
                    return newFighter;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public CraftingDropReturn? GetLootBox(EBattleEnvironments environment)
        {
            if (_environmentLootTable.ContainsKey(environment))
            {
                var fighter = GetBattle(environment);
                return fighter.fighterDropTable.GenerateItems();
            }
            return null;
        }
    }
}
