using NUnit.Framework;

using System;
using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using Inventory;
using TurnBased.Scripts;
using UnityEngine;
using EconomyProject.Scripts.UI.Config;
using EconomyProject.Scripts.GameEconomy;

namespace Tests.Economy
{
	public class ConfigSystemTests : TestSystem
	{

		[SetUp]
		public void Setup()
		{
			Init();
		}

		/********************************************Config*********************************************/

		[Test]
		public void Config_ObjectInScene()
		{
			Assert.NotNull(configSystem);
			Assert.NotNull(GameObject.FindObjectOfType<ConfigUI>());
			Assert.NotNull(GameObject.FindObjectOfType<ListConfigItems>());
			Assert.NotNull(GameObject.FindObjectOfType<ListConfigResources>());
			Assert.NotNull(GameObject.FindObjectOfType<ListConfigAgents>());
			Assert.NotNull(GameObject.FindObjectOfType<ListConfigCraft>());
		}

		/// <summary>
		/// Set the default price of the items
		/// </summary>
		[Test]
		public void Config_SetItemsDefaultPrices()
		{
			List<BaseItemPrices> defaultPrices = agentShopSubSystem.basePrices;

			//Set a new random list of BaseItemPrices
			List<BaseItemPrices> newPrices = new List<BaseItemPrices>();
			for (int i = 0; i < defaultPrices.Count; i++)
			{
				UsableItem itemToModify = defaultPrices[i].item;
				int newPrice = UnityEngine.Random.Range(1, 200);

				newPrices.Add(new BaseItemPrices
				{
					item = itemToModify,
					price = newPrice
				});
			}

			configSystem.SetItemsDefaultPrices(newPrices);
			Assert.AreNotEqual(agentShopSubSystem.basePrices, defaultPrices, "Default prices have not been set");
			Assert.AreEqual(agentShopSubSystem.basePrices, newPrices);
			Assert.AreNotEqual(defaultPrices, newPrices, "Randomly equal");
		}

		/// <summary>
		/// Set durability and damage by default of the items
		/// </summary>
		[Test]
		public void Config_SetDefaultItemDetails()
		{
			Dictionary<string, int> defaultDurabilities = ItemData.BaseDurabilities;
			Dictionary<string, int> defaultItemDamage = ItemData.BaseDamages;

			Assert.AreEqual(defaultDurabilities.Count, defaultItemDamage.Count, "Not the same count of items");

			Dictionary<string, int> newDurabilities = new Dictionary<string, int>();
			Dictionary<string, int> newItemDamage = new Dictionary<string, int>();
			foreach (var item in defaultDurabilities)
			{
				newDurabilities.Add(item.Key, UnityEngine.Random.Range(1, 200));
			}
			foreach (var item in defaultItemDamage)
			{
				newItemDamage.Add(item.Key, UnityEngine.Random.Range(1, 200));
			}

			configSystem.SetDefaultItemDetails(newDurabilities, newItemDamage);

			Assert.AreNotEqual(defaultDurabilities, ItemData.BaseDurabilities);
			Assert.AreNotEqual(defaultItemDamage, ItemData.BaseDamages);
			Assert.AreNotEqual(defaultDurabilities, newDurabilities);
			Assert.AreNotEqual(defaultItemDamage, newItemDamage);
			Assert.AreEqual(newDurabilities, ItemData.BaseDurabilities);
			Assert.AreEqual(newItemDamage, ItemData.BaseDamages);
		}

		/// <summary>
		/// Set default price of the resources
		/// </summary>
		[Test]
		public void Config_SetResourceDefaultPrices()
		{
			Dictionary<CraftingResources, int> defaultResourcePrices = requestShopSystemBehaviour.system.requestSystem.defaultResourcePrices;
			Dictionary<CraftingResources, int> newResourcePrices = new Dictionary<CraftingResources, int>();

			foreach (var item in defaultResourcePrices)
			{
				newResourcePrices.Add(item.Key, UnityEngine.Random.Range(1, 200));
			}

			configSystem.SetResourceDefaultPrices(newResourcePrices);

			Assert.AreEqual(requestShopSystemBehaviour.system.requestSystem.defaultResourcePrices, newResourcePrices);
			Assert.AreNotEqual(defaultResourcePrices, newResourcePrices);
		}

