using System.Collections.Generic;
using System.Globalization;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.ShopUI.ScrollLists
{
    public class AgentShopScrollList : ShopScrollList
    {
        public GetCurrentAgent<Agent> currentAgent;

        public Text myGoldDisplay;

        public UpdateItemUi updateItem;

        public override List<ShopItem> ItemList => ShopAgent.ItemList;

        public ShopAgent ShopAgent => currentAgent.CurrentAgent.GetComponent<ShopAgent>();

        public double Gold
        {
            get
            {
                if (ShopAgent != null && ShopAgent.Wallet)
                {
                    return ShopAgent.Wallet.Money;
                }
                return 0;
            }
            set => ShopAgent.Wallet?.SetMoney(value);
        }

        public override void SelectItem(ShopItem item, int number = 1)
        {
            updateItem.SetVisible(item, marketPlace, ShopAgent);
        }

        public override bool RefreshDisplay()
        {
            var valid = base.RefreshDisplay();
            myGoldDisplay.text = "Gold: " + Gold.ToString(CultureInfo.InvariantCulture);
            return valid;
        }
    }
}
