using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    public class ShopInput : AgentInput<ShopAgent, EShopScreen, EShopAgentChoices>
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
        
        public override EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices> GetEconomySystem(ShopAgent agent)
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

        public IEnumerable<EnabledInput> GetActionMask(ShopAgent agent)
        {
            var inputsEnabled = GetEconomySystem(agent).GetEnabledInputs(agent);
            return inputsEnabled;
        }
    }
}
