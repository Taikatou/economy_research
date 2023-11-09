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
			BaseAdventurerAgent.OnEpisodeBegin();

			if (BaseAdventurerAgent.Inventory.Items.Count > 1)
			{
				DebugItemsInInventory(BaseAdventurerAgent);
				Assert.True(false, "adventurerAgent.inventory.Items.Count : " + BaseAdventurerAgent.Inventory.Items.Count);
			}

			Assert.AreEqual(1, BaseAdventurerAgent.Inventory.Items.Count, "Unarmed by default");
			// TODO FIX THIS PLEASE
			// Assert.True(adventurerAgent.inventory.Items.ContainsKey("Unarmed"), "Unarmed by default");

			Assert.AreEqual(1, BaseAdventurerAgent.Inventory.Items.Count, "Unarmed by default");
		}

		/// <summary>
		/// Test caracteristics of the objects
		/// </summary>
		[Test]
		public void Inventory_UnarmedDetails()
		{
			//Reset inventory
			BaseAdventurerAgent.OnEpisodeBegin();

			//Unarmed
			//TODO FIX THIS PLEASE
			UsableItemDetails itemDetails = BaseAdventurerAgent.Inventory.Items["Unarmed"][0].itemDetails;
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
			BaseAdventurerAgent.OnEpisodeBegin();

			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;
			
			for(int i = 0; i < basePrices.Count; i++)
			{
				//Sword
				UsableItem itemToAdd = basePrices[i].item;

				//Add to the inventory
				BaseAdventurerAgent.Inventory.AddItem(itemToAdd);

				//Check if contains it
				Assert.True(BaseAdventurerAgent.Inventory.ContainsItem(itemToAdd));
			}
		}

		/// <summary>
		/// Test to remove an item in the inventory
		/// </summary>
		[Test]
		public void Inventory_RemoveItem()
		{
			//Reset inventory
			BaseAdventurerAgent.OnEpisodeBegin();

			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;

			for (int i = 0; i < basePrices.Count; i++)
			{
				//Sword
				UsableItem itemToAdd = basePrices[i].item;

				//Add to the inventory
				BaseAdventurerAgent.Inventory.AddItem(itemToAdd);

				//Remove from the inventory
				BaseAdventurerAgent.Inventory.RemoveItem(itemToAdd);

				//Check if contains it
				Assert.False(BaseAdventurerAgent.Inventory.ContainsItem(itemToAdd));
			}
		}

		/// <summary>
		/// Test to decrease the durability of an equipped sword
		/// </summary>
		[Test]
		public void Inventory_DecreaseDurability()
		{
			//Reset inventory
			BaseAdventurerAgent.OnEpisodeBegin();

			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;

			for (int i = 0; i < basePrices.Count; i++)
			{
				//Sword
				UsableItem itemToAdd = basePrices[i].item;
				itemToAdd.itemDetails.ResetDurability();

				//Add to the inventory
				BaseAdventurerAgent.Inventory.AddItem(itemToAdd);
				
				//TODO FIX THIS PLEASE
				UsableItem itemInInventory = BaseAdventurerAgent.Inventory.Items[itemToAdd.ToString()][0];

				//Check durability by default
				int durability = itemInInventory.itemDetails.durability;
				Assert.AreEqual(itemInInventory.itemDetails.durability, itemInInventory.itemDetails.baseDurability);

				BaseAdventurerAgent.Inventory.DecreaseDurability(itemInInventory);

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
			BaseAdventurerAgent.OnEpisodeBegin();
			//Reset Item
			BaseAdventurerAgent.AdventurerInventory.EquipedItem.itemDetails.damage = ItemData.BaseDamages[BaseAdventurerAgent.AdventurerInventory.EquipedItem.ToString()];

			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;

			for (int i = 0; i < basePrices.Count; i++)
			{
				//Reset inventory
				BaseAdventurerAgent.OnEpisodeBegin();

				//Sword
				UsableItem itemToAdd = basePrices[i].item;

				//Add to the inventory
				BaseAdventurerAgent.Inventory.AddItem(itemToAdd);

				//Check if contains it
				Assert.AreEqual(itemToAdd, BaseAdventurerAgent.AdventurerInventory.EquipedItem, "i : " + i + " - Equiped Item : " + BaseAdventurerAgent.AdventurerInventory.EquipedItem.ToString());
			}

			//Try to check if the player equips the stronger sword
			BaseAdventurerAgent.Inventory.AddItem(basePrices[0].item);
			UsableItem ultimateSword = GetSwordByName("Ultimate Sword");
			Assert.AreEqual(ultimateSword,BaseAdventurerAgent.AdventurerInventory.EquipedItem, 
				"Should keep the best equipment. Equiped Item : " + BaseAdventurerAgent.AdventurerInventory.EquipedItem.ToString());

		}

		/********************************************Wallet*********************************************/
		/// <summary>
		/// Test Wallet of the adventurer agent
		/// </summary>
		[Test]
		public void Wallet_Wallet()
		{
			BaseAdventurerAgent.OnEpisodeBegin();
			Assert.AreEqual(BaseAdventurerAgent.Wallet.startMoney, BaseAdventurerAgent.Wallet.Money, "Start money should be 100");

			int money = BaseAdventurerAgent.Wallet.Money;
			Assert.IsNotNull(money);

			BaseAdventurerAgent.Wallet.EarnMoney(10, false);
			Assert.AreEqual(money + 10, BaseAdventurerAgent.Wallet.Money);

			BaseAdventurerAgent.Wallet.LoseMoney(-20);
			Assert.AreEqual(money - 10, BaseAdventurerAgent.Wallet.Money);

			BaseAdventurerAgent.Wallet.SpendMoney(30);
			Assert.AreEqual(money - 40, BaseAdventurerAgent.Wallet.Money);

			BaseAdventurerAgent.Wallet.SetMoney(234);
			Assert.AreEqual(234, BaseAdventurerAgent.Wallet.Money);

			BaseAdventurerAgent.Wallet.LoseMoney(1000);
			Assert.AreEqual(0, BaseAdventurerAgent.Wallet.Money);
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
			bool isStartExist = Enum.IsDefined(typeof(EBattleState), "Start");
			bool isPlayerTurnExist = Enum.IsDefined(typeof(EBattleState), "PlayerTurn");
			bool isEnemyTurnExist = Enum.IsDefined(typeof(EBattleState), "EnemyTurn");
			bool isWonExist = Enum.IsDefined(typeof(EBattleState), "Won");
			bool isLostExist = Enum.IsDefined(typeof(EBattleState), "Lost");
			bool isFleeExist = Enum.IsDefined(typeof(EBattleState), "Flee");
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
			BaseAdventurerAgent[] adventurerAgents = getAdventurerAgent.GetAgents;
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