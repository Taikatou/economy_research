using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Adventurer
{
    public class UiAccessor : MonoBehaviour
    {
        public GameObject coreGameSystem;

        public AdventurerInput AdventurerInput => coreGameSystem.GetComponentInChildren<AdventurerInput>();
    }
}
