using EconomyProject.Scripts.Inventory;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class AdventurerFighterData : MonoBehaviour
    {
        public BaseFighterData FighterData =>
            new PlayerFighterData
                (sprite,
                "Player",
                hp,
                adventurerInventory);

        public AdventurerInventory adventurerInventory;

        public int hp = 20;

        public Sprite sprite;
    }
}
