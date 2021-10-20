﻿using UnityEngine;

namespace TurnBased.Scripts
{
    public abstract class BaseFighterData
    {
        public string UnitName;
        public Sprite Sprite;
        public int MaxHp;
        public int CurrentHp { get; set; }

        public float HpPercent => (float)CurrentHp / MaxHp;

        public abstract int Damage { get; }
        protected abstract void AfterAttack();
        public bool IsDead => CurrentHp <= 0;
        protected virtual double Accuracy => 0.8;

        private void TakeDamage(int dmg)
        {
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

        public void Attack(BaseFighterData enemy)
        {
            var rand = new System.Random();
            var randomFloat = rand.NextDouble();
            if (randomFloat < Accuracy)
            {
                enemy.TakeDamage(Damage);   
            }
            AfterAttack();
        }
    }
}
