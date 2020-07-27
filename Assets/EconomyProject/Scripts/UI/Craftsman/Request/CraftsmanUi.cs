using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman;
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
        public ShopAgent CraftsmanAgent => CraftsmanUiControls.CraftsmanAgent;
        void Update()
        {
            var number = requestSystem.GetRequestNumber(CraftsmanAgent.CraftingInventory);
            requestText.text = number.ToString();
        }
    }
}
