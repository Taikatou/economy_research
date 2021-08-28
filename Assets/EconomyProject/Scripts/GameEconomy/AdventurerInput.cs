using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy
{
    public class AdventurerInput : AgentInput<AdventurerAgent, EAdventurerScreen>, IAdventureSense
    {
        public static MainMenuSystemBehaviour mainMenuSystem => FindObjectOfType<MainMenuSystemBehaviour>();

        public static AdventurerShopSystemBehaviour adventurerShopSystem  => FindObjectOfType<AdventurerShopSystemBehaviour>();

        public static RequestAdventurerSystemBehaviour requestSystem  => FindObjectOfType<RequestAdventurerSystemBehaviour>();

        public static AdventurerSystemBehaviour adventurerSystem  => FindObjectOfType<AdventurerSystemBehaviour>();

        public static EconomySystem<AdventurerAgent, EAdventurerScreen>[] GetSystems()
        {
            return new EconomySystem<AdventurerAgent, EAdventurerScreen>[]
            {
                mainMenuSystem.system,
                adventurerSystem.system,
                requestSystem.system,
                adventurerSystem.system
            };
        }

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

        public static int GetObservationLength()
        {
            var observationMaxSize = 0;
            foreach (var system in GetSystems())
            {
                if (system.ObservationSize > observationMaxSize)
                {
                    observationMaxSize = system.ObservationSize;
                }
            }

            return observationMaxSize;
        }

        public float[] GetObservations(AdventurerAgent agent)
        {
            return GetEconomySystem(agent).GetObservations(agent);
        }
    }
}
