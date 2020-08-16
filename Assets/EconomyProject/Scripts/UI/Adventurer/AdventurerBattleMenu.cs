using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Adventurer
{
    [Serializable]
    public struct AdventurerGameObject
    {
        public GameObject menu;
        public AdventureStates screen;
    }
    public class AdventurerBattleMenu : BaseMenuManager<AdventureStates>
    {
        public AdventurerSystem adventurerSystem;
        public GetCurrentAdventurerAgent getCurrentAdventurerAgent;
        public List<AdventurerGameObject> adventurerGameObjects;

        private Dictionary<AdventureStates, OpenedMenu> _openedMenus;
        protected override Dictionary<AdventureStates, OpenedMenu> OpenedMenus => _openedMenus;
        public void Start()
        {
            _openedMenus = new Dictionary<AdventureStates, OpenedMenu>();
            foreach (var menu in adventurerGameObjects)
            {
                var closedMenus = new List<GameObject>();
                foreach (var menu2 in adventurerGameObjects)
                {
                    if (menu2.menu != menu.menu)
                    {
                        closedMenus.Add(menu2.menu);
                    }
                }

                _openedMenus.Add(menu.screen, new OpenedMenu(menu.menu, closedMenus));
            }
        }
        protected override bool Compare(AdventureStates a, AdventureStates b)
        {
            return a == b;
        }
        
        private void Update()
        {
            var state = adventurerSystem.GetInputMode(getCurrentAdventurerAgent.CurrentAgent);
            
            SwitchMenu(state);
        }
    }
}
