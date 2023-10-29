using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.UI.Adventurer;
using EconomyProject.Scripts.UI.Config;
using Inventory;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace EconomyProject.Scripts
{
	public class ConfigSystem : MonoBehaviour
	{
		public bool showConfig = false;
		[Header("System GameObjects")]
		public RequestShopSystemBehaviour requestShopSystemBehaviour;
		public ShopCraftingSystemBehaviour shopCraftingSystemBehaviour;
		protected ShopCraftingSystem shopCraftingSystem;
		protected AgentShopSubSystem agentShopSubSystem;
		public GetCurrentShopAgent getCurrentShopAgent;
		public GetCurrentAdventurerAgent getCurrentAdventurerAgent;
		public SystemSpawner adventurerSpawner;
		public SystemSpawner shopSpawner;
		public Transform listAdventurerAgents;
		public Transform listShopAgents;
		public AdventurerAgentDropDown adventurerAgentDropDown;
		public UI.ShopUI.ShopAgentDropDown shopAgentDropDown;

		[Header("Lists")]
		public ListConfigItems listConfigItems;
		public ListConfigResources listConfigResources;
		public ListConfigAgents listConfigAgents;
		public ListConfigCraft listConfigCraft;

		[Header("UI")]
		public GameObject UIBlocker;
		public GameObject ConfigUI;

		public void Start()
		{
			if (shopCraftingSystemBehaviour != null)
			{
				shopCraftingSystem = shopCraftingSystemBehaviour.system;
				agentShopSubSystem = shopCraftingSystem.shopSubSubSystem;	
			}

			if (!showConfig)
			{
				Invoke(nameof(StartButton), 1);
			}
		}

		/// <summary>
		/// Set default price of the items
		/// </summary>
		public void SetItemsDefaultPrices(List<BaseItemPrices> listPrice)
		{
			//Modify the shopSystem
			if (agentShopSubSystem != null)
			{
				agentShopSubSystem.basePrices = listPrice;
			}
		}

		/// <summary>
		/// Set durability and damage by default of the items
		/// </summary>
		public void SetDefaultItemDetails(Dictionary<string, int> newDurabilities, Dictionary<string, int> newItemDamage)
		{
			ItemData.SetDefaultDurabilities(newDurabilities);
			ItemData.SetDefaultDamages(newItemDamage);
		}

		/// <summary>
		/// Set default price of the resources
		/// </summary>
		public void SetResourceDefaultPrices(Dictionary<ECraftingResources, int> newResourcePrices)
		{
			if (requestShopSystemBehaviour != null)
			{
				requestShopSystemBehaviour.system.requestSystem.DefaultResourcePrices = newResourcePrices;	
			}
		}

		/// <summary>
		/// Set The StartMoney of the adventurer and the shop agents
		/// </summary>
		public void SetStartMoneyAgents(Dictionary<AgentType, int> newStartMoney)
		{
			// requestShopSystemBehaviour.system.requestSystem.StartMoney = newStartMoney;
		}

		/// <summary>
		/// Set the number of agent to spawn in the SpawnAgent objects
		/// </summary>
		public void SetSpawnNumber(Dictionary<AgentType, int> nbrAgents)
		{
			foreach(var agentType in nbrAgents)
			{
				switch (agentType.Key)
				{
					case AgentType.Adventurer:
						
						break;
					case AgentType.Shop:
						shopSpawner.numLearningAgents = agentType.Value;
						break;
					default:
						Debug.Log("Wrong agentType : " + agentType.Key);
						break;
				}
			}

			ResetAgentSpawn();
		}

		/// <summary>
		/// Delete the previous agents and spawn new agents
		/// </summary>
		public void ResetAgentSpawn()
		{
			//Delete previous agents
			getCurrentAdventurerAgent.ClearGetAgents();

			// skipShopSetup = skipShop;
			
			if (!TrainingConfig.SkipShopSetup)
			{
				getCurrentShopAgent.ClearGetAgents();
				shopSpawner.StartSpawn();
			}

			//Generate new agents
			adventurerSpawner.StartSpawn();
		}

		/// <summary>
		/// Set the list of resource requirement to craft items
		/// </summary>
		public void SetResourceRequirements(List<CraftingMap> listRequirements)
		{
			if (shopCraftingSystemBehaviour != null)
			{
				shopCraftingSystemBehaviour.system.craftingSubSubSystem.craftingRequirement = listRequirements;
			}
		}

		/// <summary>
		/// Apply all the default parameters to the system
		/// 1 - Item prices
		/// 2 - Resource prices
		/// 3 - Agents start money
		/// </summary>
		public void StartButton()
		{
			if (!TrainingConfig.SkipShopSetup)
			{
				//Item
				List<BaseItemPrices> newItemPrices = listConfigItems.GetParameters();
				SetItemsDefaultPrices(newItemPrices);
				SetDefaultItemDetails(listConfigItems.GetDefaultDurabilities(), listConfigItems.GetDefaultDamages());

				//Resource prices
				Dictionary<ECraftingResources, int> newResourcePrices = listConfigResources.GetParameters();
				SetResourceDefaultPrices(newResourcePrices);

				//CraftResourceRequirement
				List<CraftingMap> listRequirement = listConfigCraft.GetParameters();
				SetResourceRequirements(listRequirement);	
			}
			
			//Agents start money
			Dictionary<AgentType, int> newStartMoney = listConfigAgents.GetParameters();
			SetStartMoneyAgents(newStartMoney);
			SetSpawnNumber(listConfigAgents.GetDefaultNbrAgents());

			//Hide Configuration
			UIBlocker.SetActive(false);
			ConfigUI.SetActive(false);
		}
	}
}

