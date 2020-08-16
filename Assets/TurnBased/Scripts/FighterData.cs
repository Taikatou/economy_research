using System;
using UnityEngine;

namespace TurnBased.Scripts
{
    [Serializable]
    public class FighterData : BaseFighterData
    {
        public int damage;

        public override int Damage => damage;
        
        public override void AfterAttack()
        {
            
        }

        public FighterData(Sprite sprite, string unitName, int maxHp, int currentHp, int damage) : base(sprite, unitName, maxHp)
        {
            this.damage = damage;
        }
		
        public FighterData(FighterData original) : base(original.Sprite, original.UnitName, original.MaxHp)
        {
            MaxHp = original.MaxHp;
            damage = original.damage;
        }
    }
}
