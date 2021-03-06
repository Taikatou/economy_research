﻿namespace TurnBased.Scripts
{
	public delegate void OnWinDelegate();
	public enum BattleState { Start, PlayerTurn, EnemyTurn, Won, Lost, Flee }
	public enum BattleAction { Attack, Heal, Flee }

	public class BattleSubSystem
	{
		public BattleState CurrentState { get; private set; }
		public BaseFighterData PlayerFighterUnit { get; }
		public BaseFighterData EnemyFighterUnit { get; }
		private readonly FighterDropTable _fighterDropTable;
		public string DialogueText { get; private set; }
		private OnWinDelegate winDelegate;
		public BattleSubSystem(BaseFighterData playerUnit, BaseFighterData enemyUnit, FighterDropTable fighterDropTable, OnWinDelegate winDelegate)
		{
			CurrentState = BattleState.Start;
			PlayerFighterUnit = playerUnit;
			EnemyFighterUnit = enemyUnit;

			CurrentState = BattleState.PlayerTurn;
			PlayerTurn();

			_fighterDropTable = fighterDropTable;
			this.winDelegate += winDelegate;
		}

		public bool GameOver()
		{
			return CurrentState == BattleState.Lost || CurrentState == BattleState.Won || CurrentState == BattleState.Flee;
		}

		private void PlayerAttack()
		{
			PlayerFighterUnit.Attack(EnemyFighterUnit);

			DialogueText = "The attack is successful!";

			if(EnemyFighterUnit.IsDead)
			{
				CurrentState = BattleState.Won;
				winDelegate?.Invoke();
				EndBattle();
			}
			else
			{
				CurrentState = BattleState.EnemyTurn;
				EnemyTurn();
			}
		}

		private void EnemyTurn()
		{
			EnemyFighterUnit.Attack(PlayerFighterUnit);
			DialogueText = EnemyFighterUnit.UnitName + " attacks!";

			if(PlayerFighterUnit.IsDead)
			{
				CurrentState = BattleState.Lost;
				EndBattle();
			}
			else
			{
				CurrentState = BattleState.PlayerTurn;
				PlayerTurn();
			}
		}

		private void EndBattle()
		{
			if (CurrentState == BattleState.Won)
			{
				DialogueText = "You won the battle!";
			}
			else if (CurrentState == BattleState.Lost)
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

		public void OnFleeButton()
		{
			if (CurrentState != BattleState.PlayerTurn)
				return;

			CurrentState = BattleState.Flee;
		}

		public void SetInput(BattleAction action)
		{
			switch (action)
			{
				case BattleAction.Attack:
						OnAttackButton();
					break;
				case BattleAction.Heal:
						OnHealButton();
					break;
				case BattleAction.Flee:
						OnFleeButton();
					break;
			}
		}

		public CraftingDropReturn GetCraftingDropItem()
		{
			return _fighterDropTable.GenerateItems();
		}

		public float[] GetSenses()
		{
			return new []
			{
				PlayerFighterUnit.Damage, PlayerFighterUnit.HpPercent,
				EnemyFighterUnit.Damage, EnemyFighterUnit.HpPercent
			};
		}

		public const int GetSenseSize = 4;
	}
}