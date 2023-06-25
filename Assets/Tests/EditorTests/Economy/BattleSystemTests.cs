using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using TurnBased.Scripts;
using UnityEngine;

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

			Assert.AreEqual(adventurerSystem.GetAdventureStates(adventurerAgent), EAdventureStates.OutOfBattle, "AdventureStates.OutOfBattle by default");

			Assert.AreEqual(adventurerSystem.GetObservations(adventurerAgent, null), new float[5], "[0,0,0,0,0] by default");
		}

		/// <summary>
		/// Test to have a new fighter according the battle environment and test if they have a droppable resource with them
		/// </summary>
		[Test]
		public void Battle_FighterInstantation()
		{
			foreach (EBattleEnvironments env in ListEnvironments)
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
			foreach (EBattleEnvironments env in ListEnvironments)
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
			Assert.AreEqual(adventurerSystem.GetAdventureStates(adventurerAgent), EAdventureStates.OutOfBattle, "AdventureStates.OutOfBattle by default");

			//Start battle
	//TODO		adventurerSystem.StartBattle(adventurerAgent, EBattleEnvironments.Forest);

			Assert.AreEqual(adventurerSystem.GetAdventureStates(adventurerAgent), EAdventureStates.InBattle, "AdventureStates.InBattle while a battle");
		}

		/// <summary>
		/// Test the fighterData of the player
		/// </summary>
		[Test]
		public void Battle_FighterDataPlayer()
		{
			BattleSubSystemInstance<AdventurerAgent> battleSubSystemInstance = StartBattle(EBattleEnvironments.Forest);

			/**Player**/
			Assert.IsNotNull(battleSubSystemInstance.PlayerFighterUnits.Instance);

			//Start with 20HP
			Assert.AreEqual(battleSubSystemInstance.PlayerFighterUnits.Instance.MaxHp, 20);
			Assert.AreEqual(battleSubSystemInstance.PlayerFighterUnits.Instance.CurrentHp, battleSubSystemInstance.PlayerFighterUnits.Instance.MaxHp);
			Assert.AreEqual(battleSubSystemInstance.PlayerFighterUnits.Instance.HpPercent, 1);

			//Must have a sprite
			Assert.IsNotNull(battleSubSystemInstance.PlayerFighterUnits.Instance.Sprite);

			//Name by default
			Assert.IsNotNull(battleSubSystemInstance.PlayerFighterUnits.Instance.UnitName);

			//Alive by default
			Assert.True(battleSubSystemInstance.PlayerFighterUnits.Instance.IsDead == false);



			//Heal Damage with full hp
			battleSubSystemInstance.PlayerFighterUnits.Instance.Heal(10);
			Assert.AreEqual(battleSubSystemInstance.PlayerFighterUnits.Instance.CurrentHp, battleSubSystemInstance.PlayerFighterUnits.Instance.MaxHp);
		}

		/// <summary>
		/// Test the fighterData of the enemy
		/// </summary>
		[Test]
		public void Battle_FighterDataEnemy()
		{
			BattleSubSystemInstance<AdventurerAgent> battleSubSystemInstance = StartBattle(EBattleEnvironments.Forest);

			/**Enemy**/
			Assert.IsNotNull(battleSubSystemInstance.EnemyFighterUnits.Instance);

			//Start with MaxHp
			Assert.AreEqual(battleSubSystemInstance.EnemyFighterUnits.Instance.CurrentHp, battleSubSystemInstance.EnemyFighterUnits.Instance.MaxHp);
			Assert.AreEqual(battleSubSystemInstance.EnemyFighterUnits.Instance.HpPercent, 1);

			//Must have a sprite
			Assert.IsNotNull(battleSubSystemInstance.EnemyFighterUnits.Instance.Sprite);

			//Name by default
			Assert.IsNotNull(battleSubSystemInstance.EnemyFighterUnits.Instance.UnitName);

			//Alive by default
			Assert.True(battleSubSystemInstance.EnemyFighterUnits.Instance.IsDead == false);



			//Heal with full hp
			battleSubSystemInstance.EnemyFighterUnits.Instance.Heal(10);
			Assert.AreEqual(battleSubSystemInstance.EnemyFighterUnits.Instance.CurrentHp, battleSubSystemInstance.EnemyFighterUnits.Instance.MaxHp);
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
			Assert.IsNotNull(adventurerData.PlayerData);
			Assert.IsNotNull(adventurerData.adventurerInventory);
		}

		/// <summary>
		/// Test GameOver function
		/// </summary>
		[Test]
		public void Battle_GameOver()
		{
			//Start Battle
			//TODO adventurerSystem.StartBattle(adventurerAgent, EBattleEnvironments.Forest);
			// BattleSubSystemInstance<AdventurerAgent> battleSubSystemInstance = adventurerSystem.GetSubSystem(adventurerAgent);

			//Assert.True(battleSubSystemInstance.GameOver() == false);

			
			// adventurerSystem.OnFleeButton(adventurerAgent);
			//Assert.True(battleSubSystemInstance.GameOver() == true);
		}


		/// <summary>
		/// Test BattleState.PlayerTurn
		/// </summary>
		[Test]
		public void Battle_StatePlayerTurnByDefault()
		{
			BattleSubSystemInstance<AdventurerAgent> battleSubSystemInstance = StartBattle(EBattleEnvironments.Forest);
			Assert.AreEqual(EBattleState.PlayerTurn, battleSubSystemInstance.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystemInstance.CurrentState);
		}


		/// <summary>
		/// Test BattleState.Won
		/// </summary>
		[Test]
		public void Battle_StateWon()
		{
			BattleSubSystemInstance<AdventurerAgent> battleSubSystemInstance = StartBattle(EBattleEnvironments.Forest);

			//Attack => Won
			battleSubSystemInstance.EnemyFighterUnits.Instance.CurrentHp = 1;
			// adventurerSystem.OnAttackButton(adventurerAgent);
			Assert.AreEqual(EBattleState.Won, battleSubSystemInstance.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystemInstance.CurrentState);
		}

		/// <summary>
		/// Test BattleState.Lost
		/// </summary>
		[Test]
		public void Battle_StateLost()
		{
			BattleSubSystemInstance<AdventurerAgent> battleSubSystemInstance = StartBattle(EBattleEnvironments.Forest);

			//EnemyAttack => Loose
			battleSubSystemInstance.PlayerFighterUnits.Instance.CurrentHp = 1;
			// adventurerSystem.OnAttackButton(adventurerAgent);
			Assert.AreEqual(EBattleState.Lost, battleSubSystemInstance.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystemInstance.CurrentState);
		}

		/// <summary>
		/// Test Heal feature
		/// </summary>
		[Test]
		public void Battle_ActionHeal()
		{
			BattleSubSystemInstance<AdventurerAgent> battleSubSystemInstance = StartBattle(EBattleEnvironments.Forest);

			AdventurerFighterData adventurerData = adventurerAgent.GetComponent<AdventurerFighterData>();

			//adventurerSystem.OnHealButton(adventurerAgent);

			battleSubSystemInstance.PlayerFighterUnits.Instance.CurrentHp = 1;
			battleSubSystemInstance.PlayerFighterUnits.Instance.Heal(5);
			Assert.AreEqual(6, battleSubSystemInstance.PlayerFighterUnits.Instance.CurrentHp);

			battleSubSystemInstance.PlayerFighterUnits.Instance.CurrentHp = 1;
			battleSubSystemInstance.PlayerFighterUnits.Instance.Heal(100);
			Assert.AreEqual(adventurerData.startHp, battleSubSystemInstance.PlayerFighterUnits.Instance.CurrentHp, "Healing can't overcome the start hp  threshold");
		}

		/// <summary>
		/// Test Flee feature
		/// </summary>
		[Test]
		public void Battle_ActionFlee()
		{
			BattleSubSystemInstance<AdventurerAgent> battleSubSystemInstance = StartBattle(EBattleEnvironments.Forest);

			//Flee
			// adventurerSystem.OnFleeButton(adventurerAgent);
			Assert.True(battleSubSystemInstance.CurrentState == EBattleState.Flee);
		}

		/// <summary>
		/// Test HP management when attack
		/// </summary>
		[Test]
		public void Battle_ActionAttack()
		{
			BattleSubSystemInstance<AdventurerAgent> battleSubSystemInstance = StartBattle(EBattleEnvironments.Forest);

			//Attack Dmg
			int enemyHPBeforeAttack = battleSubSystemInstance.EnemyFighterUnits.Instance.CurrentHp;
			battleSubSystemInstance.PlayerFighterUnits.Instance.Attack(battleSubSystemInstance.EnemyFighterUnits.Instance);
			Assert.AreEqual(enemyHPBeforeAttack - battleSubSystemInstance.PlayerFighterUnits.Instance.Damage, battleSubSystemInstance.EnemyFighterUnits.Instance.CurrentHp,
				"enemyHPBeforeAttack : " + enemyHPBeforeAttack + " - battleSubSystem.PlayerFighterUnit.Damage : " + battleSubSystemInstance.PlayerFighterUnits.Instance.Damage);

			//HP always positives
			battleSubSystemInstance.EnemyFighterUnits.Instance.CurrentHp = 1;
			battleSubSystemInstance.PlayerFighterUnits.Instance.Attack(battleSubSystemInstance.EnemyFighterUnits.Instance);
			Assert.AreEqual(0, battleSubSystemInstance.EnemyFighterUnits.Instance.CurrentHp);
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
			ECraftingResources randomECraftingResource = listCraftingResources[Random.Range(0, listCraftingResources.Count)];
			
			// TODO FIX THIS
			//Make a request
			// requestShopSystem.MakeChoice(shopAgent, randomCraftingResource);

			//Count the requests
			Assert.AreEqual(1, requestSystem.GetAllCraftingRequests(shopAgent.craftingInventory).Count, "A request should be created");
			Assert.AreEqual(1, requestSystem.GetAllCraftingRequests().Count, "One request should be created");

			// TODO FIX THIS
			//Make another request with the same resource
			// requestShopSystem.MakeChoice(shopAgent, randomCraftingResource);

			//Count the requests
			Assert.AreEqual(1, requestSystem.GetAllCraftingRequests().Count, "Only one request should be created");


			//randomCraftingResource1 != randomCraftingResource2
			ECraftingResources randomECraftingResource2 = listCraftingResources[Random.Range(0, listCraftingResources.Count)];
			if (randomECraftingResource2 == randomECraftingResource && listCraftingResources.Count > 1)
			{
				randomECraftingResource2 = (ECraftingResources)((int)(randomECraftingResource + 1) % listCraftingResources.Count);
			}
			//Make another request
			// TODO FIX THIS
			//requestShopSystem.MakeChoice(shopAgent, randomCraftingResource2);

			//Count the requests
			Assert.AreEqual(2, requestSystem.GetAllCraftingRequests().Count, "2 requests should be created");
		}


		/// <summary>
		/// Test adventurer taking a request
		/// </summary>
		[Test]
		public void Request_TakeResourceRequest()
		{
			ECraftingResources randomECraftingRessource = listCraftingResources[UnityEngine.Random.Range(0, listCraftingResources.Count)];

			//Make a request
			// TODO FIX THIS
			// requestShopSystem.MakeChoice(shopAgent, randomCraftingRessource);

			CraftingResourceRequest requestMade = requestShopSystem.requestSystem.GetAllCraftingRequests()[0];
			Assert.NotNull(requestMade, "CraftingResourceRequest empty");

			//Take a request
			adventurerAgent.requestTaker.TakeRequest(requestMade);

			//Check the taken quest
			var takenQuests = adventurerAgent.requestTaker.GetItemList();
			Assert.AreEqual(1, takenQuests.Count(), "The adventurer should have one request");
		}

		/// <summary>
		/// Test succeed a resource request
		/// </summary>
		[Test]
		public void Request_CompleteResourceRequest()
		{
			ECraftingResources randomECraftingRessource = listCraftingResources[UnityEngine.Random.Range(0, listCraftingResources.Count)];

			//Make a request
			// TODO FIX THIS
			// requestShopSystem.MakeChoice(shopAgent, randomCraftingRessource);
			CraftingResourceRequest requestMade = requestShopSystem.requestSystem.GetAllCraftingRequests()[0];
			int reward = requestMade.Reward;

			//Take a request
			adventurerAgent.requestTaker.TakeRequest(requestMade);

			//Give the ressource = Complete request
			adventurerAgent.requestTaker.CheckItemAdd(randomECraftingRessource, 1);

			//No more requests?
			Assert.AreEqual(0, requestSystem.GetAllCraftingRequests().Count, "No more requests after giving the resource");
			//Reward obtained?
			Assert.AreEqual(adventurerAgent.wallet.startMoney + reward, adventurerAgent.wallet.Money);
			//Resource given?
			Assert.AreEqual(0, adventurerAgent.requestTaker.GetCurrentStock(randomECraftingRessource));
		}

		/********************************************TearDown*********************************************/
		[TearDown]
		public new void TearDown()
		{
			base.TearDown();
		}
	}
}
