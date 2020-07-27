using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.UI.ShopUI
{
    public class ShopAgentDropDown : AgentDropDown<ShopAgent, EShopScreen>
    {
        protected override GetCurrentAgent<ShopAgent> GetCurrentAgent => shopAgent;

        public GetCurrentShopAgent shopAgent;
    }
}
