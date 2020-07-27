using EconomyProject.Scripts.GameEconomy.Systems;

namespace EconomyProject.Scripts.MLAgents.Shop
{
    public class ShopMarketPlace : EconomySystem<ShopAgent, EShopScreen>
    {
        public MarketPlace marketPlace;
        protected override EShopScreen ActionChoice => EShopScreen.MarketPlace;
        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }
    }
}
