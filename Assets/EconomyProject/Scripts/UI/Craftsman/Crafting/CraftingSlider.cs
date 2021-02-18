using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Crafting
{
    public class CraftingSlider : MonoBehaviour
    {
        public Slider slider;
        public ShopCraftingSystemBehaviour shopSystem;
        public GetCurrentShopAgent getCurrentAgent;

        private void Update()
        {
			if(getCurrentAgent.CurrentAgent == null)
			{
				return;
			}
            Time.timeScale = 1.0f;
            slider.value = shopSystem.system.craftingSubSubSystem.Progress(getCurrentAgent.CurrentAgent);
        }
    }
}
