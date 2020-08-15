using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace TurnBased.Scripts
{
	public enum BattleState { Start, PlayerTurn, EnemyTurn, Won, Lost }

	public class BattleSubSystem
	{
		public BattleState CurrentState;
		
		private readonly FighterUnit _playerFighterUnit;
		private readonly FighterUnit _enemyFighterUnit;

		public string DialogueText { get; private set; }
		public BattleSubSystem(FighterUnit playerUnit, FighterUnit enemyUnit)
		{
			CurrentState = BattleState.Start;
			_playerFighterUnit = playerUnit;
			_enemyFighterUnit = enemyUnit;

			CurrentState = BattleState.PlayerTurn;
			PlayerTurn();
		}

		public bool GameOver()
		{
			return CurrentState == BattleState.Lost || CurrentState == BattleState.Won;
		}

		private void PlayerAttack()
		{
			var isDead = _enemyFighterUnit.TakeDamage(_playerFighterUnit.damage);
			
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
			DialogueText = _enemyFighterUnit.unitName + " attacks!";

			var isDead = _playerFighterUnit.TakeDamage(_enemyFighterUnit.damage);

			if(isDead)
			{
				CurrentState = BattleState.Lost;
				EndBattle();
			} else
			{
				CurrentState = BattleState.PlayerTurn;
				PlayerTurn();
			}
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
			_playerFighterUnit.Heal(5);
			
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
	}
}