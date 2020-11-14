using System;
using UnityEngine;

namespace EconomyProject.Scripts.UI
{
    public class ToggleButton : MonoBehaviour
    {
        public GameObject craftMenu;

        public GameObject adventurerMenu;

        public bool craftActive;

        private bool showPlayMenus;

        private void Start()
        {
            showPlayMenus = Math.Abs(Time.timeScale - 1) < 0.01f;
            UpdateMenu();
        }

        public void SwitchButton()
        {
            craftActive = !craftActive;
            UpdateMenu();
        }

        private void UpdateMenu()
        {
            if (showPlayMenus)
            {
                craftMenu.SetActive(craftActive);
                adventurerMenu.SetActive(!craftActive);   
            }
        }
    }
}
