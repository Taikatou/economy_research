using System.Collections.Generic;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;

namespace TurnBased.Scripts
{
	public enum BattleState { Start, PlayerTurn, EnemyTurn, Won, Lost }

	public class BattleSubSystem
	{
		public BattleState CurrentState { get; private set; }
		public BaseFighterData PlayerFighterUnit { get; }
		public BaseFighterData EnemyFighterUnit { get; }
		private readonly FighterDropTable _fighterDropTable;
		public string DialogueText { get; private set; }
		public BattleSubSystem(BaseFighterData playerUnit, BaseFighterData enemyUnit, FighterDropTable fighterDropTable)
		{
			CurrentState = BattleState.Start;
			PlayerFighterUnit = playerUnit;
			EnemyFighterUnit = enemyUnit;

			CurrentState = BattleState.PlayerTurn;
			PlayerTurn();

			_fighterDropTable = fighterDropTable;
		}

		public bool GameOver()
		{
			return CurrentState == BattleState.Lost || CurrentState == BattleState.Won;
		}

		private void PlayerAttack()
		{
			var isDead = EnemyFighterUnit.TakeDamage(PlayerFighterUnit.Damage);
			PlayerFighterUnit.AfterAttack();
			
			DialogueText = "The attack is successful!";

			if(isDead)
			{
				CurrentState = BattleState.Won;
				EndBattle();
			} else
			{
				CurrentState = BattleState.EnemyTurn;
				EnemyTurn();
			}
		}

		private void EnemyTurn()
		{
			DialogueText = EnemyFighterUnit.UnitName + " attacks!";

			var isDead = PlayerFighterUnit.TakeDamage(EnemyFighterUnit.Damage);

			if(isDead)
			{
				CurrentState = BattleState.Lost;
				EndBattle();
			} else
			{
				CurrentState = BattleState.PlayerTurn;
				PlayerTurn();
			}
			CurrentState = BattleState.PlayerTurn;
		}

		private void EndBattle()
		{
			if(CurrentState == BattleState.Won)
			{
				DialogueText = "You won the battle!";
			} else if (CurrentState == BattleState.Lost)
			{
				DialogueText = "You were defeated.";
			}
		}

		private void PlayerTurn()
		{
			DialogueText = "Choose an action:";
		}

		private void PlayerHeal()
		{
			PlayerFighterUnit.Heal(5);
			
			DialogueText = "You feel renewed strength!";

			CurrentState = BattleState.EnemyTurn;
			EnemyTurn();
		}

		public void OnAttackButton()
		{
			if (CurrentState != BattleState.PlayerTurn)
				return;
			
			PlayerAttack();
		}

		public void OnHealButton()
		{
			if (CurrentState != BattleState.PlayerTurn)
				return;

			PlayerHeal();
		}

		public CraftingDropReturn GetCraftingDropItem()
		{
			return _fighterDropTable.GenerateItems();
		}
	}
}