using EconomyProject.Scripts.MLAgents.Craftsman;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Crafting
{
    public class CraftingSlider : MonoBehaviour
    {
        public Slider slider;

        public CraftsmanUIControls CraftsmanUiControls => GetComponentInParent<CraftsmanUIControls>();
        public CraftingAbility CraftingAbility => CraftsmanUiControls.CraftsmanAgent.CraftingAbility;

        private void Update()
        {
            Time.timeScale = 1.0f;
            slider.value = CraftingAbility.Progress;
        }
    }
}
