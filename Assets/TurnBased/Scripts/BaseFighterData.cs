using UnityEngine;

namespace TurnBased.Scripts
{
    public abstract class BaseFighterData
    {
        public readonly string UnitName;
        
        public readonly Sprite Sprite;
        
        public int MaxHp;
        
        public int CurrentHp { get; protected set; }

        public abstract int Damage { get; }

        public abstract void AfterAttack();
        protected BaseFighterData(Sprite sprite, string unitName, int maxHp)
        {
            Sprite = sprite;
            UnitName = unitName;
            MaxHp = maxHp;
            CurrentHp = MaxHp;
        }
        
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
