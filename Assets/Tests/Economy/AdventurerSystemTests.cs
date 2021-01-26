using NUnit.Framework;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.MLAgents;

using EconomyProject.Monobehaviours;
using EconomyProject.Scripts;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using Inventory;
using TurnBased.Scripts;

using LootBoxes.Generated;


namespace Tests.Economy
{
	public class AdventurerSystemTests
	{
		AdventurerSystemBehaviour adventurerSystemBehaviour;
		AdventurerSystem adventurerSystem;
		GetCurrentAdventurerAgent getAdventurerAgent;
		AdventurerAgent adventurerAgent;

		TravelSubSystem travelSubsystem;
		AdventurerInventory adventurerInventory;
		AgentInventory adventurerAgentInventory;

		ShopCraftingSystemBehaviour shopCraftingSystemBehaviour;
		CraftingSubSystem craftingSubSystem;
		AgentShopSubSystem agentShopSubSubSystem;
		GetCurrentShopAgent getShopAgent;
		ShopAgent shopAgent;
		AgentInventory shopAgentInventory;

		RequestShopSystemBehaviour requestShopSystemBehaviour;
		RequestShopSystem requestShopSystem;
		RequestSystem requestSystem;

		public List<BattleEnvironments> listEnvironments = new List<BattleEnvironments> { BattleEnvironments.Forest, BattleEnvironments.Mountain, BattleEnvironments.Sea, BattleEnvironments.Volcano };
		public List<CraftingResources> listCraftingResources = new List<CraftingResources> { CraftingResources.Wood, CraftingResources.Metal, CraftingResources.Gem, CraftingResources.DragonScale };


