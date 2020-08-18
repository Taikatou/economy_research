using System;
using System.Collections.Generic;
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

        public AdventurerSystem battleSystem;


        protected override EconomySystem<AdventurerAgent, AgentScreen> GetEconomySystem(AdventurerAgent agent)
        {
            switch (GetScreen(agent, AgentScreen.Main))
            {
                case AgentScreen.Main:
                    return mainMenuSystem;
                case AgentScreen.Shop:
                    return adventurerShopSystem;
                case AgentScreen.Request:
                    return requestSystem;
                case AgentScreen.Adventurer:
                    return battleSystem;
            }
            return null;
        }

        public void SetAgentAction(AdventurerAgent agent, int action)
        {
            switch (GetScreen(agent, AgentScreen.Main))
            {
                
            }
        }

        enum MainAction {Stay = 0, Auction = 1, Quest = 2}

        private readonly Dictionary<MainAction, AgentScreen> _mainActionScreenMap = new Dictionary<MainAction, AgentScreen>
        {
            {MainAction.Auction, AgentScreen.Auction},
            {MainAction.Quest, AgentScreen.Quest},
        };
        
        private void SetMainAction(AdventurerAgent agent, int choice)
        {
            if (choice >= 0)
            {
                var action = (MainAction)choice;
                if (_mainActionScreenMap.ContainsKey(action))
                {
                    ChangeScreen(agent, _mainActionScreenMap[action]);   
                }
            }
        }

        public void Reset()
        {
            EconomyScreens.Clear();
        }
    }
}
