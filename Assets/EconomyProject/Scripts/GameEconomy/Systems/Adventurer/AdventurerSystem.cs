﻿using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public enum EAdventureStates { OutOfBattle, InQueue, ConfirmBattle, ConfirmAbilities, InBattle }
    
    [Serializable]
    public class AdventurerSystem : EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>, ISetup
    {
        public BattleSubSystem battleSubSystem;

        public Dictionary<AdventurerAgent, EAdventureStates> AdventureStates;

        public static int ObservationSize => TrainingConfig.SimpleAdventurerSystem? 8 : 75;
        public override EAdventurerScreen ActionChoice => TrainingConfig.StartScreen;

        public AdventurerLocationSelect locationSelect;
        public BattleLocationSelect battleLocationSelect;
        public ConfirmBattleLocationSelect confirmLocationSelect;
        public ConfirmAbilitiesLocationSelect confirmAbilitiesSelect;

        public TravelSubSystem travelSubsystem;
        public BattleEnvironmentDataLogger dataLogger;

        public void Start()
        {
            battleSubSystem = new BattleSubSystem(travelSubsystem, SetAdventureState, dataLogger);
            AdventureStates = new Dictionary<AdventurerAgent, EAdventureStates>();
        }

        public void Setup()
		{
            battleSubSystem.Setup();
            AdventureStates.Clear();
        }

        public EAdventureStates GetAdventureStates(AdventurerAgent agent)
        {
            var toReturn = EAdventureStates.OutOfBattle;
            if (agent != null)
            {
                if (!AdventureStates.ContainsKey(agent))
                {
                    AdventureStates.Add(agent, EAdventureStates.OutOfBattle);
                }

                toReturn = AdventureStates[agent];
            }
            return toReturn;
        }

        private void SetAdventureState(AdventurerAgent agent, EAdventureStates state)
        {
            if (!AdventureStates.ContainsKey(agent))
            {
                AdventureStates.Add(agent, state);
            }
            else
            {
                AdventureStates[agent] = state;
            }
            Refresh(agent);
        }
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return GetAdventureStates(agent) == EAdventureStates.OutOfBattle;
        }

        public override ObsData[] GetObservations(AdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            if (TrainingConfig.SimpleAdventurerSystem)
            {
                var obsData = locationSelect.GetTravelObservations(agent, this);
                return obsData;
            }
            
            ObsData[] GetSubsystemData()
            {
                ObsData[] toReturn;
                
                var subSystem = battleSubSystem.GetSubSystem(agent);
                if (subSystem != null)
                {
                    var i = battleLocationSelect.GetObs(agent);
                    var obs = subSystem.GetSubsystemObservations(i, agent.GetHashCode());
                    toReturn = obs;
                }
                else
                {
                    toReturn = BlankArray(BattleSubSystemInstance<AdventurerAgent>.SensorCount);
                }

                return toReturn;
            }
            ObsData[] BlankArray(int sensorCount)
            {
                var toReturn = new ObsData[sensorCount];
                for (var i = 0; i < sensorCount; i++)
                {
                    toReturn[i] = new SingleObsData();
                }

                return toReturn;
            }
            
            var state = GetAdventureStates(agent);
            var battleState = new List<ObsData> { new CategoricalObsData<EAdventureStates>(state) {Name="AdventureState"}};

            var output = state == EAdventureStates.InBattle
                ? GetSubsystemData()
                : BlankArray(BattleSubSystemInstance<AdventurerAgent>.SensorCount);
            battleState.AddRange(output);

            var output2 = state == EAdventureStates.OutOfBattle
                ? locationSelect.GetTravelObservations(agent, this)
                : BlankArray(AdventurerLocationSelect.SensorCount);

            var output3 = state == EAdventureStates.ConfirmBattle
                ? confirmLocationSelect.GetConfirmationObservations(agent, this)
                : BlankArray(ConfirmBattleLocationSelect.SensorCount);

            var output5 = state == EAdventureStates.ConfirmAbilities
                ? confirmAbilitiesSelect.GetObservations(agent)
                : BlankArray(ConfirmAbilitiesLocationSelect.SensorCount);

            var obsize = new List<ObsData>();
            foreach (EBattleEnvironments battle in Enum.GetValues(typeof(EBattleEnvironments)))
            {
                for (var i = 0; i < SystemTraining.PartySize; i++)
                {
                    var party = battleSubSystem.CurrentParties[battle];
                    if (i < party.PendingAgents.Count)
                    {
                        var a = party.PendingAgents[i];
                        obsize.Add(new CategoricalObsData<EAdventurerTypes>(a.AdventurerType));
                    }
                    else
                    {
                        obsize.Add(new CategoricalObsData<EAdventurerTypes>());
                    }
                }
            }
            
            battleState.AddRange(obsize);
            battleState.AddRange(output2);
            battleState.AddRange(output3);
            battleState.AddRange(output5);
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
                        if (GetAdventureStates(agent) == EAdventureStates.InQueue)
                        {
                            battleSubSystem.RemoveAgentFromQueue(agent);
                        }
                        else if(SystemTraining.IncludeShop)
                        {
                            AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);
                        }
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
        
        public void RemoveAgent(AdventurerAgent agent)
        {
            var state = GetAdventureStates(agent);
            if (state != EAdventureStates.OutOfBattle)
            {
                battleSubSystem.RemoveAgentFromParty(agent);    
            }
            
            switch (state)
            {
                case EAdventureStates.ConfirmBattle:
                    battleSubSystem.CancelConfirmation(agent);
                    break;
                case EAdventureStates.ConfirmAbilities:
                    battleSubSystem.CancelAbilities(agent);
                    break;
                case EAdventureStates.InQueue:
                    battleSubSystem.RemoveAgentFromQueue(agent);
                    break;
                case EAdventureStates.InBattle:
                    battleSubSystem.BattleSystems[agent].EndBattle();
                    break;
            }
            
            AdventureStates[agent] = EAdventureStates.OutOfBattle;
        }

        public void Select(AdventurerAgent agent)
        {
            var adventurerState = GetAdventureStates(agent);
            switch (adventurerState)
            {
                case EAdventureStates.InBattle:
                    var action = battleLocationSelect.GetBattleAction(agent);
                    battleSubSystem.SelectBattle(agent, action);
                    break;
                case EAdventureStates.OutOfBattle:
                    if (!TrainingConfig.SimpleAdventurerSystem)
                    {
                        var location = locationSelect.GetBattle(agent);
                        battleSubSystem.StartBattle(agent, location);
                    }
                    else
                    {
                        var location = locationSelect.GetBattle(agent);
                        var getLootBox = travelSubsystem.GetLootBox(location);
                        if (getLootBox.HasValue)
                        {
                            var craftingInventory = agent.GetComponent<AdventurerRequestTaker>();
                            craftingInventory.CheckItemAdd(agent, getLootBox.Value.Resource, getLootBox.Value.Count, battleSubSystem.OnItemAdd, battleSubSystem.OnRequestComplete);  
                        }
                    }
                    break;
                case EAdventureStates.ConfirmBattle:
                    var confirm = confirmLocationSelect.GetConfirmation(agent);
                    battleSubSystem.Confirmation(confirm, agent);
                    break;
                case EAdventureStates.ConfirmAbilities:
                    var ability = confirmAbilitiesSelect.GetAbility(agent);
                    battleSubSystem.ConfirmAbilities(ability, agent);
                    break;
            }
        }

        public void UpDown(AdventurerAgent agent, int movement)
        {
            LocationSelect<AdventurerAgent> location = null;
            switch (GetAdventureStates(agent))
            {
                case EAdventureStates.InBattle:
                    location = battleLocationSelect;
                    break;
                case EAdventureStates.InQueue:
                    location = battleLocationSelect;
                    break;
                case EAdventureStates.OutOfBattle:
                    location = locationSelect;
                    break;
                case EAdventureStates.ConfirmBattle:
                    location = confirmLocationSelect;
                    break;
                case EAdventureStates.ConfirmAbilities:
                    location = confirmAbilitiesSelect;
                    break;

            }
            if(location != null)
                location.MovePosition(agent, movement);
        }

        public int GetBattleCount()
        {
            var count = 0;
            foreach (var entry in battleSubSystem.BattleSystems)
            {
                count++;
            }
            return count;
        }

        public override EnabledInput[] GetEnabledInputs(AdventurerAgent agent)
        {
            var inputChoices = new List<EAdventurerAgentChoices>
            {
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Down,
                EAdventurerAgentChoices.Select
            };

            if (SystemTraining.IncludeShop)
            {
                inputChoices.Add(EAdventurerAgentChoices.Back);
            }

            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices);

            return outputs;
        }

        public void Update()
        {
            battleSubSystem.Update();
        }
    }
}
