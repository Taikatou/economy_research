using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Craftsman.Request
{
    public class DebugText : MonoBehaviour
    {
        public Text moneyText;
        public GetCurrentShopAgent shopAgent;

        // Update is called once per frame
        void Update()
        {
			if(shopAgent.CurrentAgent == null)
			{
				return;
			}
            moneyText.text = shopAgent.CurrentAgent.wallet.Money.ToString(CultureInfo.CurrentCulture);
        }
    }
}
