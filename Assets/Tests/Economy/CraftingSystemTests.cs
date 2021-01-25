using NUnit.Framework;

using System;
using UnityEngine;

using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using System.Collections.Generic;
using EconomyProject.Monobehaviours;
using Inventory;

namespace Tests.Economy
{
    public class CraftingSystemTests
    {
		ShopCraftingSystemBehaviour shopCraftingSystemBehaviour;
		GetCurrentShopAgent getShopAgent;
		ShopAgent shopAgent;
		ShopCraftingSystem shopCraftingSystem;
		CraftingSubSystem craftingSubSystem;
		AgentShopSubSystem agentShopSubSubSystem;

		[SetUp]
		public void Setup()
		{
			shopCraftingSystemBehaviour = Resources.FindObjectsOfTypeAll<ShopCraftingSystemBehaviour>()[0];
			agentShopSubSubSystem = shopCraftingSystemBehaviour.system.shopSubSubSystem;
			craftingSubSystem = shopCraftingSystemBehaviour.system.craftingSubSubSystem;

			//Get ShopAgent
			SpawnAgents();
			getShopAgent = Resources.FindObjectsOfTypeAll<GetCurrentShopAgent>()[0];
			shopAgent = getShopAgent.CurrentAgent;


			//Can't due to serialize
			//shopCraftingSystem = Resources.FindObjectsOfTypeAll<ShopCraftingSystem>()[0];



		}

		/*
		/// <summary>
		/// Test initialization of the shopCraftingSystem
		/// </summary>
		[Test]
        public void Shop_Init()
        {
            var shop = new ShopCraftingSystem
            {
                craftingSubSubSystem = new CraftingSubSystem(),
                shopSubSubSystem = new AgentShopSubSystem()
            };

			float[] a = shop.GetSenses(shopAgent);

			int i = 0;
			foreach(float f in a)
			{
				Debug.Log(i + " : " + f);
				i++;
			}
        }
		*/

		/********************************************Enumerations*********************************************/

		/// <summary>
		/// Test enumeration CraftingInput
		/// </summary>
		[Test]
		public void Enum_CraftingInput()
		{
			bool isExist1 = Enum.IsDefined(typeof(CraftingInput), "CraftItem");
			bool isExist2 = Enum.IsDefined(typeof(CraftingInput), "IncreasePrice");
			bool isExist3 = Enum.IsDefined(typeof(CraftingInput), "DecreasePrice");
			bool isExist4 = Enum.IsDefined(typeof(CraftingInput), "SubmitToShop");
			bool isExist5 = Enum.IsDefined(typeof(CraftingInput), "Quit");
			Assert.True(isExist1 && isExist2 && isExist3 && isExist4 && isExist5 == true);
		}

		/// <summary>
		/// Test enumeration CraftingChoice
		/// </summary>
		[Test]
		public void Enum_CraftingChoice()
		{
			bool isExist1 = Enum.IsDefined(typeof(CraftingChoice), "BeginnerSword");
			bool isExist2 = Enum.IsDefined(typeof(CraftingChoice), "IntermediateSword");
			bool isExist3 = Enum.IsDefined(typeof(CraftingChoice), "AdvancedSword");
			bool isExist4 = Enum.IsDefined(typeof(CraftingChoice), "EpicSword");
			bool isExist5 = Enum.IsDefined(typeof(CraftingChoice), "UltimateSwordOfPower");
			Assert.True(isExist1 && isExist2 && isExist3 && isExist4 && isExist5 == true);
		}

		/// <summary>
		/// Test enumeration CraftingResources { Nothing, Wood, Metal, Gem, DragonScale }
		/// </summary>
		[Test]
		public void Enum_CraftingResources()
		{
			bool isExist1 = Enum.IsDefined(typeof(CraftingResources), "Nothing");
			bool isExist2 = Enum.IsDefined(typeof(CraftingResources), "Wood");
			bool isExist3 = Enum.IsDefined(typeof(CraftingResources), "Metal");
			bool isExist4 = Enum.IsDefined(typeof(CraftingResources), "Gem");
			bool isExist5 = Enum.IsDefined(typeof(CraftingResources), "DragonScale");
			Assert.True(isExist1 && isExist2 && isExist3 && isExist4 && isExist5 == true);
		}

