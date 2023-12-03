using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI;
using LevelSystem;
using TurnBased.Scripts;
using TurnBased.Scripts.AI;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public delegate void SetAdventureState<in T>(T agent, EAdventureStates state) where T : Agent;
    public class BattleSubSystem : ILastUpdate
    {
        private readonly SetAdventureState<BaseAdventurerAgent> _setAdventureState;
        
        public Dictionary<BaseAdventurerAgent, BattleSubSystemInstance<BaseAdventurerAgent>> BattleSystems { get; }
        public Dictionary<EBattleEnvironments, BattlePartySubsystem<BaseAdventurerAgent>> CurrentParties { get; private set; }
        private Dictionary<BaseAdventurerAgent, EBattleEnvironments> ReverseCurrentParties { get; }
        
        private static IEnumerable<EBattleEnvironments> BattleAsArray =>
            Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();

        public static int SensorCount => BattleSubSystemInstance<BaseAdventurerAgent>.SensorCount + Adventurer.ConfirmAbilities<BaseAdventurerAgent>.SensorCount;

        public DateTime LastUpdated { get; private set; }

        public BattleSubSystem(TravelSubSystem travelSubsystem, SetAdventureState<BaseAdventurerAgent> setAdventureState,
            BattleEnvironmentDataLogger dataLogger) : this(travelSubsystem, dataLogger)
        {
            _setAdventureState = setAdventureState;
        }

        public BattleSubSystem(TravelSubSystem travelSubsystem, BattleEnvironmentDataLogger dataLogger)
        {
            

            void SetupNewBattle(BaseAdventurerAgent[] agents, FighterObject enemyFighter, SimpleMultiAgentGroup party,
                Dictionary<BaseAdventurerAgent, HashSet<EAttackOptions>> selectedOptions)
            {
                foreach (var agent in agents)
                {
                    ReverseCurrentParties.Remove(agent);
                    if (!selectedOptions.ContainsKey(agent))
                    {
                        foreach (var agentb in agents)
                        {
                            RemoveAgentFromQueue(agentb);
                            return;
                        }
                    }
                }

                var playerData = new PlayerFighterData[agents.Length];
                for (var i = 0; i < playerData.Length; i++)
                {
                    var fighterData = agents[i].GetComponent<AdventurerFighterData>().FighterData;
                    fighterData.ResetHp();
                    playerData[i] = fighterData;
                    playerData[i].HashCode = agents[i].GetHashCode();
                    var levelComp = agents[i].GetComponent<LevelUpComponent>();
                    if (levelComp != null)
                    {
                        playerData[i].level = levelComp.Level;
                    }

                    playerData[i].AttackOptions = selectedOptions[agents[i]].ToList();
                }

                var enemyData = FighterData.Clone(enemyFighter.data);
                var newSystem = new BattleSubSystemInstance<BaseAdventurerAgent>(playerData,
                    enemyData,
                    enemyFighter.fighterDropTable,
                    OnWin,
                    OnComplete,
                    OnLose,
                    party,
                    agents);
                foreach (var agent in agents)
                {
                    BattleSystems.Add(agent, newSystem);
                    _setAdventureState.Invoke(agent, EAdventureStates.InBattle);
                }
            }

            void AskConfirmation(BaseAdventurerAgent[] agents, FighterObject enemyFighter, SimpleMultiAgentGroup party, Dictionary<BaseAdventurerAgent, HashSet<EAttackOptions>> selectedOptions)
            {
                foreach (var agent in agents)
                {
                    _setAdventureState.Invoke(agent, EAdventureStates.ConfirmBattle);
                }
                if (!TrainingConfig.RequireConfirmation)
                {
                    foreach (var agent in agents)
                    {
                        Confirmation(EConfirmBattle.Confirm, agent);
                    }
                }
            }

            void CreateBattleSystem(EBattleEnvironments battle)
            {
                var party = new BattlePartySubsystem<BaseAdventurerAgent>(SystemTraining.PartySize, battle, travelSubsystem, dataLogger);
                if (CurrentParties.ContainsKey(battle))
                {
                    CurrentParties.Remove(battle);
                }

                CurrentParties[battle] = party;
                party.SetupNewBattle = SetupNewBattle;
                party.SetupNewBattle += (_, _, _, _) => CreateBattleSystem(battle);
                
                party.AskConfirmation = AskConfirmation;
                party.AskConfirmAbilities = AskConfirmAbilities;
                party.CancelAgent = RemoveAgentFromQueue;
            }
            BattleSystems = new Dictionary<BaseAdventurerAgent, BattleSubSystemInstance<BaseAdventurerAgent>>();
            CurrentParties = new Dictionary<EBattleEnvironments, BattlePartySubsystem<BaseAdventurerAgent>>();
            ReverseCurrentParties = new Dictionary<BaseAdventurerAgent, EBattleEnvironments>();
            foreach (var battle in BattleAsArray)
            {
                CreateBattleSystem(battle);
            }
        }

        public void CancelConfirmation(BaseAdventurerAgent agent)
        {
            if (ReverseCurrentParties.ContainsKey(agent))
            {
                CurrentParties[ReverseCurrentParties[agent]].Confirmation(EConfirmBattle.Back, agent);
            }
        }

        public void CancelAbilities(BaseAdventurerAgent agent)
        {
            if (ReverseCurrentParties.ContainsKey(agent))
            {
                CurrentParties[ReverseCurrentParties[agent]].CancelConfirmation();
            }
        }

        public void Confirmation(EConfirmBattle confirmation, BaseAdventurerAgent agent)
        {
            CurrentParties[ReverseCurrentParties[agent]].Confirmation(confirmation, agent);
            Refresh();
        }
        
        void AskConfirmAbilities(BaseAdventurerAgent[] agents, FighterObject enemyFighter, SimpleMultiAgentGroup party, Dictionary<BaseAdventurerAgent, HashSet<EAttackOptions>> selectedOptions)
        {
            foreach (var agent in agents)
            {
                _setAdventureState.Invoke(agent, EAdventureStates.ConfirmAbilities);
            }
        }

        public void ConfirmAbilities(EAttackOptions confirmation, BaseAdventurerAgent agent)
        {
            if (ReverseCurrentParties.ContainsKey(agent))
            {
                var party = CurrentParties[ReverseCurrentParties[agent]];
                party.ConfirmAbilities.ConfirmAbility(agent, confirmation);
                if (party.ConfirmAbilities.Complete(SystemTraining.PartySize))
                {
                    party.StartBattle();
                }   
            }
        }

        private void EndBattle(BaseAdventurerAgent agent)
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

            foreach (var system in BattleSystems)
            {
                system.Value.FinishBattle();
            }
            BattleSystems.Clear();
            ReverseCurrentParties.Clear();
        }

        public BattleSubSystemInstance<BaseAdventurerAgent> GetSubSystem (BaseAdventurerAgent agent)
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

        public void SelectBattle(BaseAdventurerAgent agent, EBattleAction action)
        {
            var battleSystem = GetSubSystem(agent);
            battleSystem.SetInput(action, agent.GetHashCode());
        }

        public void StartBattle(BaseAdventurerAgent agent, EBattleEnvironments location)
        {
            if (!CurrentParties[location].Full)
            {
                _setAdventureState.Invoke(agent, EAdventureStates.InQueue);
                ReverseCurrentParties.Add(agent, location);
                CurrentParties[location].AddAgent(agent);
            }
        }

        public void RemoveAgentFromQueue(BaseAdventurerAgent agent)
        {
            if (ReverseCurrentParties.ContainsKey(agent))
            {
                CurrentParties[ReverseCurrentParties[agent]].RemoveFromQueue(agent);
                ReverseCurrentParties.Remove(agent);
                _setAdventureState.Invoke(agent, EAdventureStates.OutOfBattle);   
            }
        }

        public void RemoveAgentFromParty(BaseAdventurerAgent agent)
        {
            var subsystem = GetAgentParty(agent);
            if (subsystem != null)
            {
                subsystem.RemoveAgent(agent);
            }
        }

        public PartySubSystem<BaseAdventurerAgent> GetAgentParty(BaseAdventurerAgent agent)
        {
            if (agent != null)
            {
                if (ReverseCurrentParties.ContainsKey(agent))
                {
                    return CurrentParties[ReverseCurrentParties[agent]];
                }
            }
            return null;
        }

        public EBattleEnvironments? GetBattleEnvironment(BaseAdventurerAgent agent)
        {
            if (agent != null)
            {
                if (ReverseCurrentParties.ContainsKey(agent))
                {
                    return ReverseCurrentParties[agent];
                }   
            }

            return null;
        }

        public void OnComplete(IBattleSubSystemInstance<BaseAdventurerAgent> systemInstance)
        {
            foreach (var agent in systemInstance.BattleAgents)
            {
                if(systemInstance.CurrentState == EBattleState.Lost)
                {
                    agent.Wallet.SpendMoney(5);
                    var fighterData = agent.GetComponent<AdventurerFighterData>();
                    fighterData.PlayerData.ResetHp();
                }
            
                _setAdventureState(agent, EAdventureStates.OutOfBattle);
                EndBattle(agent);   
            }
        }

        public static void OnLose(IBattleSubSystemInstance<BaseAdventurerAgent> battle)
        {
            if (TrainingConfig.OnLose)
            {
                AddTeamReward(battle, TrainingConfig.OnLoseReward);
            }

            if (TrainingConfig.LoseMoney)
            {
                foreach (var agent in battle.BattleAgents)
                {
                    agent.Wallet.LoseMoney(TrainingConfig.MoneyToLose);
                }
            }
        }

        private static void AddTeamReward(IBattleSubSystemInstance<BaseAdventurerAgent> battle, float reward)
        {
            if (SystemTraining.PartySize > 1)
            {
                battle.AgentParty.AddGroupReward(TrainingConfig.OnWinReward);   
            }
            else
            {
                foreach (var agent in battle.BattleAgents)
                {
                    agent.AddReward(reward);
                }
            }
        }
        
        public void OnItemAdd(BaseAdventurerAgent agent)
        {
            var battle = GetSubSystem(agent);
            if (TrainingConfig.OnResource)
            {
                battle.AddReward(TrainingConfig.OnResourceReward);   
            }
        }
        public void OnRequestComplete(BaseAdventurerAgent agent)
        {
            if (TrainingConfig.OnResourceComplete)
            {
                agent.AddReward(TrainingConfig.OnResourceCompleteReward);
            }
        }

        public void OnWin(IBattleSubSystemInstance<BaseAdventurerAgent> battle)
        {
            if (TrainingConfig.OnWin)
            {
                AddTeamReward(battle, TrainingConfig.OnWinReward);
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

                craftingInventory.CheckItemAdd(agent, craftingDrop.Resource, craftingDrop.Count, OnItemAdd, OnRequestComplete);   
            }
        }
        
        public void Refresh()
        {
            LastUpdated = DateTime.Now;
        }

        public static void UpdateArray<T, L>(Dictionary<T, L> itterDict) where L : IUpdate
        {
            var values = itterDict.Values.ToArray();
            for (var i = 0; i < values.Length;)
            {
                var size = values.Length;
                values[i].Update();
                if (size != values.Length)
                {
                    values = itterDict.Values.ToArray();
                }
                else
                {
                    i++;
                }
            }
        }

        public void Update()
        {
            UpdateArray(CurrentParties);
            UpdateArray(BattleSystems);
        }
    }
}
