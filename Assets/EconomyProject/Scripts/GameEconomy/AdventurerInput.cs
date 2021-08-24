using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy
{
    public class AdventurerInput : AgentInput<AdventurerAgent, EAdventurerScreen>, IAdventureSense
    {
        public MainMenuSystemBehaviour mainMenuSystem;

        public AdventurerShopSystemBehaviour adventurerShopSystem;

        public RequestAdventurerSystemBehaviour requestSystem;

        public AdventurerSystemBehaviour adventurerSystem;

        public override EconomySystem<AdventurerAgent, EAdventurerScreen> GetEconomySystem(AdventurerAgent agent)
        {
            var screen = GetScreen(agent, EAdventurerScreen.Main);
            switch (screen)
            {
                case EAdventurerScreen.Main:
                    return mainMenuSystem.system;
                case EAdventurerScreen.Shop:
                    return adventurerShopSystem.system;
                case EAdventurerScreen.Request:
                    return requestSystem.system;
                case EAdventurerScreen.Adventurer:
                    return adventurerSystem.system;
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
            mainMenuSystem.system.AgentInput = this;
            adventurerShopSystem.system.AgentInput = this;
            requestSystem.system.AgentInput = this;
            adventurerSystem.system.AgentInput = this;
        }

        public float[] GetObservations(AdventurerAgent agent, int limit)
        {
            return GetEconomySystem(agent).GetObservations(agent, limit);
        }
    }
}
