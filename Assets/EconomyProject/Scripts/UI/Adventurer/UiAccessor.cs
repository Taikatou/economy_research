using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class UiAccessor : MonoBehaviour
    {
        public GameObject coreGameSystem;

        public GameQuests GameQuests => coreGameSystem.GetComponentInChildren<GameQuests>();

        public PlayerInput PlayerInput => coreGameSystem.GetComponentInChildren<PlayerInput>();

        public GameAuction GameAuction => coreGameSystem.GetComponentInChildren<GameAuction>();

    }
}
