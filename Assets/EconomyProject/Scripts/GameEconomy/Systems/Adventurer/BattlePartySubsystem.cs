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
        public SetupNewBattle SetupNewBattle;
        public SetupNewBattle AskConfirmation;
        public SetupNewBattle AskConfirmAbilities;
        public CancelAgent CancelAgent;

        private TravelSubSystem _travelSubsystem;
        private EBattleEnvironments _environment;
        private SimpleMultiAgentGroup _agentGroup;
        private HashSet<AdventurerAgent> _confirmedAgents;

        // Start is called before the first frame update
        public BattlePartySubsystem(int partySize, EBattleEnvironments environment, TravelSubSystem travelSubsystem) : base(partySize)
        {
            _environment = environment;
            _travelSubsystem = travelSubsystem;
            confirmAbilities = new ConfirmAbilities();
        }

        public override void CompleteParty(SimpleMultiAgentGroup agentGroup)
        {
            _agentGroup = agentGroup;
            _confirmedAgents = new HashSet<AdventurerAgent>();
            AskConfirmation.Invoke(PendingAgents.ToArray(), null, agentGroup);
        }

        public void StartBattle()
        {
            Debug.Log("StartBattle");
            var fighter = _travelSubsystem.GetBattle(_environment);

            if (fighter)
            {
                SetupNewBattle.Invoke(PendingAgents.ToArray(), fighter, _agentGroup);
            }

            base.CompleteParty(_agentGroup);
        }

        public void ConfirmAbilities()
        {
            _confirmedAgents.Clear();
        }

        public void Confirmation(EConfirmBattle confirmation, AdventurerAgent agent)
        {
            if (confirmation == EConfirmBattle.Confirm)
            {
                if (!_confirmedAgents.Contains(agent))
                {
                    _confirmedAgents.Add(agent);
                    if (_confirmedAgents.Count == PendingAgents.Count)
                    {
                        AskConfirmAbilities.Invoke(PendingAgents.ToArray(), null, _agentGroup);
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
        
        public readonly ConfirmAbilities confirmAbilities;
    }
}
