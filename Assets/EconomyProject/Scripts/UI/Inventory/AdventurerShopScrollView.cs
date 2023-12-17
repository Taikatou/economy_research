using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerShopScrollView : AbstractScrollList<ShopItem, ShopInventoryScrollButton>
    {
        public ShopCraftingSystemBehaviour shopSubSystem;
        public GetCurrentAdventurerAgent currentAdventurerAgent;
		
		public AdventurerShopSystemBehaviour adventurerShopSystem;
        protected override ILastUpdate LastUpdated => shopSubSystem.system.shopSubSubSystem;

        protected override List<ShopItem> GetItemList()
        {
	        // TODO fix this please
	        var items = shopSubSystem.system.shopSubSubSystem.GetAllUsableItems();

	        var toreturn = new List<ShopItem>();
	        var i = 0;
	        foreach (var item in items)
	        {
		        toreturn.Add(new ShopItem
		        {
			        Seller = item.Item2,
			        Item=item.Item1,
			        Price = shopSubSystem.system.shopSubSubSystem.GetPrice(item.Item2, item.Item1.itemDetails),
			        Number = shopSubSystem.system.shopSubSubSystem.GetNumber(item.Item2, item.Item1.itemDetails),
			        Index = i
		        });
		        i++;
	        }
	        return toreturn;
        }

        protected override void Update()
        {
	        base.Update();

	        var index = adventurerShopSystem.system.adventurerShopSubSystem.GetCurrentLocation(
		        currentAdventurerAgent.CurrentAgent);
	        foreach (var button in buttons)
	        {
		        button.UpdateData(index);
	        }
        }

        public override void SelectItem(ShopItem item, int number = 1)
        {

		}
    }
}
