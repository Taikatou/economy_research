using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy
{
    public class ShopInput : AgentInput<ShopAgent, EShopScreen>
    {
        public ShopSystem shopSystem;
        public CraftingSystem craftSystem;
        public RequestShopSystem requestSystem;
        public ShopMarketPlace shopMarketPlace;
        protected override EconomySystem<ShopAgent, EShopScreen> GetEconomySystem(ShopAgent agent)
        {
            switch (GetScreen(agent, EShopScreen.Main))
            {
                case EShopScreen.Main:
                    return shopSystem;
                case EShopScreen.Craft:
                    return craftSystem;
                case EShopScreen.Request:
                    return requestSystem;
                case EShopScreen.MarketPlace:
                    return shopMarketPlace;
            }
            return null;
        }

        protected override void SetupScreens()
        {
            shopSystem.ShopInput = this;
            craftSystem.ShopInput = this;
            requestSystem.ShopInput = this;
            shopMarketPlace.ShopInput = this;
        }

        public void SetAction(ShopAgent agent, int action)
        {
            GetEconomySystem(agent).SetChoice(agent, action);
        }
    }
}
