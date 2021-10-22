using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI;
using TurnBased.Scripts;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public delegate void SetAdventureState(AdventurerAgent agent, EAdventureStates state);
    public class BattleSubSystem
    {
        public Dictionary<AdventurerAgent, BattleSubSystemInstance<AdventurerAgent>> battleSystems { get; }
        private Dictionary<EBattleEnvironments, BattlePartySubsystem> currentParties { get; }
        private Dictionary<AdventurerAgent, EBattleEnvironments> reverseCurrentParties { get; }
        
        private static IEnumerable<EBattleEnvironments> BattleAsArray =>
            Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();

        public static int SensorCount => BattleSubSystemInstance<AdventurerAgent>.SensorCount;

        private readonly SetAdventureState SetAdventureState;

        public BattleSubSystem(TravelSubSystem travelSubsystem, SetAdventureState setAdventureState)
        {
            SetAdventureState = setAdventureState;
            
            void SetupNewBattle(AdventurerAgent[] agents, FighterObject enemyFighter, SimpleMultiAgentGroup party)
            {
                var playerData = new BaseFighterData[agents.Length];
                for (var i = 0; i < playerData.Length; i++)
                {
                    playerData[i] = agents[i].GetComponent<AdventurerFighterData>().FighterData;   
                }
                
                var enemyData = FighterData.Clone(enemyFighter.data);
            
                var newSystem = new BattleSubSystemInstance<AdventurerAgent>(   playerData,
                                                                                enemyData,
                                                                                enemyFighter.fighterDropTable,
                                                                                OnWin,
                                                                                OnComplete,
                                                                                party,
                                                                                agents);

                foreach (var agent in agents)
                {
                    battleSystems.Add(agent, newSystem);
                    SetAdventureState.Invoke(agent, EAdventureStates.InBattle);   
                }
            }
            
            battleSystems = new Dictionary<AdventurerAgent, BattleSubSystemInstance<AdventurerAgent>>();
            currentParties = new Dictionary<EBattleEnvironments, BattlePartySubsystem>();
            reverseCurrentParties = new Dictionary<AdventurerAgent, EBattleEnvironments>();
            foreach (var battle in BattleAsArray)
            {
                var party = new BattlePartySubsystem(SystemTraining.partySize, battle, travelSubsystem);
                currentParties.Add(battle, party);
                party.setupNewBattle = SetupNewBattle;
            }
        }

        private void RemoveAgent(AdventurerAgent agent)
        {
            battleSystems.Remove(agent);
            reverseCurrentParties.Remove(agent);
        }

        public void Setup()
        {
            foreach (var battle in BattleAsArray)
            {
                currentParties[battle].Setup();
            }
            battleSystems.Clear();
            reverseCurrentParties.Clear();
        }

        public BattleSubSystemInstance<AdventurerAgent> GetSubSystem(AdventurerAgent agent)
        {
            if (battleSystems.ContainsKey(agent))
            {
                return battleSystems[agent];
            }
            return null;
        }

        public void SelectBattle(AdventurerAgent agent, EBattleAction action)
        {
            var battleSystem = GetSubSystem(agent);
            battleSystem.SetInput(action);
        }

        public void StartBattle(AdventurerAgent agent, EBattleEnvironments location)
        {
            SetAdventureState.Invoke(agent, EAdventureStates.InQueue);
            currentParties[location].AddAgent(agent);
            reverseCurrentParties.Add(agent, location);
        }

        private void OnComplete(BattleSubSystemInstance<AdventurerAgent> systemInstance)
        {
            foreach (var agent in systemInstance.BattleAgents)
            {
                if (reverseCurrentParties.ContainsKey(agent))
                {
                    var location = reverseCurrentParties[agent];
                    currentParties[location].RemoveAgent(agent);
                    reverseCurrentParties.Remove(agent);
                }

                if(systemInstance.CurrentState == EBattleState.Lost)
                {
                    agent.wallet.SpendMoney(5);
                    var fighterData = agent.GetComponent<AdventurerFighterData>();
                    fighterData.playerData.ResetHp();
                }
            
                SetAdventureState(agent, EAdventureStates.OutOfBattle);
                RemoveAgent(agent);   
            }
        }

        private static void OnWin(BattleSubSystemInstance<AdventurerAgent> battle)
        {
            void OnItemAdd()
            {
                if (TrainingConfig.OnResource)
                    battle.AddReward(0.1f);
            }
            void OnRequestComplete()
            {
                if (TrainingConfig.OnResourceComplete)
                    battle.AddReward(0.2f);
            }
            
            if (TrainingConfig.OnWin)
            {
                if (SystemTraining.partySize > 1)
                {
                    battle.AgentParty.AddGroupReward(0.2f);   
                }
                else
                {
                    battle.BattleAgents[0].AddReward(0.2f);
                }
            }
            OverviewVariables.WonBattle();
            foreach (var agent in battle.BattleAgents)
            {
                var craftingDrop = battle.GetCraftingDropItem();
                var craftingInventory = agent.GetComponent<AdventurerRequestTaker>();
            
                craftingInventory.CheckItemAdd(craftingDrop.Resource, craftingDrop.Count, OnItemAdd, OnRequestComplete);   
            }
        }
    }
}
