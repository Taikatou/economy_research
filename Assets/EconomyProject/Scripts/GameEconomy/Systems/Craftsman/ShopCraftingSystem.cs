using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    [System.Serializable]
    public struct CraftingMap
    {
        public CraftingChoice choice;
        public CraftingRequirements resource;
    }
    
    public class CraftingRequest
    {
        public CraftingRequirements CraftingRequirements;
        public float CraftingTime;

        public float Progress => CraftingTime / CraftingRequirements.timeToCreation;

        public bool Complete => CraftingTime >= CraftingRequirements.timeToCreation;

        public float[] GetSenses()
        {
            return new float [SenseCount]
            {
                Progress,
                Complete? 1f : 0f
            };
        }

        public const int SenseCount = 2;
    }

    public enum CraftingInput { CraftItem = CraftingChoice.UltimateSwordOfPower+1, IncreasePrice, DecreasePrice, SubmitToShop, Quit}

    public enum CraftingChoice { BeginnerSword, IntermediateSword, AdvancedSword, EpicSword, UltimateSwordOfPower }

    public class ShopCraftingSystem : StateEconomySystem<CraftingInput, ShopAgent, EShopScreen>
    {
        protected override EShopScreen ActionChoice => EShopScreen.Craft;
        protected override CraftingInput IsBackState => CraftingInput.Quit;
        protected override CraftingInput DefaultState => CraftingInput.CraftItem;

        public CraftingSystem craftingSubSystem;

        public AgentShopSubSystem shopSubSubSystem;

        public List<UsableItem> usableItems;
        public override bool CanMove(ShopAgent agent)
        {
            return !craftingSubSystem.HasRequest(agent);
        }

        public override float[] GetSenses(ShopAgent agent)
        {
            var outputs = new float [1 + shopSubSubSystem.SenseCount + craftingSubSystem.SenseCount];
            outputs[0] = (float) GetInputMode(agent);
            var sensesA = shopSubSubSystem.GetSenses(agent, usableItems);
            sensesA.CopyTo(outputs, 1);
            var sensesB = craftingSubSystem.GetSenses(agent);
            sensesB.CopyTo(outputs, 1 + sensesA.Length);
            return outputs;
        }

        protected override void MakeChoice(ShopAgent shopAgent, int input)
        {
            switch (GetInputMode(shopAgent))
            {
                case CraftingInput.CraftItem:
                    craftingSubSystem.MakeRequest(shopAgent, input);
                    break;
                case CraftingInput.IncreasePrice:
                    shopSubSubSystem.SetCurrentPrice(shopAgent, input, 1);
                    break;
                case CraftingInput.DecreasePrice:
                    shopSubSubSystem.SetCurrentPrice(shopAgent, input, -1);
                    break;
                case CraftingInput.SubmitToShop:
                    shopSubSubSystem.SubmitToShop(shopAgent, input);
                    break;
            }
        }

        protected override void GoBack(ShopAgent agent)
        {
            AgentInput.ChangeScreen(agent, EShopScreen.Main);
        }

        private void Update()
        {
            RequestDecisions();
        }
    }
}
