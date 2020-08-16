using System;
using System.Collections.Generic;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.TravelSystem
{
    public enum BattleEnvironments {Forest, Mountain, Sea, Volcano}
    
    [Serializable]
    public class EnvironmentBattleLootBox
    {
        public BattleLootBox lootBox;
        
        public BattleEnvironments environment;
    }
    public class TravelSubSystem : MonoBehaviour
    {
        public List<EnvironmentBattleLootBox> environmentBattleLootBoxes;
        private Dictionary<BattleEnvironments, BattleLootBox> _environmentLootTable;

        public void Start()
        {
            _environmentLootTable = new Dictionary<BattleEnvironments, BattleLootBox>();
            foreach (var item in environmentBattleLootBoxes)
            {
                item.lootBox.ValidateTable();
                _environmentLootTable.Add(item.environment, item.lootBox);
            }
        }

        public FighterUnit GetBattle(BattleEnvironments environment)
        {
            if (_environmentLootTable.ContainsKey(environment))
            {
                var battle = _environmentLootTable[environment].PickLootDropItem();
                return battle.item;
            }
            return null;
        }
    }
}
