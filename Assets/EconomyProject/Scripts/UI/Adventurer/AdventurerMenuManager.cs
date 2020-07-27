using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Adventurer
{

    public class AdventurerMenuManager : BaseMenuManager<AgentScreen>
    {
        public GameObject auctionMenu;

        public GameObject questMenu;

        public GameObject mainMenu;

        public GetCurrentAdventurerAgent getCurrentAgent;

        public UiAccessor uiAccessor;

        private AdventurerAgent AdventurerAgent => getCurrentAgent.CurrentAgent.GetComponent<AdventurerAgent>();

        private PlayerInput PlayerInput => uiAccessor.PlayerInput;

        protected override Dictionary<AgentScreen, OpenedMenu> OpenedMenus => new Dictionary<AgentScreen, OpenedMenu>
        {
            { AgentScreen.Auction, new OpenedMenu(new List<GameObject>{auctionMenu}, new List<GameObject>{questMenu, mainMenu}) },
            { AgentScreen.Main, new OpenedMenu(new List<GameObject>{mainMenu}, new List<GameObject>{questMenu, auctionMenu}) },
            { AgentScreen.Quest, new OpenedMenu(new List<GameObject>{questMenu}, new List<GameObject>{mainMenu, auctionMenu}) }
        };

        protected override bool Compare(AgentScreen a, AgentScreen b)
        {
            return a == b;
        }

        private void Update()
        {
            var playerInput = PlayerInput;
            if (getCurrentAgent.CurrentAgent != null && playerInput != null)
            {
                var screen = playerInput.GetScreen(AdventurerAgent, AgentScreen.Main);
                
                SwitchMenu(screen);
            }
        }

        public void StartAuction()
        {
            PlayerInput.ChangeScreen(AdventurerAgent, AgentScreen.Auction);
        }

        public void StartQuest()
        {
            PlayerInput.ChangeScreen(AdventurerAgent, AgentScreen.Quest);
        }

        public void MainMenu()
        {
            PlayerInput.ChangeScreen(AdventurerAgent, AgentScreen.Main);
        }

        public void Bid()
        {
            PlayerInput.SetAuctionChoice(AdventurerAgent, AuctionChoice.Bid);
        }
    }
}
