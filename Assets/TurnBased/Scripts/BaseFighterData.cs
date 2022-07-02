using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBased.Scripts
{
    public delegate string AttackAction(EnemyFighterGroup enemyFighterUnits, PlayerFighterData instance);
    public abstract class BaseFighterData<T> where T : Enum
    {
        public string UnitName;
        public Sprite Sprite;
        public int MaxHp;
        public int CurrentHp { get; set; }

        public float GetObs => CurrentHp / MaxHp;

        public float HpPercent => (float)CurrentHp / MaxHp;

        public abstract int Damage { get; }
        protected abstract void AfterAttack();
        public bool IsDead => CurrentHp <= 0;
        protected virtual double Accuracy => 0.8;

        public abstract float BlockReduction { get; }
        public abstract int Level { get; }

        public abstract Dictionary<T, AttackAction> AttackActionMap { get; }

        public AttackAction GetAttackAction(T action)
        {
            if (AttackActionMap.ContainsKey(action))
            {
                return AttackActionMap[action];
            }

            return null;
        }

        private void TakeDamage(int dmg)
        {
            if (Blocking)
            {
                dmg /= (int) BlockReduction;
            }
            var newDmg = CurrentHp - dmg;
            if (newDmg < 0)
            {
                newDmg = 0;
            }

            CurrentHp = newDmg;
        }

        public void Heal(int amount)
        {
            CurrentHp += amount;
            if (CurrentHp > MaxHp)
            {
                CurrentHp = MaxHp;	
            }
        }

        public void Attack<TQ>(BaseFighterData<TQ> enemy) where TQ : Enum
        {
            var rand = new System.Random();
            var randomFloat = rand.NextDouble();
            if (randomFloat < Accuracy)
            {
                enemy.TakeDamage(Damage);   
            }
            AfterAttack();
            Blocking = false;
        }

        public void Block()
        {
            Blocking = true;
        }

        public void Wait()
        {
            Blocking = false;
        }
        
        public bool Blocking { get; private set; }

        public int HashCode { get; set; }
    }
}
