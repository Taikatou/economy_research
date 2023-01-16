using Data;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Monobehaviours
{
    public class AdventurerShopSystemBehaviour : MonoBehaviour, ISetup, IGetSystem
    {
        public AdventurerShopSystem system;
        public void Setup()
        {
            // system.Setup();
        }

        public EconomySystem<AdventurerAgent, EAdventurerScreen, EAdventurerAgentChoices> GetSystem => system;
    }
}
