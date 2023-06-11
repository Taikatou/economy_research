using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using LevelSystem;
using UnityEngine;
using UnityEngine.UI;

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

                if (menu.screen == EAdventurerScreen.Request && TrainingConfig.AdventurerNoRequestMenu)
                {
                    
                }
                else
                {
                    _openedMenus.Add(menu.screen, new OpenedMenu(menu.menu, closedMenus));   
                }
            }
        }

        protected override bool Compare(EAdventurerScreen a, EAdventurerScreen b)
        {
            return a == b;
        }

        public Text levelText;

        private void Update()
        {
            var playerInput = uiAccessor.AdventurerInput;
            if (getCurrentAgent.CurrentAgent != null && playerInput != null)
            {
                var levelUpComponent = getCurrentAgent.CurrentAgent.GetComponent<LevelUpComponent>();
                levelText.text = levelUpComponent.Level.ToString();
                var screen = playerInput.GetScreen(AdventurerAgent, TrainingConfig.StartScreen);

                if (_openedMenus.ContainsKey(screen))
                {
                    SwitchMenu(screen);   
                }
            }
        }
    }
}
