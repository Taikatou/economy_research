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

        public GameObject shopMenu;
        
        public GameObject requestMenu;

        public GetCurrentAdventurerAgent getCurrentAgent;

        public UiAccessor uiAccessor;

        private AdventurerAgent AdventurerAgent => getCurrentAgent.CurrentAgent.GetComponent<AdventurerAgent>();

        private PlayerInput PlayerInput => uiAccessor.PlayerInput;

        protected override Dictionary<AgentScreen, OpenedMenu> OpenedMenus => new Dictionary<AgentScreen, OpenedMenu>
        {
            { AgentScreen.Auction, new OpenedMenu(new List<GameObject>{auctionMenu}, new List<GameObject>{questMenu, mainMenu, shopMenu, requestMenu}) },
            { AgentScreen.Main, new OpenedMenu(new List<GameObject>{mainMenu}, new List<GameObject>{questMenu, auctionMenu, shopMenu, requestMenu}) },
            { AgentScreen.Quest, new OpenedMenu(new List<GameObject>{questMenu}, new List<GameObject>{mainMenu, auctionMenu, shopMenu, requestMenu}) },
            { AgentScreen.Shop, new OpenedMenu(new List<GameObject>{shopMenu}, new List<GameObject>{mainMenu, auctionMenu, questMenu, requestMenu}) },
            { AgentScreen.Request, new OpenedMenu(new List<GameObject>{requestMenu}, new List<GameObject>{shopMenu, mainMenu, auctionMenu, questMenu}) }
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

        public void ShopMenu()
        {
            PlayerInput.ChangeScreen(AdventurerAgent, AgentScreen.Shop);
        }
        
        public void RequestMenu()
        {
            PlayerInput.ChangeScreen(AdventurerAgent, AgentScreen.Request);
        }

        public void Bid()
        {
            PlayerInput.SetAuctionChoice(AdventurerAgent, AuctionChoice.Bid);
        }
    }
}
