using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.UI.Craftsman.Request;
using TurnBased.Scripts;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public enum EAdventureStates { OutOfBattle, InQueue, InBattle }
    
    [Serializable]
    public class AdventurerSystem : EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>, ISetup
    {
        public int partySize = 1;
        public static int SensorCount => 1;

        public Dictionary<AdventurerAgent, BattleSubSystem> battleSystems;
        public Dictionary<BattleSubSystem, AdventurerAgent[]> activeAgents;
        public Dictionary<AdventurerAgent, EAdventureStates> adventureStates;

        public static int ObservationSize => SensorCount + BattleSubSystem.SensorCount + AdventurerLocationSelect.SensorCount;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Adventurer;

        public AdventurerLocationSelect locationSelect;
        public BattleLocationSelect battleLocationSelect;
        public Dictionary<EBattleEnvironments, BattlePartySubsystem> currentParties;
        
        public TravelSubSystem travelSubsystem;

        EBattleEnvironments [] BattleAsArray = Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();

        public void Start()
        {
            void SetupNewBattle(AdventurerAgent agent, FighterObject enemyFighter, SimpleMultiAgentGroup party)
            {
                var playerData = agent.GetComponent<AdventurerFighterData>().FighterData;
                var enemyData = FighterData.Clone(enemyFighter.data);
            
                var newSystem = new BattleSubSystem(playerData, enemyData, enemyFighter.fighterDropTable, OnWin, OnComplete, party);
                if (battleSystems.ContainsKey(agent))
                {
                    battleSystems.Remove(agent);
                    Debug.Log("What");
                }
                battleSystems.Add(agent, newSystem);
            
                SetAdventureState(agent, EAdventureStates.InBattle);
                activeAgents.Add(newSystem, new [] { agent });
            }
            
            currentParties = new Dictionary<EBattleEnvironments, BattlePartySubsystem>();
            foreach (var battle in BattleAsArray)
            {
                var party = new BattlePartySubsystem(partySize, battle, travelSubsystem);
                currentParties.Add(battle, party);
                party.setupNewBattle = SetupNewBattle;
            }
            battleSystems = new Dictionary<AdventurerAgent, BattleSubSystem>();
            adventureStates = new Dictionary<AdventurerAgent, EAdventureStates>();
            activeAgents = new Dictionary<BattleSubSystem, AdventurerAgent[]>();
        }

        public void Setup()
		{
            foreach (var battle in BattleAsArray)
            {
                currentParties[battle].Setup();
            }

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
            return GetAdventureStates(agent) == EAdventureStates.OutOfBattle;
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
                : BlankArray(AdventurerLocationSelect.SensorCount);
            battleState.AddRange(output2);
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
                    // we have to do this first
                    SetAdventureState(agent, EAdventureStates.InQueue);
                    
                    var location = locationSelect.GetBattle(agent);
                    currentParties[location].AddAgent(agent);
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

        private static void OnWin()
        {
            OverviewVariables.WonBattle();
        }

        public void OnComplete(BattleSubSystem system)
        {
            foreach (var agent in activeAgents[system])
            {
                switch (system.CurrentState)
                {
                    case EBattleState.Lost:
                        SpendMoney(agent);
                        var fighterData = agent.GetComponent<AdventurerFighterData>();
                        fighterData.playerData.ResetHp();
                        break;
                    case EBattleState.Won:
                        var craftingDrop = system.GetCraftingDropItem();
                        var craftingInventory = agent.GetComponent<AdventurerRequestTaker>();
                            
                        craftingInventory.CheckItemAdd(craftingDrop.Resource, craftingDrop.Count);
                        break;
                }

                system.AgentParty.UnregisterAgent(agent);
                battleSystems.Remove(agent);
                SetAdventureState(agent, EAdventureStates.OutOfBattle);
            }

            activeAgents.Remove(system);
        }

        private void SpendMoney(AdventurerAgent agent)
        {
            agent.wallet.SpendMoney(5);
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
            var inputChoices = new[]
            {
                EAdventurerAgentChoices.Up,
                EAdventurerAgentChoices.Down,
                EAdventurerAgentChoices.Select,
                EAdventurerAgentChoices.Back
            };

            var outputs = EconomySystemUtils<EAdventurerAgentChoices>.GetInputOfType(inputChoices);

            return outputs;
        }
    }
}
