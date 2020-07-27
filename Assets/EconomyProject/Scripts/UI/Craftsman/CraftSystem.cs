using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.UI.Craftsman
{
    public class CraftSystem : EconomySystem<ShopAgent, EShopScreen>
    {
        protected override EShopScreen ActionChoice => EShopScreen.Craft;
        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }
    }
}
