using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    public class ShopInput : AgentInput<ShopAgent, EShopScreen>, IShopSense
    {
        public MainShopSystemBehaviour mainSystem;
        public ShopCraftingSystemBehaviour shopCraftingSystem;
        public RequestShopSystemBehaviour requestSystem;

        public override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
        }
        
        public override EconomySystem<ShopAgent, EShopScreen> GetEconomySystem(ShopAgent agent)
        {
            switch (GetScreen(agent, EShopScreen.Main))
            {
                case EShopScreen.Main:
                    return mainSystem.system;
                case EShopScreen.Craft:
                    return shopCraftingSystem.system;
                case EShopScreen.Request:
                    return requestSystem.system;
            }
            return null;
        }

        protected override void SetupScreens()
        {
            mainSystem.system.AgentInput = this;
            shopCraftingSystem.system.AgentInput = this;
            requestSystem.system.AgentInput = this;
        }

        public void SetAction(ShopAgent agent, int action)
        {
            GetEconomySystem(agent).SetChoice(agent, action);
        }

        public float[] GetSenses(ShopAgent agent)
        {
            return GetEconomySystem(agent).GetObservations(agent);
        }
    }
}
