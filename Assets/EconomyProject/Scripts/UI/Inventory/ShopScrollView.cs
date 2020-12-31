using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
   
    public class ShopScrollView : AbstractScrollList<ShopItemUi, ShopInventoryScrollButton>
    {
        public ShopCraftingSystemBehaviour shopSubSystem;
        public GetCurrentShopAgent shopAgent;
        protected override ILastUpdate LastUpdated => shopSubSystem.system.shopSubSubSystem;
        // Update is called once per frame
        protected override List<ShopItemUi> GetItemList()
        {
            var toReturn = new List<ShopItemUi>();
            var items = shopSubSystem.system.shopSubSubSystem.GetShopItems(shopAgent.CurrentAgent);
            foreach (var item in items)
            {
                toReturn.Add(new ShopItemUi
                {
                    Item = item,
                    Price = shopSubSystem.system.shopSubSubSystem.GetPrice(shopAgent.CurrentAgent, item.itemDetails),
                    Number = shopSubSystem.system.shopSubSubSystem.GetNumber(shopAgent.CurrentAgent, item.itemDetails)
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
