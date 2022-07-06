using System;
using UnityEngine;

namespace TurnBased.Scripts
{
    public interface GetCurrentHP
    {
        int CurrentHp { get; }
    }
    public delegate string AttackAction(EnemyFighterGroup enemyFighterUnits, PlayerFighterData instance);
    public delegate DamageModifier GetDamageModifier<T, Q>(T a, Q b);
    public abstract class BaseFighterData<T, Q> : GetCurrentHP where T : Enum where Q : Enum
    {
        public abstract GetDamageModifier<T, Q> GetDamageModifier { get; }
        public string UnitName;
        public Sprite Sprite;
        public int MaxHp;
        public int CurrentHp { get; set; }

        public float GetObs => (float)CurrentHp / MaxHp;

        public float HpPercent => (float)CurrentHp / MaxHp;

        public abstract int Damage { get; }
        protected abstract void AfterAttack();
        public bool IsDead => CurrentHp <= 0;
        protected virtual double Accuracy => 0.8;

        public abstract float BlockReduction { get; }
        public abstract int Level { get; }

        protected abstract T DamageType { get; }

        private void TakeDamage(int dmg, Q damageFrom)
        {
            void AttackHit(int damage)
            {
                var newDmg = CurrentHp - damage;
                if (newDmg < 0)
                {
                    newDmg = 0;
                }
                CurrentHp = newDmg;
            }
            switch (SecondaryAbilityStatus)
            {
                case SecondaryAbilityStatus.Blocking:
                    AttackHit(dmg / (int) BlockReduction);
                    break;
                case SecondaryAbilityStatus.Parrying:
                    var rand = new System.Random();
                    var randomFloat = (float) rand.NextDouble();
                    if (randomFloat <= ParryChance)
                    {
                        // TODO FIX THIS
                        // Attack(this, (Q) 0);
                    }
                    else
                    {
                        AttackHit(dmg);
                    }
                    break;
                case SecondaryAbilityStatus.None:
                    AttackHit(dmg);
                    break;
                case SecondaryAbilityStatus.Evade:

                    break;
            }
        }

        private float ParryChance = 0.1f;

        public void Heal(int amount)
        {
            CurrentHp += amount;
            if (CurrentHp > MaxHp)
            {
                CurrentHp = MaxHp;	
            }
        }

        public void Attack(BaseFighterData<Q, T> enemy)
        {
            var rand = new System.Random();
            var randomFloat = rand.NextDouble();
            if (randomFloat < Accuracy)
            {
                var modifier = GetDamageModifier.Invoke(DamageType, enemy.DamageType);
                float dmg = Damage;
                switch (modifier)
                {
                    case DamageModifier.Strong:
                        dmg *= 1.5f;
                        break;
                    case DamageModifier.Weak:
                        dmg /= 2.0f;
                        break;
                }
                enemy.TakeDamage((int)dmg, DamageType);   
            }
            AfterAttack();
            SecondaryAbilityStatus = SecondaryAbilityStatus.None;
        }

        public void Block()
        {
            SecondaryAbilityStatus = SecondaryAbilityStatus.Blocking;
        }

        public void Parry()
        {
            SecondaryAbilityStatus = SecondaryAbilityStatus.Parrying;
        }

        public void Evade()
        {
            SecondaryAbilityStatus = SecondaryAbilityStatus.Evade;
        }

        public void Wait()
        {
            SecondaryAbilityStatus = SecondaryAbilityStatus.None;
        }

        public SecondaryAbilityStatus SecondaryAbilityStatus { get; private set; }

        public int HashCode { get; set; }
    }
    
    public enum SecondaryAbilityStatus {None, Blocking, Parrying, Evade}
}
