using NUnit.Framework;

using System;
using System.Collections.Generic;

using Inventory;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;


namespace Tests.Economy
{
	public class ShopSystemTests : TestSystem
	{
		[SetUp]
		public void Setup()
		{
			Init();
		}

		/********************************************Shop*********************************************/

		/// <summary>
		/// Test submitting an item the shopAgent doesn't have
		/// </summary>
		[Test]
		public void Shop_ShopByDefault()
		{
			//ShopEmptyByDefault
			List<UsableItem> shop = agentShopSubSystem.GetShopUsableItems(shopAgent);
			Assert.IsEmpty(shop, "Should have an empty shop");
			Assert.AreEqual(0, shop.Count);
		}

		/// <summary>
		/// Test submitting an item in the shop
		/// </summary>
		[Test]
		public void Shop_SellOneItem()
		{
			ECraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];

			//Add sword to the inventory
			UsableItem sword = AddItemInInventory(shopAgent.agentInventory, randomSword);

			//Submit it into the shop
			agentShopSubSystem.SubmitToShop(shopAgent, sword);

			List<UsableItem> shop = agentShopSubSystem.GetShopUsableItems(shopAgent);
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
			ECraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
			UsableItem sword = GetUsableItemByCraftingChoice(randomSword);

			//Should not working
			agentShopSubSystem.SubmitToShop(shopAgent, sword);

			List<UsableItem> shop = agentShopSubSystem.GetShopUsableItems(shopAgent);
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
			ECraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
			UsableItem sword = GetUsableItemByCraftingChoice(randomSword);

			//Add between 5 to 10 items
			int randomItemNumber = UnityEngine.Random.Range(5, 10);
			for (int i = 0; i < randomItemNumber; i++)
			{
				AddItemInInventory(shopAgent.agentInventory, randomSword);
				agentShopSubSystem.SubmitToShop(shopAgent, sword);
			}

			List<UsableItem> shop = agentShopSubSystem.GetShopUsableItems(shopAgent);
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
			for (int i = 0; i < randomItemNumber; i++)
			{
				ECraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
				UsableItem sword = AddItemInInventory(shopAgent.agentInventory, randomSword);
				agentShopSubSystem.SubmitToShop(shopAgent, sword);
			}

			List<UsableItem> shop = agentShopSubSystem.GetShopUsableItems(shopAgent);
			Assert.NotNull(shop, "Empty shop");
			AgentData aData = agentShopSubSystem.GetShop(shopAgent);
			Assert.NotNull(aData, "Empty AgentData");

			//Check the number of items in the shop
			int nbrSwords = 0;
			int nbrSwords2 = 0;
			foreach (UsableItem item in shop)
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
			//To be able to buy any sword because some swords cost more than the default start money
			BaseAdventurerAgent.Wallet.EarnMoney(1000, true);

			//ShopAgent sells a sword
			ECraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
			UsableItem sword = AddItemInInventory(shopAgent.agentInventory, randomSword);
			agentShopSubSystem.SubmitToShop(shopAgent, sword);

			//Adventurer buy the sword
			agentShopSubSystem.PurchaseItem(shopAgent, sword, BaseAdventurerAgent.Wallet, BaseAdventurerAgent.Inventory);

			//Check if no more sword in the ShopAgent inventory
			List<UsableItem> shop = agentShopSubSystem.GetShopUsableItems(shopAgent);
			Assert.IsEmpty(shop, "Should have an empty shop");
			Assert.AreEqual(0, shop.Count);
			Assert.AreEqual(0, agentShopSubSystem.GetNumber(shopAgent, sword.itemDetails));

			//Check if adventurer agent get the sword equipped
			Assert.True(BaseAdventurerAgent.AdventurerInventory.agentInventory.ContainsItem(sword));
			Assert.AreEqual(sword, BaseAdventurerAgent.AdventurerInventory.EquipedItem);

			//Check wallet of the agents
			int priceSword = GetPriceByItemName(sword.itemDetails.itemName);
			Assert.AreEqual(BaseAdventurerAgent.Wallet.startMoney + 1000 - priceSword, BaseAdventurerAgent.Wallet.Money, "Current adventurerAgent money should be StartMoney - PriceSword");
			Assert.AreEqual(shopAgent.wallet.startMoney + priceSword, shopAgent.wallet.Money, "Current shopAgent money should be StartMoney + PriceSword");
		}

		/// <summary>
		/// Test setting a sword price
		/// </summary>
		[Test]
		public void Shop_SetPriceItem()
		{
			//Submit a sword to the shop
			ECraftingChoice randomSword = listCraftingChoices[UnityEngine.Random.Range(0, listCraftingChoices.Count)];
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

		/********************************************TearDown*********************************************/
		[TearDown]
		public new void TearDown()
		{
			base.TearDown();
		}
	}
}
