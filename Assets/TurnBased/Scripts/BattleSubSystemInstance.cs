using System;
using System.Collections.Generic;
using Codice.Client.Common;
using Data;
using TurnBased.Scripts.AI;
using Unity.MLAgents;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TurnBased.Scripts
{
	public delegate void OnWinDelegate<T>(BattleSubSystemInstance<T> battle) where T : Agent;

	public delegate void OnBattleComplete<T>(BattleSubSystemInstance<T> battle) where T : Agent;
	public enum EBattleState { Start, PlayerTurn, EnemyTurn, Won, Lost, Flee }
	public enum EBattleAction { PrimaryAction, SecondaryAction, BonusAction }

	public class FighterGroup<T, L, F> where T : BaseFighterData<L, F> where L : Enum where F : Enum
	{
		public int Index { get; set; }
		public T[] FighterUnits;
		public T Instance => FighterUnits[Index];
	}

	public class PlayerFighterGroup : FighterGroup<PlayerFighterData, EAdventurerTypes, EFighterType>
	{
		public PlayerFighterGroup(PlayerFighterData[] fighters)
		{
			FighterUnits = fighters;
		}

		public PlayerFighterData GetAgentPlayerData(int hashCode)
		{
			foreach (var p in FighterUnits)
			{
				if (p.HashCode == hashCode)
				{
					return p;
				}
			}

			return null;
		}

		public bool Dead
		{
			get
			{
				var dead = true;
				foreach (var p in FighterUnits)
				{
					if (!p.IsDead)
					{
						dead = false;
					}
				}

				return dead;
			}
		}
	}

	public class EnemyFighterGroup : FighterGroup<FighterData, EFighterType, EAdventurerTypes>
	{
		public readonly EFighterType FighterType;

		public EnemyFighterGroup(FighterData fighter)
		{
			FighterUnits = new FighterData[] {fighter};
			FighterType = fighter.fighterType;
		}
	}

	public class BattleSubSystemInstance<T> : ILastUpdate, IUpdate where T : Agent
	{
		public EBattleState CurrentState { get; private set; }
		public string DialogueText { get; private set; }
		
		private readonly FighterDropTable _fighterDropTable;
		private readonly OnWinDelegate<T> _winDelegate;
		private readonly OnWinDelegate<T> _loseDelegate;
		private readonly OnBattleComplete<T> _completeDelegate;

		public readonly PlayerFighterGroup PlayerFighterUnits;
		public readonly EnemyFighterGroup EnemyFighterUnits;
		
		public static int SensorCount => 8 + SensorUtils<EAdventurerTypes>.Length + (SensorUtils<EAttackOptions>.Length*3);

		private const double FleeChance = 0.8f;

		public SimpleMultiAgentGroup AgentParty { get; }

		public readonly T [] BattleAgents;

		private readonly EnemyAI _enemyAI;
		
	// @TIMER CHANGE
		public float CurrentTimerValue { get; private set; }

		private const float TurnCount = 5.0f;

		public bool TurnCountDown => false;

		public void Update()
		{
			if (CurrentState == EBattleState.PlayerTurn && TurnCountDown)
			{
				CurrentTimerValue -= Time.deltaTime;
				if (CurrentTimerValue <= 0)
				{
					_attackValue = -1;
					StartNextTurn();
					Debug.Log("Turn end due to timer");
				}	
			}
		}

		private void ResetTimer()
		{
			CurrentTimerValue = TurnCount;
		}
	// END
		public BattleSubSystemInstance(PlayerFighterData[] playerUnit, FighterData enemyUnit, FighterDropTable fighterDropTable,
			OnWinDelegate<T> winDelegate, OnBattleComplete<T> completeDelegate, OnWinDelegate<T> loseDelegate, SimpleMultiAgentGroup agentParty, T [] battleAgents)
		{
			CurrentState = EBattleState.Start;
			PlayerFighterUnits = new PlayerFighterGroup(playerUnit);
			EnemyFighterUnits = new EnemyFighterGroup (enemyUnit);

			_enemyAI = new EnemyAI(EnemyFighterUnits);

			CurrentState = EBattleState.PlayerTurn;
			PlayerTurn();

			_fighterDropTable = fighterDropTable;
			_winDelegate += winDelegate;
			_completeDelegate += completeDelegate;
			_loseDelegate = loseDelegate;
			AgentParty = agentParty;
			BattleAgents = battleAgents;
			
			Refresh();
		}

		public bool GameOver()
		{
			return CurrentState is EBattleState.Lost or EBattleState.Won or EBattleState.Flee;
		}

		public bool IsTurn(PlayerFighterData p)
		{
			return CurrentState == EBattleState.PlayerTurn && PlayerFighterUnits.Instance == p;
		}

		private void EnemyTurn()
		{
			var action = _enemyAI.DecideAction(PlayerFighterUnits);
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

			if(PlayerFighterUnits.Dead)
			{
				Debug.Log("Player Lost");
				_loseDelegate.Invoke(this);
				CurrentState = EBattleState.Lost;
				EndBattle();
			}
			else
			{
				CurrentState = EBattleState.PlayerTurn;
				PlayerTurn();
			}
		}

		public void FinishBattle()
		{
			foreach (var agent in BattleAgents)
			{
				AgentParty.UnregisterAgent(agent);
			}
		}

		public void EndBattle()
		{
			FinishBattle();
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

		private void StartNextTurn()
		{
			do
			{
				PlayerFighterUnits.Index++;
			}
			while (PlayerFighterUnits.Index < SystemTraining.PartySize && PlayerFighterUnits.Instance.IsDead);
						
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

			ResetTimer();
		}

		private void OnAttackButton(EBattleAction action)
		{
			if (CurrentState != EBattleState.PlayerTurn)
				return;

			var actionDelegate = PlayerFighterUnits.Instance.GetAttackAction(action);
			if (actionDelegate.HasValue)
			{
				_attackValue = (int) actionDelegate.Value;
				var del = PlayerActionMap.GetAttackDelegate[actionDelegate.Value];
				var str = del.Invoke(EnemyFighterUnits, PlayerFighterUnits.Instance);
				DialogueText = str;
				
				Refresh();
			
				if(EnemyFighterUnits.Instance.IsDead)
				{
					CurrentState = EBattleState.Won;
					_winDelegate?.Invoke(this);
					EndBattle();
				}
				else
				{
					StartNextTurn();
				}
				EnemyTurn();
			}
		}

		// out of use in multiplayer environment
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
				OnAttackButton(action);
			}
		}

		private bool IsTurn(int hashTurn)
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
			var index = 0;
			var player = PlayerFighterUnits.GetAgentPlayerData(hashCode);
			var map = PlayerActionMap.GetAttackActionMap(PlayerActionMap.GetAbilities(player.AdventurerType, player.Level));
			
			var yourTurn = IsTurn(hashCode)? 1.0f : 0.0f;
			EAttackOptions bonsAction = map.ContainsKey(EBattleAction.BonusAction)? map[EBattleAction.BonusAction] : EAttackOptions.None;
			var obs = new ObsData []
			{
				new CategoricalObsData<EAdventurerTypes>(player.AdventurerType)
				{
					Name="Enemy name",
				},
				new SingleObsData{data=PlayerFighterUnits.Instance.Damage, Name="PlayerFighterUnit.Damage"},
				new SingleObsData{data=PlayerFighterUnits.Instance.HpPercent, Name="PlayerFighterUnit.HpPercent"},
				new SingleObsData{data=EnemyFighterUnits.Instance.Damage, Name="EnemyFighterUnit.Damage"},
				new SingleObsData{data=EnemyFighterUnits.Instance.HpPercent,  Name="EnemyFighterUnit.HpPercent"},
				new SingleObsData{data=inputLocation, Name="InputLocation"},
				new SingleObsData{data=yourTurn, Name="yourTurn"},
				new CategoricalObsData<EAttackOptions>(map[EBattleAction.PrimaryAction]),
				new CategoricalObsData<EAttackOptions>(map[EBattleAction.SecondaryAction]),
				new CategoricalObsData<EAttackOptions>(bonsAction),
				new SingleObsData{data=CurrentTimerValue / TurnCount, Name="EnemyFighterUnit.turnTimer"},
				new SingleObsData{data=_attackValue, Name="Attack Option"}
			};
			_attackValue = 0;
			return obs;
		}

		private int _attackValue = 0;

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

		public DateTime LastUpdated { get; private set; }
		public void Refresh()
		{
			LastUpdated = DateTime.Now;
		}
	}
}