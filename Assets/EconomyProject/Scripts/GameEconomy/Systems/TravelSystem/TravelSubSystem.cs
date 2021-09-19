using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.TravelSystem
{
    public enum EBattleEnvironments {Forest = EAdventurerAgentChoices.AForest, Mountain=EAdventurerAgentChoices.AMountain, Sea = EAdventurerAgentChoices.ASea, Volcano=EAdventurerAgentChoices.AVolcano}
    
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
            if (_environmentLootTable.ContainsKey(environment))
            {
                var battle = _environmentLootTable[environment].PickLootDropItem();

                var newFighter = FighterObject.GenerateItem(battle.item);
                return newFighter;
            }
            return null;
        }
    }
}
