using System.Collections.Generic;
using System.Linq;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy
{
    
    public class AdventurerInput : AgentInput<AdventurerAgent, EAdventurerScreen>, IAdventureSense
    {
        private static MainMenuSystemBehaviour MainMenuSystem => FindObjectOfType<MainMenuSystemBehaviour>();
        private static AdventurerShopSystemBehaviour AdventurerShopSystem => FindObjectOfType<AdventurerShopSystemBehaviour>();
        private static RequestAdventurerSystemBehaviour RequestSystem => FindObjectOfType<RequestAdventurerSystemBehaviour>();
        public static AdventurerSystemBehaviour AdventurerSystem => FindObjectOfType<AdventurerSystemBehaviour>();

        private static EconomySystem<AdventurerAgent, EAdventurerScreen>[] GetSystems()
        {
            return new EconomySystem<AdventurerAgent, EAdventurerScreen>[]
            {
                MainMenuSystem.system,
                AdventurerSystem.system,
                RequestSystem.system,
                AdventurerSystem.system
            };
        }

        public override EconomySystem<AdventurerAgent, EAdventurerScreen> GetEconomySystem(AdventurerAgent agent)
        {
            var screen = GetScreen(agent, EAdventurerScreen.Main);
            switch (screen)
            {
                case EAdventurerScreen.Main:
                    return MainMenuSystem.system;
                case EAdventurerScreen.Shop:
                    return AdventurerShopSystem.system;
                case EAdventurerScreen.Request:
                    return RequestSystem.system;
                case EAdventurerScreen.Adventurer:
                    return AdventurerSystem.system;
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
            MainMenuSystem.system.AgentInput = this;
            AdventurerShopSystem.system.AgentInput = this;
            RequestSystem.system.AgentInput = this;
            AdventurerSystem.system.AgentInput = this;
        }

        public static int GetObservationLength()
        {
            return GetSystems().Select(system => system.ObservationSize).Prepend(0).Max();
        }

        public IEnumerable<EnabledInput> GetActionMask(AdventurerAgent agent)
        {
            var inputsEnabled = GetEconomySystem(agent).GetEnabledInputs();
            return inputsEnabled;
        }

        public float[] GetObservations(AdventurerAgent agent)
        {
            return GetEconomySystem(agent).GetObservations(agent);
        }
    }
}
