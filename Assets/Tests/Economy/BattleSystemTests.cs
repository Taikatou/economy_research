using NUnit.Framework;

using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using TurnBased.Scripts;


namespace Tests.Economy
{
	public class BattleSystemTests : TestSystem
	{
		[SetUp]
		public void Setup()
		{
			Init();
		}

		/********************************************Battle*********************************************/

		/// <summary>
		/// Test if there is a battle by default
		/// </summary>
		[Test]
		public void Battle_NoBattleByDefault()
		{
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
			}
		}

		/// <summary>
		/// Test to if there is a droppable resource in the enemy
		/// </summary>
		[Test]
		public void Battle_FighterDropResource()
		{
			foreach (BattleEnvironments env in listEnvironments)
			{
				FighterObject newFighter = travelSubsystem.GetBattle(env);

				//test if the fighter has an resource
				CraftingDropReturn craftingDropReturn = newFighter.fighterDropTable.GenerateItems();

				//Check if a resource has been drop and accept if the enemy didn't drop any resource
				Assert.True(craftingDropReturn.Resource >= 0 & craftingDropReturn.Count >= 0,
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

			//Count the requests
			Assert.AreEqual(1, requestSystem.GetAllCraftingRequests(shopAgent.craftingInventory).Count, "A request should be created");
			Assert.AreEqual(1, requestSystem.GetAllCraftingRequests().Count, "A request should be created");

			CraftingResources randomCraftingRessource2 = listCraftingResources[UnityEngine.Random.Range(0, listCraftingResources.Count)];

			//Make another request
			requestShopSystem.MakeChoice(shopAgent, randomCraftingRessource2);

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
	}
}
