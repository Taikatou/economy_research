﻿using NUnit.Framework;

using System;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;


namespace Tests.Economy
{
    public class CraftingSystemTests : TestSystem
    {
		[SetUp]
		public void Setup()
		{
			Init();
		}

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
			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;
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

		/// <summary>
		/// Test making a request
		/// </summary>
		[Test]
		public void Craft_HasRequest()
		{
			//No request by default
			Assert.False(craftingSubSystem.HasRequest(shopAgent), "Agent has resource request(s) by default");

			//Give resources to craft
			GiveResources();

			//Make the craft
			CraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
			craftingSubSystem.MakeRequest(shopAgent, (int)randomSword);

			//Check the craft
			Assert.True(craftingSubSystem.HasRequest(shopAgent));
		}

		/// <summary>
		/// Test crafting characteristics
		/// </summary>
		[Test]
		public void Craft_CraftingRequest()
		{
			//Give resources to craft
			GiveResources();

			CraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
			craftingSubSystem.MakeRequest(shopAgent, (int)randomSword);

			Dictionary<ShopAgent, CraftingRequest> shopRequest = craftingSubSystem.GetShopRequests();
			CraftingRequest craftingRequest = shopRequest[shopAgent];

			Assert.AreEqual(0, craftingRequest.CraftingTime);
			Assert.AreEqual(3, craftingRequest.CraftingRequirements.timeToCreation);

			UsableItem craftedItem = GetUsableItemByCraftingChoice(randomSword);
			Assert.AreEqual(craftedItem, craftingRequest.CraftingRequirements.resultingItem);

		}

		/// <summary>
		/// Test making a request with every swords
		/// </summary>
		[Test]
		public void Craft_MakeRequest()
		{
			//To be sure to have the correct agent
			shopAgent = getShopAgent.CurrentAgent;

			Time.timeScale = 100;

			//Give resources to craft
			GiveResources();
			GiveResources();

			//Craft one sword of each type
			foreach (CraftingChoice choice in listCraftingChoices)
			{
				craftingSubSystem.MakeRequest(shopAgent, (int)choice);

				Debug.Log("CraftingChoice : " + choice);
				Debug.Log("craftingSubSystem null : " + (craftingSubSystem == null));
				Debug.Log("craftingSubSystem.GetShopRequests().Count : " + craftingSubSystem.GetShopRequests().Count);
				Debug.Log("craftingSubSystem.GetShopRequests().Contains(agent) : " + craftingSubSystem.GetShopRequests().ContainsKey(shopAgent));
				Debug.Log("shopAgent : " + shopAgent);

				CraftingRequest cr = craftingSubSystem.GetShopRequests()[shopAgent];
				
				int a = 20; //To avoid infinite loop
				while (a > 0 && cr.Complete == false)
				{
					craftingSubSystem.Update();
					a--;
				}

				//No more Crafting
				Assert.True(cr.Complete, "Craft never complete : " + a);
				Assert.False(craftingSubSystem.HasRequest(shopAgent));

				//AgentInventory contains the item
				Assert.True(shopAgent.agentInventory.ContainsItem(GetUsableItemByCraftingChoice(choice)), "Do not contain the crafted item");
			}

			Time.timeScale = 1;
		}

		/********************************************TearDown*********************************************/
		[TearDown]
		public new void TearDown()
		{
			base.TearDown();
		}

	}
}
