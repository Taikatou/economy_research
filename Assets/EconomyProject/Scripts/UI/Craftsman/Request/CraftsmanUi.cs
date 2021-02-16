using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Request
{
    public class CraftsmanUi : MonoBehaviour
    {
        public Text requestText;

        public RequestSystem requestSystem;
        public CraftsmanUIControls CraftsmanUiControls => GetComponentInParent<CraftsmanUIControls>();
        public ShopAgent CraftsmanAgent => CraftsmanUiControls.shopAgent;
        void Update()
        {
			if(CraftsmanAgent == null)
			{
				return;
			}
            var number = requestSystem.GetRequestNumber(CraftsmanAgent.craftingInventory);
            requestText.text = number.ToString();
        }
    }
}
