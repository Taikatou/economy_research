﻿using System.Collections.Generic;
using Data;
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
            if (SystemTraining.IncludeShop)
            {
                return new EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>[]
                {
                    MainMenuSystem.system,
                    AdventurerSystem.system,
                    RequestSystem.system,
                    AdventurerSystem.system
                };
            }
            return new EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices>[]
            {
                AdventurerSystem.system,
            };
        }

        public override EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> GetEconomySystem(AdventurerAgent agent)
        {
            var screen = GetScreen(agent, TrainingConfig.StartScreen);
            IGetSystem system = null;
            switch (screen)
            {
                case EAdventurerScreen.Main:
                    system = MainMenuSystem;
                    break;
                case EAdventurerScreen.Shop:
                    system = AdventurerShopSystem;
                    break;
                case EAdventurerScreen.Request:
                    system = RequestSystem;
                    break;
                case EAdventurerScreen.Adventurer:
                    system = AdventurerSystem;
                    break;
            }

            if (system == null)
            {
                system = AdventurerSystem;
            }
            return system.GetSystem;
        }

        public void Reset()
        {
            EconomyScreens.Clear();
        }

        protected override void SetupScreens()
        {
            MainMenuSystem.system.AgentInput = this;
            if (AdventurerShopSystem != null)
            {
                AdventurerShopSystem.system.AgentInput = this;
            }

            if (RequestSystem != null)
            {
                RequestSystem.system.AgentInput = this;   
            }
            AdventurerSystem.system.AgentInput = this;
        }

        public IEnumerable<EnabledInput> GetActionMask(AdventurerAgent agent)
        {
            var inputsEnabled = GetEconomySystem(agent).GetEnabledInputs(agent);
            return inputsEnabled;
        }
    }
}
