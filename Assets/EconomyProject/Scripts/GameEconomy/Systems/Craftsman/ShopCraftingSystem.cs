using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Inventory;
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
                Progress
            };
        }

        public const int SenseCount = 1;
    }

    public enum CraftingInput { CraftItem = CraftingChoice.UltimateSwordOfPower+1, IncreasePrice, DecreasePrice, SubmitToShop, Quit}

    public enum CraftingChoice { BeginnerSword, IntermediateSword, AdvancedSword, EpicSword, UltimateSwordOfPower }

    [Serializable]
    public class ShopCraftingSystem : StateEconomySystem<CraftingInput, ShopAgent, EShopScreen>
    {
        public override EShopScreen ActionChoice => EShopScreen.Craft;
        protected override CraftingInput IsBackState => CraftingInput.Quit;
        protected override CraftingInput DefaultState => CraftingInput.CraftItem;

        public CraftingSubSystem craftingSubSubSystem;

        public AgentShopSubSystem shopSubSubSystem;

        public override bool CanMove(ShopAgent agent)
        {
            return !craftingSubSubSystem.HasRequest(agent);
        }

        public override float[] GetSenses(ShopAgent agent)
        {
            var outputs = new float [1 + AgentShopSubSystem.SenseCount + CraftingSubSystem.SenseCount];
            outputs[0] = (float) GetInputMode(agent);
            var sensesA = shopSubSubSystem.GetSenses(agent);
            sensesA.CopyTo(outputs, 1);
            var sensesB = craftingSubSubSystem.GetSenses(agent);
            sensesB.CopyTo(outputs, 1 + sensesA.Length);
            return outputs;
        }

        public override InputAction[] GetInputOptions(ShopAgent agent)
        {
            var outputs = new List<InputAction>();
            outputs.AddRange(EconomySystemUtils.GetStateInput<CraftingInput>());
            outputs.AddRange(EconomySystemUtils.GetStateInput<CraftingChoice>());
            return outputs.ToArray();
        }

        protected override void MakeChoice(ShopAgent shopAgent, int input)
        {
            switch (GetInputMode(shopAgent))
            {
                case CraftingInput.CraftItem:
                    craftingSubSubSystem.MakeRequest(shopAgent, input);
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

        public void Update()
        {
            RequestDecisions();
        }
    }
}
