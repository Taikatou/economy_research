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

namespace Tests.Economy
{
	public class AdventurerSystemTests: TestSystem
	{

		[SetUp]
		public void Setup()
		{
			Init();
		}

		/********************************************Inventory*********************************************/

		/// <summary>
		/// Test if only 'urnamed' object by default
		/// </summary>
		[Test]
		public void Inventory_UnarmedDefault()
		{
			//Reset inventory
			adventurerAgent.ResetEconomyAgent();

			if (adventurerAgent.inventory.Items.Count > 1)
			{
				DebugItemsInInventory(adventurerAgent);
				Assert.True(false, "adventurerAgent.inventory.Items.Count : " + adventurerAgent.inventory.Items.Count);
			}

			Assert.AreEqual(1, adventurerAgent.inventory.Items.Count, "Unarmed by default");
			Assert.True(adventurerAgent.inventory.Items.ContainsKey("Unarmed"), "Unarmed by default");

			Assert.AreEqual(1, adventurerAgent.inventory.Items.Count, "Unarmed by default");
		}

		/// <summary>
		/// Test caracteristics of the objects
		/// </summary>
		[Test]
		public void Inventory_UnarmedDetails()
		{
			//Reset inventory
			adventurerAgent.ResetEconomyAgent();

			//Unarmed
			UsableItemDetails itemDetails = adventurerAgent.inventory.Items["Unarmed"][0].itemDetails;
			Assert.AreEqual("Unarmed", itemDetails.itemName);
			Assert.AreEqual(ItemData.BaseDurabilities["Unarmed"], itemDetails.baseDurability);
			Assert.AreEqual(itemDetails.durability, itemDetails.baseDurability);
			Assert.AreEqual(ItemData.BaseDamages["Unarmed"], itemDetails.damage);
			Assert.AreEqual(true, itemDetails.unBreakable);
			Assert.AreEqual(false, itemDetails.Broken);
		}

		/// <summary>
		/// Test if the object is present in the inventory
		/// </summary>
		[Test]
		public void Inventory_ContainItem()
		{
			//Reset inventory
			adventurerAgent.ResetEconomyAgent();

			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;
			
			for(int i = 0; i < basePrices.Count; i++)
			{
				//Sword
				UsableItem itemToAdd = basePrices[i].item;

				//Add to the inventory
				adventurerAgent.inventory.AddItem(itemToAdd);

				//Check if contains it
				Assert.True(adventurerAgent.inventory.ContainsItem(itemToAdd));
			}
		}

		/// <summary>
		/// Test to remove an item in the inventory
		/// </summary>
		[Test]
		public void Inventory_RemoveItem()
		{
			//Reset inventory
			adventurerAgent.ResetEconomyAgent();

			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;

			for (int i = 0; i < basePrices.Count; i++)
			{
				//Sword
				UsableItem itemToAdd = basePrices[i].item;

				//Add to the inventory
				adventurerAgent.inventory.AddItem(itemToAdd);

				//Remove from the inventory
				adventurerAgent.inventory.RemoveItem(itemToAdd);

				//Check if contains it
				Assert.False(adventurerAgent.inventory.ContainsItem(itemToAdd));
			}
		}

		/// <summary>
		/// Test to decrease the durability of an equipped sword
		/// </summary>
		[Test]
		public void Inventory_DecreaseDurability()
		{
			//Reset inventory
			adventurerAgent.ResetEconomyAgent();

			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;

			for (int i = 0; i < basePrices.Count; i++)
			{
				//Sword
				UsableItem itemToAdd = basePrices[i].item;
				itemToAdd.itemDetails.ResetDurability();

				//Add to the inventory
				adventurerAgent.inventory.AddItem(itemToAdd);
				UsableItem itemInInventory = adventurerAgent.inventory.Items[itemToAdd.ToString()][0];

				//Check durability by default
				int durability = itemInInventory.itemDetails.durability;
				Assert.AreEqual(itemInInventory.itemDetails.durability, itemInInventory.itemDetails.baseDurability);

				adventurerAgent.inventory.DecreaseDurability(itemInInventory);

				//Check if it decreased
				Assert.AreEqual(durability - 1 , itemInInventory.itemDetails.durability);
			}
		}

		/// <summary>
		/// Test equip an item as the main sword
		/// </summary>
		[Test]
		public void Inventory_EquipItem()
		{
			//Reset inventory
			adventurerAgent.ResetEconomyAgent();
			//Reset Item
			adventurerAgent.adventurerInventory.EquipedItem.itemDetails.damage = ItemData.BaseDamages[adventurerAgent.adventurerInventory.EquipedItem.ToString()];

			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;

			for (int i = 0; i < basePrices.Count; i++)
			{
				//Reset inventory
				adventurerAgent.ResetEconomyAgent();

				//Sword
				UsableItem itemToAdd = basePrices[i].item;

				//Add to the inventory
				adventurerAgent.inventory.AddItem(itemToAdd);

				//Check if contains it
				Assert.AreEqual(itemToAdd, adventurerAgent.adventurerInventory.EquipedItem, "i : " + i + " - Equiped Item : " + adventurerAgent.adventurerInventory.EquipedItem.ToString());
			}

			//Try to check if the player equips the stronger sword
			adventurerAgent.inventory.AddItem(basePrices[0].item);
			UsableItem ultimateSword = GetSwordByName("Ultimate Sword");
			Assert.AreEqual(ultimateSword,adventurerAgent.adventurerInventory.EquipedItem, 
				"Should keep the best equipment. Equiped Item : " + adventurerAgent.adventurerInventory.EquipedItem.ToString());

		}

