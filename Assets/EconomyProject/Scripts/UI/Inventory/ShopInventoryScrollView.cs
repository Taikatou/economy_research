using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using Inventory;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.UI.Inventory
{
    public struct ItemUi
    {
        public UsableItemDetails ItemDetails;
        public int Number;
    }
    public struct ShopItemUi
    {
		public ShopAgent Seller;
        public UsableItem Item;
        public int Number;
        public int Price;
    }
    public class ShopInventoryScrollView : AbstractScrollList<ShopItemUi, ShopInventoryScrollButton>
    {
        public ShopCraftingSystemBehaviour shopSubSystem;

        public GetCurrentShopAgent shopAgent;
        protected override ILastUpdate LastUpdated => shopAgent.CurrentAgent.agentInventory;

        protected override List<ShopItemUi> GetItemList()
        {
            var itemList = new List<ShopItemUi>();
            foreach (var item in shopAgent.CurrentAgent.agentInventory.Items)
            {
                var price = shopSubSystem.system.shopSubSubSystem.GetPrice(shopAgent.CurrentAgent, item.Value[0].itemDetails);
                itemList.Add(new ShopItemUi { Item=item.Value[0], Number=item.Value.Count, Price=price});
            }
            return itemList;
        }

        public override void SelectItem(ShopItemUi shopItem, int number = 1)
        {
			shopAgent.CurrentAgent.SetAction(EShopAgentChoices.SubmitToShop, null, null, shopItem.Item);
		}
    }
}
