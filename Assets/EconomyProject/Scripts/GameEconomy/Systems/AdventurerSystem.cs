using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public enum AdventureStates { OutOfBattle, InBattle, Quit }
    public class AdventurerSystem : StateEconomySystem<AdventureStates, AdventurerAgent, AgentScreen>
    {
        public FighterUnit playerUnit;
        public FighterUnit enemyUnit;
        public Dictionary<AdventurerAgent, BattleSubSystem> battleSystems;
        protected override AgentScreen ActionChoice => AgentScreen.Battle;
        protected override AdventureStates IsBackState => AdventureStates.Quit;
        protected override AdventureStates DefaultState => AdventureStates.OutOfBattle;

        public override void Start()
        {
            base.Start();
            battleSystems = new Dictionary<AdventurerAgent, BattleSubSystem>();
        }
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return GetInputMode(agent) != AdventureStates.InBattle;
        }
        protected override void MakeChoice(AdventurerAgent agent, int input)
        {
            
        }

        private void SetupNewBattle(AdventurerAgent agent)
        {
            if (battleSystems.ContainsKey(agent))
            {
                battleSystems.Remove(agent);
            }
            
            var newSystem = new BattleSubSystem(playerUnit, enemyUnit);
            battleSystems.Add(agent, newSystem);
        }

        protected override void GoBack(AdventurerAgent agent)
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            RequestDecisions();

            foreach (var player in CurrentPlayers)
            {
                var state = GetInputMode(player);
                switch (state)
                {
                    case AdventureStates.OutOfBattle:
                        SetupNewBattle(player);
                        SetInputMode(player, AdventureStates.InBattle);
                        break;
                    case AdventureStates.InBattle:
                        var battleSystem = battleSystems[player];
                        if (battleSystem.GameOver())
                        {
                            SetInputMode(player, AdventureStates.OutOfBattle);
                        }
                        break;
                }
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
    }
}
