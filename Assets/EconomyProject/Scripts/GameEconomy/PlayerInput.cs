using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy
{
    public class PlayerInput : AgentInput<AdventurerAgent, AgentScreen>
    {
        public MainMenuSystem mainMenuSystem;

        public AdventurerShopSystem adventurerShopSystem;

        public RequestAdventurerSystem requestSystem;

        public AdventurerSystem adventurerSystem;
        
        protected override EconomySystem<AdventurerAgent, AgentScreen> GetEconomySystem(AdventurerAgent agent)
        {
            var screen = GetScreen(agent, AgentScreen.Main);
            switch (screen)
            {
                case AgentScreen.Main:
                    return mainMenuSystem;
                case AgentScreen.Shop:
                    return adventurerShopSystem;
                case AgentScreen.Request:
                    return requestSystem;
                case AgentScreen.Adventurer:
                    return adventurerSystem;
            }
            return null;
        }

        public void SetAgentAction(AdventurerAgent agent, int action)
        {
            var system = GetEconomySystem(agent);
            system.SetChoice(agent, action);
        }

        public void Reset()
        {
            EconomyScreens.Clear();
        }
        
        protected override void SetupScreens()
        {
            mainMenuSystem.AgentInput = this;
            adventurerShopSystem.AgentInput = this;
            requestSystem.AgentInput = this;
            adventurerSystem.AgentInput = this;
        }
    }
}
