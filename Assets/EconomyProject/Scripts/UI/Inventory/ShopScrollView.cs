using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
   
    public class ShopScrollView : AbstractScrollList<ShopItemUi, ShopInventoryScrollButton>
    {
        public AgentShopSubSystem shopSubSystem;
        public GetCurrentShopAgent shopAgent;
        protected override LastUpdate LastUpdated => shopSubSystem;
        // Update is called once per frame
        protected override List<ShopItemUi> GetItemList()
        {
            var toReturn = new List<ShopItemUi>();
            var items = shopSubSystem.GetShopItems(shopAgent.CurrentAgent);
            foreach (var item in items)
            {
                toReturn.Add(new ShopItemUi
                {
                    Item = item,
                    Price = shopSubSystem.GetPrice(shopAgent.CurrentAgent, item.itemDetails),
                    Number = shopSubSystem.GetNumber(shopAgent.CurrentAgent, item.itemDetails)
                });
            }

            return toReturn;
        }

        public override void SelectItem(ShopItemUi item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
