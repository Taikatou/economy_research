using UnityEngine;

namespace EconomyProject.Scripts.UI
{
    public class ToggleButton : MonoBehaviour
    {
        public GameObject craftMenu;

        public GameObject adventurerMenu;

        public bool craftActive;

        private void Start()
        {
            UpdateMenu();
        }

        public void SwitchButton()
        {
            craftActive = !craftActive;
            UpdateMenu();
        }

        private void UpdateMenu()
        {
            craftMenu.SetActive(craftActive);
            adventurerMenu.SetActive(!craftActive);
        }
    }
}
