using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public enum AgentAdventureInput{Quit=BattleEnvironments.Volcano+1}
    public enum AdventureStates { OutOfBattle, InBattle}
    public class AdventurerSystem : EconomySystem<AdventurerAgent, EAdventurerScreen>
    { 
        public TravelSubSystem travelSubsystem;

        public Dictionary<AdventurerAgent, BattleSubSystem> battleSystems;
        public Dictionary<AdventurerAgent, AdventureStates> adventureStates;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Adventurer;

        public override void Start()
        {
            base.Start();
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

        public override float[] GetSenses(AdventurerAgent agent)
        {
            var battleState = new float[1 + BattleSubSystem.GetSenseSize];
            var state = GetAdventureStates(agent);
            battleState[0] = (float) state;
            if(state == AdventureStates.InBattle)
            {
                var subSystem = GetSubSystem(agent);
                subSystem.GetSenses(agent).CopyTo(battleState, 1);
            }
            Debug.Log(string.Join(",", battleState));
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
            
            var newSystem = new BattleSubSystem(playerData, enemyData, enemyFighter.fighterDropTable);
            battleSystems.Add(agent, newSystem);
            
            SetAdventureState(agent, AdventureStates.InBattle);
        }
        
        private void Update()
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
                            
                            break;
                        case BattleState.Won:
                            var craftingDrop = battleSystem.GetCraftingDropItem();
                            var craftingInventory = agent.GetComponent<AdventurerRequestTaker>();
                            
                            craftingInventory.CheckItemAdd(craftingDrop.Resource, craftingDrop.Count);
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
    }
}
