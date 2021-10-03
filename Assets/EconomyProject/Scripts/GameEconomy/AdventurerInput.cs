﻿using System.Collections.Generic;
using System.Linq;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy
{
    
    public class AdventurerInput : AgentInput<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>
    {
        private static MainMenuSystemBehaviour MainMenuSystem => FindObjectOfType<MainMenuSystemBehaviour>();
        private static AdventurerShopSystemBehaviour AdventurerShopSystem => FindObjectOfType<AdventurerShopSystemBehaviour>();
        private static RequestAdventurerSystemBehaviour RequestSystem => FindObjectOfType<RequestAdventurerSystemBehaviour>();
        public static AdventurerSystemBehaviour AdventurerSystem => FindObjectOfType<AdventurerSystemBehaviour>();

        private static EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>[] GetSystems()
        {
            return new EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>[]
            {
                MainMenuSystem.system,
                AdventurerSystem.system,
                RequestSystem.system,
                AdventurerSystem.system
            };
        }

        public override EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> GetEconomySystem(AdventurerAgent agent)
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

        public IEnumerable<EnabledInput> GetActionMask(AdventurerAgent agent)
        {
            var inputsEnabled = GetEconomySystem(agent).GetEnabledInputs(agent);
            return inputsEnabled;
        }
    }
}
