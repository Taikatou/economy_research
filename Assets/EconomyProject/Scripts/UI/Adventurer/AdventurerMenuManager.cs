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
        public EAdventurerScreen screen;
    }

    public class AdventurerMenuManager : BaseMenuManager<EAdventurerScreen>
    {
        public List<MenuGameObject> menuGameObjects;
        public GetCurrentAdventurerAgent getCurrentAgent;
        public UiAccessor uiAccessor;

        private Dictionary<EAdventurerScreen, OpenedMenu> _openedMenus;
        private AdventurerAgent AdventurerAgent => getCurrentAgent.CurrentAgent.GetComponent<AdventurerAgent>();
        protected override Dictionary<EAdventurerScreen, OpenedMenu> OpenedMenus => _openedMenus;

        private void Start()
        {
            _openedMenus = new Dictionary<EAdventurerScreen, OpenedMenu>();
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

        protected override bool Compare(EAdventurerScreen a, EAdventurerScreen b)
        {
            return a == b;
        }

        private void Update()
        {
            var playerInput = uiAccessor.AdventurerInput;
            if (getCurrentAgent.CurrentAgent != null && playerInput != null)
            {
                var screen = playerInput.GetScreen(AdventurerAgent, EAdventurerScreen.Main);
                
                SwitchMenu(screen);
            }
        }

        public void MainMenu()
        {
			AdventurerAgent.SetAction(EAdventurerAgentChoices.MainMenu);
        }

        public void ShopMenu()
        {
			AdventurerAgent.SetAction(EAdventurerAgentChoices.Shop);
        }
        
        public void RequestMenu()
        {
			AdventurerAgent.SetAction(EAdventurerAgentChoices.FindRequest);
        }

        public void BattleMenu()
        {
			AdventurerAgent.SetAction(EAdventurerAgentChoices.Adventure);
        }
    }
}
