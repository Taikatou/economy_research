﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;
using Debug = UnityEngine.Debug;
using EconomyProject.Scripts.UI;

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

    public enum CraftingChoice { BeginnerSword, IntermediateSword, AdvancedSword, EpicSword, MasterSword, UltimateSwordOfPower }

    [Serializable]
    public class ShopCraftingSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>
    {
	    public override int ObservationSize => 2;
	    public override EShopScreen ActionChoice => EShopScreen.Craft;

	    public CraftingSubSystem craftingSubSubSystem;

        public AgentShopSubSystem shopSubSubSystem;

		public void Start()
		{
			if (craftingSubSubSystem == null)
			{
				craftingSubSubSystem = new CraftingSubSystem();
			}
		}

		public override bool CanMove(ShopAgent agent)
        {
            return !craftingSubSubSystem.HasRequest(agent);
        }

        public override float[] GetObservations(ShopAgent agent)
        {
			var outputs = new float [1 + AgentShopSubSystem.SenseCount + CraftingSubSystem.SenseCount];
			outputs[0] = 0; //(float) GetInputMode(agent);
            var sensesA = shopSubSubSystem.GetSenses(agent);
            sensesA.CopyTo(outputs, 1);
            var sensesB = craftingSubSubSystem.GetObservations(agent);
            sensesB.CopyTo(outputs, 1 + sensesA.Length);
            return outputs;
        }

        public void MakeChoiceSetPrice(ShopAgent shopAgent, EShopAgentChoices input)
		{
			// MakeChoice(shopAgent, input);
		}

        /*protected override void MakeChoice(ShopAgent shopAgent, EShopAgentChoices input)
        {
	        switch (GetInputMode(shopAgent))
            {
                case CraftingInput.CraftItem:
                    var craftingResource = (CraftingChoice) input;
                    craftingSubSubSystem.MakeRequest(shopAgent, craftingResource);
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
        }*/

		public void SetPrice(ShopAgent shopAgent, UsableItem item, int increment)
		{
			var index = GetIndexInShopList(shopAgent, item);
			if (index == -1)
			{
				return;
			}

			if(increment < 0)
			{
				SetChoice(shopAgent, EShopAgentChoices.DecreasePrice);
			}
			else
			{
				SetChoice(shopAgent, EShopAgentChoices.IncreasePrice);
			}
			
			// MakeChoiceSetPrice(shopAgent, index);
		}

		public int GetIndexInShopList(ShopAgent shopAgent, UsableItem item)
		{
			var items = shopSubSubSystem.GetShopItems(shopAgent);
			for (int i = 0; i < items.Count; i++)
			{
				if (item == items[i])
				{
					return i;
				}
			}

			Debug.LogError("Item not in shop : " + item.itemDetails.itemName);
			return -1;
		}

		public void Update()
        {
			RequestDecisions();
			craftingSubSubSystem.Update();
		}
    }
}
