using System.Collections.Generic;
using Data;
using Unity.MLAgents;

namespace TurnBased.Scripts
{
	public delegate void OnWinDelegate<T>(BattleSubSystemInstance<T> battle) where T : Agent;

	public delegate void OnBattleComplete<T>(BattleSubSystemInstance<T> battle) where T : Agent;
	public enum EBattleState { Start, PlayerTurn, EnemyTurn, Won, Lost, Flee }
	public enum EBattleAction { Attack, Heal, Flee }

	public class FighterGroup
	{
		private int Index = 0;
		public BaseFighterData[] FighterUnits;
		public BaseFighterData Instance => FighterUnits[Index];
	}

	public class BattleSubSystemInstance<T> where T : Agent
	{
		public EBattleState CurrentState { get; private set; }
		public string DialogueText { get; private set; }
		
		private readonly FighterDropTable _fighterDropTable;
		private readonly OnWinDelegate<T> _winDelegate;
		private readonly OnBattleComplete<T> _completeDelegate;

		public readonly FighterGroup PlayerFighterUnits;
		public readonly FighterGroup EnemyFighterUnits;
		
		public static int SensorCount => 6;
		
		public double FleeChance = 0.8f;

		public SimpleMultiAgentGroup AgentParty { get; }

		public readonly T [] BattleAgents;
		public BattleSubSystemInstance(BaseFighterData[] playerUnit, BaseFighterData enemyUnit, FighterDropTable fighterDropTable,
			OnWinDelegate<T> winDelegate, OnBattleComplete<T> completeDelegate, SimpleMultiAgentGroup agentParty, T [] battleAgents)
		{
			CurrentState = EBattleState.Start;
			PlayerFighterUnits = new FighterGroup {FighterUnits = playerUnit};
			EnemyFighterUnits = new FighterGroup {FighterUnits = new [] {enemyUnit}};

			CurrentState = EBattleState.PlayerTurn;
			PlayerTurn();

			_fighterDropTable = fighterDropTable;
			_winDelegate += winDelegate;
			_completeDelegate += completeDelegate;
			AgentParty = agentParty;
			BattleAgents = battleAgents;
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
				_winDelegate?.Invoke(this);
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
			EnemyFighterUnits.Instance.Attack(PlayerFighterUnits.Instance);
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
			_completeDelegate.Invoke(this);
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

		private void OnAttackButton()
		{
			if (CurrentState != EBattleState.PlayerTurn)
				return;
			
			PlayerAttack();
		}

		private void OnHealButton()
		{
			if (CurrentState != EBattleState.PlayerTurn)
				return;

			PlayerHeal();
		}

		private void OnFleeButton()
		{
			if (CurrentState != EBattleState.PlayerTurn)
				return;

			var rand = new System.Random();
			if (rand.NextDouble() < FleeChance)
			{
				CurrentState = EBattleState.Flee;
				EndBattle();
			}
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
			var playerName = UnitOneHotEncode[EnemyFighterUnits.Instance.UnitName];
			return new []
			{
				new ObsData{data=playerName, name="Enemy name"},
				new ObsData{data=PlayerFighterUnits.Instance.Damage, name="PlayerFighterUnit.Damage"},
				new ObsData{data=PlayerFighterUnits.Instance.HpPercent, name="PlayerFighterUnit.HpPercent"},
				new ObsData{data=EnemyFighterUnits.Instance.Damage, name="EnemyFighterUnit.Damage"},
				new ObsData{data=EnemyFighterUnits.Instance.HpPercent,  name="EnemyFighterUnit.HpPercent"},
				new ObsData{data=inputLocation, name="InputLocation"},
			};
		}

		private readonly Dictionary<string, int> UnitOneHotEncode = new Dictionary<string, int>()
		{
			{"Bear", 0},
			{"Buffalo", 1},
			{"Crocodile", 2},
			{"Elephant", 3},
			{"Gorilla", 4},
			{"Narwhale", 5},
			{"Owl", 6},
			{"Snake", 7}
		};

		public void AddReward(float reward)
		{
			foreach (var agent in BattleAgents)
			{
				agent.AddReward(reward);
			}
		}
	}
}