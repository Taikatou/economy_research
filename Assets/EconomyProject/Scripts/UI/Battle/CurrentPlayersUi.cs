using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using TurnBased.Scripts;

namespace EconomyProject.Scripts.UI.Battle
{
    public struct FightingData
    {
        public PlayerFighterData PlayerFighterData;
        public BattleSubSystemInstance<AdventurerAgent> BattleSubSystem;
    }
    public class CurrentPlayersUi : AbstractScrollList<FightingData,PlayerTurnSampleButton>
    {
        public AdventurerSystemBehaviour adventurerSystem;
        public GetCurrentAdventurerAgent adventurerAgent;

        protected override ILastUpdate LastUpdated => GetAdventurerBattleSubsystem();

        private BattleSubSystemInstance<AdventurerAgent> GetAdventurerBattleSubsystem()
        {
            var agent = adventurerAgent.CurrentAgent;
            var system = adventurerSystem.system.GetAdventureStates(agent);
            if (system == EAdventureStates.InBattle)
            {
                return adventurerSystem.system.battleSubSystem.GetSubSystem(agent);
            }

            return null;
        }

        protected override List<FightingData> GetItemList()
        {
            var toReturn = new List<FightingData>();
            var system = GetAdventurerBattleSubsystem();
            foreach (var p in system.PlayerFighterUnits.FighterUnits)
            {
                var item = new FightingData()
                {
                    PlayerFighterData = p,
                    BattleSubSystem = system
                };
                toReturn.Add(item);
            }
            return toReturn;
        }
        
        public override void SelectItem(FightingData item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
