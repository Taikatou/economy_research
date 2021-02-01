using NUnit.Framework;

using System;
using System.Collections.Generic;
using UnityEngine;

using Inventory;

using EconomyProject.Monobehaviours;
using EconomyProject.Scripts;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;


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
			shopAgent.wallet.Reset();

			//Adventurer Agent
			getAdventurerAgent = GameObject.FindObjectOfType<GetCurrentAdventurerAgent>();
			adventurerAgent = getAdventurerAgent.CurrentAgent;
			adventurerAgent.ResetEconomyAgent();

			//Generate travelSubsystem of the adventurerSystem
			adventurerSystem = GameObject.FindObjectOfType<AdventurerSystemBehaviour>().system;
			travelSubsystem = adventurerSystem.travelSubsystem;
			travelSubsystem.Start();
			adventurerSystem.travelSubsystem = travelSubsystem;

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
				Assert.True(shopAgent.agentInventory.ContainsItem(GetUsableItemByCraftingChoice(choice)), "Do not contain the crafted item");
			}

			Time.timeScale = 1;
		}


		/// <summary>
		/// Test submitting an item the shopAgent doesn't have
		/// </summary>
		[Test]
		public void Shop_ShopByDefault()
		{
			//ShopEmptyByDefault
			List<UsableItem> shop = agentShopSubSystem.GetShopItems(shopAgent);
			Assert.IsEmpty(shop, "Should have an empty shop");
			Assert.AreEqual(0, shop.Count);
		}

		/// <summary>
		/// Test submitting an item in the shop
		/// </summary>
		[Test]
		public void Shop_SellOneItem()
		{
			CraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];

			//Add sword to the inventory
			UsableItem sword = AddItemInInventory(shopAgent.agentInventory, randomSword);

			//Submit it into the shop
			agentShopSubSystem.SubmitToShop(shopAgent, sword);

			List<UsableItem> shop = agentShopSubSystem.GetShopItems(shopAgent);
			Assert.NotNull(shop, "Empty shop");

			//Check number
			Assert.AreEqual(1, shop.Count);
			Assert.AreEqual(1, agentShopSubSystem.GetNumber(shopAgent, sword.itemDetails));

			//check price
			int basePrice = GetPriceByItemName(sword.itemDetails.itemName);
			Assert.AreEqual(basePrice, agentShopSubSystem.GetPrice(shopAgent, sword.itemDetails));
		}

		/// <summary>
		/// Test submitting an item the shopAgent doesn't have
		/// </summary>
		[Test]
		public void Shop_TrytoSellItemNotInStock()
		{
			CraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
			UsableItem sword = GetUsableItemByCraftingChoice(randomSword);

			//Should not working
			agentShopSubSystem.SubmitToShop(shopAgent, sword);

			List<UsableItem> shop = agentShopSubSystem.GetShopItems(shopAgent);
			Assert.IsEmpty(shop, "Should have an empty shop");
			Assert.AreEqual(0, shop.Count);
			Assert.AreEqual(0, agentShopSubSystem.GetNumber(shopAgent, sword.itemDetails));
		}

		/// <summary>
		/// Test submitting an item in the shop
		/// </summary>
		[Test]
		public void Shop_SellSeveralIdenticalItems()
		{
			CraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
			UsableItem sword = GetUsableItemByCraftingChoice(randomSword);

			//Add between 5 to 10 items
			int randomItemNumber = UnityEngine.Random.Range(5, 10);
			for (int i = 0; i < randomItemNumber; i++)
			{
				AddItemInInventory(shopAgent.agentInventory, randomSword);
				agentShopSubSystem.SubmitToShop(shopAgent, sword);
			}

			List<UsableItem> shop = agentShopSubSystem.GetShopItems(shopAgent);
			Assert.NotNull(shop, "Empty shop");

			//Check the number of items in the shop
			AgentData aData = agentShopSubSystem.GetShop(shopAgent);
			Assert.NotNull(aData, "Empty AgentData");
			Assert.AreEqual(randomItemNumber, aData.GetStock(sword), "Wrong stock in AgentData");
			Assert.AreEqual(randomItemNumber, agentShopSubSystem.GetNumber(shopAgent, sword.itemDetails), "Wrong stock in AgentShopSubSystem");

			//Check price
			int basePrice = GetPriceByItemName(sword.itemDetails.itemName);
			Assert.AreEqual(basePrice, agentShopSubSystem.GetPrice(shopAgent, sword.itemDetails));
		}

		/// <summary>
		/// Test submitting an item in the shop
		/// </summary>
		[Test]
		public void Shop_SellSeveralRandomItems()
		{
			//Add between 5 to 10 items
			int randomItemNumber = UnityEngine.Random.Range(5, 10);
			for(int i = 0; i< randomItemNumber; i++)
			{
				CraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
				UsableItem sword = AddItemInInventory(shopAgent.agentInventory, randomSword);
				agentShopSubSystem.SubmitToShop(shopAgent, sword);
			}
			
			List<UsableItem> shop = agentShopSubSystem.GetShopItems(shopAgent);
			Assert.NotNull(shop, "Empty shop");
			AgentData aData = agentShopSubSystem.GetShop(shopAgent);
			Assert.NotNull(aData, "Empty AgentData");

			//Check the number of items in the shop
			int nbrSwords = 0;
			int nbrSwords2 = 0;
			foreach(UsableItem item in shop)
			{
				nbrSwords2 += agentShopSubSystem.GetNumber(shopAgent, item.itemDetails);
				nbrSwords += aData.GetStock(item);
			}
			Assert.AreEqual(randomItemNumber, nbrSwords, "Wrong stock in AgentData");
			Assert.AreEqual(randomItemNumber, nbrSwords2, "Wrong stock in AgentShopSubSystem");

			//Check price
			foreach (UsableItem item in shop)
			{
				int basePrice = GetPriceByItemName(item.itemDetails.itemName);
				Assert.AreEqual(basePrice, agentShopSubSystem.GetPrice(shopAgent, item.itemDetails));
			}
		}

		/// <summary>
		/// Test buying equipment
		/// </summary>
		[Test]
		public void Shop_PurchaseItem()
		{
			//ShopAgent sells a sword
			CraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
			UsableItem sword = AddItemInInventory(shopAgent.agentInventory, randomSword);
			agentShopSubSystem.SubmitToShop(shopAgent, sword);

			agentShopSubSystem.PurchaseItem(shopAgent, sword.itemDetails, adventurerAgent.wallet, adventurerAgent.inventory);

			//Check if no more sword in the ShopAgent inventory
			List<UsableItem> shop = agentShopSubSystem.GetShopItems(shopAgent);
			Assert.IsEmpty(shop, "Should have an empty shop");
			Assert.AreEqual(0, shop.Count);
			Assert.AreEqual(0, agentShopSubSystem.GetNumber(shopAgent, sword.itemDetails));

			//Check if adventurer agent get the sword equipped
			Assert.True(adventurerAgent.adventurerInventory.agentInventory.ContainsItem(sword));
			Assert.AreEqual(sword, adventurerAgent.adventurerInventory.EquipedItem);

			//Check wallet of the agents
			int priceSword = GetPriceByItemName(sword.itemDetails.itemName);
			Assert.AreEqual(adventurerAgent.wallet.startMoney - priceSword, adventurerAgent.wallet.Money, "Current adventurerAgent money should be StartMoney - PriceSword");
			Assert.AreEqual(shopAgent.wallet.startMoney + priceSword, shopAgent.wallet.Money, "Current shopAgent money should be StartMoney + PriceSword");
		}

		/// <summary>
		/// Test setting a sword price
		/// </summary>
		[Test]
		public void Shop_SetPriceItem()
		{
			//Submit a sword to the shop
			CraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
			UsableItem sword = AddItemInInventory(shopAgent.agentInventory, randomSword);
			agentShopSubSystem.SubmitToShop(shopAgent, sword);

			//Check default price
			int basePrice = GetPriceByItemName(sword.itemDetails.itemName);
			Assert.AreEqual(basePrice, agentShopSubSystem.GetPrice(shopAgent, sword.itemDetails));

			//Increase Price 
			int randomPositiveIncrement = UnityEngine.Random.Range(1, 50);
			agentShopSubSystem.SetCurrentPrice(shopAgent, 0, randomPositiveIncrement);
			Assert.AreEqual(basePrice + randomPositiveIncrement, agentShopSubSystem.GetPrice(shopAgent, sword.itemDetails), "Item : " + sword.ToString() + " - randomPositiveIncrement : " + randomPositiveIncrement);

			//Decrease Price 
			int randomNegativeIncrement = UnityEngine.Random.Range(-50, -1);
			agentShopSubSystem.SetCurrentPrice(shopAgent, 0, randomNegativeIncrement);
			Assert.AreEqual(basePrice + randomPositiveIncrement + randomNegativeIncrement, agentShopSubSystem.GetPrice(shopAgent, sword.itemDetails));
		}



			/********************************************Helper*********************************************/
			/// <summary>
			/// Spawn one shop agent
			/// </summary>
			public void SpawnAgents()
		{
			//Create 1 adventurer agent
			SystemSpawner[] systemSpawners = GameObject.FindObjectsOfType<SystemSpawner>();

			bool shopAgentSpawned = false;
			bool adventurerAgentSpawned = false;
			foreach (SystemSpawner spawner in systemSpawners)
			{
				if(spawner.name == "CraftShopSystem")
				{
					spawner.numLearningAgents = 1;
					spawner.Start();
					shopAgentSpawned = true;
				}
				if (spawner.name == "AdventurerGameSystem")
				{
					spawner.numLearningAgents = 1;
					spawner.Start();
					adventurerAgentSpawned = true;
				}
			}

			if(shopAgentSpawned == false){
				Debug.Log("Not found the Shop Agent Spawner : " + systemSpawners.Length);
			}
			if (adventurerAgentSpawned == false)
			{
				Debug.Log("Not found the Adventurer Agent Spawner : " + systemSpawners.Length);
			}
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

		/// <summary>
		/// Add an Item in the agent inventory
		/// </summary>
		public UsableItem AddItemInInventory(AgentInventory inventory, CraftingChoice choice)
		{
			UsableItem itemToAdd = GetUsableItemByCraftingChoice(choice);
			inventory.AddItem(itemToAdd);

			return itemToAdd;
		}

		/// <summary>
		/// Gy price by item name
		/// </summary>
		public int GetPriceByItemName(string itemName)
		{
			foreach (BaseItemPrices item in agentShopSubSystem.basePrices)
			{
				UsableItemDetails details = item.item.itemDetails;
				if(item.item.itemDetails.itemName == itemName)
				{
					return item.price;
				}
			}

			Debug.Log("Wrong Item : " + itemName);
			return 0;
		}
	}
}
