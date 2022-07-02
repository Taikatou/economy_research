using UnityEngine;

namespace TurnBased.Scripts
{
    public delegate string AttackAction(EnemyFighterGroup enemyFighterUnits, PlayerFighterData instance);
    public abstract class BaseFighterData
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

        private void TakeDamage(int dmg)
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
                        Attack(this);
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

        public void Attack(BaseFighterData enemy)
        {
            var rand = new System.Random();
            var randomFloat = rand.NextDouble();
            if (randomFloat < Accuracy)
            {
                enemy.TakeDamage(Damage);   
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
