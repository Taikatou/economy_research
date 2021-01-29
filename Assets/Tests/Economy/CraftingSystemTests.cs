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
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.GameEconomy.Systems;

namespace Tests.Economy
{
    public class CraftingSystemTests
    {
		ShopCraftingSystemBehaviour shopCraftingSystemBehaviour;
		ShopCraftingSystem shopCraftingSystem;
		CraftingSubSystem craftingSubSystem;
		AgentShopSubSystem agentShopSubSystem;

		GetCurrentShopAgent getShopAgent;
		ShopAgent shopAgent;

		AdventurerSystem adventurerSystem;
		GetCurrentAdventurerAgent getAdventurerAgent;
		AdventurerAgent adventurerAgent;
		TravelSubSystem travelSubsystem;
		AdventurerInventory adventurerInventory;
		AgentInventory adventurerAgentInventory;

		public List<CraftingChoice> listCraftingChoices = new List<CraftingChoice> { CraftingChoice.BeginnerSword, CraftingChoice.IntermediateSword, CraftingChoice.AdvancedSword, CraftingChoice.EpicSword, CraftingChoice.UltimateSwordOfPower};


		[SetUp]
		public void Setup()
		{
			shopCraftingSystemBehaviour = GameObject.FindObjectOfType<ShopCraftingSystemBehaviour>();
			shopCraftingSystemBehaviour.Start();

			agentShopSubSystem = shopCraftingSystemBehaviour.system.shopSubSubSystem;
			craftingSubSystem = shopCraftingSystemBehaviour.system.craftingSubSubSystem;

			//Shop Agent
			SpawnAgents();
			getShopAgent = GameObject.FindObjectOfType<GetCurrentShopAgent>(); ;
			shopAgent = getShopAgent.CurrentAgent;
			shopAgent.craftingInventory.ResetInventory();
			shopAgent.agentInventory.ResetInventory();

			//Adventurer Agent
			getAdventurerAgent = GameObject.FindObjectOfType<GetCurrentAdventurerAgent>();
			adventurerAgent = getAdventurerAgent.CurrentAgent;

			//Generate travelSubsystem of the adventurerSystem
			adventurerSystem = GameObject.FindObjectOfType<AdventurerSystemBehaviour>().system;
			travelSubsystem = adventurerSystem.travelSubsystem;
			travelSubsystem.Start();
			adventurerSystem.travelSubsystem = travelSubsystem;

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
			Assert.False(craftingSubSystem.HasRequest(shopAgent));

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
			Time.timeScale = 100;

			//Give resources to craft
			GiveResources();

			//Craft one sword of each type
			foreach (CraftingChoice choice in listCraftingChoices)
			{
				craftingSubSystem.MakeRequest(shopAgent, (int)choice);

				CraftingRequest cr = craftingSubSystem.GetShopRequests()[shopAgent];
				
				int a = 10; //To avoid infinite loop
				while (a > 0 && cr.Complete == false)
				{
					craftingSubSystem.Update();
					a--;
				}

				//No more Crafting
				Assert.True(cr.Complete, "Craft never complete : " + a);
				Assert.False(craftingSubSystem.HasRequest(shopAgent));

				//AgentInventory contains the item
				Assert.True(shopAgent.agentInventory.ContainsItem(GetUsableItemByCraftingChoice(choice)));
			}

			Time.timeScale = 1;
		}

		/// <summary>
		/// Test buying equipment
		/// </summary>
		[Test]
		public void Shop_PurchaseItem()
		{
			UsableItem itemBought; // To do
			//agentShopSubSystem.PurchaseItem(shopAgent, itemBought.itemDetails, adventurerAgent.wallet, adventurerAgent.adventurerInventory.agentInventory);
			Assert.True(false, "To do");
		}

		/********************************************Helper*********************************************/
		/// <summary>
		/// Spawn one shop agent
		/// </summary>
		public void SpawnAgents()
		{
			//Create 1 adventurer agent
			SystemSpawner[] systemSpawners = GameObject.FindObjectsOfType<SystemSpawner>();

			foreach(SystemSpawner spawner in systemSpawners)
			{
				if(spawner.name == "CraftShopSystem")
				{
					spawner.numLearningAgents = 1;
					spawner.Start();
					return;
				}
			}

			Debug.Log("Not found the Shop Agent Spawner : " + systemSpawners.Length);
		}

		/// <summary>
		/// Give resources to the ShopAgent
		/// </summary>
		public void GiveResources()
		{
			shopAgent.craftingInventory.AddResource(CraftingResources.Wood, 20);
			shopAgent.craftingInventory.AddResource(CraftingResources.Metal, 20);
			shopAgent.craftingInventory.AddResource(CraftingResources.Gem, 20);
			shopAgent.craftingInventory.AddResource(CraftingResources.DragonScale, 20);
		}

		/// <summary>
		/// Return UsableItem with a CraftingChoice
		/// </summary>
		public UsableItem GetUsableItemByCraftingChoice(CraftingChoice craftingChoice)
		{
			String nameToCheck = "";
			switch (craftingChoice)
			{
				case CraftingChoice.BeginnerSword:
					nameToCheck = "Beginner Sword";
					break;
				case CraftingChoice.IntermediateSword:
					nameToCheck = "Intermediate Sword";
					break;
				case CraftingChoice.AdvancedSword:
					nameToCheck = "Advanced Sword";
					break;
				case CraftingChoice.EpicSword:
					nameToCheck = "Epic Sword";
					break;
				/*
				case :
					nameToCheck = "Master Sword";
					break;
				*/
				case CraftingChoice.UltimateSwordOfPower:
					nameToCheck = "Ultimate Sword";
					break;
				default:
					Debug.Log("Wrong craftingChoice : " + craftingChoice);
					break;
			}

			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;
			foreach (BaseItemPrices item in basePrices)
			{
				if(item.item.itemDetails.itemName == nameToCheck)
				{
					return item.item;
				}
			}

			Debug.Log("Not found the item");
			return null;
		}
	}
}
