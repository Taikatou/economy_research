using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Craftsman
{
    public class CraftsmanUIControls : MonoBehaviour
    {
        public GetCurrentShopAgent getCurrentAgent;

        public ShopAgent shopAgent
		{
			get
			{
				if(getCurrentAgent.CurrentAgent == null)
				{
					return null;
				}
				return getCurrentAgent.CurrentAgent.GetComponent<ShopAgent>();
			}
		}


		public void MoveToRequest()
		{
			shopAgent.SetAction(EShopAgentChoices.RequestResource);
        }

        public void MoveToCraft()
        {
			shopAgent.SetAction(EShopAgentChoices.Craft);
        }

        public void ReturnToMain()
        {
			shopAgent.SetAction(EShopAgentChoices.Back);
        }
    }
}
