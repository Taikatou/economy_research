using EconomyProject.Scripts.Inventory;
using Inventory;
using Inventory;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class AdventurerFighterData : MonoBehaviour
    {
        public int startHp = 20;
        public Sprite sprite;
        public PlayerFighterData playerData;
        public AdventurerInventory adventurerInventory;
        public BaseFighterData FighterData => playerData;
        public void Start()
        {
            playerData = new PlayerFighterData
            {
                Sprite = sprite,
                UnitName = "Player",
                MaxHp = startHp,
                CurrentHp = startHp,
                onAfterAttack=OnAfterAttack,
                getUsableItem=GetUsableItem
            };
        }

        public void OnAfterAttack()
        {
            adventurerInventory.DecreaseDurability();
        }

        public UsableItem GetUsableItem()
        {
            return adventurerInventory.EquipedItem;
        }
    }
}
