using EconomyProject.Scripts.Inventory;
using UnityEngine;

namespace TurnBased.Scripts
{
    public class PlayerFighterData : BaseFighterData
    {
        private readonly AdventurerInventory _agentInventory;

        private UsableItem UsableItem => _agentInventory.EquipedItem;
        
        public override int Damage => UsableItem.itemDetails.damage;
        
        public override void AfterAttack()
        {
            _agentInventory.DecreaseDurability();
        }

        public PlayerFighterData(Sprite sprite, string unitName, int maxHp, AdventurerInventory agentInventory) : base(sprite, unitName, maxHp)
        {
            _agentInventory = agentInventory;
        }
    }
}
