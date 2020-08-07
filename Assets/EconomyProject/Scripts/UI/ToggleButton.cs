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
            SwitchButton();
        }

        public void SwitchButton()
        {
            craftMenu.SetActive(craftActive);
            adventurerMenu.SetActive(!craftActive);
            craftActive = !craftActive;
        }
    }
}
