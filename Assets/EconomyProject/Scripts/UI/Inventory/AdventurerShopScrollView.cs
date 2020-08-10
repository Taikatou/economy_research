using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;

namespace EconomyProject.Scripts.UI.Inventory
{
    public class AdventurerShopScrollView : ShopScrollView
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        
        public override void SelectItem(ShopItemUi item, int number = 1)
        {
            shopSystem.PurchaseItem(shopAgent.CurrentAgent, item.Item.itemDetails, currentAdventurerAgent.CurrentAgent.wallet,
                currentAdventurerAgent.CurrentAgent.adventurerInventory.agentInventory);
        }
    }
}
