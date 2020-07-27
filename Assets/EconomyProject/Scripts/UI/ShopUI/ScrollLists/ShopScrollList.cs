using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.ShopUI.Buttons;

namespace EconomyProject.Scripts.UI.ShopUI.ScrollLists
{
    public abstract class ShopScrollList : AbstractScrollList<ShopItem, ShopButton>
    {
        public MarketPlace marketPlace;

        public override LastUpdate LastUpdated => marketPlace;
    }
}
