using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
    public struct ItemUi
    {
        public UsableItemDetails ItemDetails;
        public int Number;
    }
    public struct ShopItemUi
    {
        public UsableItem Item;
        public int Number;
        public int Price;
    }
    public class ShopInventoryScrollView : AbstractScrollList<ShopItemUi, ShopInventoryScrollButton>
    {
        public AgentShopSystem shopSystem;

        public GetCurrentShopAgent shopAgent;
        protected override LastUpdate LastUpdated => shopAgent.CurrentAgent.AgentInventory;

        protected override List<ShopItemUi> GetItemList()
        {
            var itemList = new List<ShopItemUi>();
            foreach (var item in shopAgent.CurrentAgent.AgentInventory.Items)
            {
                var price = shopSystem.GetCurrentPrice(shopAgent.CurrentAgent, item.Value[0].itemDetails);
                itemList.Add(new ShopItemUi { Item=item.Value[0], Number=item.Value.Count, Price=price});
            }
            return itemList;
        }

        public override void SelectItem(ShopItemUi shopItem, int number = 1)
        {
            shopSystem.SubmitToShop(shopAgent.CurrentAgent, shopItem.Item, shopItem.Number);
        }
    }
}
