using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EconomyProject.Scripts.UI
{
    public class OpenedMenu
    {
        private readonly List<GameObject> _openedMenus;
        private readonly List<GameObject> _closedMenus;

        public OpenedMenu(List<GameObject> openedMenus, List<GameObject> closedMenus)
        {
            _openedMenus = openedMenus;
            _closedMenus = closedMenus;
        }

        public OpenedMenu(GameObject openedMenus, List<GameObject> closedMenus)
        {
            _openedMenus = new List<GameObject> {openedMenus};
            _closedMenus = closedMenus;
        }

        public void Activate()
        {
            OpenClose(true, _openedMenus);
            OpenClose(false, _closedMenus);
        }

        private void OpenClose(bool open, List<GameObject> menus)
        {
            foreach (var toOpen in menus)
            {
                toOpen.SetActive(open);
            }
        }
    }
    public abstract class BaseMenuManager <T> : MonoBehaviour where T : Enum
    {
        private bool _valid;
        private T CacheAgentScreen { get; set; }

        protected abstract Dictionary<T, OpenedMenu> OpenedMenus { get; }

        protected abstract bool Compare(T a, T b);

        protected virtual bool SwitchMenu(T whichMenu)
        {
            var toReturn = false;
            var same = Compare(whichMenu, CacheAgentScreen);
            if (!same || !_valid)
            {
                _valid = true;
                CacheAgentScreen = whichMenu;
                var openedMenu = OpenedMenus[whichMenu];
                openedMenu.Activate();
                Debug.Log(whichMenu);
                toReturn = true;
            }

            return toReturn;
        }
    }
}
