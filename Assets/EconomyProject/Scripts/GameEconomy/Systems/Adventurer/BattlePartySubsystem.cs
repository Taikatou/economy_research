using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public delegate void CancelAgent(AdventurerAgent agent);
    public delegate void SetupNewBattle(AdventurerAgent[] agent, FighterObject enemyFighter, SimpleMultiAgentGroup party);
    
    [Serializable]
    public class BattlePartySubsystem : PartySubSystem<AdventurerAgent>
    {
        private TravelSubSystem _travelSubsystem;

        public SetupNewBattle SetupNewBattle;

        public SetupNewBattle AskConfirmation;

        private EBattleEnvironments _environment;

        private SimpleMultiAgentGroup _agentGroup;

        public CancelAgent CancelAgent;
        
        // Start is called before the first frame update
        public BattlePartySubsystem(int partySize, EBattleEnvironments environment, TravelSubSystem travelSubsystem) : base(partySize)
        {
            _environment = environment;
            _travelSubsystem = travelSubsystem;
        }

        public override void CompleteParty(SimpleMultiAgentGroup agentGroup)
        {
            _agentGroup = agentGroup;
            _confirmedAgents = new HashSet<AdventurerAgent>();
            AskConfirmation.Invoke(PendingAgents.ToArray(), null, agentGroup);
        }

        public void StartBattle(EBattleEnvironments battleEnvironments)
        {
            Debug.Log("StartBattle");
            var fighter = _travelSubsystem.GetBattle(battleEnvironments);

            if (fighter)
            {
                SetupNewBattle.Invoke(PendingAgents.ToArray(), fighter, _agentGroup);
            }

            base.CompleteParty(_agentGroup);
        }
        
        private HashSet<AdventurerAgent> _confirmedAgents;

        public void Confirmation(EConfirmBattle confirmation, AdventurerAgent agent)
        {
            if (confirmation == EConfirmBattle.Confirm)
            {
                if (!_confirmedAgents.Contains(agent))
                {
                    _confirmedAgents.Add(agent);
                    if (_confirmedAgents.Count == PendingAgents.Count)
                    {
                        StartBattle(_environment);
                    }   
                }
            }
            else
            {
                var pArray = PendingAgents.ToArray();
                foreach (var a in pArray)
                {
                    CancelAgent.Invoke(a);
                }
            }
        }
    }
}
