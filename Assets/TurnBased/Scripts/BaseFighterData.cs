using UnityEngine;

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
        public abstract void AfterAttack();

        public bool TakeDamage(int dmg)
        {
            Debug.Log(CurrentHp + "\t" + dmg);
            CurrentHp -= dmg;

            return CurrentHp <= 0;
        }

        public void Heal(int amount)
        {
            CurrentHp += amount;
            if (CurrentHp > MaxHp)
            {
                CurrentHp = MaxHp;	
            }
        }
    }
}
