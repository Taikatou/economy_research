using System;
using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Adventurer
{
    [Serializable]
    public struct AdventurerGameObject
    {
        public GameObject menu;
        public EAdventureStates screen;
    }
    public class AdventurerBattleMenu : BaseMenuManager<EAdventureStates>
    {
        public AdventurerSystemBehaviour adventurerSystem;
        public GetCurrentAdventurerAgent getCurrentAdventurerAgent;
        public List<AdventurerGameObject> adventurerGameObjects;

        private Dictionary<EAdventureStates, OpenedMenu> _openedMenus;
        protected override Dictionary<EAdventureStates, OpenedMenu> OpenedMenus => _openedMenus;
        public void Start()
        {
            _openedMenus = new Dictionary<EAdventureStates, OpenedMenu>();
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
        protected override bool Compare(EAdventureStates a, EAdventureStates b)
        {
            return a == b;
        }
        
        private void Update()
        {
            var state = adventurerSystem.system.GetAdventureStates(getCurrentAdventurerAgent.CurrentAgent);
            
            SwitchMenu(state);
        }
    }
}