		/// <summary>
		/// Set The StartMoney of the adventurer and the shop agents
		/// </summary>
		[Test]
		public void Config_SetStartMoneyAgents()
		{
			Dictionary<AgentType, int> defaultStartMoney = requestShopSystemBehaviour.system.requestSystem.StartMoney;
			Dictionary<AgentType, int> newStartMoney = new Dictionary<AgentType, int>();

			foreach (var item in defaultStartMoney)
			{
				newStartMoney.Add(item.Key, UnityEngine.Random.Range(1, 200));
			}

			configSystem.SetStartMoneyAgents(newStartMoney);

			Assert.AreEqual(requestShopSystemBehaviour.system.requestSystem.StartMoney, newStartMoney);
			Assert.AreNotEqual(defaultStartMoney, newStartMoney);
		}

		/// <summary>
		/// Set the number of agent to spawn in the SpawnAgent objects
		/// </summary>
		[Test]
		public void Config_SetSpawnNumber()
		{
			Dictionary<AgentType, int> defaultNbrAgents = new Dictionary<AgentType, int>
			{
				{ AgentType.Adventurer, adventurerSpawner.numLearningAgents },
				{ AgentType.Shop, shopSpawner.numLearningAgents }
			};
			Dictionary<AgentType, int> newNbrAgents = new Dictionary<AgentType, int>();

			foreach (var item in defaultNbrAgents)
			{
				newNbrAgents.Add(item.Key, UnityEngine.Random.Range(1, 10));
			}

			configSystem.SetSpawnNumber(newNbrAgents);

			Assert.AreEqual(adventurerSpawner.numLearningAgents, newNbrAgents[AgentType.Adventurer]);
			Assert.AreEqual(shopSpawner.numLearningAgents, newNbrAgents[AgentType.Shop]);

			Assert.AreEqual(adventurerSpawner.numLearningAgents, getAdventurerAgent.GetAgents.Length);
			Assert.AreEqual(shopSpawner.numLearningAgents, getShopAgent.GetAgents.Length);
		}

		/// <summary>
		/// Set the list of resource requirement to craft items
		/// </summary>
		[Test]
		public void Config_SetResourceRequirements()
		{
			List<CraftingMap> defaultListRequirements = shopCraftingSystemBehaviour.system.craftingSubSubSystem.craftingRequirement;
			List<CraftingMap> newListRequirements = new List<CraftingMap>();

			foreach(CraftingMap craftingMap in defaultListRequirements)
			{
				ECraftingChoice newCraftingChoice = craftingMap.choice;

				UsableItem newResultingItem = craftingMap.resource.resultingItem;
				float newTimeToCreation = craftingMap.resource.timeToCreation;
				List<ResourceRequirement> newResourcesRequirements = new List<ResourceRequirement>();
				foreach(ResourceRequirement resourceRequirement in craftingMap.resource.resourcesRequirements)
				{
					CraftingResources newType = resourceRequirement.type;
					int newNumber = UnityEngine.Random.Range(1, 10);
					newResourcesRequirements.Add(new ResourceRequirement(newType, newNumber));
				}

				CraftingRequirements newCraftingRequirements = new CraftingRequirements
				{
					resultingItem = newResultingItem,
					timeToCreation = newTimeToCreation,
					resourcesRequirements = newResourcesRequirements
				};

				CraftingMap newCraftingMap = new CraftingMap
				{
					choice = newCraftingChoice,
					resource = newCraftingRequirements
				};

				newListRequirements.Add(newCraftingMap);
			}

			configSystem.SetResourceRequirements(newListRequirements);

			Assert.AreEqual(shopCraftingSystemBehaviour.system.craftingSubSubSystem.craftingRequirement, newListRequirements);
			Assert.AreNotEqual(defaultListRequirements, newListRequirements);
		}

		/********************************************TearDown*********************************************/
		[TearDown]
		public new void TearDown()
		{
			base.TearDown();
		}
	}
}
