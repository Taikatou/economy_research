using Data;
using Inventory;


namespace TurnBased.Scripts
{
    public delegate void OnAfterAttack();

    public delegate UsableItem GetUsableItem();

    public delegate int BonusDamage();
    public class PlayerFighterData : BaseFighterData
    {
        public EAdventurerTypes adventurerType;
        private UsableItem UsableItem => GetUsableItem.Invoke();

        public override int Damage
        {
            get
            {
                var damage = UsableItem.itemDetails.damage;
                if (BonusDamage != null)
                {
                    damage += BonusDamage.Invoke();
                }
                return damage;
            }
        }

        public OnAfterAttack OnAfterAttack { get; set; }
        public GetUsableItem GetUsableItem { get; set; }
        public BonusDamage BonusDamage { get; set; }

        protected override double Accuracy => 1;
        public override float BlockReduction => 2.0f;
        public override int Level => level;

        public int level = 0;

        protected override void AfterAttack()
        {
            OnAfterAttack?.Invoke();
        }
        
        public void ResetHp()
        {
            CurrentHp = MaxHp;
        }
    }
}
