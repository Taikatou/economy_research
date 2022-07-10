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

namespace EconomyProject.Scripts.GameEconomy.Systems.Adventurer
{
    public delegate void SetAdventureState(AdventurerAgent agent, EAdventureStates state);
    public class BattleSubSystem : ILastUpdate
    {
        private readonly SetAdventureState _setAdventureState;
        
        public Dictionary<AdventurerAgent, BattleSubSystemInstance<AdventurerAgent>> BattleSystems { get; }
        public Dictionary<EBattleEnvironments, BattlePartySubsystem> CurrentParties { get; private set; }
        private Dictionary<AdventurerAgent, EBattleEnvironments> ReverseCurrentParties { get; }
        
        private static IEnumerable<EBattleEnvironments> BattleAsArray =>
            Enum.GetValues(typeof(EBattleEnvironments)).Cast<EBattleEnvironments>().ToArray();

        public static int SensorCount => BattleSubSystemInstance<AdventurerAgent>.SensorCount + Adventurer.ConfirmAbilities.SensorCount;

        public DateTime LastUpdated { get; private set; }


        public BattleSubSystem(TravelSubSystem travelSubsystem, SetAdventureState setAdventureState, BattleEnvironmentDataLogger dataLogger)
        {
            _setAdventureState = setAdventureState;
            
            void SetupNewBattle(AdventurerAgent[] agents, FighterObject enemyFighter, SimpleMultiAgentGroup party, Dictionary<AdventurerAgent, HashSet<EAttackOptions>> selectedOptions)
            {
                foreach (var agent in agents)
                {
                    ReverseCurrentParties.Remove(agent);
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

            void AskConfirmation(AdventurerAgent[] agents, FighterObject enemyFighter, SimpleMultiAgentGroup party, Dictionary<AdventurerAgent, HashSet<EAttackOptions>> selectedOptions)
            {
                foreach (var agent in agents)
                {
                    _setAdventureState.Invoke(agent, EAdventureStates.ConfirmBattle);
                }
            }

            void AskConfirmAbilities(AdventurerAgent[] agents, FighterObject enemyFighter, SimpleMultiAgentGroup party, Dictionary<AdventurerAgent, HashSet<EAttackOptions>> selectedOptions)
            {
                foreach (var agent in agents)
                {
                    _setAdventureState.Invoke(agent, EAdventureStates.ConfirmAbilities);
                }
            }

            void CreateBattleSystem(EBattleEnvironments battle)
            {
                var party = new BattlePartySubsystem(SystemTraining.PartySize, battle, travelSubsystem, dataLogger);
                if (CurrentParties.ContainsKey(battle))
                {
                    CurrentParties.Remove(battle);
                }

                CurrentParties[battle] = party;
                party.SetupNewBattle = SetupNewBattle;
                party.SetupNewBattle += (_, _, _, _) => CreateBattleSystem(battle);
                
                party.AskConfirmation = AskConfirmation;
                party.AskConfirmAbilities = AskConfirmAbilities;
                party.CancelAgent = RemoveAgent;
            }
            BattleSystems = new Dictionary<AdventurerAgent, BattleSubSystemInstance<AdventurerAgent>>();
            CurrentParties = new Dictionary<EBattleEnvironments, BattlePartySubsystem>();
            ReverseCurrentParties = new Dictionary<AdventurerAgent, EBattleEnvironments>();
            foreach (var battle in BattleAsArray)
            {
                CreateBattleSystem(battle);
            }
        }

        public ObsData[] GetObs(AdventurerAgent agent)
        {
            return CurrentParties[ReverseCurrentParties[agent]].confirmAbilities.GetObservations(agent);
        }

        public void CancelConfirmation(AdventurerAgent agent)
        {
            if (ReverseCurrentParties.ContainsKey(agent))
            {
                CurrentParties[ReverseCurrentParties[agent]].Confirmation(EConfirmBattle.Back, agent);
            }
        }

        public void CancelAbilities(AdventurerAgent agent)
        {
            if (ReverseCurrentParties.ContainsKey(agent))
            {
                CurrentParties[ReverseCurrentParties[agent]].CancelConfirmation();
            }
        }

        public void Confirmation(EConfirmBattle confirmation, AdventurerAgent agent)
        {
            CurrentParties[ReverseCurrentParties[agent]].Confirmation(confirmation, agent);
            Refresh();
        }

        public void ConfirmAbilities(EAttackOptions confirmation, AdventurerAgent agent)
        {
            var party = CurrentParties[ReverseCurrentParties[agent]];
            party.confirmAbilities.ConfirmAbility(agent, confirmation);
            if (party.confirmAbilities.Complete(SystemTraining.PartySize))
            {
                party.StartBattle();
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

            foreach (var system in BattleSystems)
            {
                system.Value.FinishBattle();
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
            if (!CurrentParties[location].Full)
            {
                _setAdventureState.Invoke(agent, EAdventureStates.InQueue);
                CurrentParties[location].AddAgent(agent);
                ReverseCurrentParties.Add(agent, location);   
            }
        }

        public void RemoveAgent(AdventurerAgent agent)
        {
            if (ReverseCurrentParties.ContainsKey(agent))
            {
                CurrentParties[ReverseCurrentParties[agent]].RemoveFromQueue(agent);
                ReverseCurrentParties.Remove(agent);
                _setAdventureState.Invoke(agent, EAdventureStates.OutOfBattle);   
            }
        }

        public PartySubSystem<AdventurerAgent> GetSubsystem(AdventurerAgent agent)
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

        public EBattleEnvironments? GetBattleEnvironment(AdventurerAgent agent)
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

        private void OnComplete(BattleSubSystemInstance<AdventurerAgent> systemInstance)
        {
            foreach (var agent in systemInstance.BattleAgents)
            {
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
        
        public void Refresh()
        {
            LastUpdated = DateTime.Now;
        }

        public void Update()
        {
            var values = CurrentParties.Values.ToArray();
            for (var i = 0; i < values.Length;)
            {
                var size = values.Length;
                values[i].Update();
                if (size != values.Length)
                {
                    values = CurrentParties.Values.ToArray();
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
