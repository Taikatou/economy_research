using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI;
using LevelSystem;
using TurnBased.Scripts;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public delegate void SetAdventureState(AdventurerAgent agent, EAdventureStates state);
    public class BattleSubSystem
    {
        public Dictionary<AdventurerAgent, BattleSubSystemInstance<AdventurerAgent>> battleSystems { get; }
        public Dictionary<EBattleEnvironments, BattlePartySubsystem> currentParties { get; private set; }
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
                var party = new BattlePartySubsystem(SystemTraining.PartySize, battle, travelSubsystem);
                currentParties.Add(battle, party);
                party.setupNewBattle = SetupNewBattle;
            }
        }

        private void EndBattle(AdventurerAgent agent)
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
            if (agent != null)
            {
                if (battleSystems.ContainsKey(agent))
                {
                    return battleSystems[agent];
                }   
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

        public void RemoveAgent(AdventurerAgent agent)
        {
            currentParties[reverseCurrentParties[agent]].RemoveFromQueue(agent);
            reverseCurrentParties.Remove(agent);
            SetAdventureState.Invoke(agent, EAdventureStates.OutOfBattle);
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
                    fighterData.PlayerData.ResetHp();
                }
            
                SetAdventureState(agent, EAdventureStates.OutOfBattle);
                EndBattle(agent);   
            }
        }

        private static void OnWin(BattleSubSystemInstance<AdventurerAgent> battle)
        {
            void OnItemAdd()
            {
                if (TrainingConfig.OnResource)
                {
                    battle.AddReward(TrainingConfig.OnResourceReward);   
                }
            }
            void OnRequestComplete()
            {
                if (TrainingConfig.OnResourceComplete)
                {
                    battle.AddReward(TrainingConfig.OnResourceCompleteReward);   
                }
            }
            
            if (TrainingConfig.OnWin)
            {
                if (SystemTraining.PartySize > 1)
                {
                    battle.AgentParty.AddGroupReward(TrainingConfig.OnWinReward);   
                }
                else
                {
                    foreach (var agent in battle.BattleAgents)
                    {
                        agent.AddReward(TrainingConfig.OnWinReward);
                    }
                }
            }
            OverviewVariables.WonBattle();
            foreach (var agent in battle.BattleAgents)
            {
                var craftingDrop = battle.GetCraftingDropItem();
                var craftingInventory = agent.GetComponent<AdventurerRequestTaker>();

                var levelUpComponent = agent.GetComponent<LevelUpComponent>();
                if (levelUpComponent != null)
                {
                    var exp = battle.GetExp();
                    levelUpComponent.AddExp(exp);
                }

                craftingInventory.CheckItemAdd(craftingDrop.Resource, craftingDrop.Count, OnItemAdd, OnRequestComplete);   
            }
        }
    }
}
