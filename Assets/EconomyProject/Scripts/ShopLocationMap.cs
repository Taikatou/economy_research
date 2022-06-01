using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts
{
    public class ShopLocationMap :  LocationSelect<ShopAgent>
    {
        public ShopCraftingSystem shopSubSystem { get; set; }
        public override int GetLimit(ShopAgent agent)
        {
            var items = shopSubSystem.shopSubSubSystem.GetShopUsableItems(agent);
            return items.Count;
        }
    }
}
