using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman
{
    public class CraftsmanUIControls : MonoBehaviour
    {
        public GetCurrentShopAgent getCurrentAgent;

        public ShopInput shopInput;

        public ShopAgent CraftsmanAgent => getCurrentAgent.CurrentAgent.GetComponent<ShopAgent>();
        
        public void MoveToRequest()
        {
            shopInput.ChangeScreen(getCurrentAgent.CurrentAgent, EShopScreen.Request);
        }

        public void MoveToCraft()
        {
            shopInput.ChangeScreen(getCurrentAgent.CurrentAgent, EShopScreen.Craft);
        }

        public void ReturnToMain()
        {
            shopInput.ChangeScreen(getCurrentAgent.CurrentAgent, EShopScreen.Main);
        }
    }
}
