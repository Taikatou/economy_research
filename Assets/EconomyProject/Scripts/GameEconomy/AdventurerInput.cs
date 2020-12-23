using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy
{
    public class AdventurerInput : AgentInput<AdventurerAgent, EAdventurerScreen>, IAdventureSense
    {
        public MainMenuSystem mainMenuSystem;

        public AdventurerShopSystem adventurerShopSystem;

        public RequestAdventurerSystem requestSystem;

        public AdventurerSystem adventurerSystem;

        public AdventurerRest adventurerRest;
        
        public override EconomySystem<AdventurerAgent, EAdventurerScreen> GetEconomySystem(AdventurerAgent agent)
        {
            var screen = GetScreen(agent, EAdventurerScreen.Main);
            switch (screen)
            {
                case EAdventurerScreen.Main:
                    return mainMenuSystem;
                case EAdventurerScreen.Shop:
                    return adventurerShopSystem;
                case EAdventurerScreen.Request:
                    return requestSystem;
                case EAdventurerScreen.Adventurer:
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

        public float[] GetSenses(AdventurerAgent agent)
        {
            return GetEconomySystem(agent).GetSenses(agent);
        }
    }
}
