using System;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman
{
    public class CraftsmanSystemSelector : MonoBehaviour
    {
        public ShopMainLocationSelect locationSelect;

        public Image[] buttons;

        public GetCurrentShopAgent currentShopAgent;

        private void Update()
        {
            var area = locationSelect.GetMenu(currentShopAgent.CurrentAgent);
            var index = 1;
            foreach (var i in buttons)
            {
                var highlight = (EShopScreen)index == area;
                i.color = highlight ? Color.green : Color.white;
                index++;
            }
        }
    }
}
