using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.UI.ShopUI.ScrollLists
{
    public class PooledShopScrollList : ShopScrollList
    {
        public AgentShopScrollList otherShop;
        public override List<ShopItem> ItemList => marketPlace.ItemList;

        public override void SelectItem(ShopItem item, int number = 1)
        {
            marketPlace.TryTransferItemToOtherShop(item, otherShop);
        }
    }
}
