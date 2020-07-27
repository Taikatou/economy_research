using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    public class PlayerInput : AgentInput<AdventurerAgent, AgentScreen>
    {
        public GameAuction gameAuction;

        public GameQuests gameQuests;

        public MainMenuSystem mainMenuSystem;

        protected override EconomySystem<AdventurerAgent, AgentScreen> GetEconomySystem(AdventurerAgent agent)
        {
            switch (GetScreen(agent, AgentScreen.Main))
            {
                case AgentScreen.Main:
                    return mainMenuSystem;
                case AgentScreen.Quest:
                    return gameQuests;
                case AgentScreen.Auction:
                    return gameAuction;
            }
            return null;
        }

        public void SetAgentAction(AdventurerAgent agent, int action)
        {
            switch (GetScreen(agent, AgentScreen.Main))
            {
                case AgentScreen.Auction:
                    SetAuctionChoice(agent, action);
                        break;
                case AgentScreen.Main:
                    SetMainAction(agent, action);
                    break;
                case AgentScreen.Quest:
                    break;
            }
        }

        private void SetAuctionChoice(AdventurerAgent agent, int choice)
        {
            if (choice >= 0)
            {
                var action = (AuctionChoice) choice;
                SetAuctionChoice(agent, action);
            }
        }

        public void SetAuctionChoice(AdventurerAgent agent, AuctionChoice choice)
        {
            switch (choice)
            {
                case AuctionChoice.Ignore:
                    break;
                case AuctionChoice.Bid:
                    gameAuction.Bid(agent);
                    break;
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
