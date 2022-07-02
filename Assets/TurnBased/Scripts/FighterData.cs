using System;
using TurnBased.Scripts.AI;

namespace TurnBased.Scripts
{
    [Serializable]
    public class FighterData : BaseFighterData
    {
        public FighterType fighterType;
        public int damage;

        public override int Damage => damage;
        public override float BlockReduction => 4.0f;
        public override int Level => 0;
        private readonly EnemyAI _enemyAI;
        
        protected override void AfterAttack()
        {
            
        }

        public static FighterData Clone(FighterData original)
        {
            var data = new FighterData
            {
                damage = original.damage,
                Sprite = original.Sprite,
                UnitName = original.UnitName,
                MaxHp = original.MaxHp,
                CurrentHp = original.MaxHp
            };
            return data;
        }
    }
}
