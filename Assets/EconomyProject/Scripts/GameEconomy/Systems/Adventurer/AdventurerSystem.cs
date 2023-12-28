using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using TurnBased.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public enum EAdventureStates { OutOfBattle, InQueue, ConfirmBattle, ConfirmAbilities, InBattle }
    
    [Serializable]
    public class AdventurerSystem : EconomySystem<BaseAdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>, ISetup
    {
        public BattleSubSystem battleSubSystem;

        public Dictionary<BaseAdventurerAgent, EAdventureStates> AdventureStates;

        public static int ObservationSize => 5;
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
            AdventureStates = new Dictionary<BaseAdventurerAgent, EAdventureStates>();
        }

        public void Setup()
		{
            battleSubSystem.Setup();
            AdventureStates.Clear();
        }

        public EAdventureStates GetAdventureStates(BaseAdventurerAgent agent)
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

        private void SetAdventureState(BaseAdventurerAgent agent, EAdventureStates state)
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
        
        public override bool CanMove(BaseAdventurerAgent agent)
        {
            return GetAdventureStates(agent) == EAdventureStates.OutOfBattle;
        }

        public override ObsData[] GetObservations(BaseAdventurerAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            var obs = new CategoricalObsData<ECraftingResources>(_craftingResourceAquired);
            _craftingResourceAquired = ECraftingResources.Nothing;
            return new ObsData[] { obs };
        }

        protected override void SetChoice(BaseAdventurerAgent agent, EAdventurerAgentChoices input)
        {
            
        }
        
        public void RemoveAgent(BaseAdventurerAgent agent)
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

        private ECraftingResources _craftingResourceAquired = ECraftingResources.Nothing;

        public void SelectOutOfBattle(BaseAdventurerAgent agent, EBattleEnvironments location)
        {
            CraftingDropReturn? craftingDropReturn = null;
            for (var i = 0; i <= agent.AdventurerInventory.EquipedItem.itemDetails.damage; i+=5)
            {
                var getLootBox = travelSubsystem.GetLootBox(location);
                if (getLootBox.HasValue)
                {
                    if (!craftingDropReturn.HasValue ||
                        getLootBox.Value.Resource > craftingDropReturn.Value.Resource)
                    {
                        craftingDropReturn = getLootBox;
                        _craftingResourceAquired = getLootBox.Value.Resource;
                    }
                }
            }
                    
            Debug.Log(craftingDropReturn.Value.Resource);
            if(craftingDropReturn.HasValue)
            {
                var requestReceived = agent.RequestTaker.CheckItemAdd(agent, craftingDropReturn.Value.Resource, craftingDropReturn.Value.Count, battleSubSystem.OnItemAdd, battleSubSystem.OnRequestComplete);
                if (requestReceived)
                {
                    agent.AdventurerInventory.DecreaseDurability();
                }
            }
        }

        public void Select(BaseAdventurerAgent agent)
        {
            var adventurerState = GetAdventureStates(agent);
            switch (adventurerState)
            {
                case EAdventureStates.OutOfBattle:
                    var location = locationSelect.GetBattle(agent);

                    SelectOutOfBattle(agent, location);
                    break;
            }
        }
        
        public void UpDown(BaseAdventurerAgent agent, int movement)
        {
            switch (GetAdventureStates(agent))
            {
                case EAdventureStates.InBattle:
                    var location = battleLocationSelect;
                    location?.MovePosition(agent, movement);
                    break;
                case EAdventureStates.InQueue:
                    var locationB = battleLocationSelect;
                    locationB?.MovePosition(agent, movement);
                    break;
                case EAdventureStates.OutOfBattle:
                    var locationC = locationSelect;
                    locationC?.MovePosition(agent, movement);
                    break;
                case EAdventureStates.ConfirmBattle:
                    var locationD = confirmLocationSelect;
                    locationD?.MovePosition(agent, movement);
                    break;
                case EAdventureStates.ConfirmAbilities:
                    var locationE = confirmAbilitiesSelect;
                    locationE?.MovePosition(agent, movement);
                    break;

            }
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

        public override EAdventurerAgentChoices[] GetEnabledInputs(BaseAdventurerAgent agent)
        {
            var inputChoices = new[] { EAdventurerAgentChoices.None, EAdventurerAgentChoices.SelectForest, EAdventurerAgentChoices.SelectMountain, EAdventurerAgentChoices.SelectSea, EAdventurerAgentChoices.SelectVolcano };

            return inputChoices;
        }

        public void Update()
        {
            battleSubSystem.Update();
        }
    }
}
