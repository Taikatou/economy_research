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

			Assert.AreEqual(adventurerSystem.GetObservations(adventurerAgent), new float[5], "[0,0,0,0,0] by default");
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
			BattleSubSystem<AdventurerAgent> battleSubSystem = StartBattle(EBattleEnvironments.Forest);

			/**Player**/
			Assert.IsNotNull(battleSubSystem.PlayerFighterUnits.Instance);

			//Start with 20HP
			Assert.AreEqual(battleSubSystem.PlayerFighterUnits.Instance.MaxHp, 20);
			Assert.AreEqual(battleSubSystem.PlayerFighterUnits.Instance.CurrentHp, battleSubSystem.PlayerFighterUnits.Instance.MaxHp);
			Assert.AreEqual(battleSubSystem.PlayerFighterUnits.Instance.HpPercent, 1);

			//Must have a sprite
			Assert.IsNotNull(battleSubSystem.PlayerFighterUnits.Instance.Sprite);

			//Name by default
			Assert.IsNotNull(battleSubSystem.PlayerFighterUnits.Instance.UnitName);

			//Alive by default
			Assert.True(battleSubSystem.PlayerFighterUnits.Instance.IsDead == false);



			//Heal Damage with full hp
			battleSubSystem.PlayerFighterUnits.Instance.Heal(10);
			Assert.AreEqual(battleSubSystem.PlayerFighterUnits.Instance.CurrentHp, battleSubSystem.PlayerFighterUnits.Instance.MaxHp);
		}

		/// <summary>
		/// Test the fighterData of the enemy
		/// </summary>
		[Test]
		public void Battle_FighterDataEnemy()
		{
			BattleSubSystem<AdventurerAgent> battleSubSystem = StartBattle(EBattleEnvironments.Forest);

			/**Enemy**/
			Assert.IsNotNull(battleSubSystem.EnemyFighterUnits.Instance);

			//Start with MaxHp
			Assert.AreEqual(battleSubSystem.EnemyFighterUnits.Instance.CurrentHp, battleSubSystem.EnemyFighterUnits.Instance.MaxHp);
			Assert.AreEqual(battleSubSystem.EnemyFighterUnits.Instance.HpPercent, 1);

			//Must have a sprite
			Assert.IsNotNull(battleSubSystem.EnemyFighterUnits.Instance.Sprite);

			//Name by default
			Assert.IsNotNull(battleSubSystem.EnemyFighterUnits.Instance.UnitName);

			//Alive by default
			Assert.True(battleSubSystem.EnemyFighterUnits.Instance.IsDead == false);



			//Heal with full hp
			battleSubSystem.EnemyFighterUnits.Instance.Heal(10);
			Assert.AreEqual(battleSubSystem.EnemyFighterUnits.Instance.CurrentHp, battleSubSystem.EnemyFighterUnits.Instance.MaxHp);
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
			//TODO adventurerSystem.StartBattle(adventurerAgent, EBattleEnvironments.Forest);
			BattleSubSystem<AdventurerAgent> battleSubSystem = adventurerSystem.GetSubSystem(adventurerAgent);

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
			BattleSubSystem<AdventurerAgent> battleSubSystem = StartBattle(EBattleEnvironments.Forest);
			Assert.AreEqual(EBattleState.PlayerTurn, battleSubSystem.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystem.CurrentState);
		}


		/// <summary>
		/// Test BattleState.Won
		/// </summary>
		[Test]
		public void Battle_StateWon()
		{
			BattleSubSystem<AdventurerAgent> battleSubSystem = StartBattle(EBattleEnvironments.Forest);

			//Attack => Won
			battleSubSystem.EnemyFighterUnits.Instance.CurrentHp = 1;
			adventurerSystem.OnAttackButton(adventurerAgent);
			Assert.AreEqual(EBattleState.Won, battleSubSystem.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystem.CurrentState);
		}

		/// <summary>
		/// Test BattleState.Lost
		/// </summary>
		[Test]
		public void Battle_StateLost()
		{
			BattleSubSystem<AdventurerAgent> battleSubSystem = StartBattle(EBattleEnvironments.Forest);

			//EnemyAttack => Loose
			battleSubSystem.PlayerFighterUnits.Instance.CurrentHp = 1;
			adventurerSystem.OnAttackButton(adventurerAgent);
			Assert.AreEqual(EBattleState.Lost, battleSubSystem.CurrentState, "battleSubSystem.CurrentState : " + battleSubSystem.CurrentState);
		}

		/// <summary>
		/// Test Heal feature
		/// </summary>
		[Test]
		public void Battle_ActionHeal()
		{
			BattleSubSystem<AdventurerAgent> battleSubSystem = StartBattle(EBattleEnvironments.Forest);

			AdventurerFighterData adventurerData = adventurerAgent.GetComponent<AdventurerFighterData>();

			//adventurerSystem.OnHealButton(adventurerAgent);

			battleSubSystem.PlayerFighterUnits.Instance.CurrentHp = 1;
			battleSubSystem.PlayerFighterUnits.Instance.Heal(5);
			Assert.AreEqual(6, battleSubSystem.PlayerFighterUnits.Instance.CurrentHp);

			battleSubSystem.PlayerFighterUnits.Instance.CurrentHp = 1;
			battleSubSystem.PlayerFighterUnits.Instance.Heal(100);
			Assert.AreEqual(adventurerData.startHp, battleSubSystem.PlayerFighterUnits.Instance.CurrentHp, "Healing can't overcome the start hp  threshold");
		}

		/// <summary>
		/// Test Flee feature
		/// </summary>
		[Test]
		public void Battle_ActionFlee()
		{
			BattleSubSystem<AdventurerAgent> battleSubSystem = StartBattle(EBattleEnvironments.Forest);

			//Flee
			adventurerSystem.OnFleeButton(adventurerAgent);
			Assert.True(battleSubSystem.CurrentState == EBattleState.Flee);
		}

		/// <summary>
		/// Test HP management when attack
		/// </summary>
		[Test]
		public void Battle_ActionAttack()
		{
			BattleSubSystem<AdventurerAgent> battleSubSystem = StartBattle(EBattleEnvironments.Forest);

			//Attack Dmg
			int enemyHPBeforeAttack = battleSubSystem.EnemyFighterUnits.Instance.CurrentHp;
			battleSubSystem.PlayerFighterUnits.Instance.Attack(battleSubSystem.EnemyFighterUnits.Instance);
			Assert.AreEqual(enemyHPBeforeAttack - battleSubSystem.PlayerFighterUnits.Instance.Damage, battleSubSystem.EnemyFighterUnits.Instance.CurrentHp,
				"enemyHPBeforeAttack : " + enemyHPBeforeAttack + " - battleSubSystem.PlayerFighterUnit.Damage : " + battleSubSystem.PlayerFighterUnits.Instance.Damage);

			//HP always positives
			battleSubSystem.EnemyFighterUnits.Instance.CurrentHp = 1;
			battleSubSystem.PlayerFighterUnits.Instance.Attack(battleSubSystem.EnemyFighterUnits.Instance);
			Assert.AreEqual(0, battleSubSystem.EnemyFighterUnits.Instance.CurrentHp);
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
