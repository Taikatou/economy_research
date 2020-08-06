using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class ShopScrollView : AbstractScrollList<ShopItem, ShopInventoryScrollButton>
    {
        public AgentShopSystem shopSystem;
        public GetCurrentShopAgent shopAgent;

        protected override LastUpdate LastUpdated => shopSystem;
        // Update is called once per frame
        protected override List<ShopItem> GetItemList() => shopSystem.GetShopItems(shopAgent.CurrentAgent);
        public override void SelectItem(ShopItem item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
