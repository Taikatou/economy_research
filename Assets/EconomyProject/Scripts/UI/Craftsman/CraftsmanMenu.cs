using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman
{
    public class CraftsmanMenu : BaseMenuManager<EShopScreen>
    {
        public GameObject requestMenu;

        public GameObject craftMenu;

        public GameObject mainMenu;

        private CraftsmanUIControls CraftsmanUiControls => GetComponent<CraftsmanUIControls>();

        private void Update()
        {
            if (CraftsmanUiControls.CraftsmanAgent)
            {
                var nextScreen = CraftsmanUiControls.CraftsmanAgent.ChosenScreen;
                SwitchMenu(nextScreen);
            }
        }

        protected override Dictionary<EShopScreen, OpenedMenu> OpenedMenus => new Dictionary<EShopScreen, OpenedMenu>
        {
            { EShopScreen.Main, new OpenedMenu(new List<GameObject>{mainMenu}, new List<GameObject>{ craftMenu, requestMenu}) },
            { EShopScreen.Craft, new OpenedMenu(new List<GameObject>{craftMenu}, new List<GameObject>{ requestMenu, mainMenu}) },
            { EShopScreen.Request, new OpenedMenu(new List<GameObject>{requestMenu}, new List<GameObject>{ mainMenu, craftMenu }) }
        };

        protected override bool Compare(EShopScreen a, EShopScreen b)
        {
            return a == b;
        }

        protected override void SwitchMenu(EShopScreen whichMenu)
        {
            base.SwitchMenu(whichMenu);
            
            var children = craftMenu.GetComponentsInChildren<LastUpdate>();
            foreach (var child in children)
            {
                child.Refresh();
            }
        }
    }
}
