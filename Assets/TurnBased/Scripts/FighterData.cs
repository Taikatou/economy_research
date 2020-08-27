using System;

namespace TurnBased.Scripts
{
    [Serializable]
    public class FighterData : BaseFighterData
    {
        public int damage;

        public override int Damage => damage;
        
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
