using System;
using Data;
using TurnBased.Scripts.AI;
using Unity.MLAgents;
using UnityEngine;

namespace TurnBased.Scripts
{
    public delegate void OnWinDelegate<T>(IBattleSubSystemInstance<T> battle) where T : Agent;

    public delegate void OnBattleComplete<T>(IBattleSubSystemInstance<T> battle) where T : Agent;
    
    public abstract class IBattleSubSystemInstance<T> : IUpdate, ILastUpdate where T : Agent
    {
        protected readonly OnWinDelegate<T> _winDelegate;
        protected readonly OnWinDelegate<T> _loseDelegate;
        private readonly OnBattleComplete<T> _completeDelegate;
        private readonly FighterDropTable _fighterDropTable;
        public SimpleMultiAgentGroup AgentParty { get; }
        
        public string DialogueText { get; set; }
        public PlayerFighterGroup PlayerFighterUnits { get; protected set; }
        public EnemyFighterGroup EnemyFighterUnits { get; protected set; }
        public static int SensorCount => 8 + SensorUtils<EAdventurerTypes>.Length + (SensorUtils<EAttackOptions>.Length*3) + 
                                         SensorUtils<EnemyAction>.Length;

        public abstract void FinishBattle();

        public abstract void SetInput(EBattleAction action, int hashCode);
        
        public readonly T [] BattleAgents;
        
        public abstract ObsData[] GetSubsystemObservations(float inputLocation, int hashCode);

        public virtual void EndBattle()
        {
            _completeDelegate.Invoke(this);
        }

        public abstract bool IsTurn(PlayerFighterData p);
        public EBattleState CurrentState { get; set; }


        public IBattleSubSystemInstance(OnWinDelegate<T> winDelegate, OnBattleComplete<T> completeDelegate,
            OnWinDelegate<T> loseDelegate, T [] battleAgents, FighterDropTable fighterDropTable, SimpleMultiAgentGroup agentParty)
        {
            _winDelegate += winDelegate;
            _completeDelegate += completeDelegate;
            _loseDelegate = loseDelegate;
            BattleAgents = battleAgents;
            _fighterDropTable = fighterDropTable;
            AgentParty = agentParty;
        }

        public DateTime LastUpdated { get; private set; }
        public void Refresh()
        {
            LastUpdated = DateTime.Now;
        }
        
        public void AddReward(float reward)
        {
            foreach (var agent in BattleAgents)
            {
                agent.AddReward(reward);
            }
        }
        
        public CraftingDropReturn GetCraftingDropItem()
        {
            return _fighterDropTable.GenerateItems();
        }
        
        public float GetExp()
        {
            return _fighterDropTable.Exp;
        }
        
        public bool TurnCountDown => false;
        public float CurrentTimerValue { get; private set; }
        
        public virtual void Update()
        {
            if (CurrentState == EBattleState.PlayerTurn && TurnCountDown)
            {
                CurrentTimerValue -= Time.deltaTime;
                if (CurrentTimerValue <= 0)
                {
                    AttackValue = -1;
                    StartNextTurn();
                    Debug.Log("Turn end due to timer");
                }	
            }
        }

        protected virtual void StartNextTurn()
        {
        }

        protected const float TurnCount = 5.0f;
        
        protected void ResetTimer()
        {
            CurrentTimerValue = TurnCount;
        }
        
        protected int AttackValue = 0;
    }
}