		/********************************************Wallet*********************************************/
		/// <summary>
		/// Test Wallet of the adventurer agent
		/// </summary>
		[Test]
		public void Wallet_Wallet()
		{
			adventurerAgent.wallet.Reset();
			Assert.AreEqual(adventurerAgent.wallet.startMoney, adventurerAgent.wallet.Money, "Start money should be 100");

			int money = adventurerAgent.wallet.Money;
			Assert.IsNotNull(money);

			adventurerAgent.wallet.EarnMoney(10);
			Assert.AreEqual(money + 10, adventurerAgent.wallet.Money);

			adventurerAgent.wallet.LoseMoney(-20);
			Assert.AreEqual(money - 10, adventurerAgent.wallet.Money);

			adventurerAgent.wallet.SpendMoney(30);
			Assert.AreEqual(money - 40, adventurerAgent.wallet.Money);

			adventurerAgent.wallet.SetMoney(234);
			Assert.AreEqual(234, adventurerAgent.wallet.Money);

			adventurerAgent.wallet.LoseMoney(1000);
			Assert.AreEqual(0, adventurerAgent.wallet.Money);
		}

		/********************************************Enum**************************************************/

		/// <summary>
		/// Test if an aventurer can travel to the 4 places : forest, mountain, sea, volcano 
		/// </summary>
		[Test]
		public void Enum_TravelSystem()
		{
			bool isForestExist = Enum.IsDefined(typeof(EBattleEnvironments), "Forest");
			bool isMoutainExist = Enum.IsDefined(typeof(EBattleEnvironments), "Mountain");
			bool isSeaExist = Enum.IsDefined(typeof(EBattleEnvironments), "Sea");
			bool isVolcanoExist = Enum.IsDefined(typeof(EBattleEnvironments), "Volcano");
			Assert.True(isForestExist && isMoutainExist && isSeaExist && isVolcanoExist == true);
		}

		/// <summary>
		/// Test items : resources
		/// </summary>
		[Test]
		public void Enum_CraftingResources()
		{
			bool isNothingExist = Enum.IsDefined(typeof(ECraftingResources), "Nothing");
			bool isWoodExist = Enum.IsDefined(typeof(ECraftingResources), "Wood");
			bool isMetalExist = Enum.IsDefined(typeof(ECraftingResources), "Metal");
			bool isGemExist = Enum.IsDefined(typeof(ECraftingResources), "Gem");
			bool isDragonScaleExist = Enum.IsDefined(typeof(ECraftingResources), "DragonScale");
			Assert.False(isNothingExist && isWoodExist && isMetalExist && isGemExist && isDragonScaleExist == false, "Enumeration CraftingResources wrong");
		}

		/// <summary>
		/// Test the existence of the adventures states : InBattle and OutOfBattle
		/// </summary>
		[Test]
		public void Enum_AdventureStates()
		{
			bool isInBattleExist = Enum.IsDefined(typeof(EAdventureStates), "InBattle");
			bool isOutOfBattleExist = Enum.IsDefined(typeof(EAdventureStates), "OutOfBattle");
			Assert.True(isInBattleExist && isOutOfBattleExist == true);
		}

		/// <summary>
		/// No Battle_AdventurerFighterData
		/// //public enum BattleAction { Attack, Heal, Flee }
		/// </summary>
		[Test]
		public void Enum_BattleAction()
		{
			bool isAttackExist = Enum.IsDefined(typeof(EBattleAction), "Attack");
			bool isHealExist = Enum.IsDefined(typeof(EBattleAction), "Heal");
			bool isFleeExist = Enum.IsDefined(typeof(EBattleAction), "Flee");
			Assert.True(isAttackExist && isHealExist && isFleeExist == true);
		}

		/// <summary>
		/// Test BattleState enum
		/// </summary>
		[Test]
		public void Enum_BattleState()
		{
			bool isStartExist = Enum.IsDefined(typeof(BattleState), "Start");
			bool isPlayerTurnExist = Enum.IsDefined(typeof(BattleState), "PlayerTurn");
			bool isEnemyTurnExist = Enum.IsDefined(typeof(BattleState), "EnemyTurn");
			bool isWonExist = Enum.IsDefined(typeof(BattleState), "Won");
			bool isLostExist = Enum.IsDefined(typeof(BattleState), "Lost");
			bool isFleeExist = Enum.IsDefined(typeof(BattleState), "Flee");
			Assert.True(isStartExist && isPlayerTurnExist && isEnemyTurnExist && isWonExist && isLostExist && isFleeExist == true);
		}

		/// <summary>
		/// Test enumeration RequestActions
		/// </summary>
		[Test]
		public void Enum_RequestActions()
		{
			//TODO FIX THIS conor
	/*		bool isExist1 = Enum.IsDefined(typeof(RequestActions), "SetInput");
			bool isExist2 = Enum.IsDefined(typeof(RequestActions), "RemoveRequest");
			bool isExist3 = Enum.IsDefined(typeof(RequestActions), "DecreasePrice");
			bool isExist4 = Enum.IsDefined(typeof(RequestActions), "IncreasePrice");
			Assert.True(isExist1 && isExist2 && isExist3 && isExist4 == true);*/
		}


		/********************************************Spawn*********************************************/

		/// <summary>
		/// Should have only one adventurer agent because we spawn only one in the setup
		/// </summary>
		[Test]
		public void Spawn_OneAdventurerSpawned()
		{
			AdventurerAgent[] adventurerAgents = getAdventurerAgent.GetAgents;
			Assert.AreEqual(1, adventurerAgents.Length);
		}

		/********************************************TearDown*********************************************/
		[TearDown]
		public new void TearDown()
		{
			base.TearDown();
		}
	}
}