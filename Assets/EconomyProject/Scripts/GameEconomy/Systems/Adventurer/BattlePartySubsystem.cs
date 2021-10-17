using System;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public delegate void SetupNewBattle(AdventurerAgent agent, FighterObject enemyFighter);
    
    [Serializable]
    public class BattlePartySubsystem : PartySubSystem<AdventurerAgent>
    {
        private TravelSubSystem _travelSubsystem;

        public SetupNewBattle setupNewBattle;

        private EBattleEnvironments _environment;
        // Start is called before the first frame update
        public BattlePartySubsystem(int partySize, EBattleEnvironments environment, TravelSubSystem travelSubsystem) : base(partySize)
        {
            _environment = environment;
            _travelSubsystem = travelSubsystem;
        }

        public override void CompleteParty()
        {
            StartBattle(_environment);
            
            base.CompleteParty();
        }
        
        public void StartBattle(EBattleEnvironments battleEnvironments)
        {
            var fighter = _travelSubsystem.GetBattle(battleEnvironments);

            if (fighter)
            {
                foreach (var agent in _pendingAgents)
                {
                    setupNewBattle?.Invoke(agent, fighter);   
                }
            }
        }
    }
}