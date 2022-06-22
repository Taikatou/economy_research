using System;
using System.Collections.Generic;
using Data;
using TurnBased.Scripts.AI;
using Unity.MLAgents;

namespace TurnBased.Scripts
{
	public delegate void OnWinDelegate<T>(BattleSubSystemInstance<T> battle) where T : Agent;

	public delegate void OnBattleComplete<T>(BattleSubSystemInstance<T> battle) where T : Agent;
	public enum EBattleState { Start, PlayerTurn, EnemyTurn, Won, Lost, Flee }
	public enum EBattleAction { Attack, Block, Heal, Flee }

	public class FighterGroup
	{
		public int Index { get; set; }
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
		
		public static int SensorCount => 6 + 7 + 4;

		private double FleeChance = 0.8f;

		public SimpleMultiAgentGroup AgentParty { get; }

		public readonly T [] BattleAgents;

		private EnemyAI _enemyAI;
		public BattleSubSystemInstance(BaseFighterData[] playerUnit, BaseFighterData enemyUnit, FighterDropTable fighterDropTable,
			OnWinDelegate<T> winDelegate, OnBattleComplete<T> completeDelegate, SimpleMultiAgentGroup agentParty, T [] battleAgents)
		{
			CurrentState = EBattleState.Start;
			PlayerFighterUnits = new FighterGroup {FighterUnits = playerUnit};
			EnemyFighterUnits = new FighterGroup {FighterUnits = new [] {enemyUnit}};

			_enemyAI = new EnemyAI(EnemyFighterUnits);

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
			return CurrentState is EBattleState.Lost or EBattleState.Won or EBattleState.Flee;
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
				PlayerFighterUnits.Index++;
				if (PlayerFighterUnits.Index == SystemTraining.PartySize)
				{
					PlayerFighterUnits.Index = 0;
					CurrentState = EBattleState.EnemyTurn;
					EnemyTurn();	
				}
				else
				{
					DialogueText = "It is player: " + PlayerFighterUnits.Index + "s turn";
				}
			}
		}

		private void EnemyTurn()
		{
			var action = _enemyAI.DecideAction(PlayerFighterUnits.Instance);
			switch (action)
			{
				case EnemyAction.Attack:
					DialogueText = EnemyFighterUnits.Instance.UnitName + " attacks!";
					break;
				case EnemyAction.Block:
					DialogueText = EnemyFighterUnits.Instance.UnitName + " is preparing for an attack";
					break;
				case EnemyAction.Wait:
					DialogueText = EnemyFighterUnits.Instance.UnitName + " is doing nothing";
					break;
			}
			EnemyFighterUnits.Instance.Attack(PlayerFighterUnits.Instance);
			

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

		private void Block()
		{
			PlayerFighterUnits.Instance.Block();
			
			DialogueText = "You are preparing for an attack";

			CurrentState = EBattleState.EnemyTurn;
			
			EnemyTurn();
		}

		private void OnAttackButton()
		{
			if (CurrentState != EBattleState.PlayerTurn)
				return;
			
			PlayerAttack();
		}

		private void OnBlockButton()
		{
			if (CurrentState != EBattleState.PlayerTurn)
				return;

			Block();
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

		public void SetInput(EBattleAction action, int hashCode)
		{
			if (IsTurn(hashCode))
			{
				switch (action)
				{
					case EBattleAction.Attack:
						OnAttackButton();
						break;
					case EBattleAction.Heal:
						OnHealButton();
						break;
					case EBattleAction.Block:
						OnBlockButton();
						break;
					case EBattleAction.Flee:
						OnFleeButton();
						break;
				}	
			}
		}

		public bool IsTurn(int hashTurn)
		{
			return PlayerFighterUnits.Instance.HashCode == hashTurn;
		}

		public CraftingDropReturn GetCraftingDropItem()
		{
			return _fighterDropTable.GenerateItems();
		}

		public float GetExp()
		{
			return _fighterDropTable.Exp;
		}

		public ObsData[] GetSubsystemObservations(float inputLocation, int hashCode)
		{
			var playerName = _unitOneHotEncode[EnemyFighterUnits.Instance.UnitName];
			var yourTurn = IsTurn(hashCode)? 1.0f : 0.0f;
			return new ObsData []
			{
				new BaseCategoricalObsData(playerName, 8)
				{
					Name="Enemy name",
				},
				new SingleObsData{data=PlayerFighterUnits.Instance.Damage, Name="PlayerFighterUnit.Damage"},
				new SingleObsData{data=PlayerFighterUnits.Instance.HpPercent, Name="PlayerFighterUnit.HpPercent"},
				new SingleObsData{data=EnemyFighterUnits.Instance.Damage, Name="EnemyFighterUnit.Damage"},
				new SingleObsData{data=EnemyFighterUnits.Instance.HpPercent,  Name="EnemyFighterUnit.HpPercent"},
				new SingleObsData{data=inputLocation, Name="InputLocation"},
				new SingleObsData{data=yourTurn, Name="yourTurn"},
				new CategoricalObsData<EnemyAction>(_enemyAI.NextAction)
			};
		}

		private readonly Dictionary<string, int> _unitOneHotEncode = new()
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