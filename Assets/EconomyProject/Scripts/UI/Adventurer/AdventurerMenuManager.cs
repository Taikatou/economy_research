using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Adventurer
{
    [Serializable]
    public struct MenuGameObject
    {
        public GameObject menu;
        public AgentScreen screen;
    }

    public class AdventurerMenuManager : BaseMenuManager<AgentScreen>
    {
        public List<MenuGameObject> menuGameObjects;

        public GetCurrentAdventurerAgent getCurrentAgent;

        public UiAccessor uiAccessor;

        private AdventurerAgent AdventurerAgent => getCurrentAgent.CurrentAgent.GetComponent<AdventurerAgent>();

        private PlayerInput PlayerInput => uiAccessor.PlayerInput;

        private Dictionary<AgentScreen, OpenedMenu> _openedMenus;

        protected override Dictionary<AgentScreen, OpenedMenu> OpenedMenus => _openedMenus;

        private void Start()
        {
            _openedMenus = new Dictionary<AgentScreen, OpenedMenu>();
            foreach(var menu in menuGameObjects)
            {
                var closedMenus = new List<GameObject>();
                foreach (var menu2 in menuGameObjects)
                {
                    if (menu2.menu != menu.menu)
                    {
                        closedMenus.Add(menu2.menu);
                    }
                }
                _openedMenus.Add(menu.screen, new OpenedMenu(menu.menu, closedMenus));
            }
        }

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

        public void BattleMenu()
        {
            PlayerInput.ChangeScreen(AdventurerAgent, AgentScreen.Adventurer);
        }
    }
}
