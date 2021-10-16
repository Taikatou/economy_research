﻿using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public enum EAdventureStates { OutOfBattle, InQueue, InBattle }
    
    [Serializable]
    public class AdventurerSystem : EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>, ISetup
    {
        public static int SensorCount => 2;
        
		public TravelSubSystem travelSubsystem;

        private Dictionary<AdventurerAgent, BattleSubSystem> _battleSystems;
        public Dictionary<AdventurerAgent, BattleSubSystem> battleSystems => 
            _battleSystems??=new Dictionary<AdventurerAgent, BattleSubSystem>();

        private Dictionary<AdventurerAgent, EAdventureStates> _adventureStates;
        public Dictionary<AdventurerAgent, EAdventureStates> adventureStates =>
            _adventureStates ??= new Dictionary<AdventurerAgent, EAdventureStates>();
        
        public static int ObservationSize => SensorCount + BattleSubSystem.SensorCount;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Adventurer;

        public AdventurerLocationSelect locationSelect;
        public BattleLocationSelect battleLocationSelect;

        public void Setup()
		{
            adventureStates.Clear();
            battleSystems.Clear();
        }

        public EAdventureStates GetAdventureStates(AdventurerAgent agent)
        {
            if (!adventureStates.ContainsKey(agent))
            {
                adventureStates.Add(agent, EAdventureStates.OutOfBattle);
            }

            return adventureStates[agent];
        }

        private void SetAdventureState(AdventurerAgent agent, EAdventureStates state)
        {
            if (!adventureStates.ContainsKey(agent))
            {
                adventureStates.Add(agent, state);
            }
            else
            {
                adventureStates[agent] = state;
            }
            Refresh(agent);
        }
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return GetAdventureStates(agent) != EAdventureStates.InBattle;
        }

        public override ObsData[] GetObservations(AdventurerAgent agent)
        {
            ObsData[] GetSubsystemData(AdventurerAgent agent)
            {
                var i = battleLocationSelect.GetCurrentLocation(agent);
                var subSystem = GetSubSystem(agent);
                return subSystem.GetSubsystemObservations(i);
            }
            ObsData[] BlankArray(int sensorCount)
            {
                return new ObsData[sensorCount];
            }
            
            var state = GetAdventureStates(agent);
            var battleState = new List<ObsData> { new ObsData {data=(float) state, name="AdventureState"}};

            var output = state == EAdventureStates.InBattle
                ? GetSubsystemData(agent)
                : BlankArray(BattleSubSystem.SensorCount);
            battleState.AddRange(output);
            
            var output2 = state == EAdventureStates.OutOfBattle
                ? locationSelect.GetTravelObservations(agent)
                : BlankArray(locationSelect.SensorCount);
            battleState.AddRange(output2);
            Debug.Log(string.Join(",", battleState));
            return battleState.ToArray();
        }

        protected override void SetChoice(AdventurerAgent agent, EAdventurerAgentChoices input)
        {
            var validInput = ValidInput(agent, input);
            if (validInput)
            {
                switch(input)
                {
                    case EAdventurerAgentChoices.Back:
                        AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);
                        break;
                    case EAdventurerAgentChoices.Select:
                        Select(agent);
                        break;
                    case EAdventurerAgentChoices.Up:
                        UpDown(agent, 1);
                        break;
                    case EAdventurerAgentChoices.Down:
                        UpDown(agent, -1);
                        break;
                }
            }
        }

        public void Select(AdventurerAgent agent)
        {
            switch (GetAdventureStates(agent))
            {
                case EAdventureStates.InBattle:
                    var action = battleLocationSelect.GetBattleAction(agent);
                    var battleSystem = GetSubSystem(agent);
                    battleSystem.SetInput(action);
                    break;
                case EAdventureStates.OutOfBattle:
                    var battle = locationSelect.GetBattle(agent);
                    StartBattle(agent, battle);   
                    break;
            }
        }

        public void UpDown(AdventurerAgent agent, int movement)
        {
            var location = GetAdventureStates(agent) == EAdventureStates.InBattle
                ? (LocationSelect<AdventurerAgent>) battleLocationSelect
                : (LocationSelect<AdventurerAgent>) locationSelect;
            location.MovePosition(agent, movement);
        }

        public BattleSubSystem GetSubSystem(AdventurerAgent agent)
        {
            if (battleSystems.ContainsKey(agent))
            {
                return battleSystems[agent];
            }
            return null;
        }

        public int GetBattleCount()
        {
            var count = 0;
            foreach (var entry in battleSystems)
            {
                count++;
            }
            return count;
        }

        private void SetupNewBattle(AdventurerAgent agent, FighterObject enemyFighter)
        {
            if (battleSystems.ContainsKey(agent))
            {
                battleSystems.Remove(agent);
            }

            var playerData = agent.GetComponent<AdventurerFighterData>().FighterData;
			var enemyData = FighterData.Clone(enemyFighter.data);
            
            var newSystem = new BattleSubSystem(playerData, enemyData, enemyFighter.fighterDropTable, OnWin);
            battleSystems.Add(agent, newSystem);
            
            SetAdventureState(agent, EAdventureStates.InBattle);
        }

        private static void OnWin()
        {
            OverviewVariables.WonBattle();
        }
        
        public void FixedUpdate()
        {
            foreach (var player in CurrentPlayers)
            {
                var state = GetAdventureStates(player);
                switch (state)
                {
                    case EAdventureStates.OutOfBattle:
                        
                        break;
                    case EAdventureStates.InBattle:
                        CheckInBattle(player);
                        break;
                }
            }
        }

        private void CheckInBattle(AdventurerAgent agent)
        {
            if (battleSystems.ContainsKey(agent))
            {
                var battleSystem = battleSystems[agent];
                if (battleSystem.GameOver())
                {
                    switch (battleSystem.CurrentState)
                    {
                        case BattleState.Lost:
                            SpendMoney(agent);
                            var fighterData = agent.GetComponent<AdventurerFighterData>();
                            fighterData.playerData.ResetHp();
                            break;
                        case BattleState.Won:
                            var craftingDrop = battleSystem.GetCraftingDropItem();
                            var craftingInventory = agent.GetComponent<AdventurerRequestTaker>();
                            
                            craftingInventory.CheckItemAdd(craftingDrop.Resource, craftingDrop.Count);
                            break;
                    }

                    battleSystems.Remove(agent);
                    SetAdventureState(agent, EAdventureStates.OutOfBattle);
                }   
            }
        }

        private void SpendMoney(AdventurerAgent agent)
        {
            agent.wallet.SpendMoney(5);
        }

        public void StartBattle(AdventurerAgent agent, EBattleEnvironments battleEnvironments)
        {
            var fighter = travelSubsystem.GetBattle(battleEnvironments);

            if (fighter)
            {
                SetupNewBattle(agent, fighter);
            }
            else
            {
                Debug.Log("Fighter Invalid no fight can start");
            }
        }

        public void OnAttackButton(AdventurerAgent agent)
        {
            if (battleSystems.ContainsKey(agent))
            {
                battleSystems[agent].OnAttackButton();
            }
        }
        
        public void OnHealButton(AdventurerAgent agent)
        {
            if (battleSystems.ContainsKey(agent))
            {
                battleSystems[agent].OnHealButton();
            }
        }

        public void OnFleeButton(AdventurerAgent agent)
        {
            if (battleSystems.ContainsKey(agent))
            {
                battleSystems[agent].OnFleeButton();
            }

            SetAdventureState(agent, EAdventureStates.OutOfBattle);
        }
        
        public override EnabledInput[] GetEnabledInputs(AdventurerAgent agent)
        {
            var inputChoices = battleSystems.ContainsKey(agent)
            ? new[]
            {
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Down,
                EAdventurerAgentChoices.Select
            }
            : new[]
            {
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Down,
                EAdventurerAgentChoices.Select,
                EAdventurerAgentChoices.Select,
                EAdventurerAgentChoices.Back
            };
                
            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices);

            return outputs;
        }
    }
}
