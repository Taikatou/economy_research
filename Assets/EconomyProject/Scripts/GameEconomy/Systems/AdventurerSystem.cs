using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using TurnBased.Scripts;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public enum AdventureStates { OutOfBattle, InBattle, Quit }
    public class AdventurerSystem : StateEconomySystem<AdventureStates, AdventurerAgent, AgentScreen>
    {
        public PlayerInput playerInput;
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

        public override float[] GetSenses(AdventurerAgent agent)
        {
            return new[] {(float) GetInputMode(agent)};
        }

        protected override void MakeChoice(AdventurerAgent agent, int input)
        {
            switch (GetInputMode(agent))
            {
                case AdventureStates.OutOfBattle:
                    StartBattle(agent, (BattleEnvironments) input);
                    break;
                case AdventureStates.InBattle:
                    var battleSystem = GetSubSystem(agent);
                    battleSystem?.SetInput((BattleAction) input);
                    break;
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
                        CheckInBattle(player);
                        break;
                }
            }
        }

        private void CheckInBattle(AdventurerAgent agent)
        {
            var battleSystem = battleSystems[agent];
            if (battleSystem.GameOver())
            {
                switch (battleSystem.CurrentState)
                {
                    case BattleState.Lost:
                        SpendMoney(agent);
                        
                        break;
                    case BattleState.Won:
                        var craftingDrop = battleSystem.GetCraftingDropItem();
                        var craftingInventory = agent.GetComponent<AdventurerRequestTaker>();
                        
                        craftingInventory.CheckItemAdd(craftingDrop.Resource, craftingDrop.Count);
                        break;
                }
                SetInputMode(agent, AdventureStates.OutOfBattle);
            }
        }

        private void SpendMoney(AdventurerAgent agent)
        {
            agent.wallet.SpendMoney(5);
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

        public void OnFleeButton(AdventurerAgent agent)
        {
            if (battleSystems.ContainsKey(agent))
            {
                battleSystems[agent].OnFleeButton();
            }

            SetInputMode(agent, AdventureStates.OutOfBattle);
        }
    }
}
