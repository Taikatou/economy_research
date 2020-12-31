using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerShopScrollView : AbstractScrollList<ShopItemUi, ShopInventoryScrollButton>
    {
        public ShopCraftingSystemBehaviour shopSubSystem;
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        public ShopChooserSubSystem shopChooserSubSystem;
        protected override ILastUpdate LastUpdated => shopSubSystem.system.shopSubSubSystem;
        
        private ShopAgent ShopAgent => shopChooserSubSystem.GetCurrentShop(currentAdventurerAgent.CurrentAgent);

        protected override List<ShopItemUi> GetItemList()
        {
            var toReturn = new List<ShopItemUi>();
            var items = shopSubSystem.system.shopSubSubSystem.GetShopItems(ShopAgent);
            foreach (var item in items)
            {
                toReturn.Add(new ShopItemUi
                {
                    Item = item,
                    Price = shopSubSystem.system.shopSubSubSystem.GetPrice(ShopAgent, item.itemDetails),
                    Number = shopSubSystem.system.shopSubSubSystem.GetNumber(ShopAgent, item.itemDetails)
                });
            }

            return toReturn;
        }

        public override void SelectItem(ShopItemUi item, int number = 1)
        {
            shopSubSystem.system.shopSubSubSystem.PurchaseItem(ShopAgent, item.Item.itemDetails, 
                currentAdventurerAgent.CurrentAgent.wallet,
                currentAdventurerAgent.CurrentAgent.adventurerInventory.agentInventory);
        }
    }
}
