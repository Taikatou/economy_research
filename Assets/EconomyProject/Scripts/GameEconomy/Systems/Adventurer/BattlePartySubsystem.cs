using System;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public delegate void SetupNewBattle(AdventurerAgent[] agent, FighterObject enemyFighter, SimpleMultiAgentGroup party);
    
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

        public override void CompleteParty(SimpleMultiAgentGroup agentGroup)
        {
            StartBattle(_environment, agentGroup);
            
            base.CompleteParty(agentGroup);
        }
        
        public void StartBattle(EBattleEnvironments battleEnvironments, SimpleMultiAgentGroup agentGroup)
        {
            var fighter = _travelSubsystem.GetBattle(battleEnvironments);

            if (fighter)
            {
                setupNewBattle?.Invoke(_pendingAgents.ToArray(), fighter, agentGroup);
            }
        }
    }
}
