using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public enum AdventureStates { OutOfBattle, InBattle, Quit }
    public class AdventurerSystem : StateEconomySystem<AdventureStates, AdventurerAgent, AgentScreen>
    {
        public PlayerInput playerInput;
        public FighterUnit playerUnit;

        public TravelSubSystem travelSubsystem;

        public Dictionary<AdventurerAgent, BattleSubSystem> battleSystems;
        protected override AgentScreen ActionChoice => AgentScreen.Adventurer;
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

        public BattleSubSystem GetSubSystem(AdventurerAgent agent)
        {
            if (battleSystems.ContainsKey(agent))
            {
                return battleSystems[agent];
            }
            return null;
        }

        private void SetupNewBattle(AdventurerAgent agent, FighterUnit enemyFighter)
        {
            if (battleSystems.ContainsKey(agent))
            {
                battleSystems.Remove(agent);
            }

            var newPlayer = FighterUnit.GenerateItem(playerUnit);
            var newEnemy = FighterUnit.GenerateItem(enemyFighter);
            
            var newSystem = new BattleSubSystem(newPlayer.data, newEnemy.data);
            battleSystems.Add(agent, newSystem);
            
            SetInputMode(agent, AdventureStates.InBattle);
        }

        protected override void GoBack(AdventurerAgent agent)
        {
            playerInput.ChangeScreen(agent, AgentScreen.Main);
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

        public void StartBattle(AdventurerAgent agent, BattleEnvironments battleEnvironments)
        {
            var fighter = travelSubsystem.GetBattle(battleEnvironments);

            SetupNewBattle(agent, fighter);
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
