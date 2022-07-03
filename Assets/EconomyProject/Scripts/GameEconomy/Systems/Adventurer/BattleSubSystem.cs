using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using LevelSystem;
using TurnBased.Scripts;
using Unity.MLAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public delegate void SetAdventureState(AdventurerAgent agent, EAdventureStates state);
    public class BattleSubSystem
    {
        public Dictionary<AdventurerAgent, BattleSubSystemInstance<AdventurerAgent>> BattleSystems { get; }
        public Dictionary<EBattleEnvironments, BattlePartySubsystem> CurrentParties { get; private set; }
        private Dictionary<AdventurerAgent, EBattleEnvironments> ReverseCurrentParties { get; }
        
        private static IEnumerable<EBattleEnvironments> BattleAsArray =>
            Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();

        public static int SensorCount => BattleSubSystemInstance<AdventurerAgent>.SensorCount;

        private readonly SetAdventureState _setAdventureState;

        public BattleSubSystem(TravelSubSystem travelSubsystem, SetAdventureState setAdventureState)
        {
            _setAdventureState = setAdventureState;
            
            void SetupNewBattle(AdventurerAgent[] agents, FighterObject enemyFighter, SimpleMultiAgentGroup party)
            {
                var playerData = new PlayerFighterData[agents.Length];
                for (var i = 0; i < playerData.Length; i++)
                {
                    playerData[i] = agents[i].GetComponent<AdventurerFighterData>().FighterData;
                    playerData[i].HashCode = agents[i].GetHashCode();
                    var levelComp = agents[i].GetComponent<LevelUpComponent>();
                    if (levelComp != null)
                    {
                        playerData[i].level = levelComp.Level;   
                    }
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
                    BattleSystems.Add(agent, newSystem);
                    _setAdventureState.Invoke(agent, EAdventureStates.InBattle);   
                }
            }
            
            BattleSystems = new Dictionary<AdventurerAgent, BattleSubSystemInstance<AdventurerAgent>>();
            CurrentParties = new Dictionary<EBattleEnvironments, BattlePartySubsystem>();
            ReverseCurrentParties = new Dictionary<AdventurerAgent, EBattleEnvironments>();
            foreach (var battle in BattleAsArray)
            {
                var party = new BattlePartySubsystem(SystemTraining.PartySize, battle, travelSubsystem);
                CurrentParties.Add(battle, party);
                party.setupNewBattle = SetupNewBattle;
            }
        }

        private void EndBattle(AdventurerAgent agent)
        {
            BattleSystems.Remove(agent);
            ReverseCurrentParties.Remove(agent);
        }

        public void Setup()
        {
            foreach (var battle in BattleAsArray)
            {
                CurrentParties[battle].Setup();
            }
            BattleSystems.Clear();
            ReverseCurrentParties.Clear();
        }

        public BattleSubSystemInstance<AdventurerAgent> GetSubSystem(AdventurerAgent agent)
        {
            if (agent != null)
            {
                if (BattleSystems.ContainsKey(agent))
                {
                    return BattleSystems[agent];
                }   
            }
            return null;
        }

        public void SelectBattle(AdventurerAgent agent, EBattleAction action)
        {
            var battleSystem = GetSubSystem(agent);
            battleSystem.SetInput(action, agent.GetHashCode());
        }

        public void StartBattle(AdventurerAgent agent, EBattleEnvironments location)
        {
            _setAdventureState.Invoke(agent, EAdventureStates.InQueue);
            CurrentParties[location].AddAgent(agent);
            ReverseCurrentParties.Add(agent, location);
        }

        public void RemoveAgent(AdventurerAgent agent)
        {
            CurrentParties[ReverseCurrentParties[agent]].RemoveFromQueue(agent);
            ReverseCurrentParties.Remove(agent);
            _setAdventureState.Invoke(agent, EAdventureStates.OutOfBattle);
        }

        private void OnComplete(BattleSubSystemInstance<AdventurerAgent> systemInstance)
        {
            foreach (var agent in systemInstance.BattleAgents)
            {
                if (ReverseCurrentParties.ContainsKey(agent))
                {
                    var location = ReverseCurrentParties[agent];
                    CurrentParties[location].RemoveAgent(agent);
                    ReverseCurrentParties.Remove(agent);
                }

                if(systemInstance.CurrentState == EBattleState.Lost)
                {
                    agent.wallet.SpendMoney(5);
                    var fighterData = agent.GetComponent<AdventurerFighterData>();
                    fighterData.PlayerData.ResetHp();
                }
            
                _setAdventureState(agent, EAdventureStates.OutOfBattle);
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
