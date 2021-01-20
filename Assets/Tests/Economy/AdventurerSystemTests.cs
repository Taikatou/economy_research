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

using TurnBased.Scripts;




namespace Tests.Economy
{
	public class AdventurerSystemTests
	{
		AdventurerSystemBehaviour adventurerSystemBehaviour;
		AdventurerSystem adventurerSystem;
		GetCurrentAdventurerAgent getAdventurerAgent;
		GetCurrentShopAgent getShopAgent;
		AdventurerAgent adventurerAgent;
		ShopAgent shopAgent;
		SystemSpawner systemSpawner;
		TravelSubSystem travelSubsystem;
		AdventurerInventory adventurerInventory;
		AgentInventory agentInventory;
		//RequestShopSystem requestShopSystem;

		public List<BattleEnvironments> listEnvironments = new List<BattleEnvironments> { BattleEnvironments.Forest, BattleEnvironments.Mountain, BattleEnvironments.Sea, BattleEnvironments.Volcano };


		[SetUp]
		public void Setup()
		{
			adventurerSystemBehaviour = Resources.FindObjectsOfTypeAll<AdventurerSystemBehaviour>()[0];
			adventurerSystem = adventurerSystemBehaviour.system;

			//Create 1 adventurer agent
			systemSpawner = Resources.FindObjectsOfTypeAll<SystemSpawner>()[0];
			systemSpawner.numLearningAgents = 1;
			systemSpawner.Start();
			getAdventurerAgent = Resources.FindObjectsOfTypeAll<GetCurrentAdventurerAgent>()[0];
			adventurerAgent = getAdventurerAgent.CurrentAgent;

			//Generate PlayerFighterData
			adventurerAgent.gameObject.GetComponent<AdventurerFighterData>().Start();
			adventurerAgent.gameObject.GetComponent<AdventurerFighterData>().Start();

			//Generate adventurerInventory
			adventurerInventory = adventurerAgent.adventurerInventory;

			//Generate agentInventory
			agentInventory = adventurerAgent.inventory;
			agentInventory.ResetInventory();

			//Generate travelSubsystem
			travelSubsystem = adventurerSystem.travelSubsystem;
			travelSubsystem.Start();
			adventurerSystem.travelSubsystem = travelSubsystem;

			/*
			getShopAgent = Resources.FindObjectsOfTypeAll<GetCurrentShopAgent>()[0];
			shopAgent = getShopAgent.CurrentAgent;
			*/
		}

		/********************************************Wallet*********************************************/
		/// <summary>
		/// Test having the BattleSubSystem from the agent
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

		/// <summary>
		/// Test having the BattleSubSystem from the agent
		/// </summary>
		[Test]
		public void Wallet_ResetWallet()
		{
			adventurerAgent.ResetEconomyAgent();
		}

		/********************************************Travel**************************************************/

		/// <summary>
		/// Test if an aventurer can travel to the 4 places : forest, mountain, sea, volcano 
		/// </summary>
		[Test]
		public void Travel_TravelSystem()
		{
			bool isForestExist = Enum.IsDefined(typeof(BattleEnvironments), "Forest");
			bool isMoutainExist = Enum.IsDefined(typeof(BattleEnvironments), "Mountain");
			bool isSeaExist = Enum.IsDefined(typeof(BattleEnvironments), "Sea");
			bool isVolcanoExist = Enum.IsDefined(typeof(BattleEnvironments), "Volcano");
			Assert.True(isForestExist && isMoutainExist && isSeaExist && isVolcanoExist == true);
		}

		/********************************************Resources*********************************************/

		/// <summary>
		/// Test items : resources
		/// </summary>
		[Test]
		public void Resources_CraftingResources()
		{
			bool isNothingExist = Enum.IsDefined(typeof(CraftingResources), "Nothing");
			bool isWoodExist = Enum.IsDefined(typeof(CraftingResources), "Wood");
			bool isMetalExist = Enum.IsDefined(typeof(CraftingResources), "Metal");
			bool isGemExist = Enum.IsDefined(typeof(CraftingResources), "Gem");
			bool isDragonScaleExist = Enum.IsDefined(typeof(CraftingResources), "DragonScale");
			Assert.False(isNothingExist && isWoodExist && isMetalExist && isGemExist && isDragonScaleExist == false, "Enumeration CraftingResources wrong");
		}

		/********************************************Adventurer*********************************************/

		/// <summary>
		/// Should have only one adventurer agent because we spawn only one in the setup
		/// </summary>
		[Test]
		public void Agent_NoAgentByDefault()
		{
			AdventurerFighterData[] ls = Resources.FindObjectsOfTypeAll<AdventurerFighterData>();
			Assert.AreEqual(1, ls.Length, "Nbr Of AdventurerAdgents : " + ls.Length);
		}

