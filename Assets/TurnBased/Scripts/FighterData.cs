using System;
using Data;
using TurnBased.Scripts.AI;

namespace TurnBased.Scripts
{
    [Serializable]
    public class FighterData : BaseFighterData<EFighterType, EAdventurerTypes>
    {
        public EFighterType fighterType;
        public int damage;
        private readonly EnemyAI _enemyAI;

        public override GetDamageModifier<EFighterType, EAdventurerTypes> GetDamageModifier =>
            FightingRelationships.GetDamage;
        public override int Damage => damage;
        public override float BlockReduction => 4.0f;
        public override int Level => 0;
        protected override EFighterType DamageType => fighterType;

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