		[SetUp]
		public void Setup()
		{
			adventurerSystemBehaviour = GameObject.FindObjectOfType<AdventurerSystemBehaviour>();
			adventurerSystem = adventurerSystemBehaviour.system;

			SpawnAgents();

			//Get Adventurer Agent
			getAdventurerAgent = GameObject.FindObjectOfType<GetCurrentAdventurerAgent>();
			adventurerAgent = getAdventurerAgent.CurrentAgent;

			//Generate travelSubsystem of the adventurerSystem
			travelSubsystem = adventurerSystem.travelSubsystem;
			travelSubsystem.Start();
			adventurerSystem.travelSubsystem = travelSubsystem;

			//Generate PlayerFighterData of the adventurerAgent
			adventurerAgent.gameObject.GetComponent<AdventurerFighterData>().Start();

			//Generate adventurerInventory of the adventurerAgent
			adventurerInventory = adventurerAgent.adventurerInventory;

			//Generate agentInventory of the adventurerAgent
			adventurerAgentInventory = adventurerAgent.inventory;
			adventurerAgent.ResetEconomyAgent();

			//Generate AdventurerRequestTaker
			adventurerAgent.requestTaker.Start();


			shopCraftingSystemBehaviour = GameObject.FindObjectOfType<ShopCraftingSystemBehaviour>();
			agentShopSubSubSystem = shopCraftingSystemBehaviour.system.shopSubSubSystem;
			craftingSubSystem = shopCraftingSystemBehaviour.system.craftingSubSubSystem;

			//Get ShopAgent
			getShopAgent = GameObject.FindObjectOfType<GetCurrentShopAgent>();
			shopAgent = getShopAgent.CurrentAgent;
			shopAgent.shopInput.Awake();
			shopAgent.agentInventory.ResetInventory();
			shopAgent.craftingInventory.ResetInventory();
			shopAgent.wallet.Reset();


			requestShopSystemBehaviour = GameObject.FindObjectOfType<RequestShopSystemBehaviour>();
			requestShopSystem = requestShopSystemBehaviour.system;
			requestSystem = requestShopSystem.requestSystem;
			requestSystem.Start();
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
			Assert.AreEqual(0, itemDetails.baseDurability);
			Assert.AreEqual(0, itemDetails.durability);
			Assert.AreEqual(5, itemDetails.damage);
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

			List<BaseItemPrices> basePrices = agentShopSubSubSystem.basePrices;
			
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

			List<BaseItemPrices> basePrices = agentShopSubSubSystem.basePrices;

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

			List<BaseItemPrices> basePrices = agentShopSubSubSystem.basePrices;

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
			List<BaseItemPrices> basePrices = agentShopSubSubSystem.basePrices;

			for (int i = 0; i < basePrices.Count; i++)
			{
				//Reset inventory
				adventurerAgent.ResetEconomyAgent();

				//Sword
				UsableItem itemToAdd = basePrices[i].item;

				//Add to the inventory
				adventurerAgent.inventory.AddItem(itemToAdd);

				//Check if contains it
				Assert.AreEqual(itemToAdd, adventurerAgent.adventurerInventory.EquipedItem, "Equiped Item : " + adventurerAgent.adventurerInventory.EquipedItem.ToString());
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
			bool isForestExist = Enum.IsDefined(typeof(BattleEnvironments), "Forest");
			bool isMoutainExist = Enum.IsDefined(typeof(BattleEnvironments), "Mountain");
			bool isSeaExist = Enum.IsDefined(typeof(BattleEnvironments), "Sea");
			bool isVolcanoExist = Enum.IsDefined(typeof(BattleEnvironments), "Volcano");
			Assert.True(isForestExist && isMoutainExist && isSeaExist && isVolcanoExist == true);
		}

		/// <summary>
		/// Test items : resources
		/// </summary>
		[Test]
		public void Enum_CraftingResources()
		{
			bool isNothingExist = Enum.IsDefined(typeof(CraftingResources), "Nothing");
			bool isWoodExist = Enum.IsDefined(typeof(CraftingResources), "Wood");
			bool isMetalExist = Enum.IsDefined(typeof(CraftingResources), "Metal");
			bool isGemExist = Enum.IsDefined(typeof(CraftingResources), "Gem");
			bool isDragonScaleExist = Enum.IsDefined(typeof(CraftingResources), "DragonScale");
			Assert.False(isNothingExist && isWoodExist && isMetalExist && isGemExist && isDragonScaleExist == false, "Enumeration CraftingResources wrong");
		}

		/// <summary>
		/// Test the existence of the adventures states : InBattle and OutOfBattle
		/// </summary>
		[Test]
		public void Enum_AdventureStates()
		{
			bool isInBattleExist = Enum.IsDefined(typeof(AdventureStates), "InBattle");
			bool isOutOfBattleExist = Enum.IsDefined(typeof(AdventureStates), "OutOfBattle");
			Assert.True(isInBattleExist && isOutOfBattleExist == true);
		}

		/// <summary>
		/// No Battle_AdventurerFighterData
		/// //public enum BattleAction { Attack, Heal, Flee }
		/// </summary>
		[Test]
		public void Enum_BattleAction()
		{
			bool isAttackExist = Enum.IsDefined(typeof(BattleAction), "Attack");
			bool isHealExist = Enum.IsDefined(typeof(BattleAction), "Heal");
			bool isFleeExist = Enum.IsDefined(typeof(BattleAction), "Flee");
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
			bool isExist1 = Enum.IsDefined(typeof(RequestActions), "SetInput");
			bool isExist2 = Enum.IsDefined(typeof(RequestActions), "RemoveRequest");
			bool isExist3 = Enum.IsDefined(typeof(RequestActions), "DecreasePrice");
			bool isExist4 = Enum.IsDefined(typeof(RequestActions), "IncreasePrice");
			Assert.True(isExist1 && isExist2 && isExist3 && isExist4 == true);
		}


		/********************************************Spawn*********************************************/

		/// <summary>
		/// Should have only one adventurer agent because we spawn only one in the setup
		/// </summary>
		[Test]
		public void Spawn_OneAdventurerSpawned()
		{
			AdventurerAgent[] adventurerAgents = getAdventurerAgent.GetAgents;
			if (adventurerAgents.Length != 1)
			{
				//DebugAdventurers();
			}
			Assert.AreEqual(1, adventurerAgents.Length);
		}


		/********************************************Battle System*********************************************/
		/// <summary>
		/// Test if there is a battle by default
		/// </summary>
		[Test]
		public void Battle_NoBattleByDefault()
		{
			if(adventurerSystem.GetBattleCount() != 0)
			{
				//DebugBattleSystems();
			}
				

			Assert.AreEqual(0, adventurerSystem.GetBattleCount(), "No battle by default. Number of active battles : " + adventurerSystem.GetBattleCount());

			Assert.True(adventurerSystem.CanMove(adventurerAgent) == true, "CanMove=true by default");

			Assert.AreEqual(adventurerSystem.GetAdventureStates(adventurerAgent), AdventureStates.OutOfBattle, "AdventureStates.OutOfBattle by default");

			Assert.AreEqual(adventurerSystem.GetSenses(adventurerAgent), new float[5], "[0,0,0,0,0] by default");
		}

		/// <summary>
		/// Test to have a new fighter according the battle environment and test if they have a droppable resource with them
		/// </summary>
		[Test]
		public void Battle_FighterInstantation()
		{
			foreach (BattleEnvironments env in listEnvironments)
			{
				FighterObject newFighter = travelSubsystem.GetBattle(env);
				Assert.IsNotNull(newFighter);
				
				//test if the fighter has an resource
				CraftingDropReturn craftingDropReturn = newFighter.fighterDropTable.GenerateItems();

				Assert.True(craftingDropReturn.Resource > 0 & craftingDropReturn.Count > 0,
					"Environment : " + env + " - Resource : " + craftingDropReturn.Resource + " - Count : " + craftingDropReturn.Count);
			}
		}

		/// <summary>
		/// Test GetAdventureStates, OutOfBattle by default
		/// </summary>
		[Test]
		public void Battle_GetAdventureStates()
		{
			Assert.AreEqual(adventurerSystem.GetAdventureStates(adventurerAgent), AdventureStates.OutOfBattle, "AdventureStates.OutOfBattle by default");

			//Start battle
			adventurerSystem.StartBattle(adventurerAgent, BattleEnvironments.Forest);

			Assert.AreEqual(adventurerSystem.GetAdventureStates(adventurerAgent), AdventureStates.InBattle, "AdventureStates.InBattle while a battle");
		}

		/// <summary>
		/// Test the fighterData of the player
		/// </summary>
		[Test]
		public void Battle_FighterDataPlayer()
		{
			BattleSubSystem battleSubSystem = StartBattle(BattleEnvironments.Forest);

			/**Player**/
			Assert.IsNotNull(battleSubSystem.PlayerFighterUnit);

			//Start with 20HP
			Assert.AreEqual(battleSubSystem.PlayerFighterUnit.MaxHp, 20);
			Assert.AreEqual(battleSubSystem.PlayerFighterUnit.CurrentHp, battleSubSystem.PlayerFighterUnit.MaxHp);
			Assert.AreEqual(battleSubSystem.PlayerFighterUnit.HpPercent, 1);

			//Must have a sprite
			Assert.IsNotNull(battleSubSystem.PlayerFighterUnit.Sprite);

			//Name by default
			Assert.IsNotNull(battleSubSystem.PlayerFighterUnit.UnitName);

			//Alive by default
			Assert.True(battleSubSystem.PlayerFighterUnit.IsDead == false);



			//Heal Damage with full hp
			battleSubSystem.PlayerFighterUnit.Heal(10);
			Assert.AreEqual(battleSubSystem.PlayerFighterUnit.CurrentHp, battleSubSystem.PlayerFighterUnit.MaxHp);
		}

		/// <summary>
		/// Test the fighterData of the enemy
		/// </summary>
		[Test]
		public void Battle_FighterDataEnemy()
		{
			BattleSubSystem battleSubSystem = StartBattle(BattleEnvironments.Forest);

			/**Enemy**/
			Assert.IsNotNull(battleSubSystem.EnemyFighterUnit);

			//Start with MaxHp
			Assert.AreEqual(battleSubSystem.EnemyFighterUnit.CurrentHp, battleSubSystem.EnemyFighterUnit.MaxHp);
			Assert.AreEqual(battleSubSystem.EnemyFighterUnit.HpPercent, 1);

			//Must have a sprite
			Assert.IsNotNull(battleSubSystem.EnemyFighterUnit.Sprite);

			//Name by default
			Assert.IsNotNull(battleSubSystem.EnemyFighterUnit.UnitName);

			//Alive by default
			Assert.True(battleSubSystem.EnemyFighterUnit.IsDead == false);



			//Heal with full hp
			battleSubSystem.EnemyFighterUnit.Heal(10);
			Assert.AreEqual(battleSubSystem.EnemyFighterUnit.CurrentHp, battleSubSystem.EnemyFighterUnit.MaxHp);
		}

		/// <summary>
		/// No Battle_AdventurerFighterData
		/// </summary>
		[Test]
		public void Battle_AdventurerFighterData()
		{
			AdventurerFighterData adventurerData = adventurerAgent.GetComponent<AdventurerFighterData>();

			Assert.AreEqual(20, adventurerData.startHp);
			Assert.IsNotNull(adventurerData.sprite);
			Assert.IsNotNull(adventurerData.playerData);
			Assert.IsNotNull(adventurerData.adventurerInventory);
		}

		/// <summary>
		/// Test GameOver function
		/// </summary>
		[Test]
		public void Battle_GameOver()
		{
			//Start Battle
			adventurerSystem.StartBattle(adventurerAgent, BattleEnvironments.Forest);
			BattleSubSystem battleSubSystem = adventurerSystem.GetSubSystem(adventurerAgent);

			Assert.True(battleSubSystem.GameOver() == false);

			adventurerSystem.OnFleeButton(adventurerAgent);
			Assert.True(battleSubSystem.GameOver() == true);
		}


		/// <summary>
		/// Test BattleState.PlayerTurn
		/// </summary>
		[Test]
		public void Battle_StatePlayerTurnByDefault()
		{
			BattleSubSystem battleSubSystem = StartBattle(BattleEnvironments.Forest);
			Assert.AreEqual(BattleState.PlayerTurn, battleSubSystem.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystem.CurrentState);
		}

		/// <summary>
		/// Test BattleState.EnemyTurn
		/// </summary>
		[Test]
		public void Battle_StateEnemyTurn()
		{
			/*
			 * 
			 * Can't be tested due to private functions
			 * 
			 * 
			BattleSubSystem battleSubSystem = StartBattle(BattleEnvironments.Forest);

			//Attack
			adventurerSystem.OnAttackButton(adventurerAgent);
			Assert.AreEqual(BattleState.EnemyTurn, battleSubSystem.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystem.CurrentState);
			*/
			Assert.True(true);
		}

		/// <summary>
		/// Test BattleState.Won
		/// </summary>
		[Test]
		public void Battle_StateWon()
		{
			BattleSubSystem battleSubSystem = StartBattle(BattleEnvironments.Forest);

			//Attack => Won
			battleSubSystem.EnemyFighterUnit.CurrentHp = 1;
			adventurerSystem.OnAttackButton(adventurerAgent);
			Assert.AreEqual(BattleState.Won, battleSubSystem.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystem.CurrentState);
		}

		/// <summary>
		/// Test BattleState.Lost
		/// </summary>
		[Test]
		public void Battle_StateLost()
		{
			BattleSubSystem battleSubSystem = StartBattle(BattleEnvironments.Forest);

			//EnemyAttack => Loose
			battleSubSystem.PlayerFighterUnit.CurrentHp = 1;
			adventurerSystem.OnAttackButton(adventurerAgent);
			Assert.AreEqual(BattleState.Lost, battleSubSystem.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystem.CurrentState);
		}

		/// <summary>
		/// Test Heal feature
		/// </summary>
		[Test]
		public void Battle_ActionHeal()
		{
			BattleSubSystem battleSubSystem = StartBattle(BattleEnvironments.Forest);

			AdventurerFighterData adventurerData = adventurerAgent.GetComponent<AdventurerFighterData>();

			//adventurerSystem.OnHealButton(adventurerAgent);

			battleSubSystem.PlayerFighterUnit.CurrentHp = 1;
			battleSubSystem.PlayerFighterUnit.Heal(5);
			Assert.AreEqual(6, battleSubSystem.PlayerFighterUnit.CurrentHp);

			battleSubSystem.PlayerFighterUnit.CurrentHp = 1;
			battleSubSystem.PlayerFighterUnit.Heal(100);
			Assert.AreEqual(adventurerData.startHp, battleSubSystem.PlayerFighterUnit.CurrentHp, "Healing can't overcome the start hp  threshold");
		}

		/// <summary>
		/// Test Flee feature
		/// </summary>
		[Test]
		public void Battle_ActionFlee()
		{
			BattleSubSystem battleSubSystem = StartBattle(BattleEnvironments.Forest);

			//Flee
			adventurerSystem.OnFleeButton(adventurerAgent);
			Assert.True(battleSubSystem.CurrentState == BattleState.Flee);
		}

		/// <summary>
		/// Test HP management when attack
		/// </summary>
		[Test]
		public void Battle_ActionAttack()
		{
			BattleSubSystem battleSubSystem = StartBattle(BattleEnvironments.Forest);

			//Attack Dmg
			int enemyHPBeforeAttack = battleSubSystem.EnemyFighterUnit.CurrentHp;
			battleSubSystem.PlayerFighterUnit.Attack(battleSubSystem.EnemyFighterUnit);
			Assert.AreEqual(enemyHPBeforeAttack - battleSubSystem.PlayerFighterUnit.Damage, battleSubSystem.EnemyFighterUnit.CurrentHp, 
				"enemyHPBeforeAttack : " + enemyHPBeforeAttack + " - battleSubSystem.PlayerFighterUnit.Damage : " + battleSubSystem.PlayerFighterUnit.Damage);

			//HP always positives
			battleSubSystem.EnemyFighterUnit.CurrentHp = 1;
			battleSubSystem.PlayerFighterUnit.Attack(battleSubSystem.EnemyFighterUnit);
			Assert.AreEqual(0, battleSubSystem.EnemyFighterUnit.CurrentHp);
		}



		/********************************************Request*********************************************/
		/// <summary>
		/// Test taking random request
		/// </summary>
		[Test]
		public void Request_RequestShopSystemByDefault()
		{
			//Empty list of request by default
			int countRequests = requestSystem.GetAllCraftingRequests().Count;
			Assert.Zero(countRequests, "Some request(s) are already made by default");

			//Default CanMove
			Assert.True(requestShopSystem.CanMove(shopAgent));
		}


		/// <summary>
		/// Test making random request
		/// </summary>
		[Test]
		public void Request_MakeResourceRequest()
		{
			CraftingResources randomCraftingRessource = listCraftingResources[UnityEngine.Random.Range(0, listCraftingResources.Count)];

			//Make a request
			requestShopSystem.MakeChoice(shopAgent, randomCraftingRessource);
			//requestShopSystem.MakeChoice(shopAgent, CraftingResources.Wood);


			//Count the requests
			Assert.AreEqual(1, requestSystem.GetAllCraftingRequests(shopAgent.craftingInventory).Count, "A request should be created");
			Assert.AreEqual(1, requestSystem.GetAllCraftingRequests().Count, "A request should be created");

			CraftingResources randomCraftingRessource2 = listCraftingResources[UnityEngine.Random.Range(0, listCraftingResources.Count)];

			//Make another request
			requestShopSystem.MakeChoice(shopAgent, randomCraftingRessource2);
			//requestShopSystem.MakeChoice(shopAgent, CraftingResources.Metal);

			//Count the requests
			Assert.AreEqual(2, requestSystem.GetAllCraftingRequests().Count, "2 requests should be created");
		}


		/// <summary>
		/// Test adventurer taking a request
		/// </summary>
		[Test]
		public void Request_TakeResourceRequest()
		{
			CraftingResources randomCraftingRessource = listCraftingResources[UnityEngine.Random.Range(0, listCraftingResources.Count)];

			//Make a request
			requestShopSystem.MakeChoice(shopAgent, randomCraftingRessource);

			CraftingResourceRequest requestMade = requestShopSystem.requestSystem.GetAllCraftingRequests()[0];
			Assert.NotNull(requestMade, "CraftingResourceRequest empty");

			//Take a request
			adventurerAgent.requestTaker.TakeRequest(requestMade);

			//Check the taken quest
			List<CraftingResourceRequest> takenQuests = adventurerAgent.requestTaker.GetItemList();
			Assert.AreEqual(1, takenQuests.Count, "The adventurer should have one request");
		}

		/// <summary>
		/// Test succeed a resource request
		/// </summary>
		[Test]
		public void Request_CompleteResourceRequest()
		{
			CraftingResources randomCraftingRessource = listCraftingResources[UnityEngine.Random.Range(0, listCraftingResources.Count)];

			//Make a request
			requestShopSystem.MakeChoice(shopAgent, randomCraftingRessource);
			CraftingResourceRequest requestMade = requestShopSystem.requestSystem.GetAllCraftingRequests()[0];
			int reward = requestMade.Reward;

			//Take a request
			adventurerAgent.requestTaker.TakeRequest(requestMade);

			//Give the ressource = Complete request
			adventurerAgent.requestTaker.CheckItemAdd(randomCraftingRessource, 1);

			//No more requests?
			Assert.AreEqual(0, requestSystem.GetAllCraftingRequests().Count, "No more requests after giving the resource");
			//Reward obtained?
			Assert.AreEqual(adventurerAgent.wallet.startMoney + reward, adventurerAgent.wallet.Money);
			//Resource given?
			Assert.AreEqual(0, adventurerAgent.requestTaker.GetCurrentStock(randomCraftingRessource));
		}






		/********************************************Helper*********************************************/
		/// <summary>
		/// Spawn one adventurer agent and one shop agent
		/// </summary>
		public void SpawnAgents()
		{
			//Create 1 adventurer agent
			//SystemSpawner[] systemSpawners = Resources.FindObjectsOfTypeAll<SystemSpawner>();
			SystemSpawner[] systemSpawners = GameObject.FindObjectsOfType<SystemSpawner>();

			SystemSpawner adventurerSpawner = systemSpawners[0];
			adventurerSpawner.numLearningAgents = 1;
			adventurerSpawner.Start();

			
			SystemSpawner agentSpawner = systemSpawners[1];
			agentSpawner.numLearningAgents = 1;
			agentSpawner.Start();
		}


		/// <summary>
		/// Start a battle and return the BattleSubSystem associated to this battle
		/// </summary>
		public BattleSubSystem StartBattle(BattleEnvironments env)
		{
			adventurerSystem.StartBattle(adventurerAgent, env);
			return adventurerSystem.GetSubSystem(adventurerAgent);
		}

		/// <summary>
		/// Debug all the battle
		/// </summary>
		public void DebugBattleSystems()
		{
			Debug.Log("Number of battles : " + adventurerSystem.battleSystems.Count);
			foreach (KeyValuePair<AdventurerAgent, BattleSubSystem> battleSystem in adventurerSystem.battleSystems)
			{
				Debug.Log("Agent : " + battleSystem.Key
					+ "\n Battle : " + battleSystem.Value
					+ "\n CurrentState : " + battleSystem.Value.CurrentState
					+ "\n Enemy : " + battleSystem.Value.EnemyFighterUnit.UnitName
					);
			}
		}

		/// <summary>
		/// Debug Adventurer Agents present in the scene
		/// </summary>
		public void DebugAdventurers()
		{
			AdventurerAgent[] adventurerAgents = GameObject.FindObjectsOfType<AdventurerAgent>();

			Debug.Log("Number of AdventurerAgent : " + adventurerAgents.Length);
			foreach (AdventurerAgent agent in adventurerAgents)
			{
				Debug.Log("Agent.name : " + agent.name);
			}
		}

		/// <summary>
		/// Debug Adventurer Agents items in inventory
		/// </summary>
		public void DebugItemsInInventory(AdventurerAgent agent)
		{
			AgentInventory inventory = agent.inventory;

			Debug.Log("Number of Items : " + inventory.Items.Count);
			foreach (KeyValuePair<string, List<UsableItem>> item in inventory.Items)
			{
				Debug.Log("item : " + item.Key
					+ "\n item list count: " + item.Value.Count
					);

				foreach(UsableItem usableItem in item.Value)
				{
					Debug.Log("usableItem name : " + usableItem.itemDetails.itemName
					+ "\n baseDurability: " + usableItem.itemDetails.baseDurability
					+ "\n durability : " + usableItem.itemDetails.durability
					+ "\n damage : " + usableItem.itemDetails.damage
					+ "\n unBreakable : " + usableItem.itemDetails.unBreakable
					+ "\n Broken : " + usableItem.itemDetails.Broken
					);
				}
			}
		}

		/// <summary>
		/// Get sword UsableItem by name
		/// </summary>
		public UsableItem GetSwordByName(String swordName)
		{
			List<BaseItemPrices> basePrices = agentShopSubSubSystem.basePrices;

			foreach(BaseItemPrices item in basePrices)
			{
				if(item.item.ToString() == swordName)
				{
					return item.item;
				}
			}

			Debug.LogWarning("Not found sword called : " + swordName);
			return null;
		}

		/// <summary>
		/// Get sword UsableItem by name
		/// </summary>
		public CraftingResources[] GetCraftingResources()
		{
			return new CraftingResources[] { CraftingResources.Wood, CraftingResources.Metal, CraftingResources.Gem, CraftingResources.DragonScale };
		}
	}
}