		/******************************************Crafting*********************************************/
		/// <summary>
		/// Test CraftingUtils class
		/// </summary>
		[Test]
		public void Craft_CraftingUtils()
		{
			List<CraftingResources> listCraftingResources = CraftingUtils.GetCraftingResources();
			Assert.AreEqual(4, listCraftingResources.Count);
			Assert.Contains(CraftingResources.Wood, listCraftingResources);
			Assert.Contains(CraftingResources.Metal, listCraftingResources);
			Assert.Contains(CraftingResources.Gem, listCraftingResources);
			Assert.Contains(CraftingResources.DragonScale, listCraftingResources);
		}

		/// <summary>
		/// Test default prices and sword details
		/// </summary>
		[Test]
		public void Craft_SwordDetailsAndPrices()
		{
			List<BaseItemPrices> basePrices = agentShopSubSubSystem.basePrices;
			foreach(BaseItemPrices item in basePrices)
			{
				UsableItemDetails details = item.item.itemDetails;

				//Prices
				switch (details.itemName)
				{
					case "Beginner Sword":
						Assert.AreEqual(50, item.price);
						break;
					case "Intermediate Sword":
						Assert.AreEqual(70, item.price);
						break;
					case "Advanced Sword":
						Assert.AreEqual(90, item.price);
						break;
					case "Epic Sword":
						Assert.AreEqual(110, item.price);
						break;
					case "Master Sword":
						Assert.AreEqual(140, item.price);
						break;
					case "Ultimate Sword":
						Assert.AreEqual(180, item.price);
						break;
					default:
						Assert.False(true, "Wrong Item : " + item.item.ToString() + " - price : " + item.price);
						break;
				}

				//Details
				switch (details.itemName)
				{
					case "Beginner Sword":
						Assert.AreEqual(7, details.damage);
						Assert.AreEqual(10, details.baseDurability);
						Assert.AreEqual(false, details.unBreakable);
						break;
					case "Intermediate Sword":
						Assert.AreEqual(9, details.damage);
						Assert.AreEqual(12, details.baseDurability);
						Assert.AreEqual(false, details.unBreakable);
						break;
					case "Advanced Sword":
						Assert.AreEqual(10, details.damage);
						Assert.AreEqual(14, details.baseDurability);
						Assert.AreEqual(false, details.unBreakable);
						break;
					case "Epic Sword":
						Assert.AreEqual(12, details.damage);
						Assert.AreEqual(20, details.baseDurability);
						Assert.AreEqual(false, details.unBreakable);
						break;
					case "Master Sword":
						Assert.AreEqual(12, details.damage);
						Assert.AreEqual(22, details.baseDurability);
						Assert.AreEqual(false, details.unBreakable);
						break;
					case "Ultimate Sword":
						Assert.AreEqual(15, details.damage);
						Assert.AreEqual(25, details.baseDurability);
						Assert.AreEqual(false, details.unBreakable);
						break;
					default:
						Assert.False(true, "Wrong Item : " + details.itemName
							+ " - damage : " + details.damage
							+ " - baseDurability : " + details.baseDurability
							+" - unBreakable : " + details.unBreakable);
						break;
				}
			}
		}



		/********************************************Helper*********************************************/
		/// <summary>
		/// Spawn one shop agent
		/// </summary>
		public void SpawnAgents()
		{
			//Create 1 adventurer agent
			SystemSpawner[] systemSpawners = Resources.FindObjectsOfTypeAll<SystemSpawner>();

			SystemSpawner agentSpawner = systemSpawners[1];
			agentSpawner.numLearningAgents = 1;
			agentSpawner.Start();
		}

	}
}
