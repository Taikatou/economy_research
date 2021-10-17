using Data;
using Unity.MLAgents;

namespace TurnBased.Scripts
{
	public delegate void OnWinDelegate();

	public delegate void OnBattleComplete(BattleSubSystem battle);
	public enum EBattleState { Start, PlayerTurn, EnemyTurn, Won, Lost, Flee }
	public enum EBattleAction { Attack, Heal, Flee }

	public class FighterGroup
	{
		public int Index = 0;
		public BaseFighterData[] FighterUnits;
		public BaseFighterData Instance => FighterUnits[Index];
	}

	public class BattleSubSystem
	{
		public EBattleState CurrentState { get; private set; }
		public string DialogueText { get; private set; }
		
		private readonly FighterDropTable _fighterDropTable;
		private readonly OnWinDelegate _winDelegate;
		private readonly OnBattleComplete _completeDelegate;

		public readonly FighterGroup PlayerFighterUnits;
		public readonly FighterGroup EnemyFighterUnits;
		
		public static int SensorCount => 5;

		public SimpleMultiAgentGroup AgentParty { get; private set; }
		public BattleSubSystem(BaseFighterData playerUnit, BaseFighterData enemyUnit, FighterDropTable fighterDropTable,
			OnWinDelegate winDelegate, OnBattleComplete completeDelegate, SimpleMultiAgentGroup agentParty)
		{
			CurrentState = EBattleState.Start;
			PlayerFighterUnits = new FighterGroup {FighterUnits = new [] {playerUnit}};
			EnemyFighterUnits = new FighterGroup {FighterUnits = new [] {enemyUnit}};

			CurrentState = EBattleState.PlayerTurn;
			PlayerTurn();

			_fighterDropTable = fighterDropTable;
			_winDelegate += winDelegate;
			_completeDelegate += completeDelegate;
			AgentParty = agentParty;
		}

		public bool GameOver()
		{
			return CurrentState == EBattleState.Lost || CurrentState == EBattleState.Won || CurrentState == EBattleState.Flee;
		}

		private void PlayerAttack()
		{
			PlayerFighterUnits.Instance.Attack(EnemyFighterUnits.Instance);

			DialogueText = "The attack is successful!";

			if(EnemyFighterUnits.Instance.IsDead)
			{
				CurrentState = EBattleState.Won;
				_winDelegate?.Invoke();
				EndBattle();
			}
			else
			{
				CurrentState = EBattleState.EnemyTurn;
				EnemyTurn();
			}
		}

		private void EnemyTurn()
		{
			PlayerFighterUnits.Instance.Attack(PlayerFighterUnits.Instance);
			DialogueText = EnemyFighterUnits.Instance.UnitName + " attacks!";

			if(PlayerFighterUnits.Instance.IsDead)
			{
				CurrentState = EBattleState.Lost;
				EndBattle();
			}
			else
			{
				CurrentState = EBattleState.PlayerTurn;
				PlayerTurn();
			}
		}

		private void EndBattle()
		{
			_completeDelegate?.Invoke(this);
			if (CurrentState == EBattleState.Won)
			{
				DialogueText = "You won the battle!";
			}
			else if (CurrentState == EBattleState.Lost)
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
			PlayerFighterUnits.Instance.Heal(5);
			
			DialogueText = "You feel renewed strength!";

			CurrentState = EBattleState.EnemyTurn;
			
			EnemyTurn();
		}

		public void OnAttackButton()
		{
			if (CurrentState != EBattleState.PlayerTurn)
				return;
			
			PlayerAttack();
		}

		public void OnHealButton()
		{
			if (CurrentState != EBattleState.PlayerTurn)
				return;

			PlayerHeal();
		}

		public void OnFleeButton()
		{
			if (CurrentState != EBattleState.PlayerTurn)
				return;

			CurrentState = EBattleState.Flee;
		}

		public void SetInput(EBattleAction action)
		{
			switch (action)
			{
				case EBattleAction.Attack:
						OnAttackButton();
					break;
				case EBattleAction.Heal:
						OnHealButton();
					break;
				case EBattleAction.Flee:
						OnFleeButton();
					break;
			}
		}

		public CraftingDropReturn GetCraftingDropItem()
		{
			return _fighterDropTable.GenerateItems();
		}

		public ObsData[] GetSubsystemObservations(int inputLocation)
		{
			return new []
			{
				new ObsData{data=PlayerFighterUnits.Instance.Damage, name="PlayerFighterUnit.Damage"},
				new ObsData{data=PlayerFighterUnits.Instance.HpPercent, name="PlayerFighterUnit.HpPercent"},
				new ObsData{data=PlayerFighterUnits.Instance.Damage, name="EnemyFighterUnit.Damage"},
				new ObsData{data=PlayerFighterUnits.Instance.HpPercent,  name="EnemyFighterUnit.HpPercent"},
				new ObsData{data=inputLocation, name="InputLocation"},
			};
		}
	}
}