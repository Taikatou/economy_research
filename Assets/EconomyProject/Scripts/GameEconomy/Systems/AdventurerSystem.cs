using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.UI;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public enum AgentAdventureInput{ Quit=BattleEnvironments.Volcano+1 }
    public enum AdventureStates { OutOfBattle, InBattle}
    
    [Serializable]
    public class AdventurerSystem : EconomySystem<AdventurerAgent, EAdventurerScreen>
    {
		public TravelSubSystem travelSubsystem;

        public Dictionary<AdventurerAgent, BattleSubSystem> battleSystems;
        public Dictionary<AdventurerAgent, AdventureStates> adventureStates;
        public override int ObservationSize => 1 + BattleSubSystem.SensorCount;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Adventurer;

        public AdventurerSystem()
        {
            adventureStates = new Dictionary<AdventurerAgent, AdventureStates>();
            battleSystems = new Dictionary<AdventurerAgent, BattleSubSystem>();
        }

		public void ResetAdventurerSystem()
		{
			//End all battles 
			if(battleSystems != null)
			{
				foreach (var battle in battleSystems)
				{
					//Fully Heal adventurers
					battle.Value.PlayerFighterUnit.CurrentHp = battle.Value.PlayerFighterUnit.MaxHp;
					//End battles
					battle.Value.SetInput(BattleAction.Flee);
				}
			}
			
			adventureStates = new Dictionary<AdventurerAgent, AdventureStates>();
			battleSystems = new Dictionary<AdventurerAgent, BattleSubSystem>();
		}

        public AdventureStates GetAdventureStates(AdventurerAgent agent)
        {
            if (!adventureStates.ContainsKey(agent))
            {
                adventureStates.Add(agent, AdventureStates.OutOfBattle);
            }

            return adventureStates[agent];
        }

        private void SetAdventureState(AdventurerAgent agent, AdventureStates state)
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
            return GetAdventureStates(agent) != AdventureStates.InBattle;
        }

        public override float[] GetObservations(AdventurerAgent agent)
        {
            var state = GetAdventureStates(agent);
            var battleState = new float[1 + BattleSubSystem.SensorCount];
            battleState[0] = (float) state;
            if(state == AdventureStates.InBattle)
            {
                var subSystem = GetSubSystem(agent);
                subSystem.GetSubsystemObservations().CopyTo(battleState, 1);
				//Debug.Log(string.Join(",", battleState));
			}
            return battleState;
        }

        public override InputAction[] GetInputOptions(AdventurerAgent agent)
        {
            var outputs = new List<InputAction>();
            switch (GetAdventureStates(agent))
            {
                case AdventureStates.OutOfBattle:
                    outputs.AddRange(EconomySystemUtils.GetStateInput<BattleEnvironments>());
                    break;
                case AdventureStates.InBattle:
                    outputs.AddRange(EconomySystemUtils.GetStateInput<BattleAction>());
                    break;
            }
            outputs.AddRange(EconomySystemUtils.GetStateInput<AgentAdventureInput>());
            return outputs.ToArray();
        }

        public override void SetChoice(AdventurerAgent agent, int input)
        {
            if (Enum.IsDefined(typeof(AgentAdventureInput), input))
            {
                AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);
            }
            else
            {
                switch (GetAdventureStates(agent))
                {
                    case AdventureStates.OutOfBattle:
                        StartBattle(agent, (BattleEnvironments) input);
                        break;
                    case AdventureStates.InBattle:
                        var battleSystem = GetSubSystem(agent);
                        battleSystem.SetInput((BattleAction) input);
                        break;
                }   
            }
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
            
            SetAdventureState(agent, AdventureStates.InBattle);
        }

        private static void OnWin()
        {
            OverviewVariables.WonBattle();
        }
        
        public void Update()
        {
            RequestDecisions();

            foreach (var player in CurrentPlayers)
            {
                var state = GetAdventureStates(player);
                switch (state)
                {
                    case AdventureStates.OutOfBattle:
                        
                        break;
                    case AdventureStates.InBattle:
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
                            agent.AddReward(LearningStats.AdventurerLossPenalty);
                            break;
                        case BattleState.Won:
                            var craftingDrop = battleSystem.GetCraftingDropItem();
                            var craftingInventory = agent.GetComponent<AdventurerRequestTaker>();
                            
                            craftingInventory.CheckItemAdd(craftingDrop.Resource, craftingDrop.Count);
                            agent.AddReward(LearningStats.AdventurerWinReward);
                            break;
                    }

                    battleSystems.Remove(agent);
                    SetAdventureState(agent, AdventureStates.OutOfBattle);
                }   
            }
        }

        private void SpendMoney(AdventurerAgent agent)
        {
            agent.wallet.SpendMoney(5);
        }

        public void StartBattle(AdventurerAgent agent, BattleEnvironments battleEnvironments)
        {
            var fighter = travelSubsystem.GetBattle(battleEnvironments);

            if (fighter)
            {
                SetupNewBattle(agent, fighter);
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

            SetAdventureState(agent, AdventureStates.OutOfBattle);
        }
        
        public override EnabledInput[] GetEnabledInputs()
        {
            var inputChoices = new[]
            {
                EAdventurerAgentChoices.BattleAttack,
                EAdventurerAgentChoices.BattleFlee,
                EAdventurerAgentChoices.BattleFlee,
                EAdventurerAgentChoices.AdventureForest,
                EAdventurerAgentChoices.AdventureSea,
                EAdventurerAgentChoices.AdventureMountain,
                EAdventurerAgentChoices.AdventureVolcano
            };
            var outputs = AdventurerEconomySystemUtils.GetInputOfType(inputChoices);

            return outputs;
        }
    }
}
