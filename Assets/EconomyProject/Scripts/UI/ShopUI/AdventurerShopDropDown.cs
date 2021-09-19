using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.UI.ShopUI
{
    public class AdventurerShopDropDown : AgentDropDown<ShopAgent, EShopScreen>
    {
        public GetCurrentAdventurerAgent currentAdventurerAgent;
        public ShopChooserSubSystem shopChooserSubSystem;

        public AdventurerShopSystemBehaviour adventurerShopSystem;
        protected override GetCurrentAgent<ShopAgent> GetCurrentAgent => shopChooserSubSystem;

        protected override void Start()
        {
            base.Start();

            shopChooserSubSystem.onShopChange = updateValue;
        }

        private void updateValue(int index)
        {
            dropDown.value = index;
        }

        protected override void Update()
        {
            base.Update();
            shopChooserSubSystem.GetCurrentShop(currentAdventurerAgent.CurrentAgent);
        }

        protected override void UpdateAgent(ShopAgent agent)
        {
            shopChooserSubSystem.SetShopAgent(currentAdventurerAgent.CurrentAgent, agent);
        }

        protected override bool Selected()
        {
            var selectShop = adventurerShopSystem.system.GetChoice(currentAdventurerAgent.CurrentAgent) ==
                                    ESelectionState.SelectShop;
            return selectShop;
        }
    }
}
