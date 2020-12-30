using Inventory;


namespace TurnBased.Scripts
{
    public delegate void OnAfterAttack();

    public delegate UsableItem GetUsableItem();
    public class PlayerFighterData : BaseFighterData
    {
        private UsableItem UsableItem => getUsableItem.Invoke();
        
        public override int Damage => UsableItem.itemDetails.damage;

        public OnAfterAttack onAfterAttack;
        public GetUsableItem getUsableItem;
        
        protected override void AfterAttack()
        {
            onAfterAttack?.Invoke();
        }
        
        public void ResetHp()
        {
            CurrentHp = MaxHp;
        }
    }
}
