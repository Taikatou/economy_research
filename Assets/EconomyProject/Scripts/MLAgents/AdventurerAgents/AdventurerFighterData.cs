using EconomyProject.Scripts.Inventory;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class AdventurerFighterData : MonoBehaviour
    {
        public FighterData FighterData =>
            new FighterData
                (sprite,
                "Player",
                hp,
                hp,
                adventurerInventory.EquipedItem.itemDetails.damage);

        public AdventurerInventory adventurerInventory;

        public int hp = 20;

        public Sprite sprite;
    }
}
