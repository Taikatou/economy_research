using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.UI.ShopUI
{
    public class AdventurerShopDropDown : AgentDropDown<ShopAgent, EShopScreen>
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        public ShopChooserSubSystem shopChooserSubSystem;
        protected override GetCurrentAgent<ShopAgent> GetCurrentAgent => shopChooserSubSystem;

        protected override void Update()
        {
            shopChooserSubSystem.GetCurrentShop(currentAdventurerAgent.CurrentAgent);
        }

        protected override void UpdateAgent(ShopAgent agent)
        {
            shopChooserSubSystem.SetShopAgent(currentAdventurerAgent.CurrentAgent, agent);
        }
    }
}