		/********************************************Battle System*********************************************/
		/// <summary>
		/// Test the existence of the adventures states : InBattle and OutOfBattle
		/// </summary>
		[Test]
		public void Battle_AdventureStates()
		{
			bool isInBattleExist = Enum.IsDefined(typeof(AdventureStates), "InBattle");
			bool isOutOfBattleExist = Enum.IsDefined(typeof(AdventureStates), "OutOfBattle");
			Assert.True(isInBattleExist && isOutOfBattleExist == true);
		}

		/// <summary>
		/// Test if there is a battle by default
		/// </summary>
		[Test]
		public void Battle_NoBattleByDefault()
		{
			Assert.False(adventurerSystem.GetBattleCount() != 0, "No battle by default. Number of active battles : " + adventurerSystem.GetBattleCount());

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
			adventurerSystem.StartBattle(adventurerAgent, BattleEnvironments.Forest);

			BattleSubSystem battleSubSystem = adventurerSystem.GetSubSystem(adventurerAgent);
			Assert.IsNotNull(battleSubSystem);

			

			/******************************Player**************************************/
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
			adventurerSystem.StartBattle(adventurerAgent, BattleEnvironments.Forest);

			BattleSubSystem battleSubSystem = adventurerSystem.GetSubSystem(adventurerAgent);
			Assert.IsNotNull(battleSubSystem);

			/******************************Enemy**************************************/
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
		/// No Battle_AdventurerFighterData
		/// //public enum BattleAction { Attack, Heal, Flee }
		/// </summary>
		[Test]
		public void Battle_BattleAction()
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
		public void Battle_BattleState()
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
			BattleSubSystem battleSubSystem = StartBattle(BattleEnvironments.Forest);

			//Attack
			adventurerSystem.OnAttackButton(adventurerAgent);
			Assert.AreEqual(BattleState.EnemyTurn, battleSubSystem.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystem.CurrentState);
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

			//Heal
			adventurerSystem.OnHealButton(adventurerAgent);
			Assert.True(battleSubSystem.CurrentState == BattleState.PlayerTurn);

			//Test the HP
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







		



		/// <summary>
		/// Test 
		/// </summary>
		public BattleSubSystem StartBattle(BattleEnvironments env)
		{
			adventurerSystem.StartBattle(adventurerAgent, env);
			return adventurerSystem.GetSubSystem(adventurerAgent);
		}












		/*

		/// <summary>
		/// Test taking random request
		/// </summary>

		public void TestSetupRequestSystem()
		{
			//Check types of requests
			bool isSetInputExist = Enum.IsDefined(typeof(RequestActions), "SetInput");
			bool isRemoveRequestExist = Enum.IsDefined(typeof(RequestActions), "RemoveRequest");
			bool isIncreasePriceExist = Enum.IsDefined(typeof(RequestActions), "IncreasePrice");
			bool isDecreasePriceExist = Enum.IsDefined(typeof(RequestActions), "DecreasePrice");
			Assert.False(isSetInputExist && isRemoveRequestExist && isIncreasePriceExist && isDecreasePriceExist == false, "Enumeration RequestActions wrong");


			//Empty list of request by default
			int countRequests = requestShopSystem.requestSystem.GetAllCraftingRequests().Count;
			Assert.False(countRequests != 0, "Some request(s) are already made by default");
		}


		/// <summary>
		/// Test taking random request
		/// </summary>

		public void TestTakeRequest()
		{
			//Make a request
			CraftingResources craftResourceToRequest = CraftingResources.Wood;
			requestShopSystem.MakeChoice(shopAgent, craftResourceToRequest);

			//Check the request
			int countRequests = requestShopSystem.requestSystem.GetAllCraftingRequests().Count;
			Assert.False(countRequests != 1, "A request should be created");

			CraftingResourceRequest requestMade = requestShopSystem.requestSystem.GetAllCraftingRequests()[0];
			Assert.False(requestMade == null, "CraftingResourceRequest empty");

			//Take a request
			requestShopSystem.requestSystem.TakeRequest(adventurerAgent.requestTaker, requestMade);

			//Check the taken quest
			List<CraftingResourceRequest> takenQuests = adventurerAgent.requestTaker.GetItemList();
			Assert.False(takenQuests.Count != 1, "The adventurer should have one request");

			//Accomplish request
		}



		/// <summary>
		/// Test buying ans equipping equipments
		/// </summary>

		public void TestBuyEquipment()
		{
			Assert.True(true);
		}


		*/

	}
}
