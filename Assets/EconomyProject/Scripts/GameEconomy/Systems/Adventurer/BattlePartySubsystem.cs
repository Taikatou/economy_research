using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;
using TurnBased.Scripts.AI;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public delegate void CancelAgent(AdventurerAgent agent);
    public delegate void SetupNewBattle(AdventurerAgent[] agent, FighterObject enemyFighter, SimpleMultiAgentGroup party, Dictionary<AdventurerAgent, HashSet<EAttackOptions>> selectedOptions);

    [Serializable]
    public class BattlePartySubsystem : PartySubSystem<AdventurerAgent>
    {
        public int countDown = 10;
        public SetupNewBattle SetupNewBattle;
        public SetupNewBattle AskConfirmation;
        public SetupNewBattle AskConfirmAbilities;
        public CancelAgent CancelAgent;
        
        public ConfirmAbilities confirmAbilities { get; private set; }

        private TravelSubSystem _travelSubsystem;
        private EBattleEnvironments _environment;
        private SimpleMultiAgentGroup _agentGroup;
        private HashSet<AdventurerAgent> _confirmedAgents;

        private float _timer;
        private bool _timerActive = false;
        private bool _battleStarted;
        private static int _battleID = 0;

        private BattleEnvironmentDataLogger _dataLogger;

        // Start is called before the first frame update
        public BattlePartySubsystem(int partySize, EBattleEnvironments environment, TravelSubSystem travelSubsystem, BattleEnvironmentDataLogger dataLogger) : base(partySize)
        {
            _environment = environment;
            _travelSubsystem = travelSubsystem;
            _dataLogger = dataLogger;
            confirmAbilities = new ConfirmAbilities();
        }

        private void ResetTimer(int counter)
        {
            _timer = counter;
            _timerActive = true;
        }

        public override void CompleteParty(SimpleMultiAgentGroup agentGroup)
        {
            _agentGroup = agentGroup;
            _confirmedAgents = new HashSet<AdventurerAgent>();
            AskConfirmation.Invoke(PendingAgents.ToArray(), null, agentGroup, confirmAbilities.SelectedAttacks);
            ResetTimer(countDown);
        }

        public void StartBattle()
        {
            Debug.Log("StartBattle");
            var fighter = _travelSubsystem.GetBattle(_environment);

            if (fighter && !_battleStarted)
            {
                SetupNewBattle.Invoke(PendingAgents.ToArray(), fighter, _agentGroup, confirmAbilities.SelectedAttacks);
                _battleID++;
                foreach (var agent in PendingAgents)
                {
                    var environmentData = new EBattleEnvironmentSelection
                    {
                        BattleEnvironments = _environment,
                        Level = agent.levelComponent.Level,
                        AdventurerTypes = agent.AdventurerType,
                        ID = _battleID,
                        AverageStepCount = agent.StepCount
                    };
                    _dataLogger.AddEnvironmentSelection(environmentData);
                }
            }

            _battleStarted = true;
            base.CompleteParty(_agentGroup);
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
                        _timerActive = false;
                        confirmAbilities.StartConfirm();
                        ResetTimer(5);
                        AskConfirmAbilities.Invoke(PendingAgents.ToArray(), null, _agentGroup, confirmAbilities.SelectedAttacks);
                    }   
                }
            }
            else
            {
                CancelConfirmation();
            }
        }

        public void CancelConfirmation()
        {
            var pArray = PendingAgents.ToArray();
            foreach (var a in pArray)
            {
                CancelAgent.Invoke(a);
            }
        }

        public void Update()
        {
            if (!confirmAbilities.Confirm)
            {
                if (_timerActive)
                {
                    _timer -= Time.deltaTime;
                    if (_timer <= 0)
                    {
                        _timerActive = false;
                        CancelConfirmation();
                    }
                }   
            }

            else if(!_battleStarted)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0 && _confirmedAgents.Count == PendingAgents.Count)
                {
                    foreach (var agent in PendingAgents.ToArray())
                    {
                        foreach (var option in SensorUtils<EAttackOptions>.ValuesToArray)
                        {
                            if (option != EAttackOptions.None)
                            {
                                confirmAbilities.ConfirmAbility(agent, option);
                                if (confirmAbilities.Complete(SystemTraining.PartySize))
                                {
                                    StartBattle();
                                    return;
                                }   
                            }
                        }
                    }
                }
            }
        }
    }
}
