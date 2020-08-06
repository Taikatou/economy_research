using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    public class ShopInput : AgentInput<ShopAgent, EShopScreen>
    {
        public MainShopSystem mainSystem;
        public ShopCraftingSystem shopCraftingSystem;
        public RequestShopSystem requestSystem;

        public override void Start()
        {
            base.Start();
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
        }
        protected override EconomySystem<ShopAgent, EShopScreen> GetEconomySystem(ShopAgent agent)
        {
            switch (GetScreen(agent, EShopScreen.Main))
            {
                case EShopScreen.Main:
                    return mainSystem;
                case EShopScreen.Craft:
                    return shopCraftingSystem;
                case EShopScreen.Request:
                    return requestSystem;
            }
            return null;
        }

        protected override void SetupScreens()
        {
            mainSystem.ShopInput = this;
            shopCraftingSystem.ShopInput = this;
            requestSystem.ShopInput = this;
        }

        public void SetAction(ShopAgent agent, int action)
        {
            GetEconomySystem(agent).SetChoice(agent, action);
        }
    }
}